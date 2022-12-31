using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SortedListMethods
{
	public static KeyValuePair<T1, T2> First<T1, T2>(this SortedList<T1, T2> sortedList)
	{
		if (sortedList.Count == 0)
			return new KeyValuePair<T1, T2>();

		return new KeyValuePair<T1, T2>(sortedList.Keys[0], sortedList.Values[0]);
	}
}
