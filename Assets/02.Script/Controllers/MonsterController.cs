using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    private Stat _stat;
    [SerializeField]private float _scanRange = 10f;
    [SerializeField]private float _attackRange = 2f;
    protected override void Init()
    {
        WorldObject = Define.WorldObject.Monster;
        _stat = GetComponent<Stat>();
        if(GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

    }

    protected override void UpdateDie()
    {
        
    }

    protected override void UpdateMoving()
    {
        _destPos = _lockTarget.transform.position;

        //몬스터가 내 사거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _attackRange)
            {
                nma.SetDestination(transform.position);
                State = Define.State.Skill;
                return;
            }
        }

        Vector3 dir = _destPos - transform.position;
        nma.SetDestination(_destPos);
        nma.speed = _stat.MoveSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        
    }

    protected override void UpdateSkill()
    {
        if(_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    protected override void UpdateIdle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Failed Search Player");
            return;
        }

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= _scanRange)
        {
            _lockTarget = player;
            _destPos = _lockTarget.transform.position;
            State = Define.State.Moving;
            return;
        }
    }

    private void OnAttackEvent()
    {
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            int damage = Mathf.Max(0 ,_stat.Attack - targetStat.Defense);
            targetStat.Hp -= damage;

            if (targetStat.Hp <= 0)
            {
                Managers.Game.Despawn(targetStat.gameObject);
            }
        }
    }
    private void OnHitEvent()
    {
        if (_lockTarget != null)
        {
            float distance = (_lockTarget.transform.position - transform.position).magnitude;
            if (distance <= _attackRange)
            {
                State = Define.State.Skill;
            }
            else
            {
                State = Define.State.Moving;
            }
        }
        else
        {
            State = Define.State.Idle;
        }
    }
}
