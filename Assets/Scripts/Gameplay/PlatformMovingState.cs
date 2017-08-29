using UnityEngine;
using System.Collections;

public class PlatformMovingState : FSMState
{
	[SerializeField] private Switch platformSwitch;
	[SerializeField] private Transform[] path;
	[SerializeField] private float speed;
	private int pathIndex = 0;
	private int direction = 1;

	protected override void Awake()
	{
		ID = (int)PlatformStates.ID.Moving;
		base.Awake ();
	}

	public override void Enter ()
	{
		platformSwitch.SwitchEvent += ChangeDirection;
	}

	public override bool UpdateState ()
	{
		if (transform.position == path [pathIndex].position)
			pathIndex += direction;

		if (pathIndex == -1 || pathIndex == path.Length)
			StopMoving ();
		else
			transform.position = Vector3.MoveTowards (transform.position, path [pathIndex].position, speed * Time.deltaTime);

		return false;
	}

	public override void Exit ()
	{
		platformSwitch.SwitchEvent -= ChangeDirection;
	}

	private void ChangeDirection()
	{
		direction *= -1;
		pathIndex += direction;
	}

	private void StopMoving()
	{
		print ("StopMoving");
		ChangeDirection ();
		requestStackPop ();
		requestStackPush ((int)PlatformStates.ID.Idle);
	}
}

