using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOptionToggle : MonoBehaviour
{
	[SerializeField] Image toggleImage;
	[SerializeField] Sprite onSprite;
	[SerializeField] Sprite offSprite;
	[SerializeField] Sound type;

	public void OnValueChanged(bool isOn)
	{
		toggleImage.sprite = isOn ? onSprite : offSprite;
		SoundManager.Instance.SetAudioEnabled(isOn, type);
	}
}
