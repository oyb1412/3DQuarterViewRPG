using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    /// <summary>
    /// 현재 씬의 정보
    /// </summary>
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    /// <summary>
    /// 모든 하위 씬에서 사용되는 함수.
    /// 이벤트 시스템이 없다면 생성한다.
    /// </summary>
    protected virtual void Init()
    {
        var obj = GameObject.FindFirstObjectByType(typeof(EventSystem));
        if (obj == null)
            Managers.Resources.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();
}
