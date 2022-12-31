using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHandler : IInputHandlerBase
{
	public bool isInputDown => Input.GetTouch(0).phase == TouchPhase.Began;

	public bool isInputUp => Input.GetTouch(0).phase == TouchPhase.Ended;

	public bool isInputDrag => Input.GetTouch(0).phase == TouchPhase.Moved;

	public Vector2 inputPosition => Input.GetTouch(0).position;
}
