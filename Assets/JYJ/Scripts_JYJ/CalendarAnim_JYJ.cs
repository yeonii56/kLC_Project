using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarAnim_JYJ : MonoBehaviour
{
    // Calendar_Prefeb > Calendar > Month(1~12)에 적용
    // 달력 넘기는 기능

    SoundOneShot_JYJ soundClip;
    
    Transform center; // 중심축을 위로 만들기 위해 부모 설정

    bool isPass;

    void Awake()
    {
        center = transform.parent;
        soundClip = center.GetComponent<SoundOneShot_JYJ>();
    }

    void Start()
    {
        isPass = false;
    }

    // 컨트롤러와 닿으면 달력이 넘어감
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPass)
        {
            PassMonth();
        }
    }

    // 달력 넘어가는 기능
    void PassMonth()
    {
        SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
        StartCoroutine("PassAnim");
    }

    // 달력이 넘어가는 거 동적으로 보이기 위해서 코루틴 사용
    IEnumerator PassAnim()
    {
        isPass = true;
        int count = 0;
        while(count <= 310)
        {
            center.transform.Rotate(Vector3.right);
            count++;
            yield return null;
        }
        isPass = false;
    }
}
