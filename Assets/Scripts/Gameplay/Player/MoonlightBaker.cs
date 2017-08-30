using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoonlightBaker : MonoBehaviour
{
	[SerializeField] private float maxDistance;
	[SerializeField] private Vector3 lightDirection;
	[SerializeField] private ContactFilter2D contactFilter;
	[SerializeField] private Material lightMaterial;
	[SerializeField] private Vector3 step;
	private Mesh mesh;
	private MeshRenderer meshRenderer;
	private MeshFilter meshFilter;
	private List<Vector3> meshVertices;
	private bool isOn = false;

	private Vector3 firstHit;
	private Vector3 secondHit;
	private Vector3 firstMiddleHit;
	private Vector3 secondMiddleHit;

	void Awake()
	{
		meshVertices = new List<Vector3> ();
		mesh = new Mesh ();

		meshVertices.Add (Vector3.zero);
		meshVertices.Add (step);
		meshVertices.Add (maxDistance * lightDirection);
		meshVertices.Add (step + maxDistance * lightDirection);

		mesh.SetVertices (meshVertices);
		mesh.SetTriangles (fourVerticesTriangles, 0);

		meshRenderer = gameObject.AddComponent<MeshRenderer> ();
		meshRenderer.sharedMaterial = lightMaterial;
		meshRenderer.enabled = false;

		meshFilter = gameObject.AddComponent<MeshFilter> ();
		meshFilter.mesh = mesh;
	}
		
	private Vector3 ShootLight(Vector3 position)
	{
		RaycastHit2D[] results = new RaycastHit2D[5];
		int hitNumber = Physics2D.Raycast (position, lightDirection, contactFilter, results, maxDistance);
		Debug.DrawRay (position, maxDistance*lightDirection, Color.red, 0.01f);

		if (hitNumber > 0) 
		{
			RaycastHit2D raycast = results [0];
			raycast.collider.gameObject.SendMessage ("OnMoonlight");
			return transform.InverseTransformPoint( new Vector3(raycast.point.x, raycast.point.y, 0));
		}

		return transform.InverseTransformPoint(position) + maxDistance * lightDirection;
	}

	void Update () 
	{
		if (!isOn)
			return;
		
		firstHit = ShootLight (transform.position);
		secondHit = ShootLight (transform.position + step);

		if ((firstHit + step) == secondHit) 
		{
			SetMeshFourVertices ();
			return;
		}
		
		for (int i = 1; i < 10; i++) 
		{
			RaycastHit2D[] results = new RaycastHit2D[1];
			int hitNumber = Physics2D.Raycast (transform.position + i * step / 10, lightDirection, contactFilter, results, maxDistance);
			Debug.DrawRay (transform.position + i * step / 10, maxDistance * lightDirection, Color.yellow, 0.01f);

			if (hitNumber > 0) 
			{
				RaycastHit2D raycast = results [0];
				Vector3 test = transform.InverseTransformPoint( new Vector3(raycast.point.x, raycast.point.y, 0));
				if (firstHit + i * step / 10 == test) 
				{
					firstMiddleHit = test;
					secondMiddleHit = secondHit - (10 - i) * step / 10;
				}
			}
		}

		SetMeshSixVertices ();
	}

	private void SetMeshFourVertices()
	{
		mesh.Clear ();

		meshVertices.Clear ();
		meshVertices.Add (Vector3.zero);
		meshVertices.Add (step);
		meshVertices.Add (firstHit);
		meshVertices.Add (secondHit);

		mesh.SetVertices (meshVertices);
		mesh.SetTriangles (fourVerticesTriangles, 0);
	}

	private void SetMeshSixVertices()
	{
		mesh.Clear ();

		meshVertices.Clear ();
		meshVertices.Add (Vector3.zero);
		meshVertices.Add (step);
		meshVertices.Add (firstHit);
		meshVertices.Add (firstMiddleHit);
		meshVertices.Add (secondMiddleHit);
		meshVertices.Add (secondHit);

		mesh.SetVertices (meshVertices);
		mesh.SetTriangles (sixVerticesTriangles, 0);
	}

	public void Enable()
	{
		this.isOn = !isOn;
		meshRenderer.enabled = !meshRenderer.enabled;
	}

	private int[] fourVerticesTriangles = {0,1,3,0,3,2};
	private int[] sixVerticesTriangles  = {0,3,2,0,1,3,1,5,3,3,5,4};
}

