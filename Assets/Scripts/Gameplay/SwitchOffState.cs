using UnityEngine;
using System.Collections;

public class SwitchOffState : FSMState
{
	private Switch thisSwitch;

	protected override void Awake()
	{
		ID = (int)SwitchState.ID.Off;
		base.Awake ();

		thisSwitch = GetComponent<Switch> ();
	}

	public override void Enter ()
	{
		thisSwitch.SwitchEvent += TurnOn;
	}

	public override void Exit ()
	{
		thisSwitch.SwitchEvent -= TurnOn;
	}

	private void TurnOn()
	{
		requestStackPop ();
		requestStackPush ((int)SwitchState.ID.On);
	}
}

