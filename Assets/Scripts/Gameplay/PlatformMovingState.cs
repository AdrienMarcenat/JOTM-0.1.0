using UnityEngine;
using System.Collections;

public class PlatformMovingState : FSMState
{
	[SerializeField] private Switch platformSwitch;
	[SerializeField] private Transform[] path;
	[SerializeField] private float speed;
	private Transform player;
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

	public override void Copy(FSMState state)
	{
		PlatformMovingState newState = (PlatformMovingState) state;
		direction = newState.GetDirection ();
		pathIndex = newState.GetPathIndex ();
	}

	private void ChangeDirection()
	{
		direction *= -1;
		pathIndex += direction;
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

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			player = other.transform;
			other.transform.SetParent (transform);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
			player.parent = null;
	}

	void OnDisable()
	{
		if(player != null)
			player.parent = null;
	}
}

