namespace RoomBuildingStarterKit.Editor
{
    using RoomBuildingStarterKit.BuildSystem;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The RefreshRoomTypeToPrefabMapping class is used to refresh room mapping.
    /// </summary>
    public class RefreshRoomTypeToPrefabMapping
    {
        /// <summary>
        /// Refreshes the room type to prefab mappings.
        /// </summary>
        [MenuItem("Tools/UpdateRoomInfo/Refresh Room Type To Prefab Mappings", priority = 0)]
        public static void Refresh()
        {
            var mapping = AssetDatabase.LoadAssetAtPath($@"Assets/RoomBuildingStarterKit/Assets/ScriptableObjects/Mappings/RoomTypeToPrefabMapping.asset", typeof(RoomTypeToPrefabMapping)) as RoomTypeToPrefabMapping;
            mapping.RoomTypeEnumGameObjectDict.Clear();

            var guids = AssetDatabase.FindAssets("", new[] { $@"Assets/RoomBuildingStarterKit/Assets/Prefabs/RealRoom/Rooms" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                var bluePrint = prefab.GetComponent<BluePrint>();
                if (prefab != null && bluePrint != null)
                {
                    mapping.RoomTypeEnumGameObjectDict[bluePrint.RoomType] = prefab;
                }
            }

            EditorUtility.SetDirty(mapping);
        }
    }
}
