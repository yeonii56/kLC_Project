using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using UnityEngine.SceneManagement;

public class ButtonController_LGW : MonoBehaviour
{
    /* 테마 선택 시
     * 1. 처음에 테마3개와 메뉴얼 panel을 보여준다
     * 2. 테마 선택시 테마에 맞는 게임 방법을 보여주는 Panel을 활성화 시킨다
     * 메뉴얼 panel 선택 시
     * 1. 처음에 테마3개와 메뉴얼 panel을 보여준다
     * 2. 메뉴얼에 있는 버튼을 클릭 시
     * 3. 컨트롤러 클릭 시 InfoCanvas를 활성화 시킨다.
     * 4. 톱니바퀴 클릭 시 SettingCanvas를 활성화 시킨다.
     * 5. 문을 클릭 시 게임을 종료한다.
     */

    // main, info, setting
    public GameObject[] panels;
    public GameObject[] ClassicInfo;
    public int currentPage = 0;
    public AudioSource audioSource;
    public Slider volumeSlider = null;

    // 종료
    public void GameExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
    // 조작법
    public void GameInfo()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[1].SetActive(true);
        Debug.Log("게임설명");
    }
    // 설정
    public void GameSetting()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[2].SetActive(true);
        Debug.Log("게임세팅");
    }
    public void ClassicBt()
    {
        Debug.Log("게임시작");
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[3].SetActive(true);
    }
    public void ClassicStart()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("RoomTest_BJH");
        Debug.Log("클래식 모드 시작");
    }
    public void Undo()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        panels[0].SetActive(true);
        Debug.Log("뒤로가기");
    }
    public void NextPage()
    {
        if (currentPage == 4) return;
        // 현재 페이지를 비활성화 한다.
        // 다음 페이지를 활성화 한다.
        ClassicInfo[currentPage].SetActive(false);
        currentPage++;
        ClassicInfo[currentPage].SetActive(true);
    }

    public void PrevPage()
    {
        // 현재 페이지를 비활성화 한다.
        // 이전 페이지를 활성화 한다.
        if (currentPage == 0) return;
        ClassicInfo[currentPage].SetActive(false);
        currentPage--;
        ClassicInfo[currentPage].SetActive(true);
    }

    private void Update()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
