using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*  1.  CanGrap_BJH
 *      해당 스크립트를 컴포넌트로 추가할시, 그랩을 할 수 있게 된다.
 *      object에 tag형식이 아닌 스크립트를 추가해 그랩을 할 수 있게 함.
 *  
 *  2.  DisappearWall_BJH
 *      하나의 boxCollider에 넣어줄 스크립트.
 *      이 스크립트를 가진 콜라이더를 밟을 시, 벽이 생성
 *      ex) 문제를 풀고 나서 이 스크립트를 가진 boxCollider를 밟아 다른 통로로 가는 길을 막을 wall을 생성
 * 
 *  3.  OpenDoor_BJH
 *      문의 pivot을 변경한 빈 오브젝트에 넣어야한다.
 *      문을 열리게 하는 기능을 가짐
 *      해당 스크립트를 컴포넌트로 가진 빈 오브젝트에는 Animator를 추가해줘야한다.
 *      이때, Animator의 Controller에는 DoorPivot을 넣어줘야함.
 *      
 *  4.  OnCollider_BJH
 *      문 오브젝트 앞에 둘 BoxCollider를 가진 빈 오브젝트에 추가해주면 된다.
 *      
 *  5.  PlayerShift_BJH
 *      해당 스크립트는 오른쪽 컨트롤러에 넣어주어야한다.
 *      시프트 방식으로 이동할 수 있게 한다.
 *      lr, raycast, shift 모두 이 스크립트 안에 들어있다.
 *      
 *  6.  VRGrip_BJH
 *      해당 스크립트는 양쪽 컨트롤러에 모두 넣어준다.
 *      물건을 잡을 수 있는 기능을 가짐.
 *      이때 물건들은 CanGrap_BJH을 가지고 있어야한다.
 *      CanGrap_BJH 스크립트를 컴포넌트로 갖고 있지 않을 시 물건을 잡을 수 있는 기능을 사용할 수 없다.
 */