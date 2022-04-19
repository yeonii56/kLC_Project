using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsPung_JYJ : MonoBehaviour
{
    // Fruits_Prefeb에 적용
    // 처음에 뭉쳐있던 과일이 터지는 소리 효과

    SoundOneShot_JYJ soundClip;

    void Awake()
    {
        soundClip = GetComponent<SoundOneShot_JYJ>();
    }

    void Start()
    {
        SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
    }
}   