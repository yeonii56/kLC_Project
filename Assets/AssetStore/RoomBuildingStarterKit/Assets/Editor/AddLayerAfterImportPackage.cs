#define ASSET_STORE_PACKAGE
#if ASSET_STORE_PACKAGE
namespace RoomBuildingStarterKit.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The AddLayerAfterImportPackage class is used to automatically add layers which needed by this project after user import package.
    /// </summary>
    public class AddLayerAfterImportPackage : AssetPostprocessor
    {
        /// <summary>
        /// The layer names.
        /// </summary>
        public static List<string> LayerNames = new List<string> { "Floor", "Wall", "Tile", "Furniture", "Door", "Scene", "Outline", "Selectable", "OfficeDoor", "UnLockableObject", "MeshCombine" };

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            int assetsCount = importedAssets.Length;
            for (int i = 0; i < assetsCount; ++i)
            {
                if (importedAssets[i].EndsWith("AddLayerAfterImportPackage.cs"))
                {
                    Debug.Log("Begin to detect and add dependency layers ...");
                    AddLayers(LayerNames);
                }
            }
        }

        public static void AddLayers(List<string> layerNames)
        {
            layerNames.ForEach(n => AddLayer(n));
        }

        public static bool AddLayer(string layerName)
        {
            if (!UnityEditorInternal.InternalEditorUtility.layers.Any(n => n == layerName))
            {
                var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset"));
                var it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "layers")
                    {
                        int layersCount = it.arraySize;
                        for (int i = 8; i < layersCount; ++i)
                        {
                            var sp = it.GetArrayElementAtIndex(i);
                            if (string.IsNullOrEmpty(sp.stringValue))
                            {
                                sp.stringValue = layerName;
                                tagManager.ApplyModifiedProperties();
                                Debug.Log($"Add layer: {layerName} succeed.");
                                return true;
                            }
                        }

                        Debug.Log($"Add layer: {layerName} failed. Please add this layer manually for avoid any runtime failures");
                        return false;
                    }
                }
            }
            else
            {
                Debug.Log($"Layer: {layerName} already exists.");
            }

            return false;
        }
    }
}
#endif