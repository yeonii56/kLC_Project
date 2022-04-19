using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift_JYJ : MonoBehaviour
{
    // Gift_Prefeb > Gift > Trigger에 적용
    // 상자 뚜껑이 열리면 터지며 소리나는 기능

    Pooling_JYJ pool;

    [SerializeField] Transform trans;
    [SerializeField] GameObject top;
    [SerializeField] AudioClip clip;

    GameObject cube;

    void Awake()
    {
        pool = FindObjectOfType<Pooling_JYJ>();
    }

    // 풀링 오브젝트 받아오기
    void CubeMove(int count)
    {
        for (int i = 0; i < count; i++)
        {
            cube = pool.ActiveObj(1);
            cube.transform.position = trans.position;;
        }
    }

    // 뚜껑이 열리면 풀링 오브젝트 생성해서 터지는 효과
    void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject == top)
        {
            SoundSystem_JYJ.source.PlayOneShot(clip);
            CubeMove(30);
        }
    }
}
