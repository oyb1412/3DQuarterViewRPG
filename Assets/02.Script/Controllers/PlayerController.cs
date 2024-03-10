using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Transform _tr;
    private Animator _anime;
    //마우스로 클릭한 지점
    private Vector3 _destPos;

    //StatePatten용 플레이어 상태
    private enum PlayerState
    {
        DIE,
        MOVING,
        IDLE,
    }

    private PlayerState _state = PlayerState.IDLE;
    [SerializeField] private float _speed = 10f;

    private void Awake()
    {
        _anime = GetComponent<Animator>();
        _tr = GetComponent<Transform>();
    }

    private void Start()
    {
        //Manager에서 관리하는 UI를 제외한 마우스 클릭시 발동하는 이벤트에
        //플레이어 마우스클릭 함수를 연동한다.
        Managers.Input.MouseAction += OnMouseClicked;
    }

    //플레이어 사망 상태일때 실행되는 함수
    private void UpdateDid()
    {
        
    }

    //플레이어가 이동중일때 실행되는 함수
    private void UpdateMoving()
    {
        //마우스로 클릭한 지점과 플레이어간의 거리를 계산한다.
        Vector3 dir = _destPos - transform.position;
        //플레이어가 클릭한 지점에 도착하면 IDLE로 상태 변경
        if (dir.magnitude < 0.0001f)
        {
            _state = PlayerState.IDLE;
        }
        //플레이어가 아직 도착하지 않았으면(이동중이면)
        else
        {
            //플레이어의 이동 스칼라는, 항상 0과 마우스로 클릭한 지점과 플레이어간의 거리사이가 된다.
            //즉, 플레이어가 초과해서 이동하지 않게 된다.
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime,0,dir.magnitude);
            _tr.position += dir.normalized * moveDist;
            //플레이어의 회전은, Slerp를 사용해 부드럽게 회전한다.
            _tr.rotation = Quaternion.Slerp(_tr.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
        
        _anime.SetFloat("speed", _speed);
    }

    /// <summary>
    /// 플레이어 대기중일때 실행되는 함수
    /// </summary>
    private void UpdateIdle()
    {
        _anime.SetFloat("speed", 0);
    }
    
    /// <summary>
    /// 각 상태에 따라 플레이어의 행동을 결정한다.
    /// </summary>
    private void Update()
    {
        switch (_state)
        {
            case PlayerState.IDLE:
                UpdateIdle();
                break;
            
            case PlayerState.MOVING:
                UpdateMoving();
                break;
            
            case PlayerState.DIE:
                UpdateDid();
                break;
        }
    }

    /// <summary>
    /// 마우스로 이동 지점을 클릭했을때 실행되는 함수
    /// 단독으로 실행되지 않고, InputManager의 Action에 연동해서 실행한다.
    /// </summary>
    /// <param name="evt"></param>
    private void OnMouseClicked(Define.MouseEvent evt)
    {
        //사망상태면 리턴
        if (_state == PlayerState.DIE)
            return;
        
        //발사할 레이를 설정한다. 레이는 2d모니터상의 마우스 위치로 발사된다.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100f,Color.red,1f);
        LayerMask mask =  LayerMask.GetMask("Ground");

        //레이를 발사한다.
        if (Physics.Raycast(ray, out var hit, 100f, mask))
        {
            //플레이어가 이동할 위치를 저장한다.
            _destPos = hit.point;
            //플레이어의 상태를 변경한다.
            _state = PlayerState.MOVING;
        }
    }
}
