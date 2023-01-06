using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockFactory
{
	// 블럭 생성 함수
    public static Block SpawnBlock(BlockType blockType)
	{
		Block block = new Block(blockType);

		// 블럭타입이 기본형이면 블럭의 종류를 랜덤으로 생성한다.
		if (blockType == BlockType.BASIC)
		{
			block.breed = (BlockBreed)Random.Range(0, 5);
			Debug.Assert((int)block.breed <= 4, $"error breed{block.breed}");
		}
		// 빈블럭이면 종류를 설정하지 않는다
		else if (blockType == BlockType.EMPTY)
			block.breed = BlockBreed.NA;

		return block;
	}
}
