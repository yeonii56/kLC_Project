namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using UnityEngine;

    /// <summary>
    /// The shop list used for reorderable list.
    /// </summary>
    [Serializable]
    public class ShopList : ReorderableList<ShopItem>
    {
    }

    /// <summary>
    /// The ShopList base class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "ShopListBase", menuName = "ShopList/ShopListBase", order = 1)]
    public class ShopListBase : ScriptableObject
    {
        /// <summary>
        /// The shop items.
        /// </summary>
        [ShopList]
        [SerializeField]
        public ShopList ShopList = new ShopList();
    }
}