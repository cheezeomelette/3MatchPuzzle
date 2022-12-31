using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 실제 움직임
public class Action2D 
{
    public static IEnumerator MoveTo(Transform target, Vector3 to, float duration, bool bSelfRemove = false)
	{
		Vector2 startPos = target.transform.position;

		float elapsed = 0.0f;	// 경과시간

		while(elapsed < duration)
		{
			elapsed += Time.smoothDeltaTime;
			target.transform.position = Vector2.Lerp(startPos, to, elapsed / duration);

			yield return null;
		}

		target.transform.position = to;
		if (bSelfRemove)
			Object.Destroy(target.gameObject, 0.1f);

		yield break;
	}
}
