using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus_JYJ : MonoBehaviour
{
    // Octopus_Prefeb >> Octopus에 적용
    // Player에 가까워지면 도망감
    // Controller에 닿으면 먹물 뿜음

    Transform player; // player
    Transform trans; 
    Animator anim;
    ParticleSystem particle;

    Quaternion oriRotates;

    float distance;
    float speed;

    void Awake()
    {
        player = GameObject.FindWithTag("PlayerHead").transform;
        trans = GetComponent<Transform>();
        anim = transform.GetComponentInChildren<Animator>();
        particle = transform.GetComponentInChildren<ParticleSystem>();
    }

    void Start()
    {
        OctoPusInit();
    }   

    void Update()
    {
        RunAway();
        RotateUp();
    }

    // Controller랑 닿으면 먹물 뿜음
    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            StopCoroutine("Bubble");
            StartCoroutine("Bubble");
        }
    }

    // 초기화 함수
    void OctoPusInit()
    {
        oriRotates = trans.rotation;
        particle.Stop();
        anim.SetBool("isMove", false);
        speed = 0.1f;
    }

    // Player랑 가까워지면 도망감
    void RunAway()
    {
        distance = Vector3.Distance(player.position, trans.position);

        if(distance < 2f)
        {
            Vector3 direction = transform.position - player.position;
            transform.Translate(direction * Time.deltaTime * speed, Space.World);
            anim.SetBool("isMove", true);
        }
        else
        {
            anim.SetBool("isMove", false);
        }
    }

    void RotateUp()
    {
        if (trans.rotation != oriRotates)
        {
            trans.rotation = oriRotates;
        }
    }

    // 먹물 동적으로 뿜는 코루틴 함수
    IEnumerator Bubble()
    {
        particle.Play();
        yield return new WaitForSeconds(1f);

        particle.Stop();
    }
}
