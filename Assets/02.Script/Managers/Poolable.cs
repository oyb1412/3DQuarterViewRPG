using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 풀링 대상 확인용 클래스
/// 이 컴포넌트를 소지한 객체들만이 풀링 대상이 된다.
/// </summary>
public class Poolable : MonoBehaviour
{
    public bool IsUse { get; set;}
}
