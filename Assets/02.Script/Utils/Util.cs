using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util 
{
    /// <summary>
    /// 게임오브젝트go에 T컴포넌트가 있는지 조사 후, 없으면 T컴포넌트를 추가해 리턴
    /// </summary>
    /// <param name="go">조사할 게임오브젝트</param>
    /// <typeparam name="T">유무를 조사할 컴포넌트</typeparam>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform tr = FindChild<Transform>(go, name, recursive);
        if (tr != null)
            return tr.gameObject;
        else
            return null;
    }

    public static bool IsValid(GameObject go)
    {
        return go != null || go.activeSelf;
    }
    
    /// <summary>
    /// go오브젝트의 자식 오브젝트중 name과 동일한 이름의 오브젝트를 찾는 함수
    /// </summary>
    /// <param name="go">조사할 오브젝트(이 함수를 호출한 게임오브젝트)</param>
    /// <param name="name">조사할 오브젝트의 이름</param>
    /// <param name="recursive">재귀 유무(자식의 자식까지 조사할건지에 대한 불리언)</param>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            //조사할 오브젝트의 자식들을 조사한다.
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                //조사할 오브젝트의 이름이 없거나 조사할 이름의 오브젝트를 발견하면
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    //해당 오브젝트 반환
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            //조사할 오브젝트의 모든 자식들을 조사한다.
            foreach (var components in go.GetComponentsInChildren<T>())
            {
                //조사할 이름의 오브젝트를 발견하면 반환한다.
                if (string.IsNullOrEmpty(name) || components.name == name)
                    return components;
            }
        }

        return null;
    }
}
