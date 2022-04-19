namespace RoomBuildingStarterKit.CustomReorderableList.Implementations
{
    using UnityEditorInternal;
    using UnityEditor;
    using UnityEngine;
    using RoomBuildingStarterKit.Common;
    using System.IO;

    /// <summary>
    /// The check item parameters.
    /// </summary>
    struct CheckItemParams
    {
        public string Path;
    }

    /// <summary>
    /// The check list base class.
    /// </summary>
    [CustomPropertyDrawer(typeof(CheckListAttribute))]
    public class CheckListBase : UniqueReorderableList
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
                    new Rect(rect.x, rect.y, 45, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Enable", "Whether the check box is enabled"));
                EditorGUI.PropertyField(
                    new Rect(rect.x + 45 + 2, rect.y, 20, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("IsEnable"),
                    GUIContent.none);

                EditorGUI.PrefixLabel(
                    new Rect(rect.x + 45 + 20 + 5, rect.y, 36, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Fatal", "Whether the condition is fatal."));
                EditorGUI.PropertyField(
                    new Rect(rect.x + 45 + 20 + 5 + 36, rect.y, 20, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("IsFatal"),
                    GUIContent.none);

                EditorGUI.PrefixLabel(
                    new Rect(rect.x + 45 + 20 + 5 + 36 + 3 + 20, rect.y, 20, EditorGUIUtility.singleLineHeight),
                    new GUIContent("UI", "Whether the check item need to display on UI"));
                EditorGUI.PropertyField(
                    new Rect(rect.x + 45 + 20 + 5 + 36 + 3 + 20 + 20, rect.y, 20, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("AddToUICheckList"),
                    GUIContent.none);

                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 45 + 20 + 5 + 36 + 3 + 20 + 20 + 20 + 5, rect.y, rect.width - (45 + 20 + 5 + 36 + 3 + 20 + 20 + 20 + 5) - 20, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Item"),
                    GUIContent.none);
                EditorGUI.EndDisabledGroup();
            };
        }

        /// <summary>
        /// Sets the add drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void OnAddDropdownCallback(SerializedProperty property)
        {
            this.list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                var guids = AssetDatabase.FindAssets("", new[] { "Assets/RoomBuildingStarterKit/Assets/ScriptableObjects/CheckList/CheckItems" });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    menu.AddItem(
                        new GUIContent(Path.GetFileNameWithoutExtension(path)),
                        false,
                        (target) => this.ClickHandler(property, target),
                        new CheckItemParams { Path = path });
                }

                menu.ShowAsContext();
            };
        }

        /// <summary>
        /// Gets the unique key.
        /// </summary>
        /// <param name="element">The serialized property.</param>
        /// <returns>The unique key.</returns>
        protected override string GetUniqueKey(SerializedProperty element)
        {
            return ((CheckItemBase)element.FindPropertyRelative("Item").objectReferenceValue).GetType().ToString();
        }

        /// <summary>
        /// Clicks element on drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="target">The parameters.</param>
        private void ClickHandler(SerializedProperty property, object target)
        {
            var data = (CheckItemParams)target;
            var item = AssetDatabase.LoadAssetAtPath(data.Path, typeof(CheckItemBase)) as CheckItemBase;
            if (this.DoesElementAlreadyExist(item.GetType().ToString()))
            {
                return;
            }

            var index = this.list.serializedProperty.arraySize;
            ++this.list.serializedProperty.arraySize;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("IsEnable").boolValue = true;
            element.FindPropertyRelative("AddToUICheckList").boolValue = false;
            element.FindPropertyRelative("Item").objectReferenceValue = item;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
