using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plank_JYJ : MonoBehaviour
{
    // PlankGame > Plank에 적용
    // 바닥에 닿으면 큐브 생성되는 기능

    SoundOneShot_JYJ soundClip;
    Pooling_JYJ pool;

    [SerializeField] Transform createPos;

    GameObject cube;

    Vector3 cubePos;

    void Awake()
    {
        pool = FindObjectOfType<Pooling_JYJ>();
        soundClip = GetComponent<SoundOneShot_JYJ>();
        cubePos = createPos.localPosition;
    }

    // 바닥과 닿으면 소리나고 풀링 큐브 생성
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("CanTeleport"))
        {
            SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
            Cube(collision);
        }        
    }

    // 바닥에 닿으면 반대쪽에 풀링 cube 가져와서 생성함
    void Cube(Collision collision)
    {       
        ContactPoint contact = collision.contacts[0];

        // 바닥이 닿은 쪽이 어딘지 확인해서 반대쪽에 createPos 두기
        if (contact.point.x < transform.position.x)
        {
            createPos.localPosition = cubePos;
        }
        else
        {
            Vector3 pos = new Vector3(cubePos.x, -cubePos.y, cubePos.z);
            createPos.localPosition = pos;
        }

        // 풀링 큐브 createPos 위치에 가져오기
        cube = pool.ActiveObj(1);
        cube.transform.position = createPos.position;
    }

}
