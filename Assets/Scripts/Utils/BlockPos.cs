using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockPos
{
    public int row { get; set; }
    public int col { get; set; }

    public BlockPos(int nRow, int nCol)
	{
        row = nRow;
        col = nCol;
	}

	public bool IsValidPos()
	{
		return row >= 0 && col >= 0;
	}

	public override bool Equals(object obj)
	{
		return obj is BlockPos pos && row == pos.row && col == pos.row;
	}

	public override int GetHashCode()
	{
		var hashCode = -928284752;
		hashCode = hashCode * -1521134295 + row.GetHashCode();
		hashCode = hashCode * -1521134295 + col.GetHashCode();
		return hashCode;
	}

	public override string ToString()
	{
		return $"(row = {row}, col = {col})";
	}
}
