using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : BaseController
{
    private PlayerStat _stat;
    private int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    private bool _stopSkill = false;


    protected override void Init()
    {
        WorldObject = Define.WorldObject.Player;

        _stat = GetComponent<PlayerStat>();
        Managers.Input.MouseAction += OnMouseEvent;
        
        if(GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    //플레이어 사망 상태일때 실행되는 함수
    protected override void UpdateDie()
    {
        
    }
    
    protected override void UpdateSkill()
    {
        if(_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - _tr.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    private void OnAttackEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            PlayerStat myStat = GetComponent<PlayerStat>();
            int damage = Mathf.Max(0 ,myStat.Attack - targetStat.Defense);
            targetStat.Hp -= damage;

            if (targetStat.Hp <= 0)
            {
                Managers.Game.Despawn(targetStat.gameObject);
            }
        }
    }
    private void OnHitEvent()
    {
        if (_stopSkill)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }
    }
    //플레이어가 이동중일때 실행되는 함수
    protected override void UpdateMoving()
    {
        //몬스터가 내 사거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            float distance = (_destPos - _tr.position).magnitude;
            if (distance <= 1f)
            {
                State = Define.State.Skill;
                return;
            }
        }

        //마우스로 클릭한 지점과 플레이어간의 거리를 계산한다.
        Vector3 dir = _destPos - _tr.position;
        //플레이어가 클릭한 지점에 도착하면 IDLE로 상태 변경
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        //플레이어가 아직 도착하지 않았으면(이동중이면)
        else
        {
            Debug.DrawRay(_tr.position + Vector3.up * .5f,dir.normalized, Color.green);
            //충돌체와 충돌 시 이동 중지
            if (Physics.Raycast(_tr.position + Vector3.up * .5f, dir, 1f, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;
            }
            
            //플레이어의 이동 스칼라는, 항상 0과 마우스로 클릭한 지점과 플레이어간의 거리사이가 된다.
            //즉, 플레이어가 초과해서 이동하지 않게 된다.
            //플레이어의 회전은, Slerp를 사용해 부드럽게 회전한다.
            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime,0,dir.magnitude);
            transform.position += dir.normalized * moveDist;

            _tr.rotation = Quaternion.Slerp(_tr.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
        
    }

    /// <summary>
    /// 플레이어 대기중일때 실행되는 함수
    /// </summary>
    protected override void UpdateIdle()
    {
    }

    private void OnMouseEvent(Define.MouseEvent evt)
    {
        switch (State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
                if (evt == Define.MouseEvent.PointerUp)
                    _stopSkill = true;
                break;
        }
        
    }
    
    /// <summary>
    /// 마우스로 이동 지점을 클릭했을때 실행되는 함수
    /// 단독으로 실행되지 않고, InputManager의 Action에 연동해서 실행한다.
    /// </summary>
    /// <param name="evt"></param>
    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        //사망상태면 리턴
        if (State == Define.State.Die)
            return;
        
        //발사할 레이를 설정한다. 레이는 2d모니터상의 마우스 위치로 발사된다.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool raycastHit = Physics.Raycast(ray, out hit, 100f, _mask);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
            {
                if (raycastHit)
                {
                    State = Define.State.Moving;
                    _stopSkill = false;
                    if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                    {
                        _lockTarget = hit.collider.gameObject;
                        _destPos = _lockTarget.transform.position;
                    }
                    else
                    {
                        _lockTarget = null;
                        _destPos = hit.point;
                    }
                }
            }
                break;
            case Define.MouseEvent.Press:
            {
                if (_lockTarget == null && raycastHit)
                    _destPos = hit.point;
            }
                break;
            case Define.MouseEvent.PointerUp:
                _stopSkill = true;
                break;
        }
    }
}
