using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonLightSensitive : MonoBehaviour 
{
	private GameObject presentVersion;
	private GameObject pastVersion;
	private FSM presentFSM;
	private FSM pastFSM;

	private bool isInMoonLight = false;
	private bool isInThePast = false;

	void Awake()
	{
		presentVersion = transform.Find ("PresentVersion").gameObject;
		pastVersion = transform.Find ("PastVersion").gameObject;
		presentFSM = presentVersion.GetComponent<FSM> ();
		pastFSM = pastVersion.GetComponent<FSM> ();
	}

	void OnEnable()
	{
		pastFSM.FSMChange += presentFSM.AddChange;
	}

	void OnDisable()
	{
		pastFSM.FSMChange -= presentFSM.AddChange;
	}

	private void OnMoonLightEnter()
	{
		isInThePast = true;
		print(gameObject.name + " enters MoonLight");
		presentVersion.SetActive (false);
		pastVersion.SetActive (true);
	}

	private void OnMoonLightExit()
	{
		isInThePast = false;
		print(gameObject.name + " exits MoonLight");
		presentVersion.SetActive (true);
		pastVersion.SetActive (false);
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
