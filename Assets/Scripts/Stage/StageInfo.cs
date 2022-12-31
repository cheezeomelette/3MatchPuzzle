using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo 
{
    public int row;				// 스테이지 행 개수
    public int col;				// 스테이지 열 개수
	public int movingEnergy;    // 움직일 수 있는 에너지
	public int goalScore;		//  목표 점수

    public int[] cells;			// 스테이지 형태

	public override string ToString()
	{
		return JsonUtility.ToJson(this);
	}

	public CellType GetCellType(int nRow, int nCol)
	{
		Debug.Assert(cells != null && cells.Length > nRow * col + nCol, $"Invalid Row/Col = {nRow}, {nCol}");

		int revisedRow = (row - 1) - nRow;
		if (cells.Length > revisedRow * col + nCol)
			return (CellType)cells[revisedRow * col + nCol];

		Debug.Assert(false);

		return CellType.EMPTY;
	}

	public bool DoValidation()
	{
		Debug.Assert(cells.Length == row * col);
		Debug.Log($"cell length : {cells.Length}, row, col = ({row}, {col})");

		if (cells.Length != row * col)
			return false;

		return true;
	}
}
