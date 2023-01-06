using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
	public BlockStatus status;
	public BlockQuestType questType;

	public MatchType match = MatchType.NONE;

	BlockType mBlockType;
	protected BlockBreed mBreed;

	public Block upBlock;
	public Block downBlock;
	public Block leftBlock;
	public Block rightBlock;

	int mPriority;
	public int priority
	{
		get { return mPriority; }
	}

	public bool isMoved;

	bool isEvaluated;

	int mRow;
	int mCol;
	public int row
	{
		get { return mRow; }
	}
	public int col
	{
		get { return mCol; }
	}

	private bool mHorizonMatch;
	public bool horizonMatch
	{
		get { return mHorizonMatch; }
	}
	private bool mVerticalMatch;
	public bool verticalMatch
	{
		get { return mVerticalMatch; }
	}
	public BlockType type
	{
		get { return mBlockType; }
		set { mBlockType = value; }
	}

	public BlockBreed breed
	{
		get { return mBreed; }
		set
		{
			mBreed = value;
			mBlockBehaviour?.UpdateView(true);
		}
	}
	BlockActionBehaviour mBlockActionBehaviour;
	public bool isMoving
	{
		get
		{
			return blockObj != null && mBlockActionBehaviour.isMoving;
		}
	}
	public Vector2 dropDistance
	{
		set
		{
			mBlockActionBehaviour?.MoveDrop(value);
			isMoved = true;
		}
	}

	Board mBoard;
	protected BlockBehaviour mBlockBehaviour;
	public BlockBehaviour blockBehaviour
	{
		get { return mBlockBehaviour; }
		set
		{
			mBlockBehaviour = value;
			mBlockBehaviour.SetBlock(this);
		}
	}

	public Transform blockObj { get { return mBlockBehaviour?.transform; } }
	Vector2Int mDuplicate;

	public int horzDuplicate
	{
		get { return mDuplicate.x; }
		set { mDuplicate.x = value; }
	}

	public int vertDuplicate
	{
		get { return mDuplicate.y; }
		set { mDuplicate.y = value; }
	}

	int mDurability;                    // 내구도
	public virtual int durability
	{
		get { return mDurability; }
		set { mDurability = value; }
	}

	public Block(BlockType blockType)
	{
		mBlockType		= blockType;
		status			= BlockStatus.NORMAL;
		questType		= BlockQuestType.CLEAR_SIMPLE;
		match			= MatchType.NONE;
		mBreed			= BlockBreed.NA;
		mHorizonMatch	= false;
		mVerticalMatch	= false;
		mDurability		= 1;
	}

	public Block InstantiateBlockObj(GameObject blockPrefab, Transform containerObj, Board board)
	{
		if (IsValidate() == false)
			return null;

		mBoard = board;
		GameObject newObj = Object.Instantiate(blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

		newObj.transform.parent = containerObj;

		this.blockBehaviour = newObj.GetComponent<BlockBehaviour>();
		mBlockActionBehaviour = newObj.GetComponent<BlockActionBehaviour>();

		return this;
	}

	// 블럭이 할당되어 있는지
	public bool IsValidate()
	{
		return type != BlockType.EMPTY;
	}

	internal void Move(float x, float y)
	{
		blockBehaviour.transform.position = new Vector3(x, y, 0);
	}

	// 우선순위가 가장높은 블럭 승급
	public void RepresentativeBlockEvaluate()
	{
		// 이미 검사한 블럭은 패스 (1번만 순회)
		if (isEvaluated)
			return;

		// 인접 블럭 검사
		EvaluateAdjecentBlock();

		// 계산 이후 3매치 이상 블럭 폭탄으로 승급
		if ((int)match > (int)MatchType.THREE)
		{
			ChangeBlockToBomb();
			mBoard.GoalCheck(this);
		}
	}

	// 기본 블럭은 클리어상태로 바꾸고 폭탄블럭은 보드에 폭탄범위 파괴 요청
	public bool DoEvaluation()
	{
		// status가 clear상태면 리턴
		if (!IsEvaluatable())
			return false;

		// 매칭 상태일 경우
		if (status == BlockStatus.MATCH)
		{
			// 매칭된 블럭이 일반 블럭일 경우(폭탄x)
			if (questType == BlockQuestType.CLEAR_SIMPLE)
			{
				durability--;
			}
			// 폭탄 블럭일 경우
			else
			{
				// 폭발 범위를 가져와서 보드에서 폭발 범위 블럭 제거요청
				mBoard.AddBombRangeBlocks(mRow, mCol, questType);
				durability--;
				status = BlockStatus.CLEAR;
				return true;
			}

			// 블럭의 내구도가 0이면 블럭을 클리어상태로 바꾼다.
			if (mDurability <= 0)
			{
				status = BlockStatus.CLEAR;
				return false;
			}
		}

		return false;
	}

	// 인접한 블록이 같은 종류일 때 매칭처리한다.
	void EvaluateAdjecentBlock()
	{
		// 이미 검사한 블록은 패스 (1번만 순회)
		if (isEvaluated)
			return;
		isEvaluated = true;
		Debug.Log($"Block [{row},{col}]  isMoved : {isMoved}");

		// 가로로 3매치이면 매칭처리
		if (IsHorizontalMatched())
		{
			leftBlock.EvaluateAdjecentBlock();
			rightBlock.EvaluateAdjecentBlock();
		}
		// 세로로 3매치면 매칭처리
		if(IsVerticalMatched())
		{
			upBlock.EvaluateAdjecentBlock();
			downBlock.EvaluateAdjecentBlock();
		}
		// 매칭처리함수
		DoEvaluation();
	}

	// 매칭 시 블럭의 매칭종류를 바꿔주는 함수(3, 3*3, 3*4...)
	public void UpdateBlockStatusMatched(MatchType matchType, bool isHorizon)
	{
		// 블럭을 매칭상태로 바꿔준다
		this.status = BlockStatus.MATCH;
		mHorizonMatch = isHorizon || mHorizonMatch;	// 가로 매칭 상태
		mVerticalMatch = !isHorizon || mVerticalMatch; // 세로 매칭 상태

		// 매치타입이 없다면 그대로 대입하고 이미 있다면 교차블럭으로 더해준다.
		if (match == MatchType.NONE)
		{
			match = matchType;
		}
		else
		{
			this.match = match.Add(matchType);
		}
	}

	public void ChangeBlockToBomb()
	{
		if (match == MatchType.THREE_FIVE || match == MatchType.FOUR_FIVE || match == MatchType.FIVE)
			blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_LAZER);       // 동일한 블럭 제거
		else if (match == MatchType.THREE_FOUR || match == MatchType.THREE_THREE || match == MatchType.FOUR_FOUR)
			blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_CIRCLE);       // 주변폭발
		else if (mHorizonMatch && (match == MatchType.FOUR))
			blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_VERT);
		else if (mVerticalMatch && (match == MatchType.FOUR))
			blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_HORZ);

		isEvaluated = false;
		status = BlockStatus.NORMAL;
		match = MatchType.NONE;
	}

	public void UpdateBlockStatusBombMatched()
	{
		if (isEvaluated || status == BlockStatus.CLEAR)
			return;

		isEvaluated = true;
		this.status = BlockStatus.MATCH;

		if (match == MatchType.NONE)
			match = MatchType.BOMB;

		if (questType == BlockQuestType.NONE)
			questType = BlockQuestType.CLEAR_SIMPLE;
	}
	
	public void UpdatePositionInfo(int nRow, int nCol, Block upBlock, Block downBlock, Block leftBlock, Block rightBlock)
	{
		mRow = nRow;
		mCol = nCol;

		this.upBlock = upBlock;
		this.downBlock = downBlock;
		this.leftBlock = leftBlock;
		this.rightBlock = rightBlock;
	}

	public void UpdatePriority()
	{
		if(IsHorizontalMatched())
		{
			leftBlock.mPriority += 1;
			rightBlock.mPriority += 1;
			this.mPriority += 1;
		}
		if(IsVerticalMatched())
		{
			upBlock.mPriority += 1;
			downBlock.mPriority += 1;
			this.mPriority += 1;
		}
	}

	public void ResetBeforeMove()
	{
		isEvaluated = false;
		isMoved = false;
		mPriority = 0;
		mHorizonMatch = false;
		mVerticalMatch = false;
	}


	public void ResetDuplicationInfo()
	{
		mDuplicate.x = 0;
		mDuplicate.y = 0;
	}

	public bool IsEqual(Block target)
	{
		if (target == null)
			return false;

		if (IsMatchableBlock() && this.breed == target.breed)
			return true;

		return false;
	}

	public bool IsMatchableBlock()
	{
		return !(type == BlockType.EMPTY);
	}

	public void MoveTo(Vector3 to, float duration)
	{
		mBlockBehaviour.StartCoroutine(Action2D.MoveTo(blockObj, to, duration));
		isMoved = !isMoved;
	}

	public virtual void Destroy()
	{
		Debug.Assert(blockObj != null, $"{match}");
		blockBehaviour.DoActionClear();
	}

	public bool IsSwipeable(Block baseBlock)
	{
		return true;
	}

	public bool IsEvaluatable()
	{
		if (status == BlockStatus.CLEAR || !IsMatchableBlock())
			return false;

		return true;
	}

	bool IsHorizontalMatched()
	{
		return IsEqual(leftBlock) && IsEqual(rightBlock);
	}
	bool IsVerticalMatched()
	{
		return IsEqual(upBlock) && IsEqual(downBlock);
	}
}
