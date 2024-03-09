using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]private GameObject _player;
    [SerializeField]private Define.CameraMode _mode = Define.CameraMode.QUARTER_VIEW;
    [SerializeField]private Vector3 _delta;
    private void LateUpdate()
    {
        if (_mode == Define.CameraMode.QUARTER_VIEW)
        {
            if (Physics.Raycast(_player.transform.position, _delta, out var hit, _delta.magnitude,
                    LayerMask.GetMask("Wall")))
            {
                float dist = (hit.point - _player.transform.position).magnitude * .8f;
                transform.position = _player.transform.position + _delta.normalized * dist;
            }
            else
            {
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }
            

        }
    }
}
