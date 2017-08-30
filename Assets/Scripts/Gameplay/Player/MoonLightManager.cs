using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class MoonLightManager : MonoBehaviour 
{
	private MoonLightRayCaster[] moonlightRayCasters;
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
		moonlightRayCasters = GetComponentsInChildren<MoonLightRayCaster>();
	}

	public void EnableLight()
	{
		past = !past;
		StartCoroutine (ChangeTimeScale());
		foreach (MoonLightRayCaster rayCaster in moonlightRayCasters)
			rayCaster.Enable();
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
