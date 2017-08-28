using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class MoonLightManager : MonoBehaviour 
{
	[SerializeField] private List<MoonLightRayCaster> moonlightRayCasters;


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
		foreach (MoonLightRayCaster rayCaster in moonlightRayCasters)
			rayCaster.Enable();
	}
}
