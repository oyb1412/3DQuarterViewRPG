using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX 
{
    //현재 씬을 저장하는 프로퍼티(이 프로퍼티가 호출될 때 마다 GameObject.FindFirstObjectByType<BaseScene>() 이 정보를 새롭게 호출한다)
    public BaseScene CurrentScene { get { return GameObject.FindFirstObjectByType<BaseScene>(); } }
    
    /// <summary>
    /// 새로운 씬으로 이동
    /// </summary>
    /// <param name="type">이동할 씬</param>
    public void LoadScene(Define.Scene type)
    {
        //이동 전에 모든 정보를 초기화한다.
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    /// <summary>
    /// enum형의 타입을 넣으면 string형으로 반환해주는 함수
    /// </summary>
    /// <param name="type">씬의 타입</param>
    /// <returns>이동할 씬의 이름</returns>
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    /// <summary>
    /// 저장해둔 현재 씬 정보 초기화
    /// </summary>
    public void Clear()
    {
        CurrentScene.Clear();
    }
}
