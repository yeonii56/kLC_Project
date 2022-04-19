namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.Common.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The SaveLoadScrollView class is an UI gameObject used to display save/load scroll view.
    /// </summary>
    public class SaveLoadScrollView : MonoBehaviour
    {
        /// <summary>
        /// The scroll view content child.
        /// </summary>
        public Transform Content;

        /// <summary>
        /// The scroll view contents under Content gameObject.
        /// </summary>
        private List<GameObject> contents = new List<GameObject>();

        /// <summary>
        /// The content rect transform.
        /// </summary>
        private RectTransform contentRect;

        /// <summary>
        /// The content anchor position.
        /// </summary>
        private Vector2 contentPos;

        /// <summary>
        /// The scroll view records.
        /// </summary>
        public List<LoadGameScrollViewData> ScrollViewDatas { get; private set; } = new List<LoadGameScrollViewData>();

        /// <summary>
        /// Initializes the scroll view.
        /// </summary>
        public void Init()
        {
            this.Content.GetChilds(ref this.contents);
            this.ScrollViewDatas = this.contents.Select(c => c.GetComponent<LoadGameScrollViewData>()).ToList();
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.contentRect = this.Content.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Executes when gameObject enable.
        /// </summary>
        private void OnEnable()
        {
            this.contentPos.x = 0f;
            this.contentPos.y = this.contentRect.anchoredPosition.y;
            this.contentRect.anchoredPosition = this.contentPos;
        }
    }
}