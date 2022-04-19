using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beop_JYJ : MonoBehaviour
{
    // Beop_Prefeb > Beop에 적용
    // BeopBottom과 닿으면 소리나는 효과

    SoundOneShot_JYJ soundClip;

    [SerializeField] Transform beop;

    Transform beopTrigger;
    Rigidbody rig; // 법봉의 리지드바디

    RaycastHit hit;

    void Awake()
    {
        soundClip = GetComponent<SoundOneShot_JYJ>();
        rig = beop.GetComponent<Rigidbody>();
        beopTrigger = beop.GetChild(0);
    }

    // 법봉이 잡힌 상태로 법봉 판과 부딪히면 소리남
    void OnTriggerEnter(Collider other)
    {
        if(other.transform == beopTrigger && rig.isKinematic)
        {
            if (Physics.Raycast(transform.position, Vector3.up, out hit, 0.1f))
            {
                if (hit.transform == beop)
                {
                    SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
                }
            }
        }
    }
}
