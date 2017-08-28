using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonLightSensitive : MonoBehaviour 
{
	public List<GameObject> PresentVersions;
	public List<GameObject> PastVersions;

	private bool isInMoonLight = false;
	private bool isInThePast = false;
	private int versionIndex = 0;

	private void OnMoonLightEnter()
	{
		isInThePast = true;
		print(gameObject.name + " enters MoonLight");
		PresentVersions [versionIndex].SetActive (false);
		PastVersions [versionIndex].SetActive (true);
	}

	private void OnMoonLightExit()
	{
		isInThePast = false;
		print(gameObject.name + " exits MoonLight");
		PresentVersions [versionIndex].SetActive (true);
		PastVersions [versionIndex].SetActive (false);
	}

	void LateUpdate () 
	{
		if (isInMoonLight && !isInThePast)
			OnMoonLightEnter ();
		else if (!isInMoonLight && isInThePast)
			OnMoonLightExit ();
		isInMoonLight = false;
	}

	public void OnMoonlight()
	{
		this.isInMoonLight = true;
	}
}
