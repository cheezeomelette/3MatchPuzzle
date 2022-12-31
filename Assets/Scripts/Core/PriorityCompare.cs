using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityCompare : IComparer<bool>
{
	public int Compare(bool x, bool y)
	{
		if (x)
			return 1;
		else if (y)
			return -1;
		else return 0;
	}
}
