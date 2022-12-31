using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockScoreUI : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;

	private void Start()
	{
		scoreText = GetComponent<TMP_Text>();
	}

	public void SetScore(Vector2 position, int score)
	{
		transform.position = position;
		scoreText.text = score.ToString();
		StartCoroutine(MoveUp());
	}

	IEnumerator MoveUp()
	{
		float currenttime = 0f;
		while(currenttime < 1f)
		{
			transform.position += Vector3.up * 100 * Time.deltaTime;
			currenttime += Time.deltaTime;
			yield return null;
		}
		ScorePool.Instance.ReturnObject(this);
	}
}
