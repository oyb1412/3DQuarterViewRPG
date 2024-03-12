using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]private GameObject _player;
    [SerializeField]private Define.CameraMode _mode = Define.CameraMode.Quarterview;
    [SerializeField]private Vector3 _delta;

    public void SetPlayer(GameObject go){ _player = go;}
    /// <summary>
    /// 카메라의 위치를 플레이어 위치를 기반으로 조정
    /// </summary>
    private void LateUpdate()
    {
        //카메라의 모드가 쿼터뷰 모드일때
        if (_mode == Define.CameraMode.Quarterview)
        {
            if (Util.IsValid(_player) == false)
                return;
            //플레이어 위치를 기반으로, 카메라 방향으로, 둘 사이의 거리만큼, 벽 대상으로만 작동하는 레이캐스트를 쏜다.
            //레이캐스트가 적중하면, 플레이어가 벽에 가려져서 카메라에 비추지 않는 상태라는 뜻이 된다.
            if (Physics.Raycast(_player.transform.position, _delta, out var hit, _delta.magnitude,
                    LayerMask.GetMask("Block")))
            {
                //레이가 적중한 위치와 플레이어간의 거리보다 약간 짧은 거리를 지정한다.
                float dist = (hit.point - _player.transform.position).magnitude * .8f;
                //카메라의 위치를 플레이어 위치 + 카메라 방향 * 레이가 적중한 위치와 플레이어간의 거리보다 약간 짧은 거리로 변경한다.
                transform.position = _player.transform.position + _delta.normalized * dist;
            }
            //카메라가 벽에 가려지지 않은 상태라면
            else
            {
                //카메라는 항상 플레이어의 머리위에 위치한다.
                transform.position = _player.transform.position + _delta;
                //카메라는 항상 플레이어를 바라본다.
                transform.LookAt(_player.transform);
            }
            

        }
    }
}
