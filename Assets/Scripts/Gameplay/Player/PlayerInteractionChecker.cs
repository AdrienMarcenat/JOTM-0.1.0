using UnityEngine;
using System.Collections;

public class PlayerInteractionChecker : MonoBehaviour
{
	[SerializeField] private LayerMask pastLayerMask;
	[SerializeField] private LayerMask presentLayerMask;
	[SerializeField] private float raycastMargin;

	private Transform lightDirection;
	private Vector3 lightVector;
	private bool check = false;

	void Awake()
	{
		lightDirection = GameObject.FindGameObjectWithTag ("Light").transform;
		Physics2D.SetLayerCollisionMask (LayerMask.NameToLayer ("Player"), presentLayerMask.value);
	}

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += OnMoonLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= OnMoonLight;
	}

	void Update ()
	{
		lightVector = lightDirection.localPosition;
		lightVector.Normalize ();

		if (!check)
			return;

		//RaycastHit2D hit = Physics2D.Raycast(transform.position - raycastMargin * lightVector
	}

	private void OnMoonLight(bool enable)
	{
		check = enable;
	}
}

