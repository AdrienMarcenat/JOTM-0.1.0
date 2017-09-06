using UnityEngine;
using System.Collections;

public class KeyUnholdState : FSMState
{
	private KeyTrigger keyTrigger;

	protected override void Awake()
	{
		ID = (int)KeyStates.ID.Unhold;
		base.Awake ();

		keyTrigger = GetComponentInChildren<KeyTrigger> ();
	}

	public override bool UpdateState ()
	{
		if (Input.GetButtonDown ("Action"))
			Hold ();

		return false;
	}

	private void Hold()
	{
		if (keyTrigger.IsPlayerInTrigger())
		{
			requestStackPop ();
			requestStackPush ((int)KeyStates.ID.Hold);
		}
	}
}

