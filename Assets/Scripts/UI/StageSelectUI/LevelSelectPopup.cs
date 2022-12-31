using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectPopup : MonoBehaviour
{
	[SerializeField] TMP_Text levelText;
	[SerializeField] TMP_Text movingCountText;
	[SerializeField] TMP_Text goalCountText;

	public void SetStageInfo(int level)
	{
		levelText.text = $"Stage {level}";

		TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(level)}");
		if (textAsset != null)
		{
			Debug.Log(textAsset.name);
			StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);
			int[] cells = stageInfo.cells;
			int goalCount = 0;
			foreach(int i in cells)
			{
				if (i == (int)CellType.GOAL)
					goalCount += 1;
			}
			movingCountText.text = stageInfo.movingEnergy.ToString();
			goalCountText.text = goalCount.ToString();
		}
	}
	string GetFileName(int nStage)
	{
		return string.Format("stage_{0:D4}", nStage);
	}
}
