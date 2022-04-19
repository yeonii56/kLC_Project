namespace RoomBuildingStarterKit.CustomReorderableList.Implementations
{
    using RoomBuildingStarterKit.Common;
    using System.IO;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// The choose shop list parameters.
    /// </summary>
    struct ChooseShopListParams
    {
        public string Path;
    }

    /// <summary>
    /// The ChooseShopList base class.
    /// </summary>
    [CustomPropertyDrawer(typeof(ChooseShopListAttribute))]
    public class ChooseShopListBase : UniqueReorderableList
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
                EditorGUI.PrefixLabel(
                    new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    new GUIContent("ShopList", "The shop list."));
                EditorGUI.PropertyField(
                    new Rect(rect.x + 60 + 10, rect.y, rect.width - 60 - 10, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("ShopList"),
                    GUIContent.none);
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
                var guids = AssetDatabase.FindAssets("", new[] { "Assets/RoomBuildingStarterKit/Assets/ScriptableObjects/ShopList/FurnitureShopList" });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    menu.AddItem(
                        new GUIContent(Path.GetFileNameWithoutExtension(path)),
                        false,
                        (target) => this.ClickHandler(property, target),
                        new ChooseShopListParams { Path = path });
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
            return ((RoomTypeShopList)element.FindPropertyRelative("ShopList").objectReferenceValue).RoomType.ToString();
        }

        /// <summary>
        /// Clicks element on drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        /// <param name="target">The parameters.</param>
        private void ClickHandler(SerializedProperty property, object target)
        {
            var data = (ChooseShopListParams)target;
            var item = AssetDatabase.LoadAssetAtPath(data.Path, typeof(Common.ShopListBase)) as RoomTypeShopList;
            if (this.DoesElementAlreadyExist(item.RoomType.ToString()))
            {
                return;
            }

            var index = this.list.serializedProperty.arraySize;
            ++this.list.serializedProperty.arraySize;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("ShopList").objectReferenceValue = item;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}