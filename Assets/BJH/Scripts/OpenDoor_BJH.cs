using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 준비물 : 문의 Pivot을 바꾼 빈 오브젝트, 문 오브젝트
// 실제 문 오브젝트의 부모를 문의 Pivot을 바꾼 빈 오브젝트로 설정한다.
// 이때 이 스크립트는 문의 Pivot을 바꾼 빈 오브젝트에 넣어준다.
// InteractDoor이라는 이름을 가진 Collider 위에 플레이어가 올라갈 시 DoorPivot 애니메이션 작동

public class OpenDoor_BJH : MonoBehaviour
{
    private Animator doorAnimator;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    public void OnTriggerEnterDoor()
    {
        doorAnimator.SetBool("IsOnTrigger", true);
    }

    public void OnTriggerExitDoor()
    {
        doorAnimator.SetBool("IsOnTrigger", false);
    }

}
