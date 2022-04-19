using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_JYJ : MonoBehaviour
{
    // Book_Prefeb > BookCenter > 자식 6개에 적용
    // 닿았는지 확인해서 BookArray에 넘겨주기

    public bool isTriggerPage; // 넘겨줄 불린 변수

    void Start()
    {
        isTriggerPage = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // 컨트롤러랑 닿으면 페이지 넘어갈 때 조건으로 사용할 isTriggerPage 넘겨주기
        if (other.CompareTag("Player") && !isTriggerPage)
        {
            isTriggerPage = true;
        }
    }
}
