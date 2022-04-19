using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chocolate_JYJ : MonoBehaviour
{
    // Chocolate_Prefeb에 적용
    // 초콜릿 먹은 후에 다시 활성화되는 기능

    GameObject[] chocolates;

    Vector3[] choPos;

    bool isCoroutine;

    void Start()
    {
        Init();
    }

    void Update()
    {
        ChocolateReset();
    }

    // 초콜렛이 먹혔으면 1초 후에 다시 활성화됨
    void ChocolateReset()
    {
        for (int i = 0; i < chocolates.Length; i++)
        {
            if (!chocolates[i].activeInHierarchy && !isCoroutine)
            {
                StartCoroutine(ChocolateActive(i));
            }
        }
    }

    // 초기화
    void Init()
    {
        // 코루틴 한번만 호출하기 위해 불값 지정
        isCoroutine = false;

        // 자식으로 있는 초콜릿들 배열에 넣어줌
        chocolates = new GameObject[transform.childCount];
        for (int i = 0; i < chocolates.Length; i++)
        {
            chocolates[i] = transform.GetChild(i).gameObject;
        }

        // 초콜릿이 재생성될 위치 배열에 담아둠
        choPos = new Vector3[transform.childCount];
        for (int i = 0; i < choPos.Length; i++)
        {
            choPos[i] = chocolates[i].transform.position;
        }
    }

    // 초콜릿이 먹히고 3초가 지나면 처음 자리에서 다시 활성화 됨
    IEnumerator ChocolateActive(int i)
    {
        isCoroutine = true;
        yield return new WaitForSeconds(3f);

        chocolates[i].transform.position = choPos[i];
        chocolates[i].SetActive(true);
        isCoroutine = false;
    }
}
