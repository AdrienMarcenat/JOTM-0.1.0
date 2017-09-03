using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoonlightMeshRenderer : MonoBehaviour
{
	[SerializeField] private float maxDistance;
	[SerializeField] private Vector3 lightDirection;
	[SerializeField] private Material lightMaterial;
	[SerializeField] private ContactFilter2D contactFilter;
	[SerializeField] private float raycastMargin = 0.05f;
	[SerializeField] private float lightRadius =  20f;

	private BoxCollider2D boxCollider;
	private Vector3 lightOrthogonalDirection;
	private Mesh lightMesh;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private List<Vector3> meshVertices;
	private PolygonCollider2D[] allPolygonCollider;
	private List<Vertex> allVertices = new List<Vertex>();
	private List<SweepLine> sweepLines;
	private List<Mesh> meshes;
	private bool isOn = false;

	private int[] triangles = {0,2,1, 0,3,2};

	public class Vertex
	{
		public Vector3 pos;
		public float distanceFromLight;
		public float distanceFromLightOrthogonal;
		public PolygonCollider2D collider;
		public int colliderIndex;

		public Vertex() {}

		public Vertex(Vertex v)
		{
			this.pos = v.pos;
			this.distanceFromLight = v.distanceFromLight;
			this.distanceFromLightOrthogonal = v.distanceFromLightOrthogonal;
			this.collider = v.collider;
			this.colliderIndex = v.colliderIndex;
		}
	}

	public class SweepLine
	{
		public Vertex leftEnd;
		public Vertex rightEnd;

		public SweepLine(Vertex leftEnd, Vertex rightEnd)
		{
			this.leftEnd = new Vertex(leftEnd);
			this.rightEnd = new Vertex(rightEnd);
		}
	}

	void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D> ();
		meshVertices = new List<Vector3> ();
		meshes = new List<Mesh> ();
		sweepLines = new List<SweepLine> ();
		lightMesh = new Mesh ();

		meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		meshRenderer.sharedMaterial = lightMaterial;

		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshFilter.mesh = lightMesh;
	}

	public void Enable ()
	{
		this.isOn = !isOn;
		meshRenderer.enabled = !meshRenderer.enabled;
	}

	void Update ()
	{
		lightDirection.Normalize ();
		lightOrthogonalDirection = Vector3.Cross(lightDirection, new Vector3(0,0,1));

		GetAllPolygonCollider ();
		GetVisibleVertices ();
		Sweep ();
		RenderLightMesh ();
	}

	private void GetAllPolygonCollider ()
	{
		Collider2D[] allColl2D = Physics2D.OverlapBoxAll (boxCollider.offset, boxCollider.size, 0f, contactFilter.layerMask);
		int numbreOfCollider = allColl2D.Length;
		allPolygonCollider = new PolygonCollider2D[numbreOfCollider];

		for (int i = 0; i < numbreOfCollider; i++)
			allPolygonCollider[i] = (PolygonCollider2D)allColl2D[i];
	}

	private void GetVisibleVertices ()
	{
		allVertices.Clear ();

		RaycastHit2D[] results = new RaycastHit2D[2];
		int hitNumber = 0;
		Vector3 leftEndVertex = Vector3.zero;
		Vector3 rightEndVertex = Vector3.zero;
		float leftEndDistanceFromLightOrthogonal = 0;
		float rightEndDistanceFromLightOrthogonal = 0;
		bool rightEndIsVisible = false;
		bool leftEndIsVisible = false;
	
		for (int i = 0; i < allPolygonCollider.Length; i++)
		{
			PolygonCollider2D mf = allPolygonCollider [i];

			for (int j = 0; j < mf.GetTotalPointCount (); j++)
			{
				Vector3 worldPoint = mf.transform.TransformPoint (mf.points [j]);
				hitNumber = Physics2D.Raycast (worldPoint - raycastMargin * lightDirection, -lightDirection, contactFilter, results, maxDistance);
				Debug.DrawRay (worldPoint - raycastMargin * lightDirection, -lightDirection, Color.yellow, 0.01f);

				Vertex v = CreateVertex (worldPoint, mf, j);
				// If we hit nothing this point is reached by the light
				if (hitNumber == 0)
					allVertices.Add (v);

				if (v.distanceFromLightOrthogonal < rightEndDistanceFromLightOrthogonal || rightEndVertex == Vector3.zero)
				{
					rightEndVertex = worldPoint;
					rightEndDistanceFromLightOrthogonal = v.distanceFromLightOrthogonal;
					rightEndIsVisible = (hitNumber == 0);
				}
				if (v.distanceFromLightOrthogonal > leftEndDistanceFromLightOrthogonal || leftEndVertex == Vector3.zero)
				{
					leftEndVertex = worldPoint;
					leftEndDistanceFromLightOrthogonal = v.distanceFromLightOrthogonal;
					leftEndIsVisible = (hitNumber == 0);
				}
			}

			if(rightEndVertex != Vector3.zero && rightEndIsVisible)
				RayCastBelow (rightEndVertex, mf);
			if(leftEndVertex != Vector3.zero && leftEndIsVisible)
				RayCastBelow (leftEndVertex, mf);
		}

		if (allVertices.Count == 0)
			return;

		SortList (allVertices);
	}

	private void RayCastBelow(Vector3 worldPoint, Collider2D collider)
	{
		RaycastHit2D[] results = new RaycastHit2D[2];
		int hitNumber = Physics2D.Raycast (worldPoint, lightDirection, contactFilter, results, maxDistance);

		if (hitNumber == 0)
			return;

		RaycastHit2D rayBelow = results [0];
		if (rayBelow.collider == collider)
		{
			if(hitNumber == 1)
				return;
			else
				rayBelow = results [1];
		}

		Vertex v = CreateVertex (rayBelow.point, (PolygonCollider2D)rayBelow.collider, 0);
		allVertices.Add (v);
		Debug.DrawLine (worldPoint, rayBelow.point, Color.green);
	}

	private void Sweep()
	{
		meshes.Clear ();

		Vertex rightEnd = CreateVertex (lightRadius * (lightOrthogonalDirection - lightDirection), null, 0);
		Vertex leftEnd = CreateVertex (lightRadius * (-lightOrthogonalDirection - lightDirection), null, 0);
		SweepLine initialSweepLine = new SweepLine (rightEnd, leftEnd);
		DebugSweepLine (initialSweepLine);

		sweepLines.Clear ();
		sweepLines.Add (initialSweepLine);

		foreach (Vertex v in allVertices)
		{
			int sweepLineIndex = FindSweepLine (v);
			SweepLine s = sweepLines [sweepLineIndex];
			print ("pos " + v.pos + " distance " + v.distanceFromLight);
			print ("s distance " + s.leftEnd.distanceFromLight + " s index " + sweepLineIndex + " s rightpos " + s.rightEnd.pos + " s leftpos " + s.leftEnd.pos);

			bool addLeft = true;
			bool addRight = true;
			if (CheckIfEdge (v, s.leftEnd))
			{
				print ("Make polygon left" + v.pos + " " + s.leftEnd.pos);
				addLeft = false;
				MakePolygon (v, s.leftEnd);
			}
			if (CheckIfEdge (v, s.rightEnd))
			{
				print ("Make polygon right" + v.pos + " " + s.rightEnd.pos);
				addRight = false;
				MakePolygon (v, s.rightEnd);
			}

			if (addLeft)
			{
				Vector3 newLeftEndPos = s.leftEnd.pos + (v.distanceFromLight - s.leftEnd.distanceFromLight) * lightDirection;
				RaycastHit2D[] results = new RaycastHit2D[2];
				int hitNumber = 0;
				hitNumber = Physics2D.Raycast (v.pos + 0.05f * lightOrthogonalDirection, lightOrthogonalDirection, contactFilter, results, (newLeftEndPos - v.pos).magnitude);
				Debug.DrawRay (v.pos + 0.05f * lightOrthogonalDirection, lightOrthogonalDirection, Color.green, 0.01f);

				if (hitNumber != 0 && results[0].collider != v.collider)
				{
					MakePolygon (CreateVertex(results [0].point, null, 0), s.leftEnd);
					s.leftEnd.pos = results [0].point;
				}
				else
					s.leftEnd.pos = newLeftEndPos;
				
				s.leftEnd.distanceFromLight = v.distanceFromLight;
				SweepLine newLeftSweepLine = new SweepLine (s.leftEnd, v);
				sweepLines.Add (newLeftSweepLine);
				//DebugSweepLine (newLeftSweepLine);
			}
			if (addRight)
			{
				Vector3 newrightEndPos = s.rightEnd.pos + (v.distanceFromLight - s.rightEnd.distanceFromLight) * lightDirection;
				RaycastHit2D[] results = new RaycastHit2D[2];
				int hitNumber = 0;
				hitNumber = Physics2D.Raycast (v.pos - 0.05f * lightOrthogonalDirection, -lightOrthogonalDirection, contactFilter, results, (v.pos - newrightEndPos).magnitude);
				Debug.DrawRay (v.pos - 0.05f * lightOrthogonalDirection, -lightOrthogonalDirection, Color.blue, 0.01f);

				if (hitNumber != 0 && results[0].collider != v.collider)
				{
					MakePolygon (CreateVertex(results [0].point, null, 0), s.rightEnd);
					s.rightEnd.pos = results [0].point;
				}
				else
					s.rightEnd.pos = newrightEndPos;
				
				s.rightEnd.distanceFromLight = v.distanceFromLight;
				SweepLine newRightSweepLine = new SweepLine(v, s.rightEnd);
				sweepLines.Add (newRightSweepLine);
				//DebugSweepLine (newRightSweepLine);
			}

			sweepLines.Remove (s);
		}
		foreach (SweepLine s in sweepLines)
		{
			DebugSweepLine (s);
			MakePolygon (s.leftEnd, s.rightEnd);
		}
	}

	private void RenderLightMesh()
	{
		CombineInstance[] combine = new CombineInstance[meshes.Count];
		for (int i = 0; i < meshes.Count; i++)
		{
			combine [i].mesh = meshes [i];
			combine [i].transform = transform.localToWorldMatrix;
		}
		lightMesh.Clear ();
		lightMesh.CombineMeshes (combine);
	}

	private void MakePolygon(Vertex v1, Vertex v2)
	{
		if (v1.distanceFromLightOrthogonal < v2.distanceFromLightOrthogonal)
		{
			Vertex temp = v1;
			v1 = v2;
			v2 = temp;
		}

		Mesh m = new Mesh ();
		Vector3[] vertices = new Vector3[4];
		vertices [0] = v1.pos;
		vertices [1] = v2.pos;
		vertices [2] = v2.pos - lightRadius * lightDirection;
		vertices [3] = v1.pos - lightRadius * lightDirection;

		m.vertices = vertices;
		m.triangles = triangles;
		meshes.Add (m);
	}

	private void SortList(List<Vertex> list)
	{
		list.Sort((item1, item2) => (item1.distanceFromLight.CompareTo(item2.distanceFromLight)));
	}

	private float GetDistanceFromLight (Vector3 worldPoint, Vector3 light)
	{
		return Vector3.Dot (light, worldPoint);
	}

	private Vertex CreateVertex(Vector3 worldPoint, PolygonCollider2D collider, int colliderIndex)
	{
		Vertex v = new Vertex ();	
		v.pos = worldPoint;
		v.distanceFromLight = GetDistanceFromLight (worldPoint, lightDirection);
		v.distanceFromLightOrthogonal = GetDistanceFromLight (worldPoint, lightOrthogonalDirection);
		v.collider = collider;
		v.colliderIndex = colliderIndex;

		return v;
	}

	private bool CheckIfEdge(Vertex v1, Vertex v2)
	{
		if (v1.collider == null || v2.collider == null || v1.collider != v2.collider)
			return false;
		
		int i = v1.colliderIndex;
		int j = v2.colliderIndex;
		int pointsCount = v1.collider.points.Length;
		return (i == j + 1)
		|| (i == j - 1)
		|| (i == 0 && j == pointsCount - 1)
		|| (j == 0 && i == pointsCount - 1);
	}

	private int FindSweepLine(Vertex v)
	{
		/*int lowBound = 0;
		int upBound = sweepLines.Count - 1;
		int index = (upBound - lowBound) / 2;
		while (lowBound != upBound)
		{
			if (v.distanceFromLightOrthogonal > sweepLines [index].rightEnd.distanceFromLightOrthogonal)
				lowBound = index;
			else
			if (v.distanceFromLightOrthogonal < sweepLines [index].leftEnd.distanceFromLightOrthogonal)
				upBound = index;
			else
				break;
			index = (upBound - lowBound) / 2;
		}

		return index;*/

		for(int i = 0; i < sweepLines.Count; i++)
		{
			SweepLine s = sweepLines [i];
			if (v.distanceFromLightOrthogonal >= s.rightEnd.distanceFromLightOrthogonal
				&& v.distanceFromLightOrthogonal <= s.leftEnd.distanceFromLightOrthogonal)
				return i;
		}

		return 0;
	}

	private void DebugSweepLine(SweepLine s)
	{
		Debug.DrawLine(transform.TransformPoint(s.rightEnd.pos), transform.TransformPoint(s.leftEnd.pos), Color.red);
	}
}

