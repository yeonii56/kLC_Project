namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Effect;
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// The PauseMenu class.
    /// </summary>
    public class PauseMenu : BlockMouseEventUIBase
    {
        /// <summary>
        /// The game name logo gameObject.
        /// </summary>
        public GameObject GameNameLogo;

        /// <summary>
        /// The animation complete callback.
        /// </summary>
        private Action animationendcallback;

        /// <summary>
        /// The animator component.
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();
        }

        /// <summary>
        /// Executes when gameObject enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            this.GameNameLogo.SetActive(true);
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
        /// Executes when continue game button clicked.
        /// </summary>
        public void OnContinueGameButtonClicked()
        {
            this.animator.SetTrigger("Exit");

            this.animationendcallback = () =>
            {
                this.gameObject.SetActive(false);
                MenuManager.inst.Menus[Menus.InGameMenu].SetActive(true);
                this.GameNameLogo.SetActive(false);

                GaussianBlurController.inst.Animator.SetTrigger("StopGaussianBlur");
            };

        }

        /// <summary>
        /// Executes when save game button clicked.
        /// </summary>
        public void OnSaveGameButtonClicked()
        {
            this.animator.SetTrigger("Exit");

            this.animationendcallback = () =>
            {
                this.gameObject.SetActive(false);
                MenuManager.inst.Menus[Menus.SaveLoadGameMenu].SetActive(true);
            };
        }

        /// <summary>
        /// Executes when settings button clicked.
        /// </summary>
        public void OnSettingsButtonClicked()
        {
            this.animator.SetTrigger("Exit");

            this.animationendcallback = () =>
            {
                this.gameObject.SetActive(false);
                MenuManager.inst.Menus[Menus.SettingsMenu].SetActive(true);
            };
        }

        /// <summary>
        /// Executes when main menu button clicked.
        /// </summary>
        public void OnMainMenuButtonClicked()
        {
            this.animator.SetTrigger("Exit");
            this.animationendcallback = () =>
            {
                this.gameObject.SetActive(false);
                SceneManager.LoadScene("StartMenu");
            };

        }

        /// <summary>
        /// Executes when animation completes.
        /// </summary>
        public void QuitAnimation()
        {
            this.animationendcallback?.Invoke();
        }
    }
}