namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using System;
    using System.IO;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The save load mode.
    /// </summary>
    public enum SaveLoad
    {
        Save,
        Load,
    }

    /// <summary>
    /// The SaveLoadGame class.
    /// </summary>
    public class SaveLoadGame : BlockMouseEventUIBase
    {
        [Tooltip("The panel is used for save or load game.")]
        public SaveLoad mode; 

        /// <summary>
        /// The element scroll view.
        /// </summary>
        public SaveLoadScrollView ScrollView;

        /// <summary>
        /// Executes when save game button clicked.
        /// </summary>
        /// <param name="index">The button index.</param>
        public void OnSaveGameButtonClicked(int index)
        {
            SaveLoader.inst.Save(index.ToString());
            this.OnEnable();
        }

        /// <summary>
        /// Executes when back to start menu button clicked.
        /// </summary>
        public void OnBackToStartMenuButtonClicked()
        {
            if (this.mode == SaveLoad.Load)
            {
                MenuManager.inst.Menus[Menus.StartMenu].SetActive(true);
                MenuManager.inst.Menus[Menus.SaveLoadGameMenu].SetActive(false);
            }
            else
            {
                MenuManager.inst.Menus[Menus.SaveLoadGameMenu].SetActive(false);
                MenuManager.inst.Menus[Menus.PauseMenu].SetActive(true);
            }
        }

        /// <summary>
        /// Executes when gameObject enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            string screenShotPath = $@"{Application.dataPath}/RoomBuildingStarterKit/Save/ScreenShot/";
            string gameDataPath = $@"{Application.dataPath}/RoomBuildingStarterKit/Save/GameData/";

            if (Directory.Exists(screenShotPath) == false)
            {
                Directory.CreateDirectory(screenShotPath);
            }

            if (Directory.Exists(gameDataPath) == false)
            {
                Directory.CreateDirectory(gameDataPath);
            }

            var filePaths = Directory.GetFiles(gameDataPath, "*.json");
            foreach (var filePath in filePaths)
            {
                var fileName = filePath.Split('/').Last().Split('.').First();
                int index = 0;
                int.TryParse(fileName, out index);
                var scrollViewData = this.ScrollView.ScrollViewDatas[index];
                var tex2D = new Texture2D(512, 512);
                try
                {
                    tex2D.LoadImage(ImageHelper.GetImageByte($"{screenShotPath}{fileName}.png"));
                }
                catch (Exception)
                {
                    tex2D = null;
                }

                scrollViewData.Initialize(index == 0 ? UIText.AUTO_SAVE : UIText.MANUAL_SAVE, Directory.GetLastWriteTimeUtc(filePath).ToString("yyyy/MM/dd HH:mm:ss"), tex2D, fileName);
            }
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.ScrollView.Init();
        }
    }
}