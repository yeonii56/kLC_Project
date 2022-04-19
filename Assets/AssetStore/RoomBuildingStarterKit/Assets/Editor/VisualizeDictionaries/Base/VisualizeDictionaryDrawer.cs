namespace RoomBuildingStarterKit.VisualizeDictionary
{
    using RoomBuildingStarterKit.Common.Extensions;
    using RoomBuildingStarterKit.CustomReorderableList;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The VisualizeDictionary class is used to draw visualize dictionary property.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class VisualizeDictionary<TKey, TValue> : UniqueReorderableList
    {
        /// <summary>
        /// Draws the list elements.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void DrawElementCallback(SerializedProperty property)
        {
            this.list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = this.list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.PrefixLabel(
                    new Rect(rect.x, rect.y, 25, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Key", "Key of the visualize dictionary"));

                EditorGUI.PropertyField(
                    new Rect(rect.x + 25 + 5, rect.y, 180, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Key"),
                    GUIContent.none);

                EditorGUI.PrefixLabel(
                    new Rect(rect.x + 25 + 5 + 180 + 10, rect.y, 35, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Value", "Value of the visualize dictionary"));

                EditorGUI.PropertyField(
                    new Rect(rect.x + 25 + 5 + 180 + 10 + 35 + 5, rect.y, rect.width - (25 + 5 + 180 + 10 + 35 + 5) - 20, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Value"),
                    GUIContent.none);
            };
        }

        /// <summary>
        /// Gets unique key for avoid repeat keys.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The unique key.</returns>
        protected override string GetUniqueKey(SerializedProperty element)
        {
            return element.FindPropertyRelative("Key").GetValue<TKey>().ToString();
        }
    }
}