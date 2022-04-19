using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scourer_JYJ : MonoBehaviour
{
    // WasingDishes_Prefeb > Scourer에 적용
    // 퐁퐁과 닿은 후 다른 물체와 닿으면 거품나며 닦이는 기능
    [SerializeField] DishSoap_JYJ dishSoapTop;

    [SerializeField] Transform dish;
    [SerializeField] AudioClip soundClip;

    GameObject[] pongs;
    Rigidbody rig;
    ParticleSystem dishBubble;
    ParticleSystem bubble;

    bool canBubble;

    void Awake()
    {
        bubble = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        dishBubble = dish.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        rig = GetComponent<Rigidbody>();
    }

    void Start()
    {
        ScourerInit();
    }

     void OnTriggerEnter(Collider other)
    {
        // 충돌체가 퐁퐁이면 스펀지에 거품 나고 최소 한번은 퐁퐁과 닿아야 설거지 가능함
        for (int i = 0; i < pongs.Length; i++)
        {
            if (other.gameObject == pongs[i])
            {
                if (!canBubble)
                {
                    canBubble = true;
                }
                StopCoroutine("bubbling");
                StartCoroutine("bubbling");
            }
        }

        // 퐁퐁과 한 번은 닿았고, 충돌체가 접시이고 닿은 채로 스펀지를 움직이면 접시에 거품남.
        if (other.transform == dish && canBubble && rig.isKinematic)
        {
            dishBubble.Play();
            SoundSystem_JYJ.source.PlayOneShot(soundClip);
        }
    }


    void OnTriggerExit(Collider other)
    {
        // 접시에서 떨어지면 거품 안 나고, 소리도 멈춤
        if (other.transform == dish && dishBubble.isPlaying)
        {
            dishBubble.Pause();
            SoundSystem_JYJ.source.Stop();

        }
    }

    // 멤버 초기화
    void ScourerInit()
    {
        canBubble = false;
        bubble.Stop();
        dishBubble.Stop();

        pongs = new GameObject[dishSoapTop.pongs.Length];
        pongs = dishSoapTop.pongs;
    }

    // 퐁퐁에 닿았을 때 거품을 좀 더 동적으로 보이게 함
    IEnumerator bubbling()
    {
        bubble.Play();
        yield return new WaitForSeconds(1f);
        bubble.Pause();
    }
}
