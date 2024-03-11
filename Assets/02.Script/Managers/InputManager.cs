using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    //모든 키 입력을 처리할 액션
    public Action KeyAction = null;
    //UI를 제외한 모든 마우스 입력을 처리할 액션, 입력 방식을 매개변수로 받는다
    public Action<Define.MouseEvent> MouseAction = null;
    //현재 마우스가 눌리고 있나?
    private bool _pressed = false;

    private float _pressTime;
    //Manager의 Update에서 실행할 함수
    public void OnUpdate()
    {
        //여기선 UI를 제외한 마우스 클릭만 처리할 예정이기 때문에, UI가 눌렸으면 리턴
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        //어떤 키라도 눌렸다면, 키 액션을 실행한다.
        if (Input.anyKey)
            KeyAction?.Invoke();

        //마우스 왼쪽클릭을 했으면
        if (Input.GetMouseButton(0))
        {
            if (!_pressed)
            {
                MouseAction?.Invoke(Define.MouseEvent.PointerDown);
                _pressTime = Time.time;
            }
            //마우스 액션에 Press타입을 넣어 실행한다.
            MouseAction?.Invoke(Define.MouseEvent.Press);
            //현재 Press중
            _pressed = true;
        }
        else
        {
            //현재 Press중이면 마우스 액션에 Click타입을 넣어 실행한다.
            if (_pressed)
            {
                if(Time.time < _pressTime + .2f)
                    MouseAction?.Invoke(Define.MouseEvent.Click);
                MouseAction?.Invoke(Define.MouseEvent.PointerUp);
            }
            //Press중 해제
            _pressed = false;
            _pressTime = 0;
        }
    }

    /// <summary>
    /// Scene등을 이동할 때, 데이터를 모두 초기화한다
    /// </summary>
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
