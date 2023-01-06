# 3Match Puzzle

간단한 3Match Puzzle게임입니다. 

목표 셀을 파괴하고 스테이지를 클리어하세요!

<img src="3Match%20Puzzle%20072c9dcb68a24b0bb7181caf6a9965fd/KakaoTalk_20230102_005333556_01.jpg" width="400" height="800"/>
<img src="3Match%20Puzzle%20072c9dcb68a24b0bb7181caf6a9965fd/KakaoTalk_20230102_005333556.jpg" width="400" height="800"/>

## 🔁순서도

---

![제목 없는 다이어그램.drawio.png](3Match%20Puzzle%20072c9dcb68a24b0bb7181caf6a9965fd/%25EC%25A0%259C%25EB%25AA%25A9_%25EC%2597%2586%25EB%258A%2594_%25EB%258B%25A4%25EC%259D%25B4%25EC%2596%25B4%25EA%25B7%25B8%25EB%259E%25A8.drawio.png)

## 💡 주요 기능

---

### 🌎스테이지 관리

- 스테이지를 생성할 때 필요한 정보인 행렬, 움직이는 횟수, 목표 점수, 셀 정보를 Json형태의 파일로 관리한다.

```csharp
public class StageInfo 
{
  public int row;				// 스테이지 행 개수
  public int col;				// 스테이지 열 개수
	public int movingEnergy;    // 움직일 수 있는 에너지
	public int goalScore;		//  목표 점수

  public int[] cells;			// 스테이지 형태

	// 저장된 셀타입을 리턴하는 함수
	public CellType GetCellType(int nRow, int nCol)
	{
		Debug.Assert(cells != null && cells.Length > nRow * col + nCol, $"Invalid Row/Col = {nRow}, {nCol}");

		// revisedRow == 좌측하단을 [0,0]으로 수정한row
		int revisedRow = (row - 1) - nRow;
		// [nRow,nCol] 의 셀이 cells에 내에 있으면 cells에 저장된 type을 리턴
		if (cells.Length > revisedRow * col + nCol)
			return (CellType)cells[revisedRow * col + nCol];

		return CellType.EMPTY;
	}
}
```

```json
{
    "row":9,
    "col":9,
    "movingEnergy":20,
    "goalScore":6000,
    "cells":[
                1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,1,1,
                1,1,1,1,1,1,1,3,3,
                1,1,1,1,1,1,1,3,3
             ]
}
```

### 🛠️스테이지 생성

- StageReader를 통해서 Json형태의 파일을 StageInfo로 가져와서 스테이지를 생성한다.
- StageInfo의 셀정보에 맞게 SpawnBlock을 통해 랜덤한 블럭을 생성한다.

```csharp
// 스테이지 생성 함수
public Stage ComposeStage()
{
	// 유효하지 않은 스테이지는 오류로 출력
	Debug.Assert(mStage > 0, $"Invalid Stage{mStage}");

	// Json형태로 저장된 스테이지 정보를 로드한다.
	mStageInfo = LoadStage(mStage);
	
	// 스테이지 생성
	Stage stage = new Stage(this, mStageInfo.row, mStageInfo.col, mStageInfo.movingEnergy, mStageInfo.goalScore);

	// 스테이지 정보를 바탕으로 블록과 셀을 생성한다.
	for (int nRow = 0; nRow < mStageInfo.row; nRow++)
	{
		for(int nCol = 0; nCol < mStageInfo.col; nCol++)
		{
			// 비어있는 셀은 빈 블럭을 생성하고 그 외에는 기본 블럭을 생성한다
			stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
			stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
		}
	}
	return stage;
}
```

```csharp
public class StageReader
{
	// 스테이지 정보를 불러오는 함수
    public static StageInfo LoadStage(int nStage)
	{
		Debug.Log($"Load Stage : Stage/{GetFileName(nStage)}");

		// Json형태의 텍스트 파일로 저장되어 있는 스테이지 정보를 가져온다
		TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(nStage)}");
		if(textAsset != null)
		{
			// Json파일을 StageInfo 클래스로 변환해준다.
			StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

			Debug.Assert(stageInfo.DoValidation());

			return stageInfo;
		}

		return null;
	}

	// 파일이름을 가져오는 함수
	static string GetFileName(int nStage)
	{
		return string.Format("stage_{0:D4}", nStage);
	}
}
```

```csharp
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
```

### 🔎블럭 매칭 검사

- 블럭 처리 순서 (NORMAL → MATCH → CLEAR)

```csharp
// 블럭 매칭 상태
public enum BlockStatus
{
	NORMAL,					// 생성됬을 때
	MATCH,					// 매칭이 확인된 상태
	CLEAR					  // 매칭된 상태의 블럭을 처리하고 삭제예정인 블럭상태
}
```

