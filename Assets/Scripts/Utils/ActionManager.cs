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

    // �������� ó��
    IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir)
	{
        if(!mRunning)
		{
            mRunning = true;

            Returnable<bool> bSwipeBlock = new Returnable<bool>(false);
            // �� �̵�
            yield return mStage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipeBlock);

            // �������� �Ϸ�Ǹ� ����, ��ġ�� �� ��
            if(bSwipeBlock.value)
			{
                Returnable<bool> bMatchBlock = new Returnable<bool>(false);
                yield return EvaluateBoard(bMatchBlock);

                // ��ġ�� ���� ������ ���󺹱�
                if (!bMatchBlock.value)
                {
                    yield return mStage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipeBlock);
                }

			}

            mRunning = false;
        }
        yield break;
    }

    // ��ġ ����
    IEnumerator EvaluateBoard(Returnable<bool> matchResult)
    {
        // �� ��Ī�� ���� �� ���� �ݺ�
        bool bFirstMatch = false;
        while (true)
        {
            Returnable<bool> bBlockMatched = new Returnable<bool>(false);
            // ��Ī Ȯ��
            yield return StartCoroutine(mStage.Evaluate(bBlockMatched));
            mStage.ResetSwipeBlock();

            // ��ġ�� ���� ���� ���
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

        // Ŭ���� Ȯ��
        isFinished = mStage.IsFinishedGame(out isClear);
        if (isClear)
        {
            // Ŭ���� ���� ��
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
