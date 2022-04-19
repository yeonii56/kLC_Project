namespace RoomBuildingStarterKit.VisualizeDictionary.Implementations
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.UI;
    using System;
    using UnityEngine;

    /// <summary>
    /// The visualize dictionary with <string, GameObject> entry. 
    /// </summary>
    [Serializable]
    public class StringGameObjectDict : VisualizeDictionary<StringGameObjectEntry, string, GameObject>
    {
    }

    /// <summary>
    /// The visualize dictionary with <Menu, GameObject> entry.
    /// </summary>
    [Serializable]
    public class MenuEnumGameObjectDict : VisualizeDictionary<MenuEnumGameObjectEntry, Menus, GameObject>
    {
    }

    /// <summary>
    /// The visualize dictionary with <FurnitureType, GameObject> entry.
    /// </summary>
    [Serializable]
    public class FurnitureTypeEnumGameObjectDict : VisualizeDictionary<FurnitureTypeEnumGameObjectEntry, FurnitureType, GameObject>
    {
    }

    /// <summary>
    /// The visualize dictionary with <RoomType, GameObject> entry.
    /// </summary>
    [Serializable]
    public class RoomTypeEnumGameObjectDict : VisualizeDictionary<RoomTypeEnumGameObjectEntry, RoomType, GameObject>
    {
    }
}
