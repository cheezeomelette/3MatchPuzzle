using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyStage : MonoBehaviour
{
	[SerializeField] GameObject[] stars;
	[SerializeField] GameObject stageObject;

	public int stage;

	private void Start()
	{
		stageObject.SetActive(false);
	}

	public void InitStage(int starCount)
	{
		for(int i = 0; i < starCount; i++)
		{
			stars[i].SetActive(true);
		}
	}
	public void SetIcon()
	{
		stageObject.SetActive(true);
	}

	public IEnumerator PlayClearAnimation(int starCount)
	{
		stageObject.SetActive(false);
		int starIndex = 0;
		while (starIndex < starCount)
		{
			stars[starIndex].SetActive(true);
			SoundManager.Instance.Play("starSound");
			starIndex++;
			yield return new WaitForSeconds(1f);
			Debug.Log(starIndex);
		}
	}

	public IEnumerator AppearNextStage()
	{
		stageObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
	}
}
