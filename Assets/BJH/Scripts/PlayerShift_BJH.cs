using UnityEngine;
using Valve.VR;
using DG.Tweening;

public class PlayerShift_BJH : MonoBehaviour
{
    /*
     * 이 스크립트는 우측 컨트롤러에 넣어줘야한다.
     * 플레이어를 shift방식으로 이동하게 해주는 기능을 가짐.
     * 이때 CanRaycastLayer에는 텔레포트를 가능하게 할 Layer를 지정해준다.
     * (이 Layer로 지정된 오브젝트들에 Player는 텔레포트가 가능함)
    */
    /* 우측 컨트롤러의 트리거를 이용해서 쉬프트 이동을 구현하는 코드
 * 1. 우측 컨트롤러의 트리거 값을 받아온다
 * 2. 트리거가 GetState인 경우 레이캐스트를 쏴서 바닥 레이어 확인
 * 3. 이동이 가능한 태그, 레이어 라면 레이캐스트의 Hit지점에 Point를 보여준다
 * 4. 레이캐스트의 레이져는 LineRenderer를 이용해서 Bezier곡선을 이용해 곡선으로 나타낸다
 * 5. 이후 트리거의 상태가 GetStateUp이 된다면 해당 지점으로 이동하고 LineRenderer와 Point를 안보이게 한다
 * 6. 플레이어 이동은 DoTween의 DoMove함수를 이용한다.
 */
    public bool canRay = true;
    public float playerHead = 0;

    // 컨트롤러 입력값 받아오기
    public SteamVR_Input_Sources rightHand;
    public SteamVR_Action_Boolean rightTrigger;
    
    // 레이캐스트
    public LayerMask CanRaycastLayer;

    //SetActive(false)가 된 Sphere 오브젝트를 넣어준다.
    public Transform point; //이동 지점

    private Ray ray;
    private RaycastHit hitInfo;
    private float rayDistance = 10f; //이동 가능 거리
    
    // 라인랜더러
    private LineRenderer lr;
    private int lineCount = 100;

    private bool canTeleport;

    public GameObject Player;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = lineCount;
    }

    void Update()
    {
        if(canRay == true)
        {
            if (rightTrigger.GetState(rightHand))
            {
                DrawLaser();
                if (canTeleport == false)
                {
                    EraseLr();
                }
            }
            if (rightTrigger.GetStateUp(rightHand))
            {
                EraseLr();

                if (point.gameObject.activeSelf == true)
                {
                    point.gameObject.SetActive(false);
                }
                if (canTeleport == true)
                {
                    Teleport();
                }
            }
        }
    }

    // Teleport 함수에서는 이동을 담당 DoTween 사용
    private void Teleport()
    {
        Player.transform.DOMove(new Vector3(point.position.x, playerHead, point.position.z), 0.1f).SetEase(Ease.Linear);
    }
    // DrawLaser 함수에서는 이동 가능한 지점을 표현 LineRenderer사용
    private void DrawLaser()
    {
        //Line renderer 초기값 설정
        lr.SetPosition(0, transform.position);

        //Physics.Raycast 첫 번째 매개변수를 vector를 안쓰고 ray를 쓸 예정
         ray = new Ray(transform.position, transform.forward);

        //Raycast 생성
        if (Physics.Raycast(ray, out hitInfo, rayDistance, CanRaycastLayer))
        {
            //hitInfo의 layer를 가져올 수 없기 때문에 tag를 사용
            if (hitInfo.collider.tag == "CanNotTeleport")
            {
                point.gameObject.SetActive(false);
                canTeleport = false;
            }
            if (hitInfo.collider.tag == "CanTeleport")
            {
                if (point.gameObject.activeSelf == false)
                {
                    point.gameObject.SetActive(true);
                }

                point.position = hitInfo.point;

                DrawBezier(hitInfo);

                canTeleport = true;
                
            }
        }
        else
        {
            if (point.gameObject.activeSelf == true)
            {
                point.gameObject.SetActive(false);
            }

            //lr.SetPosition(1, transform.position + transform.forward * rayDistance);

            canTeleport = false;
        }
    
    }

    // 이차곡선 line renderer
    private void DrawBezier(RaycastHit hitInfo)
    {
        //hitpoint와 player 중간 지점
        Vector3 center = Vector3.Lerp(transform.position, hitInfo.point, 0.5f);

        //중간 지점의 y값
        // ***** 라인 렌더러가 아래로 휘는 문제 해결 바람
        //center.y = Vector3.Distance(transform.position, hitInfo.point) * 0.5f;
        float rayDist = Vector3.Distance(transform.position, hitInfo.point);

        //lr이 최고차항이 음수인 이차함수가 되는 것을 막기 위한 보정값 1f
        center.y = rayDist * 0.5f + 1f;

        //lr 생성
        for (int i = 0; i < lineCount; i++)
        {
            Vector3 v1 = Vector3.Lerp(transform.position, center, (float)i / lineCount);
            Vector3 v2 = Vector3.Lerp(center, hitInfo.point, (float)i / lineCount);
            Vector3 v3 = Vector3.Lerp(v1, v2, (float)i / lineCount);

            lr.SetPosition(i, v3);
        }

    }

    private void EraseLr()
    {
        for (int i = 0; i < lineCount; i++)
            {
                lr.SetPosition(i, transform.position);
            }
    }
}
