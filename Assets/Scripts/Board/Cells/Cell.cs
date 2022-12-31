using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
	protected CellType mType;
	public CellType type
	{
		get { return mType; }
		set { mType = value; }
	}

	protected CellBehaviour mCellBehaviour;
	public CellBehaviour cellBehaviour
	{
		get { return mCellBehaviour; }
		set
		{
			mCellBehaviour = value;
			mCellBehaviour.SetCell(this);
		}
	}

	public Cell(CellType cellType)
	{
		mType = cellType;
	}

	public Cell InstantiateCellObj(GameObject cellPrefab, Transform containerObj)
	{
		GameObject newObj = Object.Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);

		newObj.transform.parent = containerObj;

		this.cellBehaviour = newObj.transform.GetComponent<CellBehaviour>();

		return this;
	}

	public void Move(float x, float y)
	{
		cellBehaviour.transform.position = new Vector3(x, y, 0);
	}

	public bool IsObstacle()
	{
		return type == CellType.EMPTY;
	}

	public bool IsGoalMatched()
	{
		if(type == CellType.GOAL)
		{
			type = CellType.BASIC;
			mCellBehaviour.UpdateView(true);
			return true;
		}
		return false;
	}
}
