using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenTest_PMK : MonoBehaviour
{
    public Transform target;
    public PathMode pathMode;
    public PathType pathType;

    void Start()
    {
        transform.DOMove(target.position, 10.0f).SetEase(Ease.InExpo);
        //Vector3 rot = target.rotation.eulerAngles;
        //transform.DOMove(target.position, 3.0f);
        //transform.DOScale(2, 3.0f);
        //transform.DORotate(rot, 3.0f);
    }
}
