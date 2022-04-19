namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using UnityEngine.Assertions;

    /// <summary>
    /// The RoomListController is used to control the ui room list.
    /// </summary>
    public class RoomListController : ItemListControllerBase
    {
        /// <summary>
        /// The room shop list.
        /// </summary>
        public ShopListBase RoomShopList;

        /// <summary>
        /// Chooses a shop list to display.
        /// </summary>
        protected override void ChooseShopList()
        {
            this.shopListType = this.RoomShopList;
        }

        /// <summary>
        /// Registers buttons.
        /// </summary>
        protected override void RegisterButtons()
        {
            Assert.IsTrue(this.itemButtons.Count == this.shopListType.ShopList.Items.Count);

            for (int i = 0; i < this.itemButtons.Count; ++i)
            {
                var item = this.shopListType.ShopList.Items[i];
                var roomType = (int)item.ShopItemUIPrefab.GetComponent<RoomShopItem>().RoomType;
                this.itemButtons[i].onClick.AddListener(() => this.OnButtonClicked(roomType));
            }
        }

        /// <summary>
        /// Clicks the build room button.
        /// </summary>
        /// <param name="i">The room type.</param>
        protected override void OnButtonClicked(int i)
        {
            if (i == (int)RoomType.OfficeFurniture)
            {
                InGameUI.inst.EnterBuildOfficeFurnitureMode();
                this.gameObject.SetActive(false);
            }
            else
            {
                GlobalRoomManager.inst.StartBuildRoom(i);
            }
        }

        /// <summary>
        /// Close the ui room list.
        /// </summary>
        protected override void OnCloseButtonClicked()
        {
            GlobalRoomManager.inst.CancelBuildRoom();
            this.gameObject.SetActive(false);
        }
    }
}