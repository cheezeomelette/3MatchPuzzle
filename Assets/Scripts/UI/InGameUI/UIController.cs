using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] TMP_Text targetCountText;
    [SerializeField] TMP_Text energyCountText;
	[SerializeField] ResultPanel resultPanel;
	[SerializeField] ScoreUI scoreUI;

	Camera cam;

	private void Start()
	{
		cam = Camera.main;
		resultPanel.gameObject.SetActive(false);
	}

	public void UpdateTargetCountText(int targetCount)
	{
		targetCountText.text = targetCount.ToString();
	}

	public void UpdateEnergyCountText(int energyCount)
	{
		energyCountText.text = energyCount.ToString();
	}

	public void ClearGame(int score, int goalScore)
	{
		resultPanel.gameObject.SetActive(true);
		resultPanel.ShowSuccessResult(score, goalScore);
		Debug.Log("Clear");
	}
	public void FailGame(int score)
	{
		resultPanel.gameObject.SetActive(true);
		resultPanel.ShowFailResult(score);

		Debug.Log("fail");
	}

	public void SetScore(Vector3 blockPosition, int score)
	{
		Vector2 screenPos = cam.WorldToScreenPoint(blockPosition);
		ScorePool.Instance.GetObject().SetScore(screenPos, score);
	}

	public void UpdateScore(int currentScore, int goalScore)
	{
		scoreUI.UpdateScore(currentScore, goalScore);
	}
}
