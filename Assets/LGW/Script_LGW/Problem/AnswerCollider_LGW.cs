using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class AnswerCollider_LGW : MonoBehaviour
{
    public GameObject nextRoom = null;
    public GameObject curRoom = null;
    public GameObject player = null;
    public Text lastScore = null;
    public Text lastTxt = null;
    public ProblemData_LGW problemData = null;
    public int answerIndex = 0;
    public int problemNum = 0;
    private void OnTriggerEnter(Collider other)
    {
        // 현재 문제의 정답 Index와 지금 이 콜라이더가 가지고 있는 인덱스가 같다면 점수추가.
        if (other.CompareTag("PlayerHead"))
        {
            if (problemData.answerIndex[problemNum] == answerIndex)
            {
                StartCoroutine(PosChange());
                StopCoroutine(PosChange());
                problemData.score += 10;
                lastScore.text = problemData.score.ToString() + "점";
                Debug.Log("정답");
                problemData.scoreTxt.text = problemData.score.ToString() + "점";
                problemData.commentaryTxt.text = problemData.commentary[problemNum];
                if (problemData.score >= 0 && problemData.score <= 30)
                {
                    lastTxt.text = "조금 더 노력해봐요!";
                }
                else if(problemData.score >= 40 && problemData.score <= 70)
                {
                    lastTxt.text = "잘했어요!";
                }
                else if(problemData.score >= 80 && problemData.score <= 100)
                {
                    lastTxt.text = "훌륭해요!";
                }
            }
            else if (problemData.answerIndex[problemNum] != answerIndex)
            {
                StartCoroutine(PosChange());
                StopCoroutine(PosChange());
                Debug.Log("오답");
                problemData.commentaryTxt.text = problemData.commentary[problemNum];
            }
        }
    }
    IEnumerator PosChange()
    {
        SteamVR_Fade.View(Color.black, 0.25f);
        yield return new WaitForSeconds(0.25f);
        nextRoom.SetActive(true);
        curRoom.SetActive(false);
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 4);
        SteamVR_Fade.View(Color.clear, 0.25f);
    }
}
