using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private int i = 0;
    private void OnTriggerEnter(Collider other)
    {

        i++;
        if(i % 2 == 0)
            Managers.Sound.Play("UnityChan/univ0001",Define.Sound.Bgm);
        else
            Managers.Sound.Play("UnityChan/univ0002",Define.Sound.Bgm);
    }
}
