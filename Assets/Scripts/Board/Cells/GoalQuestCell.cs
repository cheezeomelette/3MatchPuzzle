using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalQuestType
{
	NA = 0,
	SIMPLE = 1,
}
public class GoalQuestCell
{
	GoalQuestType goalQuestType;
	int durability;
	
	public GoalQuestCell(GoalQuestType goalQuestType, int durability)
	{
		this.goalQuestType = goalQuestType;
		this.durability = durability;
	}
}
