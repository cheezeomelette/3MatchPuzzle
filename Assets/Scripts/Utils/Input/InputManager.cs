using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    Transform mContainer;

#if UNITY_ANDROID && !UNITY_EDITOR
    IInputHandlerBase mInputHandler = new TouchHandler();
#else
    IInputHandlerBase mInputHandler = new MouseHandler();
#endif

    public InputManager(Transform container)
	{
        mContainer = container;
	}

    public bool isTouchDown => mInputHandler.isInputDown;
    public bool isTouchUp => mInputHandler.isInputUp;
    public Vector2 touchPosition => mInputHandler.inputPosition;
    public Vector2 touch2BoardPosition => TouchToPosition(mInputHandler.inputPosition);

    Vector2 TouchToPosition(Vector3 vtInput)
	{
        Vector3 vtMousePosW = Camera.main.ScreenToWorldPoint(vtInput);

        Vector3 vtContainerLocal = mContainer.transform.InverseTransformPoint(vtMousePosW);

		return vtContainerLocal;
	}

    public Swipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
	{
        return TouchEvaluator.EvalSwipeDir(vtStart, vtEnd);
	}

}
