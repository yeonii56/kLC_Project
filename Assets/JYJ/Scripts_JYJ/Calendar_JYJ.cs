using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar_JYJ : MonoBehaviour
{
    // 사용 안함...더 자연스럽도록 CalendarAnim_JYJ로 교체함 !!

    // Calendar_Prefeb > Calendar > Month(1~12)에 적용
    // 달력 넘기는 기능

    SoundOneShot_JYJ soundClip;

    [SerializeField] Transform dir; // Calendar > dir, 각도 계산을 위해 필요

    Transform center;

    float angleF;
    bool sound = false;

    void Awake()
    {
        center = transform.parent;
        soundClip = transform.parent.GetComponent<SoundOneShot_JYJ>();
    }

    void Start()
    {
        sound = false;
    }

    void Update()
    {
        PassToMonth();
    }

    // 달력을 60도 이상 회전시키면 소리가 나며 달이 넘어가는 기능
    void PassToMonth()
    {
        if (angleF >= 60f && !sound)
        {
            Quaternion angle = Quaternion.Euler(300, 0, 0);
            SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
            sound = true;
            center.rotation = angle;
        }
    }

    // 컨트롤러와 닿으면 컨트롤러의 각도로 회전함
    void OnTriggerStay(Collider other)
    {
        AngleChange(other);
    }

    // 달력 끝의 각도를 controller의 각도와 갖게 만듦.
    void AngleChange(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            angleF = Vector3.Angle(dir.position - center.position, other.transform.position - center.position);
            Quaternion angle = Quaternion.Euler(angleF, 0, 0);
            center.rotation = angle;
        }
    }
}
