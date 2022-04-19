namespace RoomBuildingStarterKit.Editor
{
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The custom property drawer for furniture type.
    /// </summary>
    [CustomPropertyDrawer(typeof(FurnitureType))]
    public class FurnitureTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var options = Enum.GetNames(typeof(FurnitureType)).ToList().Select(n =>
            {
                var idx = n.IndexOf('_');
                return $"{n.Substring(0, idx)}/{n.Substring(idx + 1)}";
            }).ToArray();

            EditorGUI.BeginChangeCheck();
            var index = EditorGUI.Popup(EditorGUI.PrefixLabel(position, label), property.enumValueIndex, options);
            if (EditorGUI.EndChangeCheck())
            {
                var path = options[index];
                var selectName = path.Replace('/', '_');
                if (Enum.TryParse<FurnitureType>(selectName, out FurnitureType result))
                {
                    property.enumValueIndex = (int)result;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Error!", $"The selected enum value {selectName} doesn't exit in FurnitureType.", "Cancel"))
                    {
                    }
                }
            }
        }
    }
}