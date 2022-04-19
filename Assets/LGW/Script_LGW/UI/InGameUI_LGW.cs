using UnityEngine;
using Valve.VR;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI_LGW : MonoBehaviour
{
    public SteamVR_Input_Sources rightHand = 0;
    public SteamVR_Input_Sources leftHand = 0;
    public SteamVR_Action_Boolean leftXButton = null;
    public SteamVR_Action_Boolean leftTrigger = null;
    public GameObject XButtonPanel = null;
    public GameObject soundPanel = null;
    public GameObject settingPanel = null;
    public GameObject commentaryPanel = null;
    public GameObject UILaser = null;
    public Slider volumeSlider = null;
    public PlayerShift_BJH playerShift= null;
    public bool canX = true;
    public ButtonController_LGW buttonCrtl = null;

    private void Start()
    {
        //buttonCrtl = FindObjectOfType<ButtonController_LGW>();
        //volumeSlider.value = buttonCrtl.volumeSlider.value;
        //Destroy(buttonCrtl.gameObject);
    }
    void Update()
    {
        LeftCtrl();
        SoundUpdate();
    }

    private void LeftCtrl()
    {
        if(canX == true)
        {
            if (leftXButton.GetStateDown(leftHand))
            {
                if (XButtonPanel.activeSelf == false)
                {
                    playerShift.canRay = false;
                    soundPanel.SetActive(false);
                    settingPanel.SetActive(true);
                    UILaser.SetActive(true);
                    XButtonPanel.SetActive(true);
                }
                else if (XButtonPanel.activeSelf == true)
                {
                    playerShift.canRay = true;
                    UILaser.SetActive(false);
                    soundPanel.SetActive(false);
                    XButtonPanel.SetActive(false);
                    settingPanel.SetActive(false);
                }
            }
        }
        if (leftTrigger.GetStateDown(leftHand))
        {
            commentaryPanel.SetActive(true);
        }
        else if (leftTrigger.GetStateUp(leftHand))
        {
            commentaryPanel.SetActive(false);
        }
    }

    public void UndoBt()
    {
        playerShift.canRay = true;
        UILaser.SetActive(false);
        XButtonPanel.SetActive(false);
        settingPanel.SetActive(false);
    }
    public void SoundSettingBt()
    {
        XButtonPanel.SetActive(false);
        soundPanel.SetActive(true);
    }
    public void SoundSettingUndoBt()
    {
        XButtonPanel.SetActive(true);
        soundPanel.SetActive(false);
    }
    public void ExitGameBt()
    {
        Debug.Log("Exit");
        SceneManager.LoadScene("CurvedTest_LGW");
    }
    public void SoundUpdate()
    {
        AudioListener.volume = volumeSlider.value;
    }
    public void GameOut()
    {
        StartCoroutine(FadeGameOut());
        StopCoroutine(FadeGameOut());
    }
    IEnumerator FadeGameOut()
    {
        SteamVR_Fade.View(Color.black, 0.25f);
        yield return new WaitForSeconds(0.25f);
        SteamVR_Fade.View(Color.clear, 0.25f);
        SceneManager.LoadScene("CurvedTest_LGW");
    }
}
