using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageReader
{
	// �������� ������ �ҷ����� �Լ�
    public static StageInfo LoadStage(int nStage)
	{
		Debug.Log($"Load Stage : Stage/{GetFileName(nStage)}");

		// Json������ �ؽ�Ʈ ���Ϸ� ����Ǿ� �ִ� �������� ������ �����´�
		TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(nStage)}");
		if(textAsset != null)
		{
			// Json������ StageInfo Ŭ������ ��ȯ���ش�.
			StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

			Debug.Assert(stageInfo.DoValidation());

			return stageInfo;
		}

		return null;
	}

	// �����̸��� �������� �Լ�
	static string GetFileName(int nStage)
	{
		return string.Format("stage_{0:D4}", nStage);
	}
}
