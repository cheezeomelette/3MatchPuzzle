using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Audio", fileName ="AudioTable")]
public class SoundTable : ScriptableObject
{
	public AudioClip blockSwapSound;
	public AudioClip blockClearSound;
	public AudioClip goalCellClearSound;
	public AudioClip successSound;
	public AudioClip bombClearSound;
	public AudioClip buttonSound;
	public AudioClip starAppearSound;
	public AudioClip backgroundSound;
}
