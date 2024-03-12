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
        gameObject.GetOrAddComponent<CursorController>();

        GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "Masa");
        Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(player);
    }

    public override void Clear()
    {
    }
}

   
