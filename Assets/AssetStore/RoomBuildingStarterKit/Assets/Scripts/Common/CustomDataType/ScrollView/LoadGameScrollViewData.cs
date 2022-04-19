namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.UI;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The LoadGameScrollViewData class represents the load game record UI.
    /// </summary>
    public class LoadGameScrollViewData : MonoBehaviour
    {
        /// <summary>
        /// The save/load mode.
        /// </summary>
        public SaveLoad Mode;

        /// <summary>
        /// The record title.
        /// </summary>
        public TextMeshProUGUIWrapper Title;

        /// <summary>
        /// The record created time.
        /// </summary>
        public TextMeshProUGUI SaveDatetime;

        /// <summary>
        /// The screen shot image.
        /// </summary>
        public Image ScreenShot;

        /// <summary>
        /// The save file name.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Initializes the save/load record.
        /// </summary>
        /// <param name="title">The record title.</param>
        /// <param name="saveDatetime">The record created time.</param>
        /// <param name="screen">The screen shot image.</param>
        /// <param name="fileName">The file name.</param>
        public void Initialize(UIText title, string saveDatetime, Texture2D screen, string fileName)
        {
            this.fileName = fileName;
            this.Title.SetGlobalText(title);
            this.SaveDatetime.text = saveDatetime;

            if (screen != null)
            {
                this.ScreenShot.sprite = Sprite.Create(screen, new Rect(0, 0, screen.width, screen.height), new Vector2(0.5f, 0.5f));
            }
        }
        
        /// <summary>
        /// Executes when the record button clicked.
        /// </summary>
        private void OnLoadGameButtonClicked()
        {
            if (this.Mode == SaveLoad.Load)
            {
                Global.inst.LoadFileName = fileName;
                MenuManager.inst.Menus[Menus.LoadSceneMenu].SetActive(true);
                MenuManager.inst.Menus[Menus.LoadSceneMenu].GetComponent<LoadingPage>().LoadScene("Demo");
                MenuManager.inst.Menus[Menus.SaveLoadGameMenu].SetActive(false);
            }
            else
            {
                MenuManager.inst.Menus[Menus.SaveLoadGameMenu].GetComponent<SaveLoadGame>().OnSaveGameButtonClicked(transform.GetSiblingIndex());
            }
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.GetComponent<Button>().onClick.AddListener(this.OnLoadGameButtonClicked);
        }
    }
}