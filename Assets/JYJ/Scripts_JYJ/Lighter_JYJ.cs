using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter_JYJ : MonoBehaviour
{
    // Lighter_Prefeb > Lighter > FireShot에 적용
    // 오큘러스 컨트롤러와 닿으면 불 켜지면서 소리나는 효과

    SoundOneShot_JYJ soundClip;

    ParticleSystem[] fires;
    GameObject fire;

    void Awake()
    {
        soundClip = GetComponent<SoundOneShot_JYJ>();

        fire = transform.GetChild(0).gameObject;
        fires = fire.transform.GetComponentsInChildren<ParticleSystem>();
    }

    // 초기화
    void Start()
    {
        fire.SetActive(false);
        Fire(false);
    }

    // 컨트롤러와 닿으면 이펙트 켜짐
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
            fire.SetActive(true);
            Fire(true);
        }
    }

    // 컨트롤러와 떨어지면 이펙트 꺼짐
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Fire(false);
        }
    }

    // 불꽃과 불빛 이펙트 온오프 함수
    void Fire(bool isFire)
    {
        if (isFire == true)
        {
            for (int i = 0; i < fires.Length; i++)
            {
                fires[i].Play();
            }
        }
        else
        {
            for (int i = 0; i < fires.Length; i++)
            {
                fires[i].Stop();
            }
        }
    }

}
