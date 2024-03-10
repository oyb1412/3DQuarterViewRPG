using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameScene : BaseScene
{
    private void Awake()
    {
        Init(); 
    }
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;
        Managers.UI.ShowSceneUI<UI_Inven>();
        Managers.UI.ShowPopupUI<UI_Button>();
    }

    public override void Clear()
    {
    }
}

   
