using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        var prefab = Load<GameObject>($"Prefabs/{path}");
        
        if (prefab == null)
        {
            Debug.Log(($"Failed to load Prefab : {path}"));
            return null;
        }
        
        return Object.Instantiate(prefab, parent);
    }

    public void Destroy(GameObject target, float time = 0f)
    {
        if (target == null)
            return;
        
        Object.Destroy(target,time);
    }
}
