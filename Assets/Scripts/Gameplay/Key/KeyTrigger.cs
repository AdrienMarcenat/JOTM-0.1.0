using UnityEngine;
using System.Collections;

public class KeyTrigger : MonoBehaviour
{
	private bool isPlayerInTrigger = false;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
			isPlayerInTrigger = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player")
			isPlayerInTrigger = false;
	}

	public bool IsPlayerInTrigger()
	{
		return isPlayerInTrigger;
	}
}

