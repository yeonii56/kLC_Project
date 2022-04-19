using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube_JYJ : MonoBehaviour
{
    // PlankGame_Prefeb > Cube에 적용
    // PlankGame, Gift에 사용할 풀링 오브젝트

    Pooling_JYJ pool;

    public Color[] color;

    MeshRenderer meshRenderer;

    float timer;
    float lifeTime;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        pool = FindObjectOfType<Pooling_JYJ>();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        ActFalse();
    }

    // lifeTime을 초과하면 풀링 Dstroy함수 호출하여 큐브 돌려줌
    void ActFalse()
    {
        if(gameObject.activeSelf)
        {
            timer += Time.deltaTime;
        }

        if(timer > lifeTime)
        {
            pool.DestroyObj(1, gameObject);
            timer = 0f;
        }
    }

    // 초기화
    void Init()
    {
        Color();
        Size();
        timer = 0f;
        lifeTime = 10f;
    }

    // 큐브 색 초기화
    void Color()
    {
        int random = Random.Range(0, 3);
        meshRenderer.material.color = color[random];
    }

    // 큐브 크기 초기화
    void Size()
    {
        float random = Random.Range(0.8f, 1.2f);
        transform.localScale *= random;
    }
}
