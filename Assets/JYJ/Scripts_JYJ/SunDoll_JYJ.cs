using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunDoll_JYJ : MonoBehaviour
{
    // SunDoll_Prefeb > Sun에 적용
    // 인형을 집으면 빛나며 소리나는 기능

    [SerializeField] Transform player;

    GameObject lightObj;
    Transform lightTrans;
    Rigidbody rig;
    AudioClip soundClip;

    void Awake()
    {
        DollInit();
        soundClip = GetComponent<SoundChange_JYJ>().clip;
    }

    void Start()
    {
        Ligting(false);
    }

    void Update()
    {
        LightDir();
        DollIsGrip();
    }   

    // 초기화
    void DollInit()
    {
        lightObj = transform.GetChild(0).gameObject;
        lightTrans = lightObj.transform;        
        rig = GetComponent<Rigidbody>();
    }

    // 빛의 방향을 카메라 쪽으로
    void LightDir()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        lightTrans.position = transform.position + dir;
    }

    // 인형을 잡으면 소리나며 빛남
    void DollIsGrip()
    {
        if (rig.isKinematic && !lightObj.activeInHierarchy && SoundSystem_JYJ.source.clip == soundClip)
        {
            SoundSystem_JYJ.source.Play();
            Ligting(true);
        }
        else if(!rig.isKinematic && lightObj.activeInHierarchy && SoundSystem_JYJ.source.clip == soundClip)
        {
            SoundSystem_JYJ.source.Stop();
            Ligting(false);
        }
    }

    // 빛나며 소리나는 오브젝트 활성화    
    void Ligting(bool bol)
    {
        lightObj.SetActive(bol);
    }
}
