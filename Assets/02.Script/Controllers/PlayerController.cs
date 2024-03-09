using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Transform _tr;
    private Vector3 _destPos;
    private Animator _anime;

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
    }

    private void Start()
    {
        _tr = GetComponent<Transform>();
        Managers.Input.MouseAction += OnMouseClicked;
    }

    private void UpdateDid()
    {
        
    }

    private void UpdateMoving()
    {
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.0001f)
        {
            _state = PlayerState.IDLE;
        }
        else
        {
            float moveDist = Mathf.Clamp(_speed * Time.deltaTime,0,dir.magnitude);
            _tr.position += dir.normalized * moveDist;
            _tr.rotation = Quaternion.Slerp(_tr.rotation, Quaternion.LookRotation(dir), 20f * Time.deltaTime);
        }
        
        _anime.SetFloat("speed", _speed);
    }

    private void UpdateIdle()
    {
        _anime.SetFloat("speed", 0);

    }
    
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

    private void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.DIE)
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100f,Color.red,1f);
        LayerMask mask =  LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out var hit, 100f, mask))
        {
            _destPos = hit.point;
            _state = PlayerState.MOVING;
        }
    }
}
