using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IntIntKV = System.Collections.Generic.KeyValuePair<int, int>;

public class Board : MonoBehaviour
{
	int mRow;
	int mCol;

	public int maxRow { get { return mRow; } }
	public int maxCol { get { return mCol; } }

	int mScore;
	public int score { get { return mScore; } }
	int mGoalScore;
	string finalClipName = "popSound";

	Cell[,] mCells;
	public Cell[,] cells { get { return mCells; } }

	Block[,] mBlocks;
	public Block[,] blocks { get { return mBlocks; } }

	List<Cell> mGoalCellList;
	public List<Cell> goalCellList { get { return mGoalCellList; } }

	Block mClickBlock;
	Block mSwipeBlock;

	Queue<Block> bombRangeBlocks;

	BoardEnemerator mEnumerator;

	Transform mContainer;
	GameObject mCellPrefab;
	GameObject mBlockPrefab;
	StageBuilder mStageBuilder;
	BombDefine mBombDefine;
	UIController mUIController;

	public Board(int nRow, int nCol)
	{
		mRow = nRow;
		mCol = nCol;

		mCells = new Cell[nRow, nCol];
		mBlocks = new Block[nRow, nCol];

		mEnumerator = new BoardEnemerator(this);
	}

	internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container, StageBuilder stageBuilder, UIController uiController, int goalScore)
	{
		mCellPrefab = cellPrefab;
		mBlockPrefab = blockPrefab;
		mContainer = container;
		mStageBuilder = stageBuilder;
		mUIController = uiController;
		mBombDefine = new(this);
		mGoalCellList = new List<Cell>();
		bombRangeBlocks = new Queue<Block>();
		mGoalScore = goalScore;

		BoardShuffler boardShuffler = new(this, true);
		boardShuffler.Shuffle();

		float initX = CalcInitX(0.5f);
		float initY = CalcInitY(0.5f);
		for (int nRow = 0; nRow < mRow; nRow++)
			for (int nCol = 0; nCol < mCol; nCol++)
			{
				Cell cell = mCells[nRow, nCol]?.InstantiateCellObj(cellPrefab, container);
				cell?.Move(initX + nCol, initY + nRow);
				// 목표 셀 등록
				if (cell.type == CellType.GOAL)
					mGoalCellList.Add(cell);

				Block block = mBlocks[nRow, nCol]?.InstantiateBlockObj(blockPrefab, container, this);
				block?.Move(initX + nCol, initY + nRow);
			}
		uiController.UpdateTargetCountText(mGoalCellList.Count);
	}

	// 블럭 교체
	public void ChangeBlock(Block block, BlockBreed notAllowedBreed)
	{
		BlockBreed genBreed;

		while (true)
		{
			genBreed = (BlockBreed)UnityEngine.Random.Range(0, 6);

			if (notAllowedBreed == genBreed)
				continue;

			break;
		}

		block.breed = genBreed;
	}

	// 매칭 검증
	public IEnumerator Evaluate(Returnable<bool> matchResult)
	{

		if (mClickBlock != null)
		{
			// 레이저를 선택한 경우 레이저 이외의 블럭과 같은 종류의 블럭 전체 삭제
			if (mClickBlock.questType == BlockQuestType.CLEAR_LAZER)
			{
				AddRainbowRange(mClickBlock, mSwipeBlock.breed);
				Debug.Log($"rainbow * breed : {mSwipeBlock.breed}");
			}
			// 레이저를 선택한 경우 레이저 이외의 블럭과 같은 종류의 블럭 전체 삭제
			else if (mSwipeBlock.questType == BlockQuestType.CLEAR_LAZER)
			{
				AddRainbowRange(mSwipeBlock, mClickBlock.breed);
				Debug.Log($"rainbow * breed : {mClickBlock.breed}");
			}

		}


		// 폭탄 조합을 사용했을 경우
		// 가로 || 세로 + 가로 || 세로 => swipeBlock의 위치에서 가로 세로 블럭 클리어
		// 가로 || 세로 + 폭탄 => swipeBlock의 위치에서 가로3줄 세로3줄 클리어
		// 폭탄 + 폭탄 => 주변 4칸 블럭 클리어


		// 모든블럭 매칭 상태 (3매칭 있으면 true)
		bool bMatchBlockFound = UpdateAllBlocksMatchedStatus();

		// 매칭 없으면 false 리턴
		if (bMatchBlockFound == false)
		{
			matchResult.value = false;
			ResetAllBlocks();

			yield break;
		}



		// 블럭 강화
		// 교차블럭검색 이후
		for (int nRow = 0; nRow < maxRow; nRow++)
		{
			for (int nCol = 0; nCol < maxCol; nCol++)
			{
				List<Block> matchedBlockList = new List<Block>();
				Block block = blocks[nRow, nCol];

				// 매칭 처리할 블럭 우선순위 갱신
				block.UpdatePriority();
			}
		}

		// 매칭된 블럭을 담아둘 큐
		Queue<Block> matchedBlockQueue = new Queue<Block>();

		for (int nRow = 0; nRow < maxRow; nRow++)
		{
			for (int nCol = 0; nCol < maxCol; nCol++)
			{
				Block block = blocks[nRow, nCol];

				if (block.priority > 0)
					matchedBlockQueue.Enqueue(block);
			}
		}

		// 매칭 큐 정렬 matchedBlockQueue.sort
		List<Block> matchedBlocks = matchedBlockQueue.OrderByDescending(x => x.priority).ThenByDescending(x => x.isMoved, new PriorityCompare()).ToList();

		while (matchedBlocks.Count > 0)
		{
			// 첫블럭 계산 후 첫블럭 삭제
			Block block = matchedBlocks.First();
			//Debug.Log($"priority : {block.priority},  isMoved : {block.ismoved)
			block.RepresentativeBlockEvaluate();
			matchedBlocks.RemoveAt(0);
		}

		while (bombRangeBlocks.Count > 0)
		{
			Block block = bombRangeBlocks.Dequeue();
			block.DoEvaluation();
		}

		// 클리어 상태 블럭 전부 제거
		List<Block> clearBlocks = new();

		// 클리어 블럭 검색
		for (int nRow = 0; nRow < maxRow; nRow++)
		{
			for (int nCol = 0; nCol < maxCol; nCol++)
			{
				Block block = mBlocks[nRow, nCol];
				if (block != null && block.status == BlockStatus.CLEAR)
				{
					clearBlocks.Add(block);

					mBlocks[nRow, nCol] = null;
				}
			}
		}

		// 모든블럭 상태 초기화
		ResetAllBlocks();

		// 효과음 초기화
		finalClipName = "popSound";
		// 블럭 제거
		foreach (Block block in clearBlocks)
		{
			// 블럭 점수 계산
			mScore += 80;
			mUIController.SetScore(new Vector3(CalcInitX(0.5f) + block.col, CalcInitY(1f) + block.row, 0), 80);
			mUIController.UpdateScore(score, mGoalScore);
			if (block.questType > BlockQuestType.CLEAR_SIMPLE)
				finalClipName = "comboSound2";
			block.Destroy();

			// 블럭을 제거하면서 스테이지 목표 체크
			GoalCheck(block);
		}
		SoundManager.Instance.Play(finalClipName, Sound.EFFECT);

		// 결과 반환
		matchResult.value = true;

		yield break;
	}

	// 모든 블럭매치상태 검사(매칭된 Block의 status를 match로 바꿈)
	// 모든 블럭의 주변 블럭 업데이트
	public bool UpdateAllBlocksMatchedStatus()
	{
		List<Block> matchedBlockList = new List<Block>();

		int nCount = 0; // 매칭 수
		for (int nRow = 0; nRow < maxRow; nRow++)
		{
			for (int nCol = 0; nCol < maxCol; nCol++)
			{
				// 블럭의 위치와 주변블럭 업데이트
				UpdateAroundBlocks(nRow, nCol);

				if (EvalBlocksIfMatched(nRow, nCol, matchedBlockList))
				{
					nCount++;
				}
				else if (blocks[nRow, nCol].status == BlockStatus.MATCH)
					nCount++;
			}
		}

		return nCount > 0;
	}

	// 한 블럭에서 매칭검사
	public bool EvalBlocksIfMatched(int nRow, int nCol, List<Block> matchedBlockList)
	{
		bool bFound = false;

		// 기준 블럭
		Block baseBlock = mBlocks[nRow, nCol];
		if (baseBlock == null)
			return false;

		// 매칭상태면 리턴
		if (baseBlock.match != MatchType.NONE || !baseBlock.IsValidate() || mCells[nRow, nCol].IsObstacle())
			return false;

		matchedBlockList.Add(baseBlock);

		Block block;
		// 가로 검사
		bool isHorizon = true;

		for (int i = nCol + 1; i < maxCol; i++)
		{
			block = mBlocks[nRow, i];
			if (!block.IsSafeEqual(baseBlock))
				break;

			matchedBlockList.Add(block);
		}
		for (int i = nCol - 1; i >= 0; i--)
		{
			block = mBlocks[nRow, i];
			if (!block.IsSafeEqual(baseBlock))
				break;

			matchedBlockList.Insert(0, block);
		}

		if (matchedBlockList.Count >= 3)
		{
			SetBlockStatusMatched(matchedBlockList, isHorizon);
			bFound = true;
		}

		matchedBlockList.Clear();

		// 세로 검사
		isHorizon = false;
		matchedBlockList.Add(baseBlock);

		for (int i = nRow + 1; i < maxCol; i++)
		{
			block = mBlocks[i, nCol];
			if (!block.IsSafeEqual(baseBlock))
				break;

			matchedBlockList.Add(block);
		}

		for (int i = nRow - 1; i >= 0; i--)
		{
			block = mBlocks[i, nCol];
			if (!block.IsSafeEqual(baseBlock))
				break;

			matchedBlockList.Insert(0, block);
		}

		if (matchedBlockList.Count >= 3)
		{
			SetBlockStatusMatched(matchedBlockList, isHorizon);
			bFound = true;
		}

		matchedBlockList.Clear();

		return bFound;
	}


	public void AddBombRangeBlocks(int row, int col, BlockQuestType questType)
	{
		BlockPos[] explosionPositions = mBombDefine.GetBombRange(row, col, questType);

		// 폭발범위 블럭 추가
		foreach (BlockPos explosionPos in explosionPositions)
		{
			Block explosionBlock = mBlocks[explosionPos.row, explosionPos.col];
			Cell explosionCell = mCells[explosionPos.row, explosionPos.col];
			if (explosionCell.IsObstacle())
			{
				Debug.Log("Wrong Cell");
				continue;
			}
			bombRangeBlocks.Enqueue(explosionBlock);
			//Debug.Log($"[{explosionPos.y}, {explosionPos.x}]");
		}
		// 폭발범위의 블럭들 매칭상태로 변환
		foreach (Block block in bombRangeBlocks)
		{

			block.UpdateBlockStatusBombMatched();
		}
	}

	void AddRainbowRange(Block rainbowBlock, BlockBreed blockBreed)
	{
		bombRangeBlocks.Enqueue(rainbowBlock);

		for (int nRow = 0; nRow < maxRow; nRow++)
		{
			for (int nCol = 0; nCol < maxCol; nCol++)
			{
				Block block = mBlocks[nRow, nCol];
				if (block.breed == blockBreed)
					bombRangeBlocks.Enqueue(block);
			}
		}
		// 폭발범위의 블럭들 매칭상태로 변환
		foreach (Block block in bombRangeBlocks)
		{
			block.UpdateBlockStatusBombMatched();
		}
	}

	public void GoalCheck(Block block)
	{
		Cell cell = cells[block.row, block.col];
		if (cell.IsGoalMatched() && mGoalCellList.Contains(cell))
		{
			mGoalCellList.Remove(cell);
			SoundManager.Instance.Play("popSound2");
			mUIController.UpdateTargetCountText(mGoalCellList.Count);
		}
	}

	public IEnumerator ArrangeBlocksAfterClean(List<IntIntKV> unfilledBlocks, List<Block> movingBlocks)
	{
		SortedList<int, int> emptyBlocks = new SortedList<int, int>();
		List<IntIntKV> emptyRemainBlocks = new List<IntIntKV>();

		for (int nCol = 0; nCol < maxCol; nCol++)
		{
			emptyBlocks.Clear();

			for (int nRow = 0; nRow < maxRow; nRow++)
			{
				if (CanBlockBeAllocatable(nRow, nCol))  
					emptyBlocks.Add(nRow, nCol);
			}

			if (emptyBlocks.Count == 0)
				continue;

			IntIntKV first = emptyBlocks.First();

			for (int nRow = first.Key + 1; nRow < maxRow; nRow++)
			{
				Block block = mBlocks[nRow, nCol];

				if (block == null || mCells[nRow, nCol].type == CellType.EMPTY)
					continue;

				block.dropDistance = new Vector2(0, nRow - first.Key);
				movingBlocks.Add(block);

				Debug.Assert(mCells[first.Key, nCol].IsObstacle() == false, $"{mCells[first.Key, nCol]}");
				mBlocks[first.Key, nCol] = block;

				mBlocks[nRow, nCol] = null;

				emptyBlocks.RemoveAt(0);

				emptyBlocks.Add(nRow, nCol);

				first = emptyBlocks.First();
				nRow = first.Key;
			}
		}
		yield return null;

		if (emptyRemainBlocks.Count > 0)
		{
			unfilledBlocks.AddRange(emptyRemainBlocks);
		}

		yield break;
	}

	public IEnumerator SpawnBlocksAfterClean(List<Block> movingBlocks)
	{
		// 열마다 순회
		for (int nCol = 0; nCol < maxCol; nCol++)
		{
			for (int nRow = 0; nRow < maxRow; nRow++)
			{
				// 블럭이 비어있을 때
				if (mBlocks[nRow, nCol] == null)
				{
					int nTopRow = nRow;
					int nSpawnBaseY = 0;
					//빈블럭부터 제일위의 블럭까지
					for (int y = nTopRow; y < maxRow; y++)
					{
						// 할당가능한 블럭이 아니면 넘어가기
						if (mBlocks[y, nCol] != null || !CanBlockBeAllocatable(y, nCol))
							continue;

						// 블럭을 생성하고 떨어뜨려준다
						Block block = SpawnBlockWithDrop(y, nCol, nSpawnBaseY, nCol);
						// 움직이고있는 블럭에 추가해준다
						if (block != null)
							movingBlocks.Add(block);

						nSpawnBaseY++;
					}

					break;
				}
			}
		}
		yield return null;
	}

	// 삭제된 블럭에 새로운블럭 생성후 드랍
	Block SpawnBlockWithDrop(int nRow, int nCol, int nSpawnedRow, int nSpawnedCol)
	{
		float fInitX = CalcInitX(Constants.BLOCK_ORG);
		float fInitY = CalcInitY(Constants.BLOCK_ORG) + maxRow;

		Block block = mStageBuilder.SpawnBlock().InstantiateBlockObj(mBlockPrefab, mContainer, this);
		if (block != null)
		{
			mBlocks[nRow, nCol] = block;
			block.Move(fInitX + (float)nSpawnedCol, fInitY + (float)nSpawnedRow);
			block.dropDistance = new Vector2(nSpawnedCol - nCol, maxRow + (nSpawnedRow - nRow));
		}

		return block;
	}

	// 매치 시 매치된 블록 상태 변경(3개매칭인지 4개매칭인지...)
	void SetBlockStatusMatched(List<Block> blockList, bool isHorizon)
	{
		int nMatchCount = blockList.Count;

		foreach (Block block in blockList)
		{
			block.UpdateBlockStatusMatched(((MatchType)nMatchCount), isHorizon);
		}
	}


	void UpdateAroundBlocks(int row, int col)
	{
		Block block = blocks[row, col];
		Block upBlock = null;
		Block downBlock = null;
		Block leftBlock = null;
		Block rightBlock = null;

		if (row < maxRow - 1 && blocks[row + 1, col].IsMatchableBlock())
			upBlock = blocks[row + 1, col];
		if (row > 0 && blocks[row - 1, col].IsMatchableBlock())
			downBlock = blocks[row - 1, col];
		if (col > 0 && blocks[row, col - 1].IsMatchableBlock())
			leftBlock = blocks[row, col - 1];
		if (col < maxCol - 1 && blocks[row, col + 1].IsMatchableBlock())
			rightBlock = blocks[row, col + 1];
		block.UpdatePositionInfo(row, col, upBlock, downBlock, leftBlock, rightBlock);
	}

	void ResetAllBlocks()
	{
		for (int nRow = 0; nRow < maxRow; nRow++)
		{
			for (int nCol = 0; nCol < maxCol; nCol++)
			{
				Block block = mBlocks[nRow, nCol];
				if (block == null)
					continue;
				// 모든블럭 상태 초기화
				block.ResetBeforeMove();
			}
		}
	}

	public bool IsSwipeable(int nRow, int nCol)
	{
		return mCells[nRow, nCol].type.IsMovableType();
	}
	public float CalcInitX(float offset = 0)
	{
		return -mCol / 2f + offset;
	}

	public float CalcInitY(float offset = 0)
	{
		return -mRow / 2f + offset;
	}

	public bool CanShuffle(int nRow, int nCol, bool bLoading)
	{
		if (!mCells[nRow, nCol].type.IsBlockAllocatableType())
			return false;

		return true;
	}

	bool CanBlockBeAllocatable(int nRow, int nCol)
	{
		if (!mCells[nRow, nCol].type.IsBlockAllocatableType())
			return false;

		return mBlocks[nRow, nCol] == null;
	}

	public void SetSwipeBlocks(Block clickBlock, Block swipeBlock)
	{
		mClickBlock = clickBlock;
		mSwipeBlock = swipeBlock;
	}
	public void ResetSwipeBlocks()
	{
		mClickBlock = null;
		mSwipeBlock = null;
	}
}
