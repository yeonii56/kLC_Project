using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenTest_LGW : MonoBehaviour
{
    public Transform target;
    PathType pathType;
    void Start()
    {
        transform.DOMove(target.position, 6.0f).SetEase(Ease.InSine);
    }

    void Update()
    {
        
    }
}
