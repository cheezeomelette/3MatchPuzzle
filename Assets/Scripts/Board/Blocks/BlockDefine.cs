using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
	EMPTY = 0,
	BASIC = 1,
}

// 블럭 종류
public enum BlockBreed
{
	NA		= -1,
	BREED_0	= 0,
	BREED_1	= 1,
	BREED_2	= 2,
	BREED_3	= 3,
	BREED_4	= 4,
}

// 블럭 매칭 상태
public enum BlockStatus
{
	NORMAL,					// 평상시
	MATCH,					// 매칭 상태
	CLEAR					// 클리어 예정
}


static class BlockMethod
{
	public static bool IsSafeEqual(this Block block, Block targetBlock)
	{
		if(block == null)
		{
			return false;
		}

		return block.IsEqual(targetBlock);
	}
}