namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.VisualizeDictionary.Implementations;

    /// <summary>
    /// The menu definitions.
    /// </summary>
    public enum Menus
    {
        StartMenu,
        SaveLoadGameMenu,
        SettingsMenu,
        LoadSceneMenu,
        PauseMenu,
        InGameMenu,
    }

    /// <summary>
    /// The menu manager class.
    /// </summary>
    public class MenuManager : Singleton<MenuManager>
    {
        /// <summary>
        /// The menus.
        /// </summary>
        [MenuEnumGameObjectDict]
        public MenuEnumGameObjectDict Menus;
    }
}