using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Block Config", fileName = "BlockConfig")]
public class BlockConfig : ScriptableObject
{
    public float[] dropSpeed;
    public Sprite[] basicBlockSprites;
    public Sprite[] circleBomeSprites;
    public Sprite[] horizontalBomeSprites;
    public Sprite[] verticalBomeSprites;
    public GameObject[] blockColors;
    public Sprite razerBombSprite;

    public GameObject GetBlockParticle(BlockBreed breed)
    {
        return Instantiate(blockColors[(int)breed]);
    }

    public Sprite GetCircleBombSprite(BlockBreed breed)
	{
        return circleBomeSprites[(int)breed];
    }

    public Sprite GetHorizontalBombSprite(BlockBreed breed)
	{
        return horizontalBomeSprites[(int)breed];
    }

    public Sprite GetVerticalBombSprite(BlockBreed breed)
	{
        return verticalBomeSprites[(int)breed];
    }
    
    public Sprite GetLazerBombSprite()
	{
        return razerBombSprite;
	}
}