- 스왑한 블럭중 무지개블럭이 있다면 같은색의 블럭을 제거하는 처리를 먼저 한다.
- 블럭을 스왑한 이후 UpdateAllBlocksMatchedStatus 함수를 통해 모든 블럭의 매칭상태를 검사한다.

```csharp
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
			block.RepresentativeBlockEvaluate();
			matchedBlocks.RemoveAt(0);
		}

		// 폭발범위에 블럭이 있다면
		while (bombRangeBlocks.Count > 0)
		{
			// 블럭을 처리해준다
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
```

- 모든 블럭을 검사할 때 EvalBlocksIfMatched 함수를 사용해 블럭 하나의 가로 세로 매칭상태를 검사한다.

```csharp
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
```

- 같은 종류의 블럭이 3개이상 연결되었으면 UpdateBlockStatusMatched 함수를 통해 매칭 타입을 정해준다.

```csharp
// 한 블럭에서 가로 세로 매칭검사
	public bool EvalBlocksIfMatched(int nRow, int nCol, List<Block> matchedBlockList)
	{
		// 3개이상 연결되었으면 true 리턴
		bool bFound = false;

		// 기준 블럭
		Block baseBlock = mBlocks[nRow, nCol];
		if (baseBlock == null)
			return false;

		// 기준 블럭이 이미 검사해서 매칭상태거나 검사가 불가능한 상태면 리턴
		if (baseBlock.match != MatchType.NONE || !baseBlock.IsValidate() || mCells[nRow, nCol].IsObstacle())
			return false;

		// 기준블럭을 매칭리스트에 추가하고 가로 세로 검사를 시작한다.
		matchedBlockList.Add(baseBlock);

		// 가로 검사
		bool isHorizon = true;

		// 같은 종류의 블럭을 리스트에 추가해준다.
		for (int i = nCol + 1; i < maxCol; i++)
		{
			Block block = mBlocks[nRow, i];
			if (!block.IsSafeEqual(baseBlock))
				break;

			matchedBlockList.Add(block);
		}
		for (int i = nCol - 1; i >= 0; i--)
		{
			Block block = mBlocks[nRow, i];
			if (!block.IsSafeEqual(baseBlock))
				break;

			matchedBlockList.Insert(0, block);
		}

		// 3개이상 연결되었으면 블럭 상태를 매치상태로 바꿔준다.
		if (matchedBlockList.Count >= 3)
		{
			SetBlockStatusMatched(matchedBlockList, isHorizon);
			bFound = true;
		}

		// 세로 검사를 하기위해 가로 검사때 썼던 리스트를 초기화한다.
		matchedBlockList.Clear();

		// 세로 검사(가로 검사와 동일한 과정)
		isHorizon = false;
		matchedBlockList.Add(baseBlock);

		for (int i = nRow + 1; i < maxCol; i++)
		{
			Block block = mBlocks[i, nCol];
			if (!block.IsSafeEqual(baseBlock))
				break;

			matchedBlockList.Add(block);
		}

		for (int i = nRow - 1; i >= 0; i--)
		{
			Block block = mBlocks[i, nCol];
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
		// 매칭된 블럭 여부를 리턴
		return bFound;
	}
```

- 블럭의 매칭상태를 바꿔준다.

```csharp
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
```

```csharp
// 매칭타입
public enum MatchType
{
	NONE = 0,
	BOMB = 1,
	THREE = 3,
	FOUR = 4,
	FIVE = 5,
	THREE_THREE = 6,
	THREE_FOUR = 7,
	THREE_FIVE = 8,
	FOUR_FIVE = 9,
	FOUR_FOUR = 10,
}

// 교차블럭을 계산해주는 함수
public static MatchType Add(this MatchType matchTypeSrc, MatchType matchTypeTarget)
{
	// 4*4블럭은 3*5와 숫자가 겹치기 때문에 예외처리
	if (matchTypeSrc == MatchType.FOUR && matchTypeTarget == MatchType.FOUR)
		return MatchType.FOUR_FOUR;

	// 교차블럭 리턴
	return (MatchType)((int)matchTypeSrc + (int)matchTypeTarget);
}
```

### 💣폭탄블럭 승급

- Evaluate함수의 일부이다.
- 블럭의 매칭상태를 업데이트한 이후 블럭의 우선순위를 업데이트한다.
- 매칭된 블럭을 우선순위대로  정렬하고 정렬된 순서대로 RepresentativeBlockEvaluate함수를 사용해 블럭을 클리어 처리한다.
- 정렬하는 이유는 폭탄블럭으로 승급할 때 움직인 블럭을 우선으로 승급시키기 위해서다.

