using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 퀘스트 블럭
public enum BlockQuestType
{
	NONE = -1,
	CLEAR_SIMPLE = 0,       // 단일 블럭만 제거
	CLEAR_HORZ = 1,         // 세로줄 제거
	CLEAR_VERT = 3,         // 가로줄 제거
	CLEAR_CIRCLE = 7,       // 주변 폭발
	CLEAR_LAZER = 15,        // 동일한 블럭 제거
	CLEAR_HORZ_BUFF = 2,    // 강화 세로
	CLEAR_VERT_BUFF = 6,    // 강화 가로
	CLEAR_CIRCLE_BUFF = 14,  // 강화 폭발
	CLEAR_LAZER_BUFF = 30,   // 동일한 블럭으로 변환
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

	public BlockPos[] GetBombRange(int row, int col, BlockQuestType questType)
	{
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

	private BlockPos[] ReturnList(int row, int col, BlockPos[] bombRange)
	{
		List<BlockPos> explosionVec = new();
		foreach(BlockPos pos in bombRange)
		{
			// 범위 내의 위치인지 체크
			if(row + pos.row >= 0 && row + pos.row < mBoard.maxRow && col + pos.col >= 0 && col + pos.col < mBoard.maxCol)
			{
				explosionVec.Add(new BlockPos(row + pos.row, col + pos.col));
			}
		}

		return explosionVec.ToArray();
	}
}
