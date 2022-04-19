namespace RoomBuildingStarterKit.VisualizeDictionary
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// The StringGameObjectDict is the property drawer for VisualizeDictionary<string, GameObject>.
    /// </summary>
    public class StringGameObjectDictBase : VisualizeDictionary<string, GameObject>
    {
        /// <summary>
        /// Initializes the custom reorderable list.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void InitList(SerializedProperty property)
        {
            base.InitList(property);
        }

        /// <summary>
        /// Executes when add new element.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void OnAddCallback(SerializedProperty property)
        {
            this.list.onAddCallback = (ReorderableList list) =>
            {
                var index = this.list.serializedProperty.arraySize;
                ++this.list.serializedProperty.arraySize;
                list.index = index;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("Key").stringValue = string.Empty;
                element.FindPropertyRelative("Value").objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            };
        }
    }
}