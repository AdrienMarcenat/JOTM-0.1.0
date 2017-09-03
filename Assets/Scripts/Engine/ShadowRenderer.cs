using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowRenderer : MonoBehaviour
{
	[SerializeField] private Transform lightDirection;
	[SerializeField] private float raycastDistance;
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
		for (int i = 0; i < allPolygonCollider.Length; i++)
		{
			PolygonCollider2D mf = allPolygonCollider [i];
			// Don't forget the offset, otherwise the point position will be wrong
			Vector3 firstWorldPoint = mf.transform.TransformPoint (mf.points [0] + mf.offset);
			Vector3 secondWorldPoint = Vector3.zero;
			RaycastHit2D firstRaycast = Physics2D.Raycast (firstWorldPoint + raycastMargin * lightVector, lightVector, raycastDistance, contactFilter.layerMask);
			RaycastHit2D secondRaycast;

			/**
			 * For each edge we cast two raycasts below from the endPoints,
			 * this gives us four points defining a shadow polygon.
			 **/
			for (int j = 0; j < mf.GetTotalPointCount (); j++)
			{
				// The last edge is point n-1 and 0
				int nextPointIndex = j == mf.GetTotalPointCount () - 1 ? 0 : j + 1;

				secondWorldPoint = mf.transform.TransformPoint (mf.points [nextPointIndex] + mf.offset);
				secondRaycast = Physics2D.Raycast (secondWorldPoint + raycastMargin * lightVector, lightVector, raycastDistance, contactFilter.layerMask);

				Vector3 firstLowPoint = firstRaycast.point;
				Vector3 secondLowPoint = secondRaycast.point;
				// If we hit nothing the point will be at maximum distance
				if (firstRaycast.collider == null)
					firstLowPoint = firstWorldPoint + raycastDistance * lightVector;
				if (secondRaycast.collider == null)
					secondLowPoint = secondWorldPoint + raycastDistance * lightVector;

				Debug.DrawLine (firstWorldPoint, firstLowPoint, Color.yellow);
				Debug.DrawLine (secondWorldPoint, secondLowPoint, Color.yellow);

				MakePolygon (firstWorldPoint, secondWorldPoint, secondLowPoint, firstLowPoint);

				firstWorldPoint = secondWorldPoint;
				firstRaycast = secondRaycast;
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

