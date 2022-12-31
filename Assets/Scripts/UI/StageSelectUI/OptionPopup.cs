using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPopup : MonoBehaviour
{
    [SerializeField] Toggle bgmToggle;
	[SerializeField] Toggle effectToggle;

	private void Start()
	{
		InitSoundToggle();
	}

	private void InitSoundToggle()
	{
		SoundManager.Instance.GetAudioSourcesEnabled(out bool isBgmOn, out bool isEffectOn);
		bgmToggle.isOn = isBgmOn;
		effectToggle.isOn = isEffectOn;
	}

	public void SetSoundEnabled()
	{
		
	}
}
