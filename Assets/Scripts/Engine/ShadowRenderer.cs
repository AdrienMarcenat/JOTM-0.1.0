using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowRenderer : MonoBehaviour
{
	[SerializeField] private Transform lightDirection;
	[SerializeField] private float shadowDistance;
	// raycastMargin is used to slightly shift the raycast in order avoid casting inside the collider
	[SerializeField] private float raycastMargin = 0.05f;
	// The layermask of the contact filter will determine which layers should cast shadows
	[SerializeField] private ContactFilter2D contactFilter;
	[SerializeField] private Material shadowMaterial;
	// Only objects overlapping the boxCollider will cast shadows
	[SerializeField] private BoxCollider2D boxCollider;

	private Vector3 lightVector;
	private Mesh shadowMesh;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private PolygonCollider2D[] allPolygonCollider;
	private List<Mesh> meshes;
	private bool isOn = false;

	// The shadow polygons are all squares thus the triangles are the following
	private int[] triangles = {0,1,2, 0,2,3};

	void Awake()
	{
		meshes = new List<Mesh> ();
		shadowMesh = new Mesh ();

		meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		meshRenderer.sharedMaterial = shadowMaterial;
		meshRenderer.enabled = false;

		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshFilter.mesh = shadowMesh;
	}

	public void Enable ()
	{
		this.isOn = !isOn;
		meshRenderer.enabled = !meshRenderer.enabled;
	}

	void Update ()
	{
		if (!isOn)
			return;

		// Recalculate the lightVector if the lightDirection has changed (rotating mirror for example)
		lightVector = lightDirection.localPosition;
		lightVector.Normalize ();

		meshes.Clear ();
		GetAllPolygonCollider ();
		SetShadows ();
		RenderShadowMesh ();
	}

	private void GetAllPolygonCollider ()
	{
		// Get all the collider of the relevant layers overlapping the boxCollider
		Collider2D[] allColl2D = Physics2D.OverlapBoxAll (boxCollider.transform.position + (Vector3)boxCollider.offset, boxCollider.size, 0f, contactFilter.layerMask);
		int numbreOfCollider = allColl2D.Length;

		allPolygonCollider = new PolygonCollider2D[numbreOfCollider];
		for (int i = 0; i < numbreOfCollider; i++)
			allPolygonCollider[i] = (PolygonCollider2D)allColl2D[i];
	}

	private void SetShadows ()
	{
		bool castShadow = true;

		for (int i = 0; i < allPolygonCollider.Length; i++)
		{
			PolygonCollider2D mf = allPolygonCollider [i];
			castShadow = mf.gameObject.GetComponent<MoonLightSensitive> ().GetCastShadow ();

			// Don't forget the offset, otherwise the point position will be wrong
			Vector3 firstWorldPoint = mf.transform.TransformPoint (mf.points [0] + mf.offset);
			Vector3 secondWorldPoint = Vector3.zero;

			for (int j = 0; j < mf.GetTotalPointCount (); j++)
			{
				// The last edge is point n-1 and 0
				int nextPointIndex = j == mf.GetTotalPointCount () - 1 ? 0 : j + 1;
				secondWorldPoint = mf.transform.TransformPoint (mf.points [nextPointIndex] + mf.offset);

				if (castShadow)
				{
					Vector3 firstLowPoint = firstWorldPoint + shadowDistance * lightVector;
					Vector3 secondLowPoint = secondWorldPoint + shadowDistance * lightVector;
					MakePolygon (firstWorldPoint, secondWorldPoint, secondLowPoint, firstLowPoint);
				}

				// For each vertex we cast a ray toward the light
				RaycastHit2D raycast = Physics2D.Raycast (firstWorldPoint - raycastMargin * lightVector, - shadowDistance * lightVector, shadowDistance, contactFilter.layerMask);
				// If nothing was hit the object is visible
				if(raycast.collider == null)
					mf.gameObject.SendMessage ("OnMoonlight");
				
				firstWorldPoint = secondWorldPoint;
			}
		}
	}

	private void RenderShadowMesh()
	{
		// Combine all shadow polygon into a single mesh
		CombineInstance[] combine = new CombineInstance[meshes.Count];
		for (int i = 0; i < meshes.Count; i++)
		{
			combine [i].mesh = meshes [i];
			combine [i].transform = transform.localToWorldMatrix;
		}
		shadowMesh.Clear ();
		shadowMesh.CombineMeshes (combine);
	}

	private void MakePolygon(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		Mesh m = new Mesh ();
		Vector3[] vertices = new Vector3[4];
		vertices [0] = v1;
		vertices [1] = v2;
		vertices [2] = v3;
		vertices [3] = v4;

		m.vertices = vertices;
		m.triangles = triangles;
		meshes.Add (m);
	}
}

