using UnityEngine;
using System.Collections;

public class PlayerNormalState : FSMState
{
	private PlayerInputManager playerInputManager;
	private PlayerController body;


	protected override void Awake()
	{
		ID = (int)PlayerStates.ID.Normal;
		base.Awake ();
	
		playerInputManager = GetComponent<PlayerInputManager> ();
		body               = GetComponent<PlayerController> ();
	}

	public override void Enter ()
	{
		playerInputManager.Move += body.Move;
	}

	public override void Exit ()
	{
		playerInputManager.Move -= body.Move;
	}

	private void GameOver()
	{
		requestStackPop ();
		requestStackPush ((int)PlayerStates.ID.GameOver);
	}
}

