using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class MoonLightManager : MonoBehaviour 
{
	[SerializeField] private List<MoonLightRayCaster> moonlightRayCasters;
	private bool past = false;

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += EnableLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= EnableLight;
	}

	public MoonLightManager()
	{
		moonlightRayCasters = new List<MoonLightRayCaster> ();
	}

	public void EnableLight()
	{
		if(past)
			StartCoroutine (SpeedTime());
		past = !past;
		foreach (MoonLightRayCaster rayCaster in moonlightRayCasters)
			rayCaster.Enable();
	}

	IEnumerator SpeedTime()
	{
		Time.timeScale = 99;
		yield return new WaitForSeconds (2);
		Time.timeScale = 1;
	}
}
