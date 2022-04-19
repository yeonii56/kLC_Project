using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishSoap_JYJ : MonoBehaviour
{
    // WasingDishes_Prefeb > DishSoap > DishSoapTop에 적용
    // 닿으면 퐁퐁 나오는 기능

    // 퐁퐁 오브젝트 배열. Scourer에서 크기 가져가므로 public
    public GameObject[] pongs;

    [SerializeField] Transform pongPos;

    // 코루틴 한번만 호출하도록 불값 지정
    bool isDown;

    void Awake()
    {
        PongsInit();
    }

    void Start()
    {
        isDown = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // 컨트롤러와 닿으면 퐁퐁 나오는 기능
        if (other.CompareTag("Player") && !isDown)
        {
            if (ActivePong() != null)
                ActivePong().SetActive(true);
            StartCoroutine("SoapTopDown");
        }
    }

    // 퐁퐁 오브젝트 배열에 넣어줌
    void PongsInit()
    {
        pongs = new GameObject[pongPos.childCount];

        for (int i = 0; i < pongPos.childCount; i++)
        {
            pongs[i] = pongPos.GetChild(i).gameObject;
        }
    }

    // 사용하고 있지 않은 퐁퐁 오브젝트 있으면 반환해주기
    GameObject ActivePong()
    {
        GameObject pong;
        for (int i = 0; i < pongs.Length; i++)
        {
            if (!pongs[i].activeInHierarchy)
            {
                pong = pongs[i];
                return pong;
            }
        }
        return null;
    }

    // 퐁퐁 펌프가 내려갔다 올라오는 기능
    IEnumerator SoapTopDown()
    {
        isDown = true;
        while (transform.localPosition.z <= -3.5f)
        {
            transform.Translate(Vector3.forward * 0.1f, Space.Self);
            yield return null;
        }
        if (transform.localPosition.z >= -3.5f)
        {
            while (transform.localPosition.z >= -4.2f)
            {
                transform.Translate(Vector3.forward * -0.1f, Space.Self);
                yield return null;
            }
        }
        isDown = false;
    }

    // 퐁퐁 1초 뒤에 사라지게하기
    IEnumerator PongFalse()
    {
        GameObject pong = ActivePong();
        pong.SetActive(true);

        yield return new WaitForSeconds(1f);
        pong.SetActive(false);
        pong.transform.position = pongPos.position;
    }
}
