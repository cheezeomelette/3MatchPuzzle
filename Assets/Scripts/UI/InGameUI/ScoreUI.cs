using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] Image scoreBar;
	[SerializeField] Image[] starImages;
    [SerializeField] Color starColor;

    int starIndex = 0;
	private void Start()
	{
        scoreText.text = "0";
	}
	public void UpdateScore(int currentScore, int goalScore)
	{
        float scoreRatio = (float)currentScore / goalScore;
        scoreBar.fillAmount = scoreRatio;
        scoreText.text = currentScore.ToString();

        if (starIndex >= starImages.Length)
            return;
        if (scoreRatio >= (starIndex + 1) / 3f)
		{
            starImages[starIndex].color = starColor;
            starIndex++;
		}
	}
}
