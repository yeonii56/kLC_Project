using System;
using UnityEngine;

public class Clock_JYJ : MonoBehaviour
{
    // Clock_Prefeb > Clock에 적용
    // 현재 시간에 맞게 돌아가는 시계 기능

    Transform[] clocks;   

    int second;
    int minute;
    int hour;

    void Awake()
    {
        SetClocks();
    }

    void Update()
    {
        SetIntClcok();
        FlowClock();
    }

    // clocks 초기화, 시분초 받아서 clocks[]에 넣어줌
    void SetClocks()
    {
        clocks = new Transform[3];

        for (int i = 0; i < clocks.Length; i++)
        {
            clocks[i] = transform.GetChild(i);
        }
    }

    // 현재 시각 받아옴
    void SetIntClcok()
    {
        second = int.Parse(DateTime.Now.ToString("ss"));
        minute = int.Parse(DateTime.Now.ToString("mm"));
        hour = int.Parse(DateTime.Now.ToString("hh"));
    }

    // 현재 시각에 맞게 시분초침 각도 회전
    void FlowClock()
    {
        Quaternion secRotate = Quaternion.Euler(0, -(360/ 60) * (second), 0);
        Quaternion minRotate = Quaternion.Euler(0, -(360/ 60) * (minute), 0);
        // hour은 같은 시각일지라도 분에 따라서 움직임 조절
        Quaternion houRotate = Quaternion.Euler(0, -((360/ 12) * (hour) + ((360 / 60)* (minute/12))), 0);

        clocks[0].localRotation = houRotate;
        clocks[1].localRotation = minRotate;
        clocks[2].localRotation = secRotate;
    }
}
