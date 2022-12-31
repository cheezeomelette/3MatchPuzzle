using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultPanel : MonoBehaviour
{
	[SerializeField] TMP_Text clearText;
	[SerializeField] TMP_Text scoreText;
	[SerializeField] GameObject successFrame;
	[SerializeField] GameObject failFrame;
	[SerializeField] GameObject[] starList;

	private void Start()
	{
		foreach (GameObject star in starList)
			star.gameObject.SetActive(false);
	}
	public void LobbyButton()
	{
		SceneManager.LoadScene("StageSelectScene");
	}
	public void RetryButton()
	{
		SceneManager.LoadScene("PlayScene");
	}

    public void ShowSuccessResult(int score, int goalScore)
	{
		successFrame.SetActive(true);
		clearText.text = "Stage Clear";
		SoundManager.Instance.Play("successClip");
		StartCoroutine(ResultProcess(score, goalScore));
	}
    public void ShowFailResult(int score)
	{
		failFrame.SetActive(true);
		clearText.text = "StageFail";
		scoreText.text = score.ToString();
		SoundManager.Instance.Play("failClip");
	}

    IEnumerator ResultProcess(int score, int goalScore)
	{
		yield return ScoreIncreaser(score, goalScore, Constants.SCORE_DURATION_TIME);
		yield return null;

	}

	IEnumerator ScoreIncreaser(float score, float goalScore, float takenTime)
	{
		float currentScore = 0;
		float targetScore = goalScore / 3f;
		int starIndex = 0;

		while(currentScore < score)
		{
			currentScore += score / takenTime * Time.deltaTime;
			scoreText.text = ((int)currentScore).ToString();
			if(currentScore > targetScore && starIndex < starList.Length)
			{
				Debug.Log("in");
				starList[starIndex].gameObject.SetActive(true);
				targetScore += goalScore / 3f;
				starIndex++;
			}
			yield return null;
		}
		scoreText.text = ((int)score).ToString();

		yield break;
	}
}