```csharp
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
	block.RepresentativeBlockEvaluate();
	matchedBlocks.RemoveAt(0);
}
```

- BFS방식으로 우선순위가 가장 높은 블럭부터 인접한  블럭을 순회하여 매칭처리한다.
- 4개 이상의 블럭이 같은종류일 경우에 블럭들 모두 MatchType이 4이상으로 되어있기 때문에 그 블럭들 중 우선순위가 높은 한 블럭만 폭탄블럭으로 승급시키기 위해 BFS 방식을 사용했다.
- 3개 초과의 블럭이 연결된 상태이면 폭탄블럭으로 승급시킨다.

```csharp
// 우선순위가 가장높은 블럭 승급
public void RepresentativeBlockEvaluate()
{
	// 이미 검사한 블럭은 패스 (1번만 순회)
	if (isEvaluated)
		return;

	// 인접 블럭 검사
	EvaluateAdjecentBlock();

	// 검사 이후 3매치 이상 블럭 폭탄으로 승급
	if ((int)match > (int)MatchType.THREE)
	{
		ChangeBlockToBomb();
		mBoard.GoalCheck(this);
	}
}
```

```csharp
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
```

- 블럭을 매칭된 상태에 따라 승급 시켜주고 매치 가능한 상태로 바꿔준다.

```csharp
// 블럭을 폭탄으로 승급시켜주는 함수
public void ChangeBlockToBomb()
{
	// 폭탄블럭의 성능이 좋은 순서대로 (무지개 폭탄 -> 주변 블럭 폭탄 -> 라인삭제 폭탄) 승급검사를 한다.
	if (match == MatchType.THREE_FIVE || match == MatchType.FOUR_FIVE || match == MatchType.FIVE)
		blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_LAZER);		// 동일한 종류의 블럭 모두 제거
	else if (match == MatchType.THREE_FOUR || match == MatchType.THREE_THREE || match == MatchType.FOUR_FOUR)
		blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_CIRCLE);       // 주변 폭발
	else if (mHorizonMatch && (match == MatchType.FOUR))
		blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_VERT);			// 세로줄 폭발
	else if (mVerticalMatch && (match == MatchType.FOUR))
		blockBehaviour.ChangeBlockQuestType(BlockQuestType.CLEAR_HORZ);			// 가로줄 폭발

	// 매칭상태에서 기본 상태로 바꿔준다.
	isEvaluated = false;
	status = BlockStatus.NORMAL;
	match = MatchType.NONE;
}
```

### 🌟블럭 처리 (+ 폭탄블럭 처리)

- 매치 상태인 블럭을 처리하는 함수이다
- 블럭이 폭탄 블럭일 경우 폭발범위의 블럭을 추가하는 AddBombRangeBlocks 함수를 실행한다.

```csharp
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
```

- 폭발범위의 위치정보를 가져와서 위치에 있는 블럭들을 매치상태로 바꿔주는함수

```csharp
// 폭발범위내의 블럭들을 bombRangeBlocks에 추가하는 함수.
public void AddBombRangeBlocks(int row, int col, BlockQuestType questType)
{
	// 폭발 범위를 가져온다
	BlockPos[] explosionPositions = mBombDefine.GetBombRange(row, col, questType);

	// 폭발범위의 블럭을 bombRangeBlocks에 추가
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
	}
	// bombRangeBlocks의 블럭들 매칭상태로 변환
	foreach (Block block in bombRangeBlocks)
	{
		block.UpdateBlockStatusBombMatched();
	}
}
```

- 폭탄 타입과 위치에 따라 폭발범위를 리턴하는 함수

```csharp
// 폭탄 타입에 따라 폭발 범위를 리턴하는 함수
public BlockPos[] GetBombRange(int row, int col, BlockQuestType questType)
{
	// 폭탄 타입에 따라서 다른 함수를 호출한다.
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
```

- 폭발범위를 리턴해주는 함수

```csharp
// 가로줄, 세로줄 범위 함수
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

// 미리 정의된 범위를 입력받아서 폭발범위를 리턴하는 함수
private BlockPos[] ReturnList(int row, int col, BlockPos[] bombRange)
{
	List<BlockPos> explosionVec = new();

	// 미리 정의해둔 폭발범위를 폭탄위치를 기준으로 바꿔준다.
	foreach(BlockPos pos in bombRange)
	{
		// 범위 내의 위치인지 체크
		if(row + pos.row >= 0 && row + pos.row < mBoard.maxRow && col + pos.col >= 0 && col + pos.col < mBoard.maxCol)
		{
			explosionVec.Add(new BlockPos(row + pos.row, col + pos.col));
		}
	}
	// 폭발범위 리턴
	return explosionVec.ToArray();
}
```
