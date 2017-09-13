using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowRenderer : MonoBehaviour
{
	[SerializeField] private float shadowDistance;
	[SerializeField] private Material shadowMaterial;
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private float raycastMargin = 0.05f;

	private PolygonCollider2D polygonCollider;
	private Transform lightDirection;
	private Vector3 lightVector;
	private Mesh shadowMesh;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private List<Mesh> meshes;
	private bool isOn = false;

	// The shadow polygons are all squares thus the triangles are the following
	private int[] triangles = {0,1,2, 0,2,3};

	void Awake()
	{
		meshes = new List<Mesh> ();
		shadowMesh = new Mesh ();

		lightDirection = GameObject.FindGameObjectWithTag ("Light").transform;
		polygonCollider = GetComponent<PolygonCollider2D> ();

		meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		meshRenderer.sharedMaterial = shadowMaterial;
		meshRenderer.enabled = false;

		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshFilter.mesh = shadowMesh;
	}

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += OnMoonLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= OnMoonLight;
	}

	public void OnMoonLight (bool enable)
	{
		isOn = enable;
		meshRenderer.enabled = enable;
	}

	void Update ()
	{
		if (!isOn)
			return;

		// Recalculate the lightVector if the lightDirection has changed (rotating mirror for example)
		lightVector = lightDirection.localPosition;
		lightVector.Normalize ();

		meshes.Clear ();
		SetShadows ();
		RenderShadowMesh ();
	}

	private void SetShadows ()
	{
		Collider2D thisCollider = (Collider2D)polygonCollider;
		// Don't forget the offset, otherwise the point position will be wrong
		Vector3 firstWorldPoint = transform.TransformPoint (polygonCollider.points [0] + polygonCollider.offset);
		Vector3 secondWorldPoint = Vector3.zero;

		for (int j = 0; j < polygonCollider.GetTotalPointCount (); j++)
		{
			// The last edge is point n-1 and 0
			int nextPointIndex = j == polygonCollider.GetTotalPointCount () - 1 ? 0 : j + 1;
			secondWorldPoint = transform.TransformPoint (polygonCollider.points [nextPointIndex] + polygonCollider.offset);

			Vector3 firstLowPoint = firstWorldPoint + shadowDistance * lightVector;
			Vector3 secondLowPoint = secondWorldPoint + shadowDistance * lightVector;

			RaycastHit2D firstRaycastHit = Physics2D.Raycast (firstWorldPoint + raycastMargin * lightVector, lightVector, shadowDistance, layerMask.value);
			RaycastHit2D secondRaycastHit = Physics2D.Raycast (secondWorldPoint + raycastMargin * lightVector, lightVector, shadowDistance, layerMask.value);
			if(firstRaycastHit.collider != thisCollider && secondRaycastHit.collider != thisCollider)
				MakePolygon (firstWorldPoint, secondWorldPoint, secondLowPoint, firstLowPoint);
			
			firstWorldPoint = secondWorldPoint;
		}
	}

	private void RenderShadowMesh()
	{
		// Combine all shadow polygon into a single mesh
		CombineInstance[] combine = new CombineInstance[meshes.Count];
		for (int i = 0; i < meshes.Count; i++)
		{
			combine [i].mesh = meshes [i];
			combine [i].transform = transform.worldToLocalMatrix;
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

