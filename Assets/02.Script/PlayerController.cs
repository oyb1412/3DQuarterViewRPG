using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Transform _tr;

    [SerializeField] private float _speed = 10f;
    private void Start()
    {
        _tr = GetComponent<Transform>();
        Managers.Input.KeyAction -= OnKeyBoard;
        Managers.Input.KeyAction += OnKeyBoard;
    }

    private void Update()
    {
       
    }

    private void OnKeyBoard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _tr.position += Vector3.forward * (_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _tr.position += Vector3.back * (_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _tr.position += Vector3.left * (_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _tr.position += Vector3.right * (_speed * Time.deltaTime);
        }
    }
}
