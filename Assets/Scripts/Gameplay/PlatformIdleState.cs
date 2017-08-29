using UnityEngine;
using System.Collections;

public class PlatformIdleState : FSMState
{
	[SerializeField] private Switch platformSwitch;

	protected override void Awake()
	{
		ID = (int)PlatformStates.ID.Idle;
		base.Awake ();
	}

	public override void Enter ()
	{
		platformSwitch.SwitchEvent += Move;
	}

	public override void Exit ()
	{
		platformSwitch.SwitchEvent -= Move;
	}

	private void Move()
	{
		print ("move");
		requestStackPop ();
		requestStackPush ((int)PlatformStates.ID.Moving);
	}
}

