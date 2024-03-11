using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    /// <summary>
    /// 게임 중 사용할 오브젝트, 파일들을 불러오는 함수
    /// 이미 생성된 적이 있다면 풀에서 꺼내 리턴한다.
    /// </summary>
    /// <param name="path">파일이 존재하는 경로</param>
    /// <typeparam name="T">파일의 타입(gameobject, audioclip등)</typeparam>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
        //가져오려는 파일이 게임오브젝트(풀 매니저로 관리가 가능한 객체) 라면
        if (typeof(T) == typeof(GameObject))
        {
            //패스명으로부터 해당 파일의 이름을 추출
            string name = path;
            int index = name.LastIndexOf("/");
            if (index > 0)
                name = name.Substring(index + 1);

            //해당 파일의 이름을 키로 딕셔너리에서 파일을 추출
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// 불러온 오브젝트를 직접 생성하는 함수
    /// </summary>
    /// <param name="path">오브젝트가 존재하는 경로</param>
    /// <param name="parent">오브젝트의 부로 </param>
    /// <returns>생성된 오브젝트</returns>
    public GameObject Instantiate(string path, Transform parent = null)
    {
        var original = Load<GameObject>($"Prefabs/{path}");
        
        if (original == null)
        {
            Debug.Log(($"Failed to load Prefab : {path}"));
            return null;
        }

        //생성하려는 객체가 풀링 대상이면, 풀링 매니저 딕셔너리에서 추출 후 리턴
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;
        
        GameObject go = Object.Instantiate(original, parent);
        
        
        //Clone이 이름에 존재할 경우, 첫 인덱스를 반환한다.
        //ex) Player(clone)인 경우, 0부터 첫 인덱스까지, 즉 Player라는 이름만 반환한다.
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);
        
        return go;
    }

    public void Destroy(GameObject go, float time = 0f)
    {
        if (go == null)
            return;

        //삭제할 객체가 풀링 대상인지 확인
        Poolable poolable = go.GetComponent<Poolable>();
        //풀링 대상인 경우, 삭제하는 것이 아닌 풀링에 반환
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go,time);
    }
}
