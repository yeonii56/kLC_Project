namespace RoomBuildingStarterKit.UI
{
    using System;
    using UnityEngine;

    /// <summary>
    /// The StartMenu class.
    /// </summary>
    public class StartMenu : MonoBehaviour
    {
        /// <summary>
        /// The animator component.
        /// </summary>
        private Animator animator;

        /// <summary>
        /// The menu transition complete callback.
        /// </summary>
        private Action menuTransitionCallback;

        /// <summary>
        /// Executes when the new game button clicked.
        /// </summary>
        public void OnNewGameButtonClicked()
        {
            this.animator.SetTrigger("Exit");
            this.menuTransitionCallback = () =>
            {
                MenuManager.inst.Menus[Menus.LoadSceneMenu].SetActive(true);
                MenuManager.inst.Menus[Menus.LoadSceneMenu].GetComponent<LoadingPage>().LoadScene("Demo");
                MenuManager.inst.Menus[Menus.StartMenu].SetActive(false);
            };
        }

        /// <summary>
        /// Executes when the load game button clicked.
        /// </summary>
        public void OnLoadGameButtonClicked()
        {
            this.animator.SetTrigger("Exit");
            this.menuTransitionCallback = () =>
            {
                MenuManager.inst.Menus[Menus.SaveLoadGameMenu].SetActive(true);
                MenuManager.inst.Menus[Menus.StartMenu].SetActive(false);
            };
        }

        /// <summary>
        /// Executes when the game settings button clicked.
        /// </summary>
        public void OnGameSettingsButtonClicked()
        {
            this.animator.SetTrigger("Exit");
            this.menuTransitionCallback = () =>
            {
                MenuManager.inst.Menus[Menus.SettingsMenu].SetActive(true);
                MenuManager.inst.Menus[Menus.StartMenu].SetActive(false);
            };
        }

        /// <summary>
        /// Executes when quit game button clicked.
        /// </summary>
        public void OnQuitGameButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Executes when the fade out animation completed.
        /// </summary>
        public void OnFadeOutCompleted()
        {
            this.menuTransitionCallback?.Invoke();
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
        }
    }
}
