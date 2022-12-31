using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ProgressData", menuName ="Data/ProgressData")]
public class ProgressData : ScriptableObject
{
    public int currentStage = 1;            // 현재 진행중인 스테이지
    public bool bPlayClearAnimation;    // 클리어한 이후 로비에서 클리어 애니메이션을 했는지

    public int lastClearStage;          // 플레이어가 가장 멀리 도달한 스테이지
    public List<StageData> stageDatas;  // 스테이지 데이터 배열


    public void ClearStage(int starCount)
	{
        bPlayClearAnimation = false;

        StageData stage = FindStage(currentStage);
        if (stage == null)
		{
            stage = new StageData(currentStage);
            stageDatas.Add(stage);
		}

		lastClearStage = Mathf.Max(lastClearStage, currentStage);
        stage.ClearStage(starCount);
	}

    public int GetStageStarCount(int stage)
	{
        StageData data = FindStage(stage);

        return data != null ? data.starCount : 0;
    }

    private StageData FindStage(int stage)
    {
        foreach (StageData stageData in stageDatas)
        {
            if (stageData.stage == stage)
                return stageData;
        }
        return null;
    }
}

[System.Serializable]
public class StageData
{
    public int stage;
    public int starCount;
    public bool isClear => starCount > 0;

    public StageData(int stage)
	{
        this.stage = stage;
        starCount = 0;
	}

    public void ClearStage(int newStarCount)
	{
        starCount = Mathf.Max(starCount, newStarCount);
    }
}
