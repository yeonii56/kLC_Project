using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPooling_JYJ : MonoBehaviour
{
    // 풀링을 사용할 오브젝트(프리펩)가 다르면 로직을 새로 작성해야 해서 이 로직 사용x
    // 새로 여러 오브젝트 이용할 수 있는 풀링 스크립트(Pool_JYJ) 작성함.

    // Pool_Prefeb > Pool에 적용하려고 했음

    [SerializeField] GameObject cube;

    Queue<Cube_JYJ> queue;

    void Awake()
    {
        InQueue();
    }

    // 오브젝트 생성
    Cube_JYJ CreateObj()
    {
        Cube_JYJ obj = Instantiate(cube, transform).GetComponent<Cube_JYJ>();

        obj.gameObject.SetActive(false);

        return obj;
    }

    // 초기화, 풀링 오브젝트 20개 만들어서 큐에 넣어둠
    void InQueue()
    {
        queue = new Queue<Cube_JYJ>();

        for (int i = 0; i < 20; i++)
        {
            queue.Enqueue(CreateObj());
        }
    }

    // 오브젝트 큐에서 꺼내서 활성화하고 넘겨줌
    public Cube_JYJ ActiveObj()
    {
        // 미리 만들어둔 오브젝트가 다 사용중이면 새로 만들어서 넘겨줌
        // 아니면 큐에 있는 오브젝트 꺼내서 넘겨줌
        if(queue.Count > 0)
        {
            Cube_JYJ obj = queue.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            Cube_JYJ obj = CreateObj();
            obj.gameObject.SetActive(true);
            return obj;
        }
    }

    // 오브젝트 활성화 끄고 다시 큐에 넣음
    public void DestroyObj(Cube_JYJ obj)
    {
        queue.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
