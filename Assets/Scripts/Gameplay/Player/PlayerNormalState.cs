using UnityEngine;
using System.Collections;

public class PlayerNormalState : FSMState
{
	private PlayerInputManager playerInputManager;
	private PlayerController playerController;


	protected override void Awake()
	{
		ID = (int)PlayerStates.ID.Normal;
		base.Awake ();
	
		playerInputManager = GetComponent<PlayerInputManager> ();
		playerController   = GetComponent<PlayerController> ();
	}

	public override void Enter ()
	{
		playerInputManager.Move += playerController.Move;
	}

	public override void Exit ()
	{
		playerInputManager.Move -= playerController.Move;
	}

	private void GameOver()
	{
		requestStackPop ();
		requestStackPush ((int)PlayerStates.ID.GameOver);
	}
}

