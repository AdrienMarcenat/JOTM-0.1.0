﻿using System.Collections;
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
	private int presentLayer;

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
		presentLayer = presentVersion.layer;
		presentVersion.layer = 10;
		pastVersion.SetActive (true);
	}

	private void OnMoonLightExit()
	{
		isInThePast = false;
		print(gameObject.name + " exits MoonLight");
		presentVersion.layer = presentLayer;
		pastVersion.SetActive (false);
	}

	void Update()
	{
		if (isInThePast)
			presentVersion.transform.position = pastVersion.transform.position;
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
