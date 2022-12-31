using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder : MonoBehaviour
{
	StageInfo mStageInfo;
    int mStage;
	int energyCount;

    public StageBuilder(int nStage)
	{
		mStage = nStage;
	}

	Block SpawnBlockForStage(int nRow, int nCol)
	{
		if (mStageInfo.GetCellType(nRow, nCol) == CellType.EMPTY)
			return SpawnEmptyBlock();
		return SpawnBlock();
	}

	public Block SpawnBlock()
	{
		return BlockFactory.SpawnBlock(BlockType.BASIC);
	}

	public Block SpawnEmptyBlock()
	{
		return BlockFactory.SpawnBlock(BlockType.EMPTY);
	}

	Cell SpawnCellForStage(int nRow, int nCol)
	{
		Debug.Assert(mStageInfo != null);
		Debug.Assert(nRow < mStageInfo.row && nCol < mStageInfo.col);

		return CellFactory.SpawnCell(mStageInfo, nRow, nCol);
	}

	public Stage ComposeStage()
	{
		Debug.Assert(mStage > 0, $"Invalid Stage{mStage}");

		mStageInfo = LoadStage(mStage);
		
		Stage stage = new Stage(this, mStageInfo.row, mStageInfo.col, mStageInfo.movingEnergy, mStageInfo.goalScore);

		for (int nRow = 0; nRow < mStageInfo.row; nRow++)
		{
			for(int nCol = 0; nCol < mStageInfo.col; nCol++)
			{
				stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
				stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
			}
		}
		return stage;
	}

	public StageInfo LoadStage(int nStage)
	{
		StageInfo stageInfo = StageReader.LoadStage(nStage);
		if (stageInfo != null)
			Debug.Log(stageInfo.ToString());

		return stageInfo;
	}
	public static Stage BuildStage(int nStage)
	{
		StageBuilder stageBuilder = new StageBuilder(nStage);
		Stage stage = stageBuilder.ComposeStage();

		return stage;
	}
}
