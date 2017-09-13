using UnityEngine;
using System.Collections;

public class MoonlightSprite : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void OnEnable()
	{
		PlayerInputManager.OnMoonLight += OnMoonLight;
	}

	void OnDisable()
	{
		PlayerInputManager.OnMoonLight -= OnMoonLight;
	}

	private void OnMoonLight(bool enable)
	{
		spriteRenderer.enabled = !spriteRenderer.enabled;
	}
}

