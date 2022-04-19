using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Answer Data", menuName = "Scriptable Object/Answer Data", order = int.MaxValue)]
public class AnswerData_LGW : ScriptableObject
{
    [SerializeField]
    private string problem;
    public string Problem { get { return problem; } }
    [SerializeField]
    private string choice1;
    public string Choice1 { get { return choice1; } }
    [SerializeField]
    private string choice2;
    public string Choice2 { get { return choice2; } }
    [SerializeField]
    private string answer;
    public string Answer { get { return answer; } }
    // 정답의 인덱스를 가져온다.
    // 만약 콜라이더를 지나갈 때 콜라이더에 들어있는 int값 0 || 1이
    // answerIndex와 같다면 점수를 +해주고 다르면 return null;
    [SerializeField]
    private int answerIndex;
    public int AnswerIndex { get { return answerIndex; } }
    [SerializeField]
    private string commentary;
    public string Commentary { get { return commentary; } }

}
