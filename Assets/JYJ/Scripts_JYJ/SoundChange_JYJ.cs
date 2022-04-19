using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundChange_JYJ : MonoBehaviour
{
    // 가까이 있을 때 항상 소리가 나야하는 오브젝트에 적용
    // 플레이어와 가장 가까워지면 해당 clip의 사운드가 재생됨

    // AudioSource.Play()를 사용하지만 항상 소리가 나는 건 아니면 Action타입
    // 나머지 가까우면 항상 소리나는 오브젝트는 Always로 지정
    enum Type { Always, Action } 
    [SerializeField] Type type;

    public AudioClip clip;

    void Update()
    {
        TypeIsAlways();
    }

    // Always타입일 경우 플레이어와 가장 가까우면 소리가 남
    void TypeIsAlways()
    {
        if (this.type == Type.Always)
        {
            if (!SoundSystem_JYJ.source.isPlaying && SoundSystem_JYJ.source.clip == clip)
            {
                SoundSystem_JYJ.source.Play();
            }
        }
    }  
}
