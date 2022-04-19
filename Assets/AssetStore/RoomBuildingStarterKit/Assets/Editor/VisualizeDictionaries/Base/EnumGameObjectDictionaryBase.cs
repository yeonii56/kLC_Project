namespace RoomBuildingStarterKit.VisualizeDictionary
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.VisualizeDictionary.Implementations;
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// The enum params.
    /// </summary>
    public struct EnumParams
    {
        public string Name;
        public GameObject Prefab;
    }

    /// <summary>
    /// The EnumGameObjectDict is the property drawer for the VisualizeDictionary<T, GameObject>
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    public class EnumGameObjectDictBase<T> : VisualizeDictionary<T, GameObject> where T : Enum
    {
        /// <summary>
        /// Sets the add drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void OnAddDropdownCallback(SerializedProperty property)
        {
            this.list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                var menu = new GenericMenu();
                foreach (var name in Enum.GetNames(typeof(T)))
                {
                    menu.AddItem(
                                    new GUIContent(name),
                                    false,
                                    (target) => this.ClickHandler(property, target),
                                    new EnumParams
                                    {
                                        Name = name
                                    });
                }

                menu.ShowAsContext();
            };
        }

        /// <summary>
        /// Clicks element on drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="target">The parameters.</param>
        protected virtual void ClickHandler(SerializedProperty property, object target)
        {
            var name = ((EnumParams)target).Name;
            if (this.DoesElementAlreadyExist(name))
            {
                return;
            }

            var index = this.list.serializedProperty.arraySize;
            ++this.list.serializedProperty.arraySize;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var a = (int)Enum.Parse(typeof(T), name);
            element.FindPropertyRelative("Key").enumValueIndex = (int)Enum.Parse(typeof(T), name);
            element.FindPropertyRelative("Value").objectReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}