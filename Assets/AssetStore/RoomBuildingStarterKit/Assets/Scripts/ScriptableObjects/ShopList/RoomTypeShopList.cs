namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using UnityEngine;

    /// <summary>
    /// The ShopListItem element used by choose shop list.
    /// </summary>
    [Serializable]
    public struct ShopListItem
    {
        /// <summary>
        /// The ShopList item.
        /// </summary>
        public RoomTypeShopList ShopList;
    }

    /// <summary>
    /// The choose shop list used for reorderable list.
    /// </summary>
    [Serializable]
    public class ChooseShopList : ReorderableList<ShopListItem>
    {
    }


    /// <summary>
    /// The FurnitureShopList class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "RoomTypeShopList", menuName = "ShopList/RoomTypeShopList", order = 1)]
    public class RoomTypeShopList : ShopListBase
    {
        public RoomType RoomType = RoomType.WorkingRoom;
    }
}