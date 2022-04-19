using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem_JYJ : MonoBehaviour
{
    // Player의 자식으로 AudioSource 오브젝트를 넣을 것임. 그 AudioSource 오브젝트에 적용
    // 가장 가까운 오브젝트의 클립으로 오디오소스의 클립을 바꿈

    public static SoundSystem_JYJ instance;
    public static AudioSource source;

    SoundChange_JYJ[] clipObjs;
    SoundChange_JYJ clipObj;

    Transform player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = transform.parent;
        source = GetComponent<AudioSource>();
        clipObjs = Resources.FindObjectsOfTypeAll<SoundChange_JYJ>();
    }

    void Update()
    {
        ClipChange(MinDistanceObj());
        ClipNonActive();
    }

    // 플레이어와 가장 가까운 오브젝트 판별
    float MinDistanceObj()
    {
        float dist = float.MaxValue;

        for (int i = 0; i < clipObjs.Length; i++)
        {
            Transform clips = clipObjs[i].transform;

            if (clipObjs[i].gameObject.activeInHierarchy)
            {
                if (dist > Vector3.Distance(clips.position, player.position))
                {
                    dist = Vector3.Distance(clips.position, player.position);
                    clipObj = clipObjs[i];
                }
            }
        }
        return dist;
    }

    // 오디오클립 바꿔주고, 거리에 따라 소리 조절
    void ClipChange(float dist)
    {
        if (clipObj != null)
        {
            if (dist <= 5f)
            {
                source.clip = clipObj.clip;
                source.volume = 1 / dist;
            }
            else
            {
                clipObj = null;
            }
        }
        else
        {
            source.clip = null;
        }
    }

    void ClipNonActive()
    {
        if (clipObj !=null && !clipObj.gameObject.activeInHierarchy)
        {
            clipObj = null;
        }
    }
}
