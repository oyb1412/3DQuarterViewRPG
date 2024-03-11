using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ILorder<Key, Value>
{
    public Dictionary<Key, Value> MakeDict();
}
public class DataManager
{
    public Dictionary<int, Stat> dictData { get; private set; } = new Dictionary<int, Stat>();
    public void Init()
    {
        dictData = LoadJson<StatData, int, Stat>("statData").MakeDict();
    }
    
    private T LoadJson<T, Key, Value>(string path) where T : ILorder<Key, Value>
    {
        TextAsset textAsset = Managers.Resources.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<T>(textAsset.text);
    }
}
