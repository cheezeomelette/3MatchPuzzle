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
		// ����ִ� ���̸��� �� ���� �����ϰ� �׿ܿ��� �⺻ ���� �����Ѵ�.
		if (mStageInfo.GetCellType(nRow, nCol) == CellType.EMPTY)
			return SpawnEmptyBlock();
		return SpawnBlock();
	}

	Cell SpawnCellForStage(int nRow, int nCol)
	{
		// ���� �����Ѵ�.
		return CellFactory.SpawnCell(mStageInfo, nRow, nCol);
	}

	// �������� ���� �Լ�
	public Stage ComposeStage()
	{
		// ��ȿ���� ���� ���������� ������ ���
		Debug.Assert(mStage > 0, $"Invalid Stage{mStage}");

		// Json���·� ����� �������� ������ �ε��Ѵ�.
		mStageInfo = LoadStage(mStage);
		
		// �������� ����
		Stage stage = new Stage(this, mStageInfo.row, mStageInfo.col, mStageInfo.movingEnergy, mStageInfo.goalScore);

		// �������� ������ �������� ��ϰ� ���� �����Ѵ�.
		for (int nRow = 0; nRow < mStageInfo.row; nRow++)
		{
			for(int nCol = 0; nCol < mStageInfo.col; nCol++)
			{
				// ����ִ� ���� �� ���� �����ϰ� �� �ܿ��� �⺻ ���� �����Ѵ�
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
