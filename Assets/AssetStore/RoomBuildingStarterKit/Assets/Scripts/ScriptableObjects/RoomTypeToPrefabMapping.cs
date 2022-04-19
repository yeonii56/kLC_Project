namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.VisualizeDictionary.Implementations;
    using UnityEngine;

    /// <summary>
    /// The FurnitureTypeToPrefabMapping class.
    /// </summary>
    [CreateAssetMenu(fileName = "RoomTypeToPrefabMapping", menuName = "BuildSystem/RoomTypeToPrefabMapping", order = 1)]
    public class RoomTypeToPrefabMapping : ScriptableObject
    {
        /// <summary>
        /// The furniture type dictionary <RoomType, GameObject>.
        /// </summary>
        [RoomTypeEnumGameObjectDict]
        public RoomTypeEnumGameObjectDict RoomTypeEnumGameObjectDict;
    }
}
