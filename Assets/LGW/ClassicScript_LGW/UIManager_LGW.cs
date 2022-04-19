using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

public class UIManager_LGW : MonoBehaviour
{
    /* 전체적인 UI 담당 스크립트
     * 1. 통로에 따라 점수 변환을 시켜준다
     * 2. 정답통로에 들어가면 +1 점씩 쌓아준다
     * 3. 게임 시작, 게임 설명, 게임 종료
     * 4. 게임 시작시 테마 설정화면 보이도록 한다.
     * 5. 테마 화면에서는 클래식 모드만 잠금이 풀려있고 공포와 보물찾기는 잠금상태
     * 6. 좌측 컨트롤러 트리거 클릭 시 컨트롤러 위에 점수가 뜨도록 설정
     */

    public GameObject scorePanel;
    public GameObject menuPanel;
    public Text scoreTxt;
    public SteamVR_Input_Sources leftHand;
    public SteamVR_Action_Boolean Trigger;
    public SteamVR_Action_Boolean addScore;
    public SteamVR_Action_Boolean leftX;
    public int score = 0;
    private bool isPause;

    void Update()
    {
        if (Trigger.GetStateDown(leftHand))
            scorePanel.SetActive(true);
        else if (Trigger.GetStateUp(leftHand))
            scorePanel.SetActive(false);

        if (addScore.GetStateDown(leftHand))
        {
            score++;
            scoreTxt.text = "현재 점수 : " + score;
        }

        if (leftX.GetStateDown(leftHand))
        {
            if (isPause == false)
            {
                isPause = true;
                Time.timeScale = 0;
                menuPanel.SetActive(isPause);
            }
            else if (isPause == true)
            {
                isPause = false;
                Time.timeScale = 1;
                menuPanel.SetActive(isPause);
            }
        }
    }
}
