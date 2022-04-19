namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using System.Collections.Generic;
    using UnityEngine;
    
    /// <summary>
    /// The CheckListUI class.
    /// </summary>
    public class CheckListUI : Singleton<CheckListUI>
    {
        /// <summary>
        /// The check list panel.
        /// </summary>
        public GameObject CheckListPanel;

        /// <summary>
        /// The content transform.
        /// </summary>
        public Transform Content;

        /// <summary>
        /// The item prefab.
        /// </summary>
        public GameObject ItemPrefab;

        /// <summary>
        /// Shows check list.
        /// </summary>
        public void Show()
        {
            if (this.CheckListPanel.activeSelf == false)
            {
                this.CheckListPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hides check list.
        /// </summary>
        public void Hide()
        {
            if (this.CheckListPanel.activeSelf == true)
            {
                this.CheckListPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Adds check item into ui list.
        /// </summary>
        /// <param name="result">The check result.</param>
        /// <param name="text">The check item text.</param>
        /// <param name="args">The check item text string format arguments.</param>
        public void Add(bool result, UIText text, params string[] args)
        {
            var item = Instantiate(ItemPrefab, this.Content).GetComponent<BuildRoomCheckListItem>();
            item.SetText(text, args);
            item.SetResult(result);
        }

        /// <summary>
        /// Clears the check list.
        /// </summary>
        public void Clear()
        {
            this.Show();

            var childs = new List<Transform>();
            for (int i = 0; i < this.Content.childCount; ++i)
            {
                childs.Add(this.Content.GetChild(i));
            }

            childs.ForEach(c => Destroy(c.gameObject));
        }
    }
}
