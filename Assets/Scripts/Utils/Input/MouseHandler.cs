using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHandler : IInputHandlerBase
{
	public bool isInputDown => Input.GetButtonDown("Fire1");

	public bool isInputUp => Input.GetButtonUp("Fire1");

	public bool isInputDrag => Input.GetButton("Fire1");

	public Vector2 inputPosition => Input.mousePosition;

}
