namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The ItemListControllerBase class.
    /// </summary>
    public class ItemListControllerBase : MonoBehaviour
    {
        /// <summary>
        /// The content rect transform.
        /// </summary>
        public RectTransform ContentRect;

        /// <summary>
        /// The close button.
        /// </summary>
        public Button CloseButton;

        /// <summary>
        /// The shop list.
        /// </summary>
        protected ShopListBase shopListType;
        
        /// <summary>
        /// The item buttons.
        /// </summary>
        protected List<Button> itemButtons = new List<Button>();

        /// <summary>
        /// The content anchor position.
        /// </summary>
        private Vector2 contentPos;

        /// <summary>
        /// The content childs;
        /// </summary>
        private List<GameObject> contentChilds = new List<GameObject>();

        /// <summary>
        /// Executes when item button clicked.
        /// </summary>
        /// <param name="i">The item index.</param>
        protected virtual void OnButtonClicked(int i)
        {
        }

        /// <summary>
        /// Executes when close button clicked.
        /// </summary>
        protected virtual void OnCloseButtonClicked()
        {
        }

        /// <summary>
        /// Registers buttons.
        /// </summary>
        protected virtual void RegisterButtons()
        {
        }

        /// <summary>
        /// Chooses a shop list.
        /// </summary>
        protected virtual void ChooseShopList()
        {
        }

        public void RefreshList()
        {
            // Clears childs under content.
            for (int i = 0; i < this.ContentRect.childCount; ++i)
            {
                this.contentChilds.Add(this.ContentRect.GetChild(i).gameObject);
            }

            this.contentChilds.ForEach(c => Destroy(c));
            this.contentChilds.Clear();

            // Recreates childs.
            this.itemButtons.Clear();
            foreach (var item in this.shopListType.ShopList.Items)
            {
                this.itemButtons.Add(Instantiate(item.ShopItemUIPrefab, this.ContentRect).GetComponent<Button>());
            }

            this.RegisterButtons();

            this.contentPos.x = this.ContentRect.anchoredPosition.x;
            this.contentPos.y = 0f;
            this.ContentRect.anchoredPosition = this.contentPos;
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.CloseButton.onClick.AddListener(this.OnCloseButtonClicked);
        }

        /// <summary>
        /// Executes after Awake.
        /// </summary>
        private void OnEnable()
        {
            this.ChooseShopList();
            this.RefreshList();
        }
    }
}