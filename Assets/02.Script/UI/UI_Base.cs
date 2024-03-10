using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    private Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();
    /// <summary>
    /// 하이어라키에 존재하는 UI오브젝트 자동 바인딩
    /// </summary>
    /// <param name="type">바인딩할 UI의 타입</param>
    /// <typeparam name="T">바인딩 할 UI의 컴포넌트</typeparam>
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        //type으로 받아온 enum에 존재하는 모든 객체를 배열형으로 담는다.
        string[] names =  Enum.GetNames(type);
        //enum의 객체 수 만큼 오브젝트를 생성한다.
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        
        //enum의 객체 수 만큼 반복하면서 자식 오브젝트를 찾는다.
        //이 객체의 모든 자식들을 순회하며 enum의 이름과 같은 이름의 객체를 찾는다.
        for(int i = 0; i< names.Length; i++)
        {
            if(typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);
        }
        
        //찾은 자식 오브젝트를 딕셔너리에 저장한다.
        _objects.Add(typeof(T), objects);
    }

    /// <summary>
    /// 딕셔너리에 저장된 오브젝트중 index번째의 오브젝트를 가져오는 함수
    /// </summary>
    /// <param name="index">가져올 오브젝트</param>
    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        //저장해둔 오브젝트 중 T컴포넌트를 지닌 오브젝트를 찾고, 없으면 null을 반환한다.
        if (_objects.TryGetValue(typeof(T), out var objects) == false)
            return null;

        //지정한 컴포넌트의 enum 데이터를 가져온다.
        return objects[index] as T;
    }

    protected GameObject GetObject(int index){ return Get<GameObject>(index);}
    protected Text GetText(int index) { return Get<Text>(index); }
    protected Button GetButton(int index) { return Get<Button>(index); }
    protected Image GetImage(int index){ return Get<Image>(index); }

    /// <summary>
    /// 오브젝트에 이벤트Action 연결
    /// </summary>
    /// <param name="go">이벤트를 추가할 UI오브젝트</param>
    /// <param name="action">추가할 액션</param>
    /// <param name="type">추가할 이벤트의 종류</param>
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        var evt = Util.GetOrAddComponent<UI_EventHandler>(go);
        //이벤트 타입에 맞게 액션을 연동한다.
        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler += action;
                break;
        }
    }
}
