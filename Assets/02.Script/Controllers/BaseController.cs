using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseController : MonoBehaviour
{
    [SerializeField]protected Define.State _state = Define.State.Idle;
    protected NavMeshAgent nma;
    protected virtual Define.State State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;

            switch (_state)
            {
                case Define.State.Idle:
                    _anime.CrossFade("Wait", .2f);
                    break;
                case Define.State.Die:
                    break;
                case Define.State.Moving:
                    _anime.CrossFade("Run", .2f);
                    break;
                case Define.State.Skill:
                    _anime.CrossFade("Attack",.2f);
                    break;
            }
        }
    }
    protected Transform _tr;
    protected Animator _anime;
    [SerializeField]protected Vector3 _destPos;
    [SerializeField]protected GameObject _lockTarget;
    public Define.WorldObject WorldObject { get; protected set; } = Define.WorldObject.Unknown;
    private void Awake()
    {
        _anime = GetComponent<Animator>();
        _tr = GetComponent<Transform>();
        nma = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        switch (_state)
        {
            case Define.State.Idle:
                UpdateIdle();
                break;
            
            case Define.State.Moving:
                UpdateMoving();
                break;
            
            case Define.State.Die:
                UpdateDie();
                break;
            
            case Define.State.Skill:
                UpdateSkill();
                break;
        }
    }

    protected abstract void Init();
    protected abstract void UpdateIdle();
    protected abstract void UpdateMoving();
    protected abstract void UpdateDie();
    protected abstract void UpdateSkill();

}
