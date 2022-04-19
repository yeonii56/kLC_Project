namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using UnityEngine.UI;

    /// <summary>
    /// The scene definitions.
    /// </summary>
    public enum Scene
    {
        StartMenu,
        Game,
    }

    /// <summary>
    /// The game settings class.
    /// </summary>
    public class GameSettings : BlockMouseEventUIBase
    {
        /// <summary>
        /// The back to start menu button.
        /// </summary>
        public Button BackToStartMenuButton;

        /// <summary>
        /// The reset to default button.
        /// </summary>
        public Button ResetSelection;

        /// <summary>
        /// The current scene.
        /// </summary>
        public Scene CurrentScene;

        /// <summary>
        /// Executes when back to start menu button clicked.
        /// </summary>
        public void OnBackToStartMenuButtonClicked()
        {
            if (this.CurrentScene == Scene.StartMenu)
            {
                MenuManager.inst.Menus[Menus.StartMenu].SetActive(true);
                MenuManager.inst.Menus[Menus.SettingsMenu].SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(false);
                MenuManager.inst.Menus[Menus.PauseMenu].SetActive(true);
            }
        }

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            this.ResetSelection.onClick.AddListener(SettingsManager.inst.ResetSettings);
        }
    }
}
