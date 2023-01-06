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

	// 유효한지 확인하는 함수
	public bool DoValidation()
	{
		Debug.Assert(cells.Length == row * col);
		Debug.Log($"cell length : {cells.Length}, row, col = ({row}, {col})");

		// 셀이 행열의 곱과 갯수가 같으면 참
		if (cells.Length != row * col)
			return false;

		return true;
	}
}
