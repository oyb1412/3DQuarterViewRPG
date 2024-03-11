using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private enum CursorType
    {
        None,
        Attack,
        Hand,
    }
    private CursorType _cursorType = CursorType.None;
    private Texture2D _attackIcon;
    private Texture2D _handIcon;
    private int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    private void Start()
    {
        _attackIcon = Managers.Resources.Load<Texture2D>("Textures/Cursor/Attack");
        _handIcon = Managers.Resources.Load<Texture2D>("Textures/Cursor/Hand");
    }

    void Update()
    {
        UpdateMouseCursor();
    }
    
    private void UpdateMouseCursor()
    {
        if (Input.GetMouseButton(0))
            return;
        
        //발사할 레이를 설정한다. 레이는 2d모니터상의 마우스 위치로 발사된다.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //레이를 발사한다.
        if (Physics.Raycast(ray, out var hit, 100f, _mask))
        {
            if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
            {
                if (_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5f, 0f), CursorMode.Auto);
                    _cursorType = CursorType.Attack;
                }
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3f, 0f), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }
}
