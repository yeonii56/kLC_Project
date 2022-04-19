using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRGrip_BJH : MonoBehaviour
{
    //ver01
    //Grab을 하는 방법 1.부모설정 2.FixedJoint

    public SteamVR_Action_Boolean grip;

    private SteamVR_Behaviour_Pose myController = null;
    private Transform myTransform = null;

    private Rigidbody otherRigidbody = null;

    //동시에 부딪힌 오브젝트들의 Rigidbody를 저장하는 변수
    //제일 가까운 물체의 Rigidbody를 반환할 예정 - 플레이어가 잡고 싶은 Object
    private List<Rigidbody> contactRigidbodies = new List<Rigidbody>();

    private Transform prefebParents_JYJ;

    void Start()
    {
        myController = GetComponent<SteamVR_Behaviour_Pose>();
        myTransform = GetComponent<Transform>();
    }

    void Update()
    {
        Grab();
        Drop();
    }
    
    public void Grab()
    {
        if (grip.GetStateDown(myController.inputSource))
        {
            otherRigidbody = GetNearestRigidbody();

            if (otherRigidbody == null)
            {
                return;
            }

            otherRigidbody.useGravity = false;
            otherRigidbody.isKinematic = true;

            otherRigidbody.transform.position = myTransform.position;
            prefebParents_JYJ = otherRigidbody.transform.parent;
            otherRigidbody.transform.parent = myTransform;
        }
    }

    public void Drop()
    {
        if (grip.GetStateUp(myController.inputSource))
        {
            if (otherRigidbody == null)
            {
                return;
            }

            otherRigidbody.useGravity = true;
            otherRigidbody.isKinematic = false;

            otherRigidbody.transform.parent = prefebParents_JYJ;

            otherRigidbody.velocity = myController.GetVelocity(); //농구공
            otherRigidbody.angularVelocity = myController.GetAngularVelocity();

            otherRigidbody = null;
        }
    }

    private Rigidbody GetNearestRigidbody()
    {
        Rigidbody nearestRigidBody = null;

        float minDistance = float.MaxValue;
        float distance = 0f;

        foreach( Rigidbody rigidbody in contactRigidbodies)
        {
            //sqrMagnitude는 거리 비교에 사용함.  Magnitude, distance와 다름
            distance = (rigidbody.transform.position - myTransform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestRigidBody = rigidbody; 
            }
        }

        return nearestRigidBody;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CanGrap_BJH>() != null) //그냥 tag보다 comparetag 효율이 더 좋음
        {
            contactRigidbodies.Add(other.gameObject.GetComponent<Rigidbody>());
            
        }   
    }

    private void OnTriggerExit(Collider other)
    {
        // 그냥 tag보다 comparetag 효율이 더 좋음
        // 07.19 다른 스크립트와 태그 충돌로 인해 문제가 생길 것 같아 조건문을 바꿈
        if (other.gameObject.GetComponent<CanGrap_BJH>() != null)  
        {
            contactRigidbodies.Remove (other.gameObject.GetComponent<Rigidbody>());

        }
    }

    // 컨트롤러와 가까운 Object 판별 함수
}
