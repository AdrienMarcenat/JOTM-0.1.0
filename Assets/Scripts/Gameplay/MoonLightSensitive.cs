using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonLightSensitive : MonoBehaviour 
{
	[SerializeField] private bool castShadow = true;

	private GameObject presentVersion;
	private GameObject pastVersion;
	private FSM presentFSM;
	private FSM pastFSM;

	private bool isInMoonLight = false;
	private bool isInThePast = false;
	private int presentLayer;

	void Awake()
	{
		presentVersion = transform.parent.Find ("PresentVersion").gameObject;
		pastVersion = transform.parent.Find ("PastVersion").gameObject;

		presentFSM = presentVersion.GetComponent<FSM> ();
		pastFSM = pastVersion.GetComponent<FSM> ();
	}

	void Start()
	{
		pastVersion.SetActive (false);
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
		// Move the present in background, it will still be simulated but not rendered
		presentLayer = presentVersion.layer;
		presentVersion.layer = 10;
		// Override the present states with the past ones
		presentFSM.CopyState(pastFSM);
		pastVersion.SetActive (true);
	}

	private void OnMoonLightExit()
	{
		isInThePast = false;
		// Restore the present's layer
		presentVersion.layer = presentLayer;
		presentVersion.transform.position = pastVersion.transform.position;
		pastVersion.SetActive (false);
	}

	void LateUpdate () 
	{
		if (isInMoonLight && !isInThePast)
			OnMoonLightEnter ();
		else if (!isInMoonLight && isInThePast)
			OnMoonLightExit ();
		isInMoonLight = false;

		if (isInThePast)
			transform.position = pastVersion.transform.position;
		else
			transform.position = presentVersion.transform.position;
	}

	public void OnMoonlight()
	{
		this.isInMoonLight = true;
	}

	public bool GetCastShadow()
	{
		return castShadow;
	}
}
