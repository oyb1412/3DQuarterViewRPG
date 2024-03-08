using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers Instance
    {
        get {return _instance;}
    }
    private static Managers _instance;

    private InputManager _input = new InputManager();
    private ResourceManager _resources = new ResourceManager();

    public static InputManager Input
    {
        get { return Instance._input; }
    }
    public static ResourceManager Resources
    {
        get { return Instance._resources; }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        _input.OnUpdate();
    }
}
