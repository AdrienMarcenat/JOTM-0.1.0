using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2D : MonoBehaviour
{
	[SerializeField] float followSpeed;
	[SerializeField] float zoomFactor = 1.0f;
	[SerializeField] float zoomSpeed  = 5.0f;
	[SerializeField] Transform trackingTarget;

	private Camera mainCamera;
	private Transform player;

	void Awake()
	{
		mainCamera = GetComponent<Camera>();
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		SetZoom(zoomFactor);
	}

	void Update()
	{
		if (trackingTarget == null)
			trackingTarget = player;
		
		float xTarget = trackingTarget.position.x;
		float yTarget = trackingTarget.position.y;

		float xNew = Mathf.Lerp (transform.position.x, xTarget, Time.deltaTime * followSpeed);
		float yNew = Mathf.Lerp (transform.position.y, yTarget, Time.deltaTime * followSpeed);
	
		transform.position = new Vector3(xNew, yNew, transform.position.z);
	}
		
	public void SetZoom(float zoomFactor)
	{
		if(mainCamera == null)
			mainCamera = GetComponent<Camera>();
		this.zoomFactor = zoomFactor;
		StartCoroutine (Zoom());
	}

	IEnumerator Zoom()
	{
		float targetSize = zoomFactor;
		if(targetSize < mainCamera.orthographicSize)
			while (targetSize < GetComponent<Camera>().orthographicSize)
			{
				mainCamera.orthographicSize -= Time.deltaTime * zoomSpeed;
				yield return null;
			}
		else
			while (targetSize > mainCamera.orthographicSize)
			{
				mainCamera.orthographicSize += Time.deltaTime * zoomSpeed;
				yield return null;
			}
	}

	public void SetTrackingTarget(Transform newTarget)
	{
		trackingTarget = newTarget;
	}
}
