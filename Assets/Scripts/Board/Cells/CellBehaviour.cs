using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
	[SerializeField] CellConfig cellConfig;

	SpriteRenderer mSpriteRenderer;
	Cell mCell;

	private void Start()
	{
		mSpriteRenderer = GetComponent<SpriteRenderer>();

		UpdateView(false);
	}
	

	public void SetCell(Cell cell)
	{
		mCell = cell;
	}

	public void UpdateView(bool bValueChanged)
	{
		mSpriteRenderer.color = cellConfig.cellTypeColors[(int)mCell.type];
		
	}
}
