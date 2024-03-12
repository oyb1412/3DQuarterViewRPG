using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    //실 데이터를 지니고 있는 매니저의 인스턴스
    private static Managers s_instance;
    //매니저의 데이터를 사용하기 위한 프로퍼티
    private static Managers Instance
    {
        get {return s_instance;}
    }
    private GameManager _game = new GameManager();
    private DataManager _data = new DataManager();
    private InputManager _input = new InputManager();
    private ResourceManager _resources = new ResourceManager();
    private UIManager _ui = new UIManager();
    private SceneManagerEX _scene = new SceneManagerEX();
    private SoundManager _sound = new SoundManager();
    private PoolManager _pool = new PoolManager();
    
    public static GameManager Game{ get { return Instance._game; }}
    public static DataManager Data{ get { return Instance._data; }}
    public static InputManager Input{ get { return Instance._input; }}
    public static ResourceManager Resources { get { return Instance._resources; }}
    public static UIManager UI { get { return Instance._ui; } }
    public static SceneManagerEX Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static PoolManager Pool { get { return Instance._pool; } }

    private void Awake()
    {
        //초기화
        Init();
    }
    private void Update()
    {
        //입력 액션 발동을 이곳에서 모아서 처리한다.
        _input.OnUpdate();
    }
    private void Init()
    {
        //인스턴스가 존재하지 않는다면
        if (s_instance == null)
        {
            //매니저 오브젝트를 찾는다.
            GameObject go = GameObject.Find("@Managers");
            //없다면 새로 생성한다.
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            //매니저를 영구지속 오브젝트로 지정한다.
            DontDestroyOnLoad(go);
            //인스턴스를 넣어준다.
            s_instance = go.GetComponent<Managers>();
            
            //Sound의 초기화를 진행한다.
            s_instance._sound.Init();
            
            //Pool 초기화
            s_instance._pool.Init();
            
            //데이터 초기화
            s_instance._data.Init();
        }
    }

    /// <summary>
    /// Scene이 이동될 때, 각종 데이터를 초기화한다.
    /// </summary>
    public static void Clear()
    {
        Sound.Clear();
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        
        Pool.Clear();
    }
}
