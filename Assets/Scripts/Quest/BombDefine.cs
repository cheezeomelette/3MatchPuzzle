using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����Ʈ ��
public enum BlockQuestType
{
	NONE = -1,
	CLEAR_SIMPLE = 0,       // ���� ���� ����
	CLEAR_HORZ = 1,         // ������ ����
	CLEAR_VERT = 3,         // ������ ����
	CLEAR_CIRCLE = 7,       // �ֺ� ����
	CLEAR_LAZER = 15,        // ������ �� ����
	CLEAR_HORZ_BUFF = 2,    // ��ȭ ����
	CLEAR_VERT_BUFF = 6,    // ��ȭ ����
	CLEAR_CIRCLE_BUFF = 14,  // ��ȭ ����
	CLEAR_LAZER_BUFF = 30,   // ������ ������ ��ȯ
}

public class BombDefine
{
	Board mBoard;
	int maxRow;
	int maxCol;

	BlockPos[] defaultBomb = { new BlockPos(0, 0) };
	BlockPos[] circleBomb = { 
			  new BlockPos(0, 2)
			, new BlockPos(-1, 1), new BlockPos(0, 1), new BlockPos(1, 1)
			, new BlockPos(-2, 0), new BlockPos(-1, 0), new BlockPos(0, 0), new BlockPos(1, 0), new BlockPos(2, 0)
			, new BlockPos(-1, -1), new BlockPos(0, -1), new BlockPos(1, -1)
			, new BlockPos(0, -2)
	};

	public BombDefine(Board board)
	{
		mBoard = board;
		maxRow = mBoard.maxRow;
		maxCol = mBoard.maxCol;
	}

	// ��ź Ÿ�Կ� ���� ���� ������ �����ϴ� �Լ�
	public BlockPos[] GetBombRange(int row, int col, BlockQuestType questType)
	{
		// ��ź Ÿ�Կ� ���� �ٸ� �Լ��� ȣ���Ѵ�.
		switch (questType)
		{
			case BlockQuestType.CLEAR_CIRCLE:
				return ReturnList(row, col, circleBomb);
			case BlockQuestType.CLEAR_HORZ:
				return GetLineRange(row, col, true);
			case BlockQuestType.CLEAR_VERT:
				return GetLineRange(row, col, false);

			default:
				return ReturnList(row,col,defaultBomb);
		}
	}

	// ������, ������ ���� �Լ�
	private BlockPos[] GetLineRange(int row, int col, bool isHorizon)
	{
		List<BlockPos> explosionVec = new();
		if(isHorizon)
			for (int nCol = 0; nCol < maxCol; nCol++)
				explosionVec.Add(new BlockPos(row, nCol));
		else
			for (int nRow = 0; nRow < maxRow; nRow++)
				explosionVec.Add(new BlockPos(nRow, col));

		return explosionVec.ToArray();
	}
	
	// �̸� ���ǵ� ������ �Է¹޾Ƽ� ���߹����� �����ϴ� �Լ�
	private BlockPos[] ReturnList(int row, int col, BlockPos[] bombRange)
	{
		List<BlockPos> explosionVec = new();

		// �̸� �����ص� ���߹����� ��ź��ġ�� �������� �ٲ��ش�.
		foreach(BlockPos pos in bombRange)
		{
			// ���� ���� ��ġ���� üũ
			if(row + pos.row >= 0 && row + pos.row < mBoard.maxRow && col + pos.col >= 0 && col + pos.col < mBoard.maxCol)
			{
				explosionVec.Add(new BlockPos(row + pos.row, col + pos.col));
			}
		}
		// ���߹��� ����
		return explosionVec.ToArray();
	}
}
