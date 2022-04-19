using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProblemData_LGW : MonoBehaviour
{
    public int Num = 10;
    public int score = 0;
    public Text[] easyTxt;
    public Text[] moderateTxt;
    public Text[] difficultyTxt;
    public Text scoreTxt = null;
    public Text commentaryTxt = null;
    public string[] commentary;
    public int[] answerIndex = new int[10];
    private int easyNum;
    private int moderateNum;
    private int difficultyNum;
    private List<int> RandNum = new List<int>();
    [SerializeField]
    public AnswerData_LGW[] answerDataEasy;
    public AnswerData_LGW[] AnswerDataEasy { set { answerDataEasy = value; } }
    [SerializeField]
    public AnswerData_LGW[] answerDataModerate;
    public AnswerData_LGW[] AnswerDataModerate { set { answerDataModerate = value; } }
    [SerializeField]
    public AnswerData_LGW[] answerDataDifficulty;
    public AnswerData_LGW[] AnswerDataDifficulty { set { answerDataDifficulty = value; } }

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            RandNum.Add(i);
        }
        // 하3, 중4, 상3 문제 및 해설 세팅
        EasyProblemSetup();
        ModeratProblemSetup();
        DifficultyProblemSetUp();
    }
    public void EasyProblemSetup()
    {
        int num = 0;
        for (int i = 0; i < 9; i += 3)
        {
            easyNum = RandomSet();
            easyTxt[i].text = answerDataEasy[easyNum].Problem;
            easyTxt[i + 1].text = answerDataEasy[easyNum].Choice1;
            easyTxt[i + 2].text = answerDataEasy[easyNum].Choice2;
            commentary[num] = answerDataEasy[easyNum].Commentary;
            answerIndex[num] = answerDataEasy[easyNum].AnswerIndex;
            num++;
        }
    }
    public void ModeratProblemSetup()
    {
        int num = 3;
        for (int i = 0; i < 12; i += 3)
        {
            moderateNum = RandomSet();
            moderateTxt[i].text = answerDataModerate[moderateNum].Problem;
            moderateTxt[i + 1].text = answerDataModerate[moderateNum].Choice1;
            moderateTxt[i + 2].text = answerDataModerate[moderateNum].Choice2;
            commentary[num] = answerDataModerate[moderateNum].Commentary;
            answerIndex[num] = answerDataModerate[moderateNum].AnswerIndex;
            num++;
        }
    }
    public void DifficultyProblemSetUp()
    {
        int num = 7;
        for (int i = 0; i < 9; i += 3)
        {
            difficultyNum = RandomSet();
            difficultyTxt[i].text = answerDataDifficulty[difficultyNum].Problem;
            difficultyTxt[i + 1].text = answerDataDifficulty[difficultyNum].Choice1;
            difficultyTxt[i + 2].text = answerDataDifficulty[difficultyNum].Choice2;
            commentary[num] = answerDataDifficulty[difficultyNum].Commentary;
            answerIndex[num] = answerDataDifficulty[difficultyNum].AnswerIndex;
            num++;
        }
    }
    private int RandomSet()
    {
        int ran = 0;
        int Log = 0;
        ran = Random.Range(0, Num);
        Log = RandNum[ran];
        RandNum.RemoveAt(ran);
        --Num;
        return Log;
    }
}
