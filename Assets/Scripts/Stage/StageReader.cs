using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageReader
{
	// 스테이지 정보를 불러오는 함수
    public static StageInfo LoadStage(int nStage)
	{
		Debug.Log($"Load Stage : Stage/{GetFileName(nStage)}");

		// Json형태의 텍스트 파일로 저장되어 있는 스테이지 정보를 가져온다
		TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(nStage)}");
		if(textAsset != null)
		{
			// Json파일을 StageInfo 클래스로 변환해준다.
			StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

			Debug.Assert(stageInfo.DoValidation());

			return stageInfo;
		}

		return null;
	}

	// 파일이름을 가져오는 함수
	static string GetFileName(int nStage)
	{
		return string.Format("stage_{0:D4}", nStage);
	}
}
