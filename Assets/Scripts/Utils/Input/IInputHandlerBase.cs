using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandlerBase 
{
    bool isInputDown { get; }
    bool isInputUp { get; }
    bool isInputDrag { get; }
    Vector2 inputPosition { get; }
}
