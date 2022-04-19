namespace RoomBuildingStarterKit.Editor
{
    using RoomBuildingStarterKit.OfficeEditor;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorInspector : Editor
    {
        private string[] mapSizeOptions = new string[] { "16x16", "32x32", "64x64", "128x128" };

        private List<int> mapSizes = new List<int> { 16, 32, 64, 128 };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mapGenerator = (MapGenerator)target;
            var oldMapSize = mapGenerator.MapSize;
            mapGenerator.MapSize = EditorGUILayout.IntPopup("Map Size", mapGenerator.MapSize, this.mapSizeOptions, this.mapSizes.ToArray());

            if (mapGenerator.MapSize != oldMapSize && !Application.isPlaying)
            {
                mapGenerator.RefreshMap();
            }

            if (GUI.changed && !Application.isPlaying)
            {
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}