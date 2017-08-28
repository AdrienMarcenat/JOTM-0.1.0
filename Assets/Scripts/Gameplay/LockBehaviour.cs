using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockBehaviour : MonoBehaviour
{
	[SerializeField] private string targetTag;

	private Transform previousTarget;
	private Camera2D camera2D;
	private bool isLocked = false;


	void Start()
	{
		camera2D = GetComponent<Camera>().GetComponent<Camera2D>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == tag && !isLocked)
		{
			isLocked = true;
			PushTarget(other.transform);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == tag && isLocked)
		{
			isLocked = false;
			PopTarget();
		}
	}

	private void PushTarget(Transform newTarget)
	{
		previousTarget = camera2D.GetTrackingTarget();
		camera2D.SetTrackingTarget(newTarget);
	}

	private void PopTarget()
	{
		camera2D.SetTrackingTarget(previousTarget);
	}

}
