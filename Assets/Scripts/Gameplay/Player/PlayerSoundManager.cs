using UnityEngine;
using System.Collections;

public class PlayerSoundManager : MonoBehaviour
{
	private PlayerInputManager playerInputManager;

	void Awake()
	{
		playerInputManager = GetComponent<PlayerInputManager>();
	}

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		
	}
}

