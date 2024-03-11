using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private int _order = 10;

    //팝업 UI의 정보를 저장할 스택
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    /// <summary>
    /// 부모로 지정할 오브젝트 생성용 프로퍼티
    /// </summary>
    public GameObject Root
    {
        get
        {
            //부모가 있으면 리턴, 없다면 생성 후 리턴한다
            GameObject root = GameObject.Find("UI_Root");
            if (root == null)
                root = new GameObject{name = "UI_Root"};
            return root;
        }
    }
    
    /// <summary>
    /// 내가 생성할 UI오브젝트 캔버스의 초기 설정
    /// </summary>
    /// <param name="go">관리할 오브젝트의 오브젝트</param>
    /// <param name="sort">sort의 적용여부</param>
    public void SetCanvas(GameObject go, bool sort = true)
    {
        //오브젝트에 캔버스가 있으면 가져오고, 없다면 생성해 가져온다.
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        //캔버스 렌더모드를 오버레이로 변경
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //부모의 소트와 상관없이 자신의 소트를 사용
        canvas.overrideSorting = true;
        //소트를 적용하는 오브젝트(Popup)라면
        if (sort)
        {
            //소트 수치를 바꿔준다.
            canvas.sortingOrder = (_order);
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    /// <summary>
    /// 팝업도, 씬도 아닌 제3의 UI를 생성하기 위한 함수
    /// </summary>
    /// <param name="parent">부모 오브젝트</param>
    /// <param name="name">이름</param>
    /// <typeparam name="T">컴포넌트</typeparam>
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        //이름이 없다면, 컴포넌트 명을 이름으로 한다.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        //지정된 경로에서 이름을 토대로 오브젝트를 생성한다.
        GameObject go = Managers.Resources.Instantiate($"UI/SubItem/{name}");
        
        //부모 지정
        if(parent != null)
            go.transform.SetParent(parent);
        
        //컴포넌트가 없다면 컴포넌트 생성후 컴포넌트를 리턴한다.
        return Util.GetOrAddComponent<T>(go);
    }
    
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        //이름이 없다면, 컴포넌트 명을 이름으로 한다.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        //지정된 경로에서 이름을 토대로 오브젝트를 생성한다.
        GameObject go = Managers.Resources.Instantiate($"UI/WorldSpace/{name}");
        
        //부모 지정
        if(parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        //컴포넌트가 없다면 컴포넌트 생성후 컴포넌트를 리턴한다.
        return Util.GetOrAddComponent<T>(go);
    }
    
    /// <summary>
    /// 씬UI 생성용 함수
    /// </summary>
    /// <param name="name">오브젝트의 이름</param>
    /// <typeparam name="T">오브젝트의 컴포넌트</typeparam>
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        //이름이 없다면, 컴포넌트 명을 이름으로 한다.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        //지정된 경로에서 이름을 토대로 오브젝트를 생성한다.
        GameObject go = Managers.Resources.Instantiate($"UI/Scene/{name}");
        
        //부모 지정
        go.transform.SetParent(Root.transform);
        
        //컴포넌트가 없다면 컴포넌트 생성후 컴포넌트를 리턴한다.
        return Util.GetOrAddComponent<T>(go);
    }
    
    /// <summary>
    /// 팝업UI 생성용 함수
    /// </summary>
    /// <param name="name">오브젝트의 이름</param>
    /// <typeparam name="T">오브젝트의 컴포넌트</typeparam>
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        //이름이 없다면, 컴포넌트 명을 이름으로 한다.
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        //지정된 경로에서 이름을 토대로 오브젝트를 생성한다.
        GameObject go = Managers.Resources.Instantiate($"UI/Popup/{name}");

        //부모 지정
        go.transform.SetParent(Root.transform);
        
        //컴포넌트가 없다면 컴포넌트 생성.
        T popup = Util.GetOrAddComponent<T>(go);

        //스택에 저장한다.
        _popupStack.Push(popup);
        
        return popup;
    }

    /// <summary>
    /// 모든 팝업UI 삭제
    /// </summary>
    /// <param name="popup"></param>
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count <= 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    /// <summary>
    /// 팝업UI닫기 함수
    /// </summary>
    public void ClosePopupUI()
    {
        if (_popupStack.Count <= 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resources.Destroy(popup.gameObject);
    }

    /// <summary>
    /// 모든 팝업UI닫기 함수
    /// </summary>
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
    }
}
