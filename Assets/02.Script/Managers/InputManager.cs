using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
    private bool _pressed = false;
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnUpdate()
    {
        if (Input.anyKey)
            KeyAction?.Invoke();

        if (Input.GetMouseButton(0))
        {
            MouseAction?.Invoke(Define.MouseEvent.PRESS);
            _pressed = true;
        }
        else
        {
            if(_pressed)
                MouseAction?.Invoke(Define.MouseEvent.CLICK);
            _pressed = false;
        }
    }
}
