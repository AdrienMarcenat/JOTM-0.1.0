using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class GameManager : Singleton<GameManager> 
{
	public static bool pause;

	public delegate void SimpleEvent();
	public static event SimpleEvent Pause;
	public static event SimpleEvent ChangeScene;

	void Start()
	{

	}

	public static void PauseEvent()
	{
		pause = !pause;
		Time.timeScale = 1.0f - Time.timeScale;
		if (Pause != null)
			Pause ();
	}

	public static void LoadScene(int index)
	{
		if(ChangeScene != null)
			ChangeScene ();
		SceneManager.LoadScene (index);
	}

	void Update ()
	{
		if (Input.GetButtonDown ("Escape")) 
		{
			PauseEvent ();
		}
	}
}
