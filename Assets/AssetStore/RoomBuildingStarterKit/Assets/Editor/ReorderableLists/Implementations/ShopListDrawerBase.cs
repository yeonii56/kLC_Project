namespace RoomBuildingStarterKit.CustomReorderableList.Implementations
{
    using RoomBuildingStarterKit.Common;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// The check list base class.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShopListAttribute))]
    public class ShopListBase : ReorderableListDrawerBase
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
                    new Rect(rect.x, rect.y, 70, EditorGUIUtility.singleLineHeight),
                    new GUIContent("ShopItemUIPrefab", "The Shop Item UI Prefab."));
                EditorGUI.PropertyField(
                    new Rect(rect.x + 45 + 70, rect.y, rect.width - 45 - 80, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("ShopItemUIPrefab"),
                    GUIContent.none);
            };
        }

        /// <summary>
        /// Executes when add new element.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void OnAddCallback(SerializedProperty property)
        {
            this.list.onAddCallback = (ReorderableList l) =>
            {
                var index = this.list.serializedProperty.arraySize;
                ++this.list.serializedProperty.arraySize;
                list.index = index;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("ShopItemUIPrefab").objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            };
        }
    }
}
