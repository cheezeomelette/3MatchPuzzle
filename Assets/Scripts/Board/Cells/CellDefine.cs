using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CellType
{
	EMPTY = 0,
	BASIC = 1,
	FIXTURE = 2,
	GOAL = 3,
	JELLY = 4,
}

static class CellTypeMethod
{
	public static bool IsBlockAllocatableType(this CellType cellType)
	{
		return !(cellType == CellType.EMPTY);
	}

	public static bool IsMovableType(this CellType cellType)
	{
		return !(cellType == CellType.EMPTY);
	}
}