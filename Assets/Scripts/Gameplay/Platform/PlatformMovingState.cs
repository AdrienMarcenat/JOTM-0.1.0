using UnityEngine;
using System.Collections;

public class PlatformMovingState : FSMState
{
	[SerializeField] private Switch platformSwitch;
	[SerializeField] private Transform[] path;
	[SerializeField] private float speed;
	private int pathIndex = 0;
	private int direction = 1;
	private Rigidbody2D body;
	private Vector2 directionVector;
	private float goalDistance;
	private float traveledDistance;

	protected override void Awake()
	{
		ID = (int)PlatformStates.ID.Moving;
		body = GetComponent<Rigidbody2D> ();
		directionVector = Vector2.zero;
		base.Awake ();
	}

	public override void Enter ()
	{
		platformSwitch.SwitchEvent += ChangeDirection;
		traveledDistance = 0;
		goalDistance = 0;
	}

	public override void FixedUpdateState ()
	{
		if (traveledDistance >= goalDistance)
			UpdateDirectionAndGoal ();

		body.velocity = speed * directionVector;
		traveledDistance += speed * Time.fixedDeltaTime;
	}

	public override void Exit ()
	{
		body.velocity = Vector2.zero;
		platformSwitch.SwitchEvent -= ChangeDirection;
	}

	private void ChangeDirection()
	{
		direction *= -1;
		UpdateDirectionAndGoal ();
	}

	private void UpdateDirectionAndGoal()
	{
		pathIndex += direction;
		if (pathIndex == -1 || pathIndex == path.Length)
			StopMoving ();
		else
		{
			directionVector = (path [pathIndex].position - transform.position);
			goalDistance = directionVector.magnitude;
			traveledDistance = 0;
			directionVector.Normalize ();
		}
	}

	private void StopMoving()
	{
		ChangeDirection ();
		requestStackPop ();
		requestStackPush ((int)PlatformStates.ID.Idle);
	}

	public int GetDirection()
	{
		return direction;
	}

	public int GetPathIndex()
	{
		return pathIndex;
	}

	public override void Copy(FSMState state)
	{
		PlatformMovingState newState = (PlatformMovingState) state;
		direction = newState.GetDirection ();
		pathIndex = newState.GetPathIndex ();
		transform.position = state.gameObject.transform.position;
	}
}

