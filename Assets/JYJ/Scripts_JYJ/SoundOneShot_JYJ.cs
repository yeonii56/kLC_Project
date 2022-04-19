using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOneShot_JYJ : MonoBehaviour
{
    // 소리가 일시적으로 나야하는 오브젝트에 모두 넣기
    // 이 컴퍼넌트를 가지고 있어야 SoundSystem_JYJ.soundSource를 받아와서 소리가 남.

    // 타입에 따라 소리 나는 상황 다르게 주기, 여러번 사용되는 기능으로 나눠줌
    enum Type { Trigger, Other, Eat, Grip, Action }
    [SerializeField] Type type;

    // Action타입의 경우 클립을 받아야 하므로 public
    public AudioClip clip;

    // Grip판별을 위해 가져옴
    Rigidbody rig;

    bool isGrip;

    void Awake()
    {
        SoundInit();
    }

    void SoundInit()
    {
        isGrip = false;

        if (this.type != Type.Other || this.type != Type.Action)
        {
            rig = GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        TypeIsGrip();
        TypeIsEat();
    }

    // Grip타입일 경우 Grip되면 소리남 -> 넘겨 받을 값이 없어서 Grip판별을 isKinematic으로 함.
    void TypeIsGrip()
    {
        if (type == Type.Grip)
        {
            if (rig.isKinematic && !isGrip) // 소리 한번만 나게 하기 위해서 bool값 사용
            {
                isGrip = true;
                SoundSystem_JYJ.source.PlayOneShot(clip);
            }
            else if (!rig.isKinematic)
            {
                isGrip = false;
            }
        }
    }

    // trigger타입일 경우 컨트롤러와 닿으면 밀쳐지면서 소리남
    void TypeIsTrigger(Collider other)
    {
        if (this.type == Type.Trigger && other.transform.CompareTag("Player"))
        {
            // 플레이어 반대 방향으로 밀쳐짐
            Vector3 dir = transform.position - other.transform.position;
            rig.AddForce(dir * 3f, ForceMode.Impulse);

            SoundSystem_JYJ.source.PlayOneShot(clip);
        }
    }

    // Eat타입일 경우 플레이어와 닿으면 사라지며 소리남
    void TypeIsEat()
    {
        if (this.type == Type.Eat && rig.isKinematic)
        {
            // 플레이어와 거리가 1f 이하일 경우
            float dist = Vector3.Distance(Camera.main.transform.position, transform.position);
            if(Mathf.Abs(dist) <= 0.3f)
            {
                SoundSystem_JYJ.source.PlayOneShot(clip);
                gameObject.SetActive(false);

                // 계속 먹히지 않기 위해서 상태 초기화
                rig.isKinematic = false;
                rig.useGravity = true;
            }
        }
    }

    // Other 경우 isTrigger가 꺼져있는 다른 물체와 충돌하면 소리남
    void TypeIsOther()
    {
        if (this.type == Type.Other)
        {
            SoundSystem_JYJ.source.PlayOneShot(clip);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        TypeIsTrigger(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        TypeIsOther();
    }
}
