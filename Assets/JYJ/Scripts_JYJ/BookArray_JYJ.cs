using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookArray_JYJ : MonoBehaviour
{
    // Book_Prefeb > BookCenter에 적용
    // 책 넘어가는 기능
    // 자연스럽게하기 위해서 책표지 돌아갈 때 센터도 절반씩 같이 돌아가게 함.

    SoundOneShot_JYJ soundClip;

    Book_JYJ[] page;
    BoxCollider[] pageTrigger;
    BoxCollider boxCol;
    Rigidbody rig;

    bool isCoroutine; // 코루틴 한번만 호출하게 하는 불린 값

    void Awake()
    {
        soundClip = GetComponent<SoundOneShot_JYJ>();
        page = transform.GetComponentsInChildren<Book_JYJ>();
        pageTrigger = TriggerInit();

        boxCol = GetComponent<BoxCollider>();
        rig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!rig.isKinematic)
        {
            FirstPagePass();
            PageIsTrigger();
        }
    }

    // pageTrigger 배열 초기화
    BoxCollider[] TriggerInit()
    {
        pageTrigger = new BoxCollider[page.Length];
        for (int i = 0; i < page.Length; i++)
        {
            pageTrigger[i] = page[i].GetComponent<BoxCollider>();
        }
        return pageTrigger;
    }

    // 첫번째 페이지 넘어갈때는 책 중심도 그 절반만큼 돌아가게 함.
    void FirstPagePass()
    {
        if (page[0].isTriggerPage && !isCoroutine)
        {
            SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
            StartCoroutine(OutSideTouch(0));
        }       
    }

    // 페이지에 닿으면 넘어가는 코루틴 호출, 다른 페이지 넘어가고 있으면 호출x
    void PageIsTrigger()
    {
        for (int i = 1; i < page.Length; i++)
        {
            if (page[i].isTriggerPage && !isCoroutine)
            {
                SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
                StartCoroutine(NextPage(i));
            }
        }
    }

    // 박스콜라이더 사이즈 책 펴지면 커지고 닫으면 작아짐
    void BoxSize(int index)
    {
        float boxZ = (index == 0 ? 12f : 1.8f);

        Vector3 boxSize = new Vector3(boxCol.size.x, boxCol.size.y, boxZ);
        boxCol.size = boxSize;
    }

    // 책 넘어가는 거 자연스럽게 보이기 위해서
    // 센터 우측으로 나머지 좌측으로 회전 45도 만큼 하고나서 첫페이지 우측으로 회전
    IEnumerator OutSideTouch(int index)
    {
        isCoroutine = true;

        BoxSize(index);

        int angle = 0;
        while (angle < 45) 
        {
            // 센터 우측회전
            transform.Rotate(Vector3.down);
            

            // 나머지 좌측회전
            for (int i = (index == 0 ? 1 : 0); i <  (index == 0 ? pageTrigger.Length : pageTrigger.Length -1); i++)
            {
                pageTrigger[i].transform.Rotate(Vector3.up);
            }
            angle++;
            yield return null;
        }

        if (index == 0) 
            yield return StartCoroutine(FirstPage());
    }

    // 센터 회전 다 하면 첫페이지 우측회전
    IEnumerator FirstPage()
    {
        int angle = 0;
        Transform trans = pageTrigger[0].transform;

        while (angle <= 45)
        {
            trans.Rotate(Vector3.down);
            angle++;
            yield return null;
        }
        pageTrigger[0].enabled = false;
        pageTrigger[1].enabled = true;

        isCoroutine = false;
        page[0].isTriggerPage = false;
    }

    // 2~6페이지 넘어가는 기능
    IEnumerator NextPage(int i)
    {
        isCoroutine = true;

        // 90도 까지 돌아감
        int angle = 0;
        int achiveAngle;
        achiveAngle = (i == page.Length-1 ? 45 : 90);
        
        while (angle <= achiveAngle)
        {
            pageTrigger[i].transform.Rotate(Vector3.down);
            angle++;

            yield return null;
        }
        // 다 돌아가면 박스콜라이더 끄기       
        pageTrigger[i].enabled = false;

        // 다음 페이지 박스콜라이더 활성화
        if (i < page.Length-1)
        {
            pageTrigger[i + 1].enabled = true;
        }      
        else
        {
            yield return StartCoroutine(OutSideTouch(page.Length-1));
            pageTrigger[0].enabled = true;
        }

        // Book_JYJ에서 넘겨받은 충돌 확인 불린 값 false로 변경해주기 -> 코루틴 여러번호출 안되게
        page[i].isTriggerPage = false;

        isCoroutine = false;
    }
}
