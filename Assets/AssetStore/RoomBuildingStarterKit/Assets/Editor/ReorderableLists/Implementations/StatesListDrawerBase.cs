namespace RoomBuildingStarterKit.CustomReorderableList.Implementations
{
    using NUnit.Framework;
    using RoomBuildingStarterKit.Common;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using PropertyAttribute = UnityEngine.PropertyAttribute;

    /// <summary>
    /// The state parameters.
    /// </summary>
    struct StateParams
    {
        public string Path;
    }

    /// <summary>
    /// The states list base class.
    /// </summary>
    public class StatesListBase : UniqueReorderableList
    {
        /// <summary>
        /// The state folder location.
        /// </summary>
        protected string statesFolder = string.Empty;

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

                var item = (StateBase)element.FindPropertyRelative("Item").objectReferenceValue;
                EditorGUI.BeginDisabledGroup(item.IsDefault);
                EditorGUI.PrefixLabel(
                    new Rect(rect.x, rect.y, 45, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Enable", "Whether the state is enabled"));

                EditorGUI.PropertyField(
                    new Rect(rect.x + 45 + 2, rect.y, 20, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("IsEnable"),
                    GUIContent.none);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 45 + 20 + 5, rect.y, rect.width - (45 + 20 + 5) - 20, EditorGUIUtility.singleLineHeight),
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
            Assert.IsNotEmpty(this.statesFolder, "States folder name must not be empty");

            this.list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                var guids = AssetDatabase.FindAssets("", new[] { $"Assets/RoomBuildingStarterKit/Assets/ScriptableObjects/States/{this.statesFolder}" });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    menu.AddItem(
                        new GUIContent(Path.GetFileNameWithoutExtension(path)),
                        false,
                        (target) => this.ClickHandler(property, target),
                        new StateParams { Path = path });
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
            return ((StateBase)element.FindPropertyRelative("Item").objectReferenceValue).GetType().ToString();
        }

        /// <summary>
        /// Clicks element on drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="target">The parameters.</param>
        private void ClickHandler(SerializedProperty property, object target)
        {
            var data = (StateParams)target;
            var item = AssetDatabase.LoadAssetAtPath(data.Path, typeof(StateBase)) as StateBase;
            if (this.DoesElementAlreadyExist(item.GetType().ToString()))
            {
                return;
            }

            var index = this.list.serializedProperty.arraySize;
            ++this.list.serializedProperty.arraySize;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("IsEnable").boolValue = true;
            element.FindPropertyRelative("Item").objectReferenceValue = item;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}