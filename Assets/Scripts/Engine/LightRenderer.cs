using UnityEngine;
using System.Collections;

public class LightRenderer : MonoBehaviour
{
	private Vector3 lightVector;
	private Collider2D trigger;

	void Awake()
	{
		lightVector = transform.localPosition;
		lightVector.Normalize ();

		trigger = GetComponent<Collider2D> ();
		trigger.enabled = false;
	}

	void Update()
	{
		// Recalculate the lightVector if the lightDirection has changed (rotating mirror for example)
		lightVector = transform.localPosition;
		lightVector.Normalize ();
	}

	public Vector3 GetLightVector()
	{
		return lightVector;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		PlayerPart playerPart = other.GetComponent<PlayerPart> ();
		if(playerPart != null)
			playerPart.AddLight (lightVector);
	}

	void OnTriggerExit2D(Collider2D other)
	{
		PlayerPart playerPart = other.GetComponent<PlayerPart> ();
		if(playerPart != null)
			playerPart.RemoveLight (lightVector);
	}

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += OnMoonLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= OnMoonLight;
	}

	private void OnMoonLight(bool enable)
	{
		trigger.enabled = !trigger.enabled;
	}
}

