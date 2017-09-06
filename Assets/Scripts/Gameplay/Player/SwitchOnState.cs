using UnityEngine;
using System.Collections;

public class SwitchOnState : FSMState
{
	private Switch thisSwitch;

	protected override void Awake()
	{
		ID = (int)SwitchState.ID.On;
		base.Awake ();

		thisSwitch = GetComponent<Switch> ();
	}

	public override void Enter ()
	{
		thisSwitch.SwitchEvent += TurnOff;
	}

	public override void Exit ()
	{
		thisSwitch.SwitchEvent -= TurnOff;
	}

	private void TurnOff()
	{
		requestStackPop ();
		requestStackPush ((int)SwitchState.ID.Off);
	}
}

