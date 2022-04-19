using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg_JYJ : MonoBehaviour
{
    // Egg_Prefeb > Egg에 적용
    // 다른 물체에 닿으면 색변하고 많이 닿으면 깨지고 병아리 나옴

    SoundOneShot_JYJ soundClip;

    // 알 깨고 나올 병아리
    [SerializeField] GameObject chick;

    MeshRenderer ren;

    // 알이 다른 물체와 부딪힌 횟수
    int count;

    void Awake()
    {
        ren = GetComponent<MeshRenderer>();
        soundClip = GetComponent<SoundOneShot_JYJ>();
    }

    void Start()
    {
        count = 0;
    }

    void Update()
    {
        Break();
    }

    // 다른 물체와 부딪히면 ShowColorChange() 호출
    void OnCollisionEnter(Collision collision)
    {
        ShowColorChange();
    }

    // 색이 변하고 소리남
    void ShowColorChange()
    {
        count++;       

        if (count >= 2)
        {
            StopCoroutine("ColorChange");
            StartCoroutine("ColorChange");
            SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
        }
    }

    // 3번 이상 부딪히면 알 깨짐
    void Break()
    {
        if (count > 3)
        {
            chick.SetActive(true);
            chick.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }

    // 색이 0.5초 동안 변함
    IEnumerator ColorChange()
    {
        ren.material.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        ren.material.color = Color.white;
    }
}
