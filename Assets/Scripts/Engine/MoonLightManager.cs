using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class MoonLightManager : MonoBehaviour 
{
	[SerializeField] private ShadowRenderer shadowRenderer;
	private bool past = false;

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += EnableLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= EnableLight;
	}

	public void EnableLight()
	{
		past = !past;
		//StartCoroutine (ChangeTimeScale());
		shadowRenderer.Enable();
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
