using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public int maxRow { get { return mBoard.maxRow; } }
    public int maxCol { get { return mBoard.maxRow; } }
    public int mMovingEnergyCount;
    public int mGoalScore;

    Board mBoard;
    public Board board { get { return mBoard; } }

    StageBuilder mStageBuilder;
    UIController mUIController;

    public Block[,] blocks { get { return mBoard.blocks; } }
    public Cell[,] cells { get { return mBoard.cells; } }

    public Stage(StageBuilder stageBuilder, int nRow, int nCol, int energyCount, int goalScore)
	{
        mStageBuilder = stageBuilder;
        mMovingEnergyCount = energyCount;
        mGoalScore = goalScore;

        mBoard = new Board(nRow, nCol);
	}

    internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container, UIController uiController)
	{
        mUIController = uiController;
        
        mBoard.ComposeStage(cellPrefab, blockPrefab, container, mStageBuilder, uiController, mGoalScore);
        mUIController.UpdateEnergyCountText(mMovingEnergyCount);
	}

    public bool IsOnValideBlock(Vector2 point, out BlockPos blockPos)
	{
		Vector2 pos = new Vector2(point.x + (maxCol / 2.0f), point.y + (maxRow / 2.0f));
        int nRow = (int)pos.y;
		int nCol = (int)pos.x;

        blockPos = new BlockPos(nRow, nCol);

        return board.IsSwipeable(nRow, nCol);
	}
    public bool IsInsideBoard(Vector2 ptOrg)
	{
        Vector2 point = new Vector2(ptOrg.x + (maxCol / 2.0f), ptOrg.y + (maxRow / 2.0f));

        if (point.y < 0 || point.x < 0 || point.y > maxRow || point.x > maxCol)
            return false;

        return true;
    }

    public bool IsValideSwipe(int nRow, int nCol, Swipe swipeDir)
    {
        switch (swipeDir)
        {
            case Swipe.DOWN:    return nRow > 0; ;
            case Swipe.UP:      return nRow < maxRow - 1;
            case Swipe.LEFT:    return nCol > 0;
            case Swipe.RIGHT:   return nCol < maxCol - 1;

            default:            return false;
        }
    }

    public IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir, Returnable<bool> actionResult)
	{
        actionResult.value = false;

        int nSwipeRow = nRow, nSwipeCol = nCol;
        nSwipeRow += swipeDir.GetTargetRow();
        nSwipeCol += swipeDir.GetTargetCol();
		Debug.Assert(nRow != nSwipeRow || nCol != nSwipeCol, $"Invalid Swipe : ({nSwipeRow}, {nSwipeCol})");
        Debug.Assert(nSwipeRow >= 0 && nSwipeRow < maxRow && nSwipeCol >= 0 && nSwipeCol < maxCol, $"Swipe 타겟 블럭 인덱스 오류 = ({nSwipeRow}, {nSwipeCol}) ");

        if(mBoard.IsSwipeable(nSwipeRow, nSwipeCol))
		{
            Block targetBlock = blocks[nSwipeRow, nSwipeCol];
            Block baseBlock = blocks[nRow, nCol];
            Debug.Assert(baseBlock != null && targetBlock != null);

            Vector3 basePos = baseBlock.blockObj.transform.position;
            Vector3 targetPos = targetBlock.blockObj.transform.position;

            if(targetBlock.IsSwipeable(baseBlock))
			{
				baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);
                SoundManager.Instance.Play("swapSound");

                yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                blocks[nRow, nCol] = targetBlock;
                blocks[nSwipeRow, nSwipeCol] = baseBlock;
                mBoard.SetSwipeBlocks(baseBlock, targetBlock);

                actionResult.value = true;
			}
        }
        yield break;
    }

    // 매치 검증 보드에 요청
    public IEnumerator Evaluate(Returnable<bool> matchResult)
	{
        yield return mBoard.Evaluate(matchResult);
	}

    public void ResetSwipeBlock()
	{
        board.ResetSwipeBlocks();

    }
    // 매치가 있을 경우 블럭 삭제 생성 하강
    public IEnumerator PostprocessAfterEvaluate()
	{
        List<KeyValuePair<int, int>> unfilledBlocks = new List<KeyValuePair<int, int>>();
        List<Block> movingBlocks = new List<Block>();

		yield return mBoard.ArrangeBlocksAfterClean(unfilledBlocks, movingBlocks);

        yield return mBoard.SpawnBlocksAfterClean(movingBlocks);

        yield return WaitForDropping(movingBlocks);
	}

    public IEnumerator WaitForDropping(List<Block> movingBlocks)
	{
		WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

        while (true)
        {
            bool bContinue = false;

            for (int i = 0; i < movingBlocks.Count; i++)
			{
                if(movingBlocks[i].isMoving)
				{
                    bContinue = true;
                    break;
				}
			}

            if (!bContinue)
                break;

            yield return waitForSeconds;
		}
        movingBlocks.Clear();
        yield break;
	}

    public void UpdateEnergyUI()
	{
        mUIController.UpdateEnergyCountText(mMovingEnergyCount -= 1);
    }

    public bool IsFinishedGame(out bool isClear)
	{
        isClear = mBoard.goalCellList.Count <= 0;
        return mMovingEnergyCount <= 0 || isClear;
    }

    public bool IsFinishedGame()
	{
        return mMovingEnergyCount <= 0 || mBoard.goalCellList.Count <= 0;
    }

    public void GetBlockInfo(int row, int col)
	{
        //Debug.Log($"QuestType : {blocks[row, col].questType}  Breed : {blocks[row, col].breed}  Status : {blocks[row, col].status}  matchType : {blocks[row, col].type}");

    }
}
