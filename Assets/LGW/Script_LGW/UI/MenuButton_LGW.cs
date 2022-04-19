using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class MenuButton_LGW : MonoBehaviour
{
    public SteamVR_Input_Sources leftHand;
    public GameObject BtPanel;
    public GameObject SettingPanel;
    public GameObject InfoPanel;
    public bool isUIOn = false;
    public void InfoBt()
    {
        InfoPanel.SetActive(true);
        BtPanel.SetActive(false);
    }
    public void SettingBt()
    {
        SettingPanel.SetActive(true);
        BtPanel.SetActive(false);
    }
    public void ExitBt()
    {
        StartCoroutine(ExitScene());
    }
    private IEnumerator ExitScene()
    {
        SteamVR_Fade.View(Color.black, 0.25f);
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene("MainMenu_LGW");
    }
    public void UndoBt()
    {
        BtPanel.SetActive(true);
        InfoPanel.SetActive(false);
        SettingPanel.SetActive(false);
    }
}
