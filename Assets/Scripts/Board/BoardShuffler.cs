using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BlockVectorKV = System.Collections.Generic.KeyValuePair<Block, UnityEngine.Vector2Int>;

public class BoardShuffler
{
	Board mBoard;
	bool mLoadingMode;

	SortedList<int, BlockVectorKV> mOrgBlocks = new SortedList<int, BlockVectorKV>();
	IEnumerator<KeyValuePair<int, BlockVectorKV>> mIt;
	Queue<BlockVectorKV> mUnusedBlocks = new Queue<BlockVectorKV>();
	bool mListComplete;

	public BoardShuffler(Board board, bool bLoadingMode)
	{
		mBoard = board;
		mLoadingMode = bLoadingMode;
	}

	public void Shuffle(bool bAnimation = false)
	{
		PrepareDuplicationDatas();

		PrepareShuffleBlocks();

		RunnShuffle(bAnimation);
	}

	BlockVectorKV NextBlock(bool bUseQueue)
	{
		if (bUseQueue && mUnusedBlocks.Count > 0)
			return mUnusedBlocks.Dequeue();

		if (!mListComplete && mIt.MoveNext())
			return mIt.Current.Value;

		mListComplete = true;

		return new BlockVectorKV(null, Vector2Int.zero);
	}

	void PrepareDuplicationDatas()
	{
		for (int nRow = 0; nRow < mBoard.maxRow; nRow++)
		{
			for (int nCol = 0; nCol < mBoard.maxCol; nCol++)
			{
				Block block = mBoard.blocks[nRow, nCol];

				if (block == null)
					continue;

				if (mBoard.CanShuffle(nRow, nCol, mLoadingMode))
					block.ResetDuplicationInfo();

				else
				{
					block.horzDuplicate = 1;
					block.vertDuplicate = 1;

					if (nCol > 0 && !mBoard.CanShuffle(nRow, nCol - 1, mLoadingMode) && mBoard.blocks[nRow, nCol - 1].IsSafeEqual(block))
					{
						block.horzDuplicate = 2;
						mBoard.blocks[nRow, nCol - 1].horzDuplicate = 2;
					}
					if (nRow > 0 && !mBoard.CanShuffle(nRow - 1, nCol, mLoadingMode) && mBoard.blocks[nRow - 1, nCol].IsSafeEqual(block))
					{
						block.vertDuplicate = 2;
						mBoard.blocks[nRow - 1, nCol].vertDuplicate = 2;
					}
				}
			}
		}
	}

	void PrepareShuffleBlocks()
	{
		for (int nRow = 0; nRow < mBoard.maxRow; nRow++)
		{
			for (int nCol = 0; nCol < mBoard.maxCol; nCol++)
			{
				if (!mBoard.CanShuffle(nRow, nCol, mLoadingMode))
					continue;

				while (true)
				{
					int nRandom = UnityEngine.Random.Range(0, 10000);

					if (mOrgBlocks.ContainsKey(nRandom))
						continue;

					mOrgBlocks.Add(nRandom, new BlockVectorKV(mBoard.blocks[nRow, nCol], new Vector2Int(nRow, nCol)));
					break;
				}
			}
		}
		mIt = mOrgBlocks.GetEnumerator();
	}

	void RunnShuffle(bool bAnimation)
	{
		for (int nRow = 0; nRow < mBoard.maxRow; nRow++)
		{
			for (int nCol = 0; nCol < mBoard.maxCol; nCol++)
			{
				if (!mBoard.CanShuffle(nRow, nCol, mLoadingMode))
					continue;

				mBoard.blocks[nRow, nCol] = GetShuffleBlock(nRow, nCol);
			}
		}
	}

	Block GetShuffleBlock(int nRow, int nCol)
	{
		BlockBreed prevBreed = BlockBreed.NA;
		Block firstBlock = null;

		bool bUseQueue = true;
		while (true)
		{
			BlockVectorKV blockInfo = NextBlock(bUseQueue);
			Block block = blockInfo.Key;

			if (block == null)
			{
				blockInfo = NextBlock(true);
				block = blockInfo.Key;
			}

			Debug.Assert(block != null, $"block can't be null : queue count -> {mUnusedBlocks.Count}");

			if (prevBreed == BlockBreed.NA)
				prevBreed = block.breed;

			if (mListComplete)
			{
				if (firstBlock == null)
				{
					firstBlock = block;
				}
				else if (System.Object.ReferenceEquals(firstBlock, block))
				{
					mBoard.ChangeBlock(block, prevBreed);
				}
			}

			Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

			if (vtDup.x > 2 || vtDup.y > 2)
			{
				mUnusedBlocks.Enqueue(blockInfo);
				bUseQueue = mListComplete || !bUseQueue;

				continue;
			}

			block.vertDuplicate = vtDup.y;
			block.horzDuplicate = vtDup.x;
			if (block.blockObj != null)
			{
				float initX = mBoard.CalcInitX(Constants.BLOCK_ORG);
				float initY = mBoard.CalcInitY(Constants.BLOCK_ORG);
				block.Move(initX + nCol, initY + nRow);
			}

			return block;
		}
	}

	Vector2Int CalcDuplications(int nRow, int nCol, Block block)
	{
		int colDup = 1, rowDup = 1;

		if (nCol > 0 && mBoard.blocks[nRow, nCol - 1].IsSafeEqual(block))
			colDup += mBoard.blocks[nRow, nCol - 1].horzDuplicate;

		if (nRow > 0 && mBoard.blocks[nRow - 1, nCol].IsSafeEqual(block))
			rowDup += mBoard.blocks[nRow - 1, nCol].vertDuplicate;

		if (nCol < mBoard.maxCol - 1 && mBoard.blocks[nRow, nCol + 1].IsSafeEqual(block))
		{
			Block rightBlock = mBoard.blocks[nRow, nCol + 1];
			colDup += rightBlock.horzDuplicate;

			if (rightBlock.horzDuplicate == 1)
				rightBlock.horzDuplicate = 2;
		}

		if (nRow < mBoard.maxRow - 1 && mBoard.blocks[nRow + 1, nCol].IsSafeEqual(block))
		{
			Block upperBlock = mBoard.blocks[nRow + 1, nCol];
			rowDup += upperBlock.vertDuplicate;

			if (upperBlock.vertDuplicate == 1)
				upperBlock.vertDuplicate = 2;
		}

		return new Vector2Int(colDup, rowDup);
	}

	
}