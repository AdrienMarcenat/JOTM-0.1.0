using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class MoonLightManager : MonoBehaviour 
{
	[SerializeField] private GameObject moonLightRaycasterPrefab;
	[SerializeField] private Vector3 step;
	[SerializeField] private int moonLightRaycasterNumber = 20;
	[SerializeField] private ShadowRenderer shadowRenderer;
	private MoonLightRayCaster[] raycasters;
	private bool past = false;

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += EnableLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= EnableLight;
	}

	void Awake()
	{
		for (int i = 0; i < moonLightRaycasterNumber; i++)
		{
			GameObject raycaster = Instantiate (moonLightRaycasterPrefab, transform);
			raycaster.transform.position = i * step + transform.position;
			if (i != 0)
			{
				GameObject mirrorRaycaster = Instantiate (moonLightRaycasterPrefab, transform);
				mirrorRaycaster.transform.position = -i * step + transform.position;
			}
		}

		raycasters = GetComponentsInChildren<MoonLightRayCaster> ();
	}

	public void EnableLight()
	{
		past = !past;
		StartCoroutine (ChangeTimeScale());
		shadowRenderer.Enable();
		foreach (MoonLightRayCaster raycaster in raycasters)
			raycaster.Enable ();
	}

	/**
	 * Going from past to present : 
	 * speed time so the present reflect the consequences of the past
	 * Going from past to present :
	 * wait for the animation to finish
	 */
	IEnumerator ChangeTimeScale()
	{
		yield return null;
		Time.timeScale = past ? 0 : 99;
		yield return new WaitForSecondsRealtime (0.5f);
		Time.timeScale = 1;
	}
}
