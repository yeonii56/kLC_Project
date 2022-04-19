namespace RoomBuildingStarterKit.Editor
{
    using RoomBuildingStarterKit.BuildSystem;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The RefreshFurnitureTypeToPrefabMapping class is used to refresh furniture mapping.
    /// </summary>
    public class RefreshFurnitureTypeToPrefabMapping
    {
        /// <summary>
        /// Refreshes the furniture type to prefab mappings.
        /// </summary>
        [MenuItem("Tools/UpdateRoomInfo/Refresh Furniture Type To Prefab Mappings", priority = 1)]
        public static void Refresh()
        {
            var mapping = AssetDatabase.LoadAssetAtPath($@"Assets/RoomBuildingStarterKit/Assets/ScriptableObjects/Mappings/FurnitureTypeToPrefabMapping.asset", typeof(FurnitureTypeToPrefabMapping)) as FurnitureTypeToPrefabMapping;
            mapping.FurnitureTypeEnumGameObjectDict.Clear();

            Directory.GetDirectories($@"{Application.dataPath}/RoomBuildingStarterKit/Assets/Prefabs/Furnitures/").ToList().ForEach(d =>
            {
                var folderName = d.Split('/').Last();
                var guids = AssetDatabase.FindAssets("", new[] { $@"Assets/RoomBuildingStarterKit/Assets/Prefabs/Furnitures/{folderName}" });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    var furnitureController = prefab.GetComponent<FurnitureController>();
                    if (prefab != null && furnitureController != null)
                    {
                        mapping.FurnitureTypeEnumGameObjectDict[furnitureController.FurnitureType] = prefab;
                    }
                }
            });

            EditorUtility.SetDirty(mapping);
        }

        /// <summary>
        /// Refreshes furniture type to prefab mappings and room type to prefab mappings.
        /// </summary>
        [MenuItem("Tools/UpdateRoomInfo/Refresh All", priority = 2)]
        public static void RefreshAll()
        {
            RefreshFurnitureTypeToPrefabMapping.Refresh();
            RefreshRoomTypeToPrefabMapping.Refresh();
        }
    }
}
