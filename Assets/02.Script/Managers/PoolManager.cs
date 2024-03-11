using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager 
{
    #region Pool
    /// <summary>
    /// 풀링할 각 객체를 모아둔 클래스
    /// </summary>
    public class Pool
    {
        //풀링할 객체의 오브젝트
        public GameObject Original { get; private set; }
        //풀링할 객체들의 부모 오브젝트의 트랜스폼
        public Transform Root { get; set; }
        //풀링한 객체들을 저장해둘 스택
        private Stack<Poolable> _poolStack = new Stack<Poolable>();

        /// <summary>
        /// 풀링할 객체의 오브젝트 지정 및 부모 오브젝트 생성
        /// </summary>
        /// <param name="original"></param>
        /// <param name="count"></param>
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = original.name + "_Root";

            //기본 지정 수 만큼 오브젝트 생성 후 비활성화, 스택에 저장
            for (int i = 0; i < count; i++)
            {
                Push((Create()));
            }
        }

        /// <summary>
        /// 실질적으로 풀링할 오브젝트를 생성
        /// </summary>
        /// <returns>생성된 오브젝트</returns>
        private Poolable Create()
        {
            GameObject go = Object.Instantiate(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }
        
        /// <summary>
        /// 풀에 오브젝트 반환
        /// </summary>
        /// <param name="poolable">반환할 객체</param>
        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.IsUse = false;
            
            _poolStack.Push(poolable);
        }

        /// <summary>
        /// 풀 매니저에서 오브젝트 추출
        /// </summary>
        /// <param name="parent">지정할 부모</param>
        /// <returns>추출한 오브젝트</returns>
        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            //풀 매니저에 객체가 생성되어 있으면 해당 객체를 반환
            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            //없다면 새롭게 객체를 생성
            else
                poolable = Create();

            //사용할 오브젝트기 때문에 바로 활성화
            poolable.gameObject.SetActive(true);

            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            
            poolable.transform.parent = parent;
            poolable.IsUse = true;
            return poolable;
        }
    }
    

    #endregion

    //모든 풀을 관리할 딕셔너리
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
    //모든 풀의 부모가 될 객체의 트랜스폼
    private Transform _root;
    
    /// <summary>
    /// 풀 루트 객체 생성 및 파괴불가 지정
    /// </summary>
    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_root" }.transform;
            Object.DontDestroyOnLoad(_root.gameObject);
        }
    }
    
    /// <summary>
    /// 풀에 객체 반환
    /// </summary>
    /// <param name="poolable">반환할 객체</param>
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        
        //딕셔너리에 반환할 객체가 존재하지 않는다면(아직 pool이 생성되지 않았다면)
        if (_pools.ContainsKey(name) == false)
        {
            Object.Destroy(poolable.gameObject);
            return;
        }
        
        _pools[name].Push(poolable);
    }

    /// <summary>
    /// 풀에서 객체 추출
    /// </summary>
    /// <param name="original">추출할 객체</param>
    /// <param name="parent">지정할 부모 객체</param>
    /// <returns>추출된 객체</returns>
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        //추출 시도시, 생성된 풀이 없는 상태라면 새롭게 풀을 생성
        if (_pools.ContainsKey(original.name) == false)
            CreatePool(original);
        
        return _pools[original.name].Pop(parent);
    }

    /// <summary>
    /// 풀 생성
    /// </summary>
    /// <param name="original">생성할 풀의 객체</param>
    /// <param name="count">생성할 풀의 객체 수</param>
    private void CreatePool(GameObject original, int count = 5)
    {
        //새롭게 풀을 생성한다.
        Pool pool = new Pool();
        //해당 풀의 초기화(original객체를 count수 만큼 생성) 
        pool.Init(original, count);
        //풀의 부모는 root객체가 된다.
        pool.Root.parent = _root.transform;
        
        //풀을 생성했으니 딕셔너리에 보관한다.
        _pools.Add(original.name,pool);
    }
    
    /// <summary>
    /// 만약 딕셔너리에 객체가 보관되어 있다면 Road하는것이 아닌,
    /// 딕셔너리에서 가져다 쓰기 위한 함수
    /// </summary>
    /// <param name="name">조사할 객체의 이름</param>
    /// <returns></returns>
    public GameObject GetOriginal(string name)
    {
        if (_pools.ContainsKey(name) == false)
            return null;
        
        return _pools[name].Original;
    }

    /// <summary>
    /// 풀 매니저 초기화
    /// </summary>
    public void Clear()
    {
        _pools.Clear();
        foreach (Transform child in _root)
        {
            Object.Destroy(child.gameObject);
        }
    }
}
