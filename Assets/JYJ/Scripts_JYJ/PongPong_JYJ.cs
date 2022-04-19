using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongPong_JYJ : MonoBehaviour
{
    // WasingDishes_Prefeb > DishSoap > DishSoapTop > SoapPos > PongPong에 적용
    // 다른 물체와 닿으면 사라지는 기능    

    Vector3 oriPos;

    void Start()
    {
        oriPos = transform.localPosition; 
    }

    // 사라지고 처음 위치로 초기화
    void OnCollisionEnter(Collision other)
    {
        gameObject.SetActive(false);
        transform.localPosition = oriPos;
    }
}
