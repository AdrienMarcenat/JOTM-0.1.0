using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;

public class MoonLightManager : MonoBehaviour 
{
	[SerializeField] private GameObject moonlightBakerPrefab;
	[SerializeField] private Vector3 step;
	[SerializeField] private float moonlightBakerNumber;
	private MoonlightBaker[] moonlightBakers;
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
		for (int i = 0; i < moonlightBakerNumber; i++)
		{
			Instantiate (moonlightBakerPrefab, transform.position + i * step, new Quaternion (0, 0, 0, 0), transform);
			if(i != 0)
				Instantiate (moonlightBakerPrefab, transform.position - i * step, new Quaternion (0, 0, 0, 0), transform);
		}
		moonlightBakers = GetComponentsInChildren<MoonlightBaker>();
	}

	public void EnableLight()
	{
		past = !past;
		StartCoroutine (ChangeTimeScale());
		foreach (MoonlightBaker baker in moonlightBakers)
			baker.Enable();
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
