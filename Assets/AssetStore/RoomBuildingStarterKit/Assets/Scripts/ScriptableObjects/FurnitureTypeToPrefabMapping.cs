namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.VisualizeDictionary.Implementations;
    using UnityEngine;

    /// <summary>
    /// The FurnitureTypeToPrefabMapping class.
    /// </summary>
    [CreateAssetMenu(fileName = "FurnitureTypeToPrefabMapping", menuName = "BuildSystem/FurnitureTypeToPrefabMapping", order = 1)]
    public class FurnitureTypeToPrefabMapping : ScriptableObject
    {
        /// <summary>
        /// The furniture type dictionary <FurnitureType, GameObject>.
        /// </summary>
        [FurnitureTypeEnumGameObjectDict]
        public FurnitureTypeEnumGameObjectDict FurnitureTypeEnumGameObjectDict;
    }
}