using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoonAnimation : MonoBehaviour
{
	[SerializeField] private Image cloudImage;
	[SerializeField] private Image moonImage;
	[SerializeField] float animationSpeed;

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += OnMoonLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= OnMoonLight;
	}

	void OnMoonLight (bool enable)
	{
		if(enable)
			StartCoroutine (FadeIn ());
		else
			StartCoroutine (FadeOut ());
	}

	IEnumerator FadeIn()
	{
		float alpha = 1f;
		Color cloudColor = cloudImage.color;
		while (alpha > 0f)
		{
			alpha -= animationSpeed * Time.deltaTime;
			cloudColor.a = alpha;
			cloudImage.color = cloudColor;
			moonImage.color = Color.Lerp (Color.white, Color.black, alpha);
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		float alpha = 0f;
		Color cloudColor = cloudImage.color;
		while (alpha < 1f)
		{
			alpha += animationSpeed * Time.deltaTime;
			cloudColor.a = alpha;
			cloudImage.color = cloudColor;
			moonImage.color = Color.Lerp (Color.white, Color.black, alpha);
			yield return null;
		}
	}

}

