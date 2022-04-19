using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soap_JYJ : MonoBehaviour
{
    // Soap_Prefeb > Soap에 적용
    // Controller와 닿으면 거품나며 소리나는 효과

    [SerializeField] ParticleSystem bubble; // 거품 프리펩

    void Start()
    {
        bubble.Stop();
    }

    // 플레이어와 닿으면 거품 효과
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bubble.Play();
        }
    }

    // 플레이어와 떨어지면 거품 효과 꺼짐
    void OnTriggerExit(Collider other) 
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            StopCoroutine("BubbleStop");
            StartCoroutine("BubbleStop");
        }
    }

    // 거품 효과 꺼질 때 1초의 시간 두기
    IEnumerator BubbleStop()
    {
        yield return new WaitForSeconds(1f);
        bubble.Stop();
    }
}
