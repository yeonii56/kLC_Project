using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearWall_BJH : MonoBehaviour
{
    //WallCollider에 추가하는 스크립트
    //기능 : 이 스크립트의 gameObject와 충돌시, 아래 public으로 받은 Object를 나타나게함. (SetActive(true)로 만듦.)

    public GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wall.SetActive(true);
        }
    }
}
