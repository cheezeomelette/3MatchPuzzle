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

	public Block SpawnBlock()
	{
		return BlockFactory.SpawnBlock(BlockType.BASIC);
	}

	public Block SpawnEmptyBlock()
	{
		return BlockFactory.SpawnBlock(BlockType.EMPTY);
	}

	Block SpawnBlockForStage(int nRow, int nCol)
	{
		// 비어있는 셀이리면 빈 블럭을 생성하고 그외에는 기본 블럭을 생성한다.
		if (mStageInfo.GetCellType(nRow, nCol) == CellType.EMPTY)
			return SpawnEmptyBlock();
		return SpawnBlock();
	}

	Cell SpawnCellForStage(int nRow, int nCol)
	{
		// 셀을 생성한다.
		return CellFactory.SpawnCell(mStageInfo, nRow, nCol);
	}

	// 스테이지 생성 함수
	public Stage ComposeStage()
	{
		// 유효하지 않은 스테이지는 오류로 출력
		Debug.Assert(mStage > 0, $"Invalid Stage{mStage}");

		// Json형태로 저장된 스테이지 정보를 로드한다.
		mStageInfo = LoadStage(mStage);
		
		// 스테이지 생성
		Stage stage = new Stage(this, mStageInfo.row, mStageInfo.col, mStageInfo.movingEnergy, mStageInfo.goalScore);

		// 스테이지 정보를 바탕으로 블록과 셀을 생성한다.
		for (int nRow = 0; nRow < mStageInfo.row; nRow++)
		{
			for(int nCol = 0; nCol < mStageInfo.col; nCol++)
			{
				// 비어있는 셀은 빈 블럭을 생성하고 그 외에는 기본 블럭을 생성한다
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
