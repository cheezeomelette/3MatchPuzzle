using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
	[SerializeField] BlockConfig mBlockConfig;

    Block mBlock;
	SpriteRenderer mSpriteRenderer;

	private void Start()
	{
		mSpriteRenderer = GetComponent<SpriteRenderer>();

		UpdateView(false);
	}
	public void SetBlock(Block nBlock)
	{
		mBlock = nBlock;
	}

	public void UpdateView(bool bValueChanged)
	{
		if(mBlock.type == BlockType.EMPTY)
		{
			mSpriteRenderer.sprite = null;
		}
		else if(mBlock.type == BlockType.BASIC)
		{
			if (mBlock.breed > BlockBreed.BREED_4)
				mBlock.breed = BlockBreed.BREED_0;
			mSpriteRenderer.sprite = mBlockConfig.basicBlockSprites[(int)mBlock.breed];
		}
	}

	public void DoActionClear()
	{
		StartCoroutine(CoStartSimpleExplosion());
	}

	public void ChangeBlockQuestType(BlockQuestType questType)
	{
		mBlock.questType = questType;
		switch(questType)
		{
			case BlockQuestType.CLEAR_LAZER:
				mSpriteRenderer.sprite = mBlockConfig.GetLazerBombSprite();
				break;
			case BlockQuestType.CLEAR_CIRCLE:
				mSpriteRenderer.sprite = mBlockConfig.GetCircleBombSprite(mBlock.breed);
				break;
			case BlockQuestType.CLEAR_HORZ:
				mSpriteRenderer.sprite = mBlockConfig.GetHorizontalBombSprite(mBlock.breed);
				break;
			case BlockQuestType.CLEAR_VERT:
				mSpriteRenderer.sprite = mBlockConfig.GetVerticalBombSprite(mBlock.breed);
				break;
		}
	}

	IEnumerator CoStartSimpleExplosion(bool bDestroy = true)
	{
		GameObject explosionObj = mBlockConfig.GetBlockParticle(mBlock.breed);
		explosionObj.SetActive(true);
		explosionObj.transform.position = this.transform.position;

		yield return new WaitForSeconds(0.1f);

		if (bDestroy)
			Destroy(gameObject);
		else
		{
			Debug.Assert(false, "UnKnown Action : GameObject no destroy after particle");
		}
	}
}
