using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockFactory
{
    public static Block SpawnBlock(BlockType blockType)
	{
		Block block = new Block(blockType);

		if (blockType == BlockType.BASIC)
		{
			block.breed = (BlockBreed)Random.Range(0, 5);
			Debug.Assert((int)block.breed <= 4, $"error breed{block.breed}");
		}
		else if (blockType == BlockType.EMPTY)
			block.breed = BlockBreed.NA;

		return block;
	}
}
