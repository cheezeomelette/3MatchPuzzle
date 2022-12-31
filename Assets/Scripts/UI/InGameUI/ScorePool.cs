using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePool : MonoBehaviour
{
    public static ScorePool Instance;

    [SerializeField] GameObject scoreObject;
    [SerializeField] Transform usedTransform;

    Queue<BlockScoreUI> poolingObjectQueue = new Queue<BlockScoreUI>();

	private void Awake()
	{
        Instance = this;
        
        Init(10);
	}

    private void Init(int initCount)
	{
        for(int i = 0; i < initCount; i++)
		{
            poolingObjectQueue.Enqueue(CreateNewObject());
		}
    }

    private BlockScoreUI CreateNewObject()
    {
        BlockScoreUI newObj = Instantiate(scoreObject, usedTransform).GetComponent<BlockScoreUI>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public BlockScoreUI GetObject()
    {
        if (Instance.poolingObjectQueue.Count > 0)
        {
            BlockScoreUI scoreUI = Instance.poolingObjectQueue.Dequeue();
            scoreUI.transform.SetParent(Instance.transform);
            scoreUI.gameObject.SetActive(true);
            return scoreUI;
        }
        else
        {
            BlockScoreUI newScoreUI = Instance.CreateNewObject();
            newScoreUI.gameObject.SetActive(true);
            newScoreUI.transform.SetParent(Instance.transform);
            return newScoreUI;
        }
    }

    public void ReturnObject(BlockScoreUI socreUI)
    {
        socreUI.gameObject.SetActive(false);
        socreUI.transform.SetParent(usedTransform);
        Instance.poolingObjectQueue.Enqueue(socreUI);
    }
}
