using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCollider_BJH : MonoBehaviour
{
    //DoorCollider_BJH 스크립트
    //해당 스크립트는 문 앞에 놓을 boxCollider에 추가해야함.
    //열릴 문을 doorObject에 넣으면 된다.
    public OpenDoor_BJH doorObject;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerHead"))
            doorObject.OnTriggerEnterDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHead"))
            doorObject.OnTriggerExitDoor();
    }
}
