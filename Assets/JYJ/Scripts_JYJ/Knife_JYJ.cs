using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife_JYJ : MonoBehaviour
{
    // SliceFruits_Prefeb > Knife에 적용
    // 과일에 닿으면 과일이 썰리고 잘린 과일은 먹을 수 있음
    // 과일 양쪽을 먹으면 새로 썰 수 있는 과일 생김

    SoundOneShot_JYJ sliceClip;

    [SerializeField] GameObject[] fruits;
    [SerializeField] GameObject[] slices;
    [SerializeField] Transform trans;

    Rigidbody rig;

    Quaternion angle;

    void Awake()
    {
        FruitsArrayInit();
        sliceClip = GetComponent<SoundOneShot_JYJ>();
        rig = GetComponent<Rigidbody>();
        angle = Quaternion.Euler(-135f, 90f, 0f);
    }

    void Update()
    {
        Eat();
    }

    void OnCollisionEnter(Collision collision)
    {
        Slice(collision);
    }

    // 짝수에 안자른 과일 넣음. 과일이 칼에 닿았을 때 잘림
    void Slice(Collision collision)
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            if (i % 2 == 0 && collision.gameObject == fruits[i] && rig.isKinematic)
            {
                SoundSystem_JYJ.source.PlayOneShot(sliceClip.clip);
                fruits[i].SetActive(false);
                fruits[i + 1].transform.position = fruits[i].transform.position;
                fruits[i + 1].SetActive(true);
            }
        }
    }

    // 과일 배열 초기화
    void FruitsArrayInit()
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            if (i % 2 == 0)
            {
                fruits[i].SetActive(true);
            }
            else
            {
                fruits[i].SetActive(false);
            }
        }
    }

    // 잘린 모두 먹으면 새로 잘를 과일 활성화됨.
    void Eat()
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            if (i % 2 == 0 && !fruits[i].activeSelf)
            {
                if (!slices[i].activeSelf && !slices[i + 1].activeSelf)
                {
                    fruits[i + 1].SetActive(false);
                    slices[i].SetActive(true);
                    slices[i + 1].SetActive(true);

                    fruits[i].transform.position = trans.position;
                    fruits[i].SetActive(true);
                }
            }
        }
    }
}
