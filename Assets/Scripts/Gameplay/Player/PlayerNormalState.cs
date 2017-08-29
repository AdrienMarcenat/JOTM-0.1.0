using UnityEngine;
using System.Collections;

public class PlayerNormalState : FSMState
{
	private PlayerInputManager playerInputManager;
	private PlayerController playerController;
	private Rigidbody2D body;

	protected override void Awake()
	{
		ID = (int)PlayerStates.ID.Normal;
		base.Awake ();
	
		playerInputManager = GetComponent<PlayerInputManager> ();
		playerController   = GetComponent<PlayerController> ();
		body               = GetComponent<Rigidbody2D> ();
	}

	public override void Enter ()
	{
		playerInputManager.Move += playerController.Move;
		PlayerInputManager.OnMoonLight += OnMoonLight;
	}

	public override void Exit ()
	{
		playerInputManager.Move -= playerController.Move;
		PlayerInputManager.OnMoonLight -= OnMoonLight;
	}

	private void OnMoonLight()
	{
		StartCoroutine (StopBody ());
	}

	IEnumerator StopBody()
	{
		body.simulated = false;
		yield return new WaitForSecondsRealtime (0.5f);
		body.simulated = true;
	}

	private void GameOver()
	{
		requestStackPop ();
		requestStackPush ((int)PlayerStates.ID.GameOver);
	}
}

