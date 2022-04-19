using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling_JYJ : MonoBehaviour
{
    // Pool_Prefeb > Pool에 적용
    // objcetPooling기능. 여러가지 프리팹을 풀링으로 활용할 수 있게 Dictionary사용
    // Basketball, Gift, PlankGame에 사용함

    // 풀링 프리팹의 배열 인덱스와 그 프리펩에 맞는 큐 연결함
    Dictionary<int, Queue<GameObject>> dict;

    [SerializeField] 
    GameObject[] prefebs;

    void Awake()
    {
        DictInit();
        QueuInit();
    }

    // 큐 초기화. 프리팹 개수만큼 큐 생성
    void QueuInit()
    {
        for (int prefebIndex = 0; prefebIndex < prefebs.Length; prefebIndex++)
        {
            InQueue(prefebIndex);
        }
    }

    // dict초기화. 프리팹 개수만큼 딕셔너리 생성해서 큐 넣어줌
    void DictInit()
    {
        dict = new Dictionary<int, Queue<GameObject>>();

        for (int prefebIndex = 0; prefebIndex < prefebs.Length; prefebIndex++)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            dict.Add(prefebIndex, queue);
        }
    }

    // 풀링 오브젝트 생성 함수
    GameObject CreateObj(int prefebIndex)
    {
        GameObject obj = Instantiate(prefebs[prefebIndex], transform);
        obj.SetActive(false);

        return obj;
    }

    // 생성한 큐에 20개씩 미리 풀링 오브젝트 만들어서 넣어둠
    void InQueue(int prefebIndex)
    {
        if (dict.TryGetValue(prefebIndex, out Queue<GameObject> queue))
        {
            for (int j = 0; j < 20; j++)
            {
                queue.Enqueue(CreateObj(prefebIndex));
            }
        }
    }

    // 큐에서 풀링 오브젝트 꺼내서 활성화하고 넘겨줌
    public GameObject ActiveObj(int prefebIndex)
    {
        // 인트값 받아와서 받아온 값에 맞는 큐 queue로 가져옴
        if (dict.TryGetValue(prefebIndex, out Queue<GameObject> queue))
        {
            // 만들어둔 풀링 오브젝트가 남아있으면 큐에서 꺼내서 넘겨줌
            if (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            // 만들어둔 풀링 오브젝트가 모두 사용중이면 새로 생성해서 넘겨줌
            else
            {
                GameObject obj = CreateObj(prefebIndex);
                obj.SetActive(true);
                return obj;
            }
        }
        else return null;    
    }

    // 넘겨준 풀링 오브젝트 다시 큐에 넣어주고 비활성화함
    public void DestroyObj(int prefebIndex, GameObject obj)
    {
        if (dict.TryGetValue(prefebIndex, out Queue<GameObject> queue))
        {
            queue.Enqueue(obj);
            obj.SetActive(false);
        }
    }
}
