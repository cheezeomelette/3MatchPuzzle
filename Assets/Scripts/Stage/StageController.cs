using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이지 최초생성 및 입력처리
public class StageController : MonoBehaviour
{
	[SerializeField] Transform mContainer;
	[SerializeField] GameObject mCellPrefab;
	[SerializeField] GameObject mBlockPrefab;
	[SerializeField] UIController mUIController;
	[SerializeField] ProgressData mProgressData;

	int score => mStage.board.score;
	int goalScore;

    bool mInit; // 스테이지 초기화 했는지
	bool isFinished;

	Stage mStage;
	InputManager mInputManager;
	ActionManager mActionManager;

	bool mTouchDown;
	BlockPos mBlockDownPos;
	Vector3 mClickPos;

	public System.Action<bool> callback;

	private void Start()
	{
		callback = FinishGame;
		int stage = mProgressData.currentStage;

		if (stage <= 0)
			InitStage(1);
		else if (stage > 0)
			InitStage(stage);
		SoundManager.Instance.Play("inGameSound", Sound.BGM);
	}

	void InitStage(int stage)
	{
		if (mInit)
			return;

		mInit = true;
		mInputManager = new InputManager(mContainer);

		BuildStage(stage);
	}

	private void Update()
	{
		if (!mInit)
			return;
		OnInputHandler();
	}

	void BuildStage(int stage)
	{
		// 스테이지 구축
		mStage = StageBuilder.BuildStage(stage);
		mActionManager = new ActionManager(mContainer, mStage, mUIController, callback);

		mStage.ComposeStage(mCellPrefab, mBlockPrefab, mContainer, mUIController);
		goalScore = mStage.mGoalScore;
	}

	void OnInputHandler()
	{
		// 게임이 종료되었으면 입력받지 않는다
		if (isFinished)
			return;
		if(mInputManager.isTouchDown)
		{
			Vector2 point = mInputManager.touch2BoardPosition;

			if (!mStage.IsInsideBoard(point))
			{
				Debug.Log("outside");
				mBlockDownPos = new BlockPos(-1, -1);
				return;
			}

			BlockPos blockPos;

			if (mStage.IsOnValideBlock(point, out blockPos))
			{
				mTouchDown = true;
				mBlockDownPos = blockPos;
				mClickPos = point;
			}
		}
		else if(mInputManager.isTouchUp)
		{
			Vector2 point = mInputManager.touch2BoardPosition;

			Swipe swipeDir = mInputManager.EvalSwipeDir(mClickPos, point);

			Debug.Log($"Swipe : {swipeDir}, Blick = {mBlockDownPos}");
			mStage.GetBlockInfo(mBlockDownPos.row, mBlockDownPos.col);	// 디버깅

			if (swipeDir != Swipe.NA && mBlockDownPos.IsValidPos())
				mActionManager.DoSwipeAction(mBlockDownPos.row, mBlockDownPos.col, swipeDir);

			mTouchDown = false;
		}

		isFinished = mStage.IsFinishedGame();
	}

	private void FinishGame(bool isClear)
	{
		// 클리어 했을 경우
		if (isClear)
		{
			mProgressData.ClearStage((int)Mathf.Clamp(score/(goalScore/3f), 0, 3));
			mUIController.ClearGame(score, goalScore);
		}
		// 실패한 경우
		else
		{
			mUIController.FailGame(score);

		}
	}
}
