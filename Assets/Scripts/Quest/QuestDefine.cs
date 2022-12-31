using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

// 매치타입 확장메서드
static class MatchTypeMethod
{
	public static short ToValue(this MatchType matchType)
	{
		return (short)matchType;
	}

	public static MatchType Add(this MatchType matchTypeSrc, MatchType matchTypeTarget)
	{
		if (matchTypeSrc == MatchType.FOUR && matchTypeTarget == MatchType.FOUR)
			return MatchType.FOUR_FOUR;

		return (MatchType)((int)matchTypeSrc + (int)matchTypeTarget);
	}
}

