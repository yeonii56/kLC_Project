namespace RoomBuildingStarterKit.VisualizeDictionary.Implementations
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.UI;
    using System;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// The MenuGameObjectDict is the property drawer for the EnumGameObjectDict<Menus>
    /// </summary>
    [CustomPropertyDrawer(typeof(MenuEnumGameObjectDictAttribute))]
    public class MenuEnumGameObjectDict : EnumGameObjectDictBase<Menus>
    {
    }

    /// <summary>
    /// The TypeEnumGameObjectDict is the property drawer for the EnumGameObjectDict<T>
    /// </summary>
    public class TypeEnumGameObjectDict<T> : EnumGameObjectDictBase<T> where T : Enum
    {
        /// <summary>
        /// Initializes the custom reorderable list.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void InitList(SerializedProperty property)
        {
            base.InitList(property);
            this.list.draggable = false;
        }

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

                EditorGUI.BeginDisabledGroup(true);
                
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

                EditorGUI.EndDisabledGroup();
            };
        }

        /// <summary>
        /// Clicks element on drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="target">The parameters.</param>
        protected override void ClickHandler(SerializedProperty property, object target)
        {
            var enumParams = ((EnumParams)target);
            if (this.DoesElementAlreadyExist(enumParams.Name))
            {
                return;
            }

            var index = this.list.serializedProperty.arraySize;
            ++this.list.serializedProperty.arraySize;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Key").enumValueIndex = (int)Enum.Parse(typeof(T), enumParams.Name);
            element.FindPropertyRelative("Value").objectReferenceValue = enumParams.Prefab;
            property.serializedObject.ApplyModifiedProperties();
        }
    }

    /// <summary>
    /// The FurnitureTypeGameObjectDict is the property drawer for the EnumGameObjectDict<FurnitureType>
    /// </summary>
    [CustomPropertyDrawer(typeof(FurnitureTypeEnumGameObjectDictAttribute))]
    public class FurnitureTypeEnumGameObjectDict : TypeEnumGameObjectDict<FurnitureType>
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

                Directory.GetDirectories($@"{Application.dataPath}/RoomBuildingStarterKit/Assets/Prefabs/Furnitures/").ToList().ForEach(d =>
                {
                    var folderName = d.Split('/').Last();
                    var guids = AssetDatabase.FindAssets("", new[] { $@"Assets/RoomBuildingStarterKit/Assets/Prefabs/Furnitures/{folderName}" });
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                        if (prefab != null && prefab.GetComponent<FurnitureController>() != null)
                        {
                            var enumName = prefab.GetComponent<FurnitureController>().FurnitureType.ToString();
                            menu.AddItem(
                                new GUIContent($"{folderName}/{enumName}"),
                                false,
                                (target) => this.ClickHandler(property, target),
                                new EnumParams
                                {
                                    Name = enumName,
                                    Prefab = prefab,
                                });
                        }
                    }
                });

                menu.ShowAsContext();
            };
        }
    }

    /// <summary>
    /// The RoomTypeGameObjectDict is the property drawer for the EnumGameObjectDict<RoomType>
    /// </summary>
    [CustomPropertyDrawer(typeof(RoomTypeEnumGameObjectDictAttribute))]
    public class RoomTypeGameObjectDict : TypeEnumGameObjectDict<RoomType>
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

                var guids = AssetDatabase.FindAssets("", new[] { $@"Assets/RoomBuildingStarterKit/Assets/Prefabs/RealRoom/Rooms" });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    if (prefab != null && prefab.GetComponent<BluePrint>() != null)
                    {
                        var enumName = prefab.GetComponent<BluePrint>().RoomType.ToString();
                        menu.AddItem(
                            new GUIContent(enumName),
                            false,
                            (target) => this.ClickHandler(property, target),
                            new EnumParams
                            {
                                Name = enumName,
                                Prefab = prefab,
                            });
                    }
                }

                menu.ShowAsContext();
            };
        }
    }
}