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

	int mDurability;                    // ������
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

	// ���� �Ҵ�Ǿ� �ִ���
	public bool IsValidate()
	{
		return type != BlockType.EMPTY;
	}

	internal void Move(float x, float y)
	{
		blockBehaviour.transform.position = new Vector3(x, y, 0);
	}

	// �켱������ ������� �� �±�
	public void RepresentativeBlockEvaluate()
	{
		// �̹� �˻��� ���� �н� (1���� ��ȸ)
		if (isEvaluated)
			return;

		// ���� �� �˻�
		EvaluateAdjecentBlock();

		// ��� ���� 3��ġ �̻� �� ��ź���� �±�
		if ((int)match > (int)MatchType.THREE)
		{
			ChangeBlockToBomb();
			mBoard.GoalCheck(this);
		}
	}

	// �⺻ ���� Ŭ������·� �ٲٰ� ��ź���� ���忡 ��ź���� �ı� ��û
	public bool DoEvaluation()
	{
		// status�� clear���¸� ����
		if (!IsEvaluatable())
			return false;

		// ��Ī ������ ���
		if (status == BlockStatus.MATCH)
		{
			// ��Ī�� ���� �Ϲ� ���� ���(��źx)
			if (questType == BlockQuestType.CLEAR_SIMPLE)
			{
				durability--;
			}
			// ��ź ���� ���
			else
			{
				// ���� ������ �����ͼ� ���忡�� ���� ���� �� ���ſ�û
				mBoard.AddBombRangeBlocks(mRow, mCol, questType);
				durability--;
				status = BlockStatus.CLEAR;
				return true;
			}

			// ���� �������� 0�̸� ���� Ŭ������·� �ٲ۴�.
			if (mDurability <= 0)
			{
				status = BlockStatus.CLEAR;
				return false;
			}
		}

		return false;
	}

	// ������ ����� ���� ������ �� ��Īó���Ѵ�.
	void EvaluateAdjecentBlock()
	{
		// �̹� �˻��� ����� �н� (1���� ��ȸ)
		if (isEvaluated)
			return;
		isEvaluated = true;
		Debug.Log($"Block [{row},{col}]  isMoved : {isMoved}");

		// ���η� 3��ġ�̸� ��Īó��
		if (IsHorizontalMatched())
		{
			leftBlock.EvaluateAdjecentBlock();
			rightBlock.EvaluateAdjecentBlock();
		}
		// ���η� 3��ġ�� ��Īó��
		if(IsVerticalMatched())
		{
			upBlock.EvaluateAdjecentBlock();
			downBlock.EvaluateAdjecentBlock();
		}
		// ��Īó���Լ�
		DoEvaluation();
	}

	// ��Ī �� ���� ��Ī������ �ٲ��ִ� �Լ�(3, 3*3, 3*4...)
	public void UpdateBlockStatusMatched(MatchType matchType, bool isHorizon)
	{
		// ���� ��Ī���·� �ٲ��ش�
		this.status = BlockStatus.MATCH;
		mHorizonMatch = isHorizon || mHorizonMatch;	// ���� ��Ī ����
		mVerticalMatch = !isHorizon || mVerticalMatch; // ���� ��Ī ����

		// ��ġŸ���� ���ٸ� �״�� �����ϰ� �̹� �ִٸ� ���������� �����ش�.
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
			blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_LAZER);       // ������ �� ����
		else if (match == MatchType.THREE_FOUR || match == MatchType.THREE_THREE || match == MatchType.FOUR_FOUR)
			blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_CIRCLE);       // �ֺ�����
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
