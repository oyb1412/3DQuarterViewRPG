using System;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UI_Button : UI_Popup
{
    private int _score;
    
    //하이어라키에 존재하는 모든 Text오브젝트들(이름이 동일해야함)
    private enum Texts
    {
        PointText,
        ScoreText,
    }
    //하이어라키에 존재하는 모든 Button오브젝트들(이름이 동일해야함)
    private enum Buttons
    {
        PointButton,
    }
    //하이어라키에 존재하는 모든 GameObject들(이름이 동일해야함)
    private enum GameObjects
    {
        TestObject,
    }
    //하이어라키에 존재하는 모든 Image오브젝트들(이름이 동일해야함)
    private enum Images
    {
        ItemIcon,
    }



    public override void Init()
    {
        //UI초기설정 및 캔버스 컴포넌트를 생성한다.
        base.Init();
        
        //게임 시작 시 하이어라키에 존재하는 모든 오브젝트들을 뒤져 바인딩한다.
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        
        GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) =>
        {
            go.transform.position = data.position;
        },Define.UIEvent.Drag);
    }

    public void OnButtonClicked(PointerEventData data)
    {
        _score++;
        GetText((int)Texts.ScoreText).text = _score.ToString();
    }
}
