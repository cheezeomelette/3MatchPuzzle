using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockActionBehaviour : MonoBehaviour
{
	[SerializeField] BlockConfig mBlockConfig;
    public bool isMoving { get; set; }

    Queue<Vector3> mMovementQueue = new Queue<Vector3>();

    public void MoveDrop(Vector2 vtDropDistance)
	{
		mMovementQueue.Enqueue(new Vector3(vtDropDistance.x, vtDropDistance.y, 1));

		if(!isMoving)
		{
			StartCoroutine(DoActionMoveDrop());
		}
	}

	IEnumerator DoActionMoveDrop(float acc = 1.0f)
	{
		isMoving = true;

		while(mMovementQueue.Count >0)
		{
			Vector2 vtDestination = mMovementQueue.Dequeue();

			int dropIndex = System.Math.Min(9, System.Math.Max(1, (int)Mathf.Abs(vtDestination.y)));
			float duration = mBlockConfig.dropSpeed[dropIndex - 1];
			yield return CoStartDropSmooth(vtDestination, duration * acc);
		}

		isMoving = false;
		yield break;
	}

	IEnumerator CoStartDropSmooth(Vector2 vtDropDistance, float duration)
	{
		Vector2 to = new Vector3(transform.position.x + vtDropDistance.x, transform.position.y - vtDropDistance.y);
		yield return Action2D.MoveTo(transform, to, duration);
	}
}
