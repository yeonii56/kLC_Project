using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarArray_JYJ : MonoBehaviour
{
    // Calendar_Prefeb에 적용
    // 달력 넘어가면 다음 달력 기능 켜지고 현재 달력 기능 꺼지게 함.

    Transform[] month;
    BoxCollider[] passTrigger;

    float passTime;
    bool isPass;

    void Start()
    {
        InitMonth();
        Init();
    }
   
    void Update()
    {
        PassMonth();
    }

    // 달력이 넘어가면 다음 달력 활성화
    void PassMonth()
    {
        for (int i = 0; i < month.Length - 1; i++)
        {
            // 현재 달력이 넘어가면
            if (passTrigger[i].enabled && month[i].rotation.eulerAngles.x >= 100f)
            {
                isPass = true;
            }

            // 다음 달력 활성화
            if (isPass)
            {
                StartCoroutine(WaitMonth(i));
            }
        }
    }

    // 이번 달 넘어가고 passTime 지나면 다음 달 넘길 수 있게 설정
    IEnumerator WaitMonth(int i)
    {
        isPass = false;
        NextMonthForward(i);

        // passTime 지나면 다음 달 넘길 수 있음
        yield return new WaitForSeconds(passTime);
        NextMonthActive(i);
        BeforeMonthRemove(i);
    }

    // 배열 초기화
    void InitMonth()
    {
        month = new Transform[12];
        passTrigger = new BoxCollider[12];

        for (int i = 0; i < month.Length; i++)
        {
            month[i] = transform.GetChild(i);
            passTrigger[i] = month[i].GetChild(0).GetComponent<BoxCollider>();
        }
    }

    // 변수 초기화
    void Init()
    {
        isPass = false;
        passTime = 0.8f;
    }

    // 이번 달 넘어가면 전 달 없애기
    void BeforeMonthRemove(int i)
    {
        if (i >= 1)
        {
            month[i - 1].gameObject.SetActive(false);
        }
    }

    void NextMonthForward(int i)
    {
        // 다음 달 달력을 맨앞으로 놓음
        if (!passTrigger[i + 1].enabled)
        {
            month[i + 1].Rotate(2, 0, 0);
        }

        // 다음 달 활성화하고, 넘어간 이번 달 넘기는 기능 끔
        month[i + 1].gameObject.SetActive(true);
        passTrigger[i].enabled = false;
    }

    void NextMonthActive(int i)
    {
        // 다다음 달 넘길 수 없게 해두고 활성화함. 다음 달 넘길 수 있게 함
        if (i < 10)
        {
            passTrigger[i + 2].enabled = false;
            month[i + 2].gameObject.SetActive(true);

            passTrigger[i + 1].enabled = true;
        }
    }
}
