using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : BaseController
{
    //StatePatten용 플레이어 상태
    public enum PlayerState
    {
        Die,
        Idle,
        Moving,
        Skill,
    }
    
    [SerializeField]private PlayerState _state = PlayerState.Idle;
    public PlayerState State
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
                case PlayerState.Idle:
                    _anime.CrossFade("Wait", .2f);
                    break;
                case PlayerState.Die:
                    break;
                case PlayerState.Moving:
                    _anime.CrossFade("Run", .2f);
                    break;
                case PlayerState.Skill:
                    _anime.CrossFade("Attack",.2f);
                    break;
            }
        }
    }
    private PlayerStat _stat;
    private Transform _tr;
    private Animator _anime;
    private Vector3 _destPos;
    private int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    private GameObject _lockTarget;


    private void Awake()
    {
        _anime = GetComponent<Animator>();
        _tr = GetComponent<Transform>();
    }

    private void Start()
    {
        _stat = GetComponent<PlayerStat>();
        //Manager에서 관리하는 UI를 제외한 마우스 클릭시 발동하는 이벤트에
        //플레이어 마우스클릭 함수를 연동한다.
        Managers.Input.MouseAction += OnMouseEvent;
        Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }
    
    /// <summary>
    /// 각 상태에 따라 플레이어의 행동을 결정한다.
    /// </summary>
    private void Update()
    {
        switch (_state)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;
            
            case PlayerState.Moving:
                UpdateMoving();
                break;
            
            case PlayerState.Die:
                UpdateDid();
                break;
            
            case PlayerState.Skill:
                UpdateSkill();
                break;
        }
    }

    //플레이어 사망 상태일때 실행되는 함수
    private void UpdateDid()
    {
        
    }
    
    private void UpdateSkill()
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
        }
    }
    private void OnHitEvent()
    {
        if (_stopSkill)
        {
            State = PlayerState.Idle;
        }
        else
        {
            State = PlayerState.Skill;
        }
    }
    //플레이어가 이동중일때 실행되는 함수
    private void UpdateMoving()
    {
        //몬스터가 내 사거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            float distance = (_destPos - _tr.position).magnitude;
            if (distance <= 1f)
            {
                State = PlayerState.Skill;
                return;
            }
        }

        //마우스로 클릭한 지점과 플레이어간의 거리를 계산한다.
        Vector3 dir = _destPos - _tr.position;
        //플레이어가 클릭한 지점에 도착하면 IDLE로 상태 변경
        if (dir.magnitude < 0.1f)
        {
            State = PlayerState.Idle;
        }
        //플레이어가 아직 도착하지 않았으면(이동중이면)
        else
        {
            Debug.DrawRay(_tr.position + Vector3.up * .5f,dir.normalized, Color.green);
            //충돌체와 충돌 시 이동 중지
            if (Physics.Raycast(_tr.position + Vector3.up * .5f, dir, 1f, LayerMask.GetMask("Block")))
            {
                if(Input.GetMouseButton(0) == false)
                    State = PlayerState.Idle;
                return;
            }
            
            //플레이어의 이동 스칼라는, 항상 0과 마우스로 클릭한 지점과 플레이어간의 거리사이가 된다.
            //즉, 플레이어가 초과해서 이동하지 않게 된다.
            //플레이어의 회전은, Slerp를 사용해 부드럽게 회전한다.
            NavMeshAgent nma = GetComponent<NavMeshAgent>();
            float moveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime,0,dir.magnitude);
            nma.Move(dir.normalized * moveDist);

            _tr.rotation = Quaternion.Slerp(_tr.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
        
    }

    /// <summary>
    /// 플레이어 대기중일때 실행되는 함수
    /// </summary>
    private void UpdateIdle()
    {
    }

    private bool _stopSkill = false;
    private void OnMouseEvent(Define.MouseEvent evt)
    {
        switch (State)
        {
            case PlayerState.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Skill:
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
        if (State == PlayerState.Die)
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
                    State = PlayerState.Moving;
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
