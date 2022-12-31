using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    Transform mContainer;
    Stage mStage;
    MonoBehaviour mMonoBehaviour;
    UIController mUIController;

	System.Action<bool> callback;

    bool mRunning;
    bool isFinished;
    bool isClear;

    public ActionManager(Transform container, Stage stage, UIController uiController, System.Action<bool> callback)
	{
        mContainer = container;
        mStage = stage;
        mUIController = uiController;
        this.callback = callback;

        mMonoBehaviour = container.GetComponent<MonoBehaviour>();
	}

    public Coroutine StartCoroutine(IEnumerator routine)
	{
        return mMonoBehaviour.StartCoroutine(routine);
	}

    public void DoSwipeAction(int nRow, int nCol, Swipe swipeDir)
	{
        Debug.Assert(nRow >= 0 && nRow < mStage.maxRow && nCol >= 0 && nCol < mStage.maxCol);
        if(mStage.IsValideSwipe(nRow,nCol,swipeDir))
		{
            StartCoroutine(CoDoSwipeAction(nRow, nCol, swipeDir));
		}
	}

    // 스와이프 처리
    IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir)
	{
        if(!mRunning)
		{
            mRunning = true;

            Returnable<bool> bSwipeBlock = new Returnable<bool>(false);
            // 블럭 이동
            yield return mStage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipeBlock);

            // 스와이프 완료되면 실행, 매치된 블럭 평가
            if(bSwipeBlock.value)
			{
                Returnable<bool> bMatchBlock = new Returnable<bool>(false);
                yield return EvaluateBoard(bMatchBlock);

                // 매치된 블럭이 없으면 원상복귀
                if (!bMatchBlock.value)
                {
                    yield return mStage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipeBlock);
                }

			}

            mRunning = false;
        }
        yield break;
    }

    // 매치 검증
    IEnumerator EvaluateBoard(Returnable<bool> matchResult)
    {
        // 블럭 매칭이 없을 때 까지 반복
        bool bFirstMatch = false;
        while (true)
        {
            Returnable<bool> bBlockMatched = new Returnable<bool>(false);
            // 매칭 확인
            yield return StartCoroutine(mStage.Evaluate(bBlockMatched));
            mStage.ResetSwipeBlock();

            // 매치된 블럭이 있을 경우
            if (bBlockMatched.value)
            {
                matchResult.value = true;
                if(!bFirstMatch)
				{
                    bFirstMatch = true;
                    mStage.UpdateEnergyUI();
                }

                yield return mStage.PostprocessAfterEvaluate();
            }
            else
                break;
        }

        // 클리어 확인
        isFinished = mStage.IsFinishedGame(out isClear);
        if (isClear)
        {
            // 클리어 했을 때
            callback?.Invoke(isClear);
            SoundManager.Instance.StopBgm();
        }
        else if (isFinished)
        {
            callback.Invoke(isClear);
            SoundManager.Instance.StopBgm();
        }

        yield break;
    }
}
