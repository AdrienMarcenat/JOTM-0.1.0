using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoonlightBaker : MonoBehaviour
{
	[SerializeField] private float maxDistance;
	[SerializeField] private Vector3 lightDirection;
	[SerializeField] private Material lightMaterial;
	[SerializeField] private float lightRadius = 20f;
	[SerializeField] private ContactFilter2D contactFilter;
	[SerializeField] private float raycastMargin = 0.05f;
	[SerializeField] private int maxCollider;

	private BoxCollider2D boxCollider;
	private Vector3 lightOrthogonalDirection;
	private Mesh lightMesh;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private List<Vector3> meshVertices;
	private PolygonCollider2D[] allPolygonCollider;
	private List<Vertex> allVertices = new List<Vertex>();
	private List<Vertex> tempVertices = new List<Vertex> ();
	private bool isOn = false;

	public class Vertex
	{
		public Vector3 pos;
		public float distanceFromLight;
		public float distanceFromLightOrthogonal;
	}

	void Awake()
	{
		boxCollider = GetComponent<BoxCollider2D> ();
		meshVertices = new List<Vector3> ();
		lightMesh = new Mesh ();
		lightDirection.Normalize ();
		lightOrthogonalDirection = Vector3.Cross(lightDirection, new Vector3(0,0,1));
			
		meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		meshRenderer.sharedMaterial = lightMaterial;
		meshRenderer.enabled = false;

		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshFilter.mesh = lightMesh;
	}
		
	public void Enable()
	{
		this.isOn = !isOn;
		meshRenderer.enabled = !meshRenderer.enabled;
	}

	void Update()
	{
		lightDirection.Normalize ();
		lightOrthogonalDirection = Vector3.Cross(lightDirection, new Vector3(0,0,1));
		getAllPolygonCollider();
		setLight ();
		renderLightMesh ();
	}

	private void getAllPolygonCollider()
	{
		Collider2D[] allColl2D = Physics2D.OverlapBoxAll (boxCollider.offset, boxCollider.size, 0f, contactFilter.layerMask);
		int numbreOfCollider = allColl2D.Length;
		allPolygonCollider = new PolygonCollider2D[numbreOfCollider];

		for (int i = 0; i < numbreOfCollider; i++)
			allPolygonCollider[i] = (PolygonCollider2D)allColl2D[i];
	}

	private void setLight () 
	{
		allVertices.Clear();
		tempVertices.Clear ();

		RaycastHit2D[] results = new RaycastHit2D[2];
		int hitNumber = 0;

		for (int i = 0; i < allPolygonCollider.Length; i++) 
		{
			PolygonCollider2D mf = allPolygonCollider[i];
			Vector3 leftEndVertex = Vector3.zero;
			Vector3 rightEndVertex = Vector3.zero;
			float leftEndDistanceFromLightOrthogonal = 0;
			float rightEndDistanceFromLightOrthogonal = 0;
			bool rightEndIsVisible = false;
			bool leftEndIsVisible = false;

			for (int j = 0; j < mf.GetTotalPointCount (); j++)
			{
				Vector3 worldPoint = mf.transform.TransformPoint (mf.points [j]);
				hitNumber = Physics2D.Raycast (worldPoint - raycastMargin * lightDirection, -lightDirection, contactFilter, results, maxDistance);
				Debug.DrawRay (worldPoint - raycastMargin * lightDirection, -lightDirection, Color.yellow, 0.01f);

				Vertex v = CreateVertex (worldPoint);
				// If we hit nothing this point is reached by the light
				if (hitNumber == 0)
					tempVertices.Add (v);

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

		if (tempVertices.Count == 0)
			return;
		
		sortListOrthogonal(tempVertices);

		float distanceFromLightOrthogonal = tempVertices[0].distanceFromLightOrthogonal;
		List<List<Vertex>> verticesLists = new List<List<Vertex>> ();
		List<Vertex> l = new List<Vertex> ();
		foreach(Vertex v in tempVertices)
		{
			if (Mathf.Abs(v.distanceFromLightOrthogonal - distanceFromLightOrthogonal) < 0.01f)
				l.Add (v);
			else
			{
				verticesLists.Add (l);
				l = new List<Vertex> ();
				l.Add (v);
				distanceFromLightOrthogonal = v.distanceFromLightOrthogonal;
			}
		}
		verticesLists.Add (l);

		float distanceFromLight = float.PositiveInfinity;
		foreach (List<Vertex> list in verticesLists)
		{
			sortList (list);
			foreach (Vertex v in list)
				print (transform.TransformPoint(v.pos));
			print("distance " + list [0].distanceFromLight);
			if (list [0].distanceFromLight < distanceFromLight - raycastMargin)
				distanceFromLight = list [list.Count - 1].distanceFromLight;
			else
			{
				print ("reverse");
				distanceFromLight = list [0].distanceFromLight;
				list.Reverse ();
			}
		}

		foreach (List<Vertex> list in verticesLists)
			foreach (Vertex v in list)
				allVertices.Add (v);

		Vertex vAbove1 = new Vertex();
		vAbove1.pos = allVertices[allVertices.Count - 1].pos - maxDistance * lightDirection;
		Vertex vAbove2 = new Vertex();
		vAbove2.pos = allVertices[0].pos - maxDistance * lightDirection;

		allVertices.Add (vAbove1);
		allVertices.Add (vAbove2);

		foreach (Vertex v in allVertices)
			print (v.pos);
		print ("bob of bob");
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

		Vertex v = CreateVertex (rayBelow.point);
		tempVertices.Add (v);
		Debug.DrawLine (worldPoint, rayBelow.point, Color.green);
	}

	private void renderLightMesh()
	{
		Vector3[] initVerticesMeshLight = new Vector3[allVertices.Count];
		Vector2[] triangulatorVertices = new Vector2[allVertices.Count];

		for (int i = 0; i < allVertices.Count; i++) 
		{
			initVerticesMeshLight [i] = allVertices [i].pos;
			triangulatorVertices  [i] = allVertices [i].pos;
		}
		
		Triangulator tr = new Triangulator (triangulatorVertices);
		int[] triangles = tr.Triangulate ();

		lightMesh.Clear ();
		lightMesh.vertices = initVerticesMeshLight;
		lightMesh.triangles = triangles;

		Vector2[] uvs = new Vector2[initVerticesMeshLight.Length];
		for (int i = 0; i < initVerticesMeshLight.Length; i++) 
			uvs[i] = new Vector2(initVerticesMeshLight[i].x, initVerticesMeshLight[i].y);
		lightMesh.uv = uvs;
	}

	private void sortList(List<Vertex> list)
	{
		list.Sort((item1, item2) => (item2.distanceFromLight.CompareTo(item1.distanceFromLight)));
	}

	private void sortListOrthogonal(List<Vertex> list)
	{
		list.Sort((item1, item2) => (item2.distanceFromLightOrthogonal.CompareTo(item1.distanceFromLightOrthogonal)));
	}
		
	private float GetDistanceFromLight (Vector3 worldPoint, Vector3 light)
	{
		return Vector3.Dot (light, worldPoint);
	}

	private Vertex CreateVertex(Vector3 worldPoint)
	{
		Vertex v = new Vertex ();	
		v.pos = transform.InverseTransformPoint (worldPoint);
		v.distanceFromLight = GetDistanceFromLight (worldPoint, lightDirection);
		v.distanceFromLightOrthogonal = GetDistanceFromLight (worldPoint, lightOrthogonalDirection);

		return v;
	}
}

