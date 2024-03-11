using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    private string _name;
    enum GameObjects
    {
        ItemIcon,
        ItemNameText,
    }
 
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;
        
        //UI핸들러 이벤트에 연동한다. 
        Get<GameObject>((int)GameObjects.ItemIcon).AddUIEvent((PointerEventData) => {Debug.Log("click" + _name);});
    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
