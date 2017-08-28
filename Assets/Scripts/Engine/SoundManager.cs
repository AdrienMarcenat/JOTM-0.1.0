using UnityEngine;
using System.Collections;

public class SoundManager : Singleton<SoundManager>
{
	[SerializeField] AudioSource efxSource;              
	[SerializeField] AudioSource musicSource;

	static public void PlaySingle(AudioClip clip)
	{
		instance.efxSource.clip = clip;
		instance.efxSource.Play ();
	}
		
	static public void PlayMultiple(AudioClip clip)
	{
		instance.efxSource.PlayOneShot(clip);
	}

	static public void PlayMusic(AudioClip clip)
	{
		if (instance.musicSource.clip != clip) 
		{
			instance.musicSource.clip = clip;
			instance.musicSource.Play ();
			instance.musicSource.loop = true;
		}
	}
}