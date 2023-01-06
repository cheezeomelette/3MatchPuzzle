using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo 
{
    public int row;				// �������� �� ����
    public int col;				// �������� �� ����
	public int movingEnergy;    // ������ �� �ִ� ������
	public int goalScore;		//  ��ǥ ����

    public int[] cells;			// �������� ����

	public override string ToString()
	{
		return JsonUtility.ToJson(this);
	}

	// ����� ��Ÿ���� �����ϴ� �Լ�
	public CellType GetCellType(int nRow, int nCol)
	{
		Debug.Assert(cells != null && cells.Length > nRow * col + nCol, $"Invalid Row/Col = {nRow}, {nCol}");

		// revisedRow == �����ϴ��� [0,0]���� ������row
		int revisedRow = (row - 1) - nRow;
		// [nRow,nCol] �� ���� cells�� ���� ������ cells�� ����� type�� ����
		if (cells.Length > revisedRow * col + nCol)
			return (CellType)cells[revisedRow * col + nCol];

		return CellType.EMPTY;
	}

	// ��ȿ���� Ȯ���ϴ� �Լ�
	public bool DoValidation()
	{
		Debug.Assert(cells.Length == row * col);
		Debug.Log($"cell length : {cells.Length}, row, col = ({row}, {col})");

		// ���� �࿭�� ���� ������ ������ ��
		if (cells.Length != row * col)
			return false;

		return true;
	}
}
