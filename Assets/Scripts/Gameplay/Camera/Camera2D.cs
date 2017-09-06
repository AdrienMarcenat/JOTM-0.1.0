using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Simple camera scripts, should be attach to the main Camera.
 * It provides a tracking method, and a zoom method.
 **/
public class Camera2D : MonoBehaviour
{
	[SerializeField] private float followSpeed;
	[SerializeField] private float zoomFactor = 10.0f;
	[SerializeField] private float zoomSpeed  = 5.0f;
	[SerializeField] private Transform trackingTarget;
	[SerializeField] private List<Transform> lanes;
	[SerializeField] private float xOffset;
	[SerializeField] private float yOffset; 
	[SerializeField] private bool isXLocked = false;
	[SerializeField] private bool isYLocked = false;

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
			
		float xTarget = trackingTarget.position.x + xOffset;
		float yTarget = trackingTarget.position.y + yOffset;

		float xNew = transform.position.x;
		if (!isXLocked)
		{
			xNew = Mathf.Lerp(transform.position.x, xTarget, Time.unscaledDeltaTime * followSpeed);
		}

		float yNew = transform.position.y;
		if (!isYLocked)
		{
			yNew = Mathf.Lerp (transform.position.y, yTarget, Time.unscaledDeltaTime * followSpeed);
		}
		else
		{
			if (lanes.Count > 1)
			{
				int i = 0;
				for (i = 0; i < lanes.Count - 1; ++i)
				{
					if ((trackingTarget.position.y > lanes [i].position.y) &&
						(trackingTarget.position.y <= lanes [i + 1].position.y))
					{
						yNew = lanes [i].position.y;
						break;
					}
				}

				if (i == lanes.Count - 1)
					yNew = lanes [lanes.Count - 1].position.y;
			}
			else
			{
				yNew = lanes [0].position.y;
			}
		}

		yNew = Mathf.Lerp(transform.position.y, yNew + yOffset, Time.deltaTime * followSpeed);
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

	public Transform GetTrackingTarget()
	{
		return trackingTarget;
	}
}
