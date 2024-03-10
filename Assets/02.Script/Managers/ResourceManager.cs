using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    /// <summary>
    /// 게임 중 사용할 오브젝트, 파일들을 불러오는 함수
    /// </summary>
    /// <param name="path">파일이 존재하는 경로</param>
    /// <typeparam name="T">파일의 타입(gameobject, audioclip등)</typeparam>
    /// <returns></returns>
    public T Load<T>(string path) where T : Object
    {
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
        var prefab = Load<GameObject>($"Prefabs/{path}");
        
        if (prefab == null)
        {
            Debug.Log(($"Failed to load Prefab : {path}"));
            return null;
        }
        
        GameObject go = Object.Instantiate(prefab, parent);
        
        //Clone이 이름에 존재할 경우, 첫 인덱스를 반환한다.
        //ex) Player(clone)인 경우, 0부터 첫 인덱스까지, 즉 Player라는 이름만 반환한다.
        int index = go.name.IndexOf("(Clone)");
        if (index > 0)
            go.name = go.name.Substring(0, index);
        
        return go;
    }

    public void Destroy(GameObject target, float time = 0f)
    {
        if (target == null)
            return;
        
        Object.Destroy(target,time);
    }
}
