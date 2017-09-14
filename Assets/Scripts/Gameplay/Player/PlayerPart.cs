using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPart : MonoBehaviour
{
	[SerializeField] private LayerMask layerMask;
	[SerializeField] private float raycastDistance = 50f;
	protected Rigidbody2D body;

	private List<Vector3> lightVectors;

	protected void Awake()
	{
		lightVectors = new List<Vector3> ();
		gameObject.layer = LayerMask.NameToLayer ("PresentPlayer");
		body = GetComponentInParent<Rigidbody2D> ();
	}

	protected void Update ()
	{
		bool light = false;
		foreach (Vector3 lightVector in lightVectors)
		{
			RaycastHit2D[] raycastHit = Physics2D.RaycastAll (transform.position, -lightVector, raycastDistance, layerMask);
			Debug.DrawRay(transform.position, - raycastDistance * lightVector, Color.yellow, 0.01f);
			if (raycastHit.Length == 0)
			{
				gameObject.layer = LayerMask.NameToLayer ("PastPlayer");
				light = true;
				break;
			}
		}
		if (!light)
			gameObject.layer = LayerMask.NameToLayer ("PresentPlayer");
	}

	public void AddLight(Vector3 lightVector)
	{
		lightVectors.Add (lightVector);
	}

	public void RemoveLight(Vector3 lightVector)
	{
		lightVectors.Remove (lightVector);
	}
}

