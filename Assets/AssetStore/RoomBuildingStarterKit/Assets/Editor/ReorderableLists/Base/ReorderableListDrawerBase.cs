namespace RoomBuildingStarterKit.CustomReorderableList
{
    using UnityEditorInternal;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// The reorderable list property drawer base class.
    /// </summary>
    public class ReorderableListDrawerBase : PropertyDrawer
    {
        /// <summary>
        /// The reorderable list.
        /// </summary>
        protected ReorderableList list;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (this.list == null)
            {
                this.InitList(property);
            }

            property.serializedObject.Update();
            this.list.DoList(rect);
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (this.list == null)
            {
                this.InitList(property);
            }

            return this.list.GetHeight();
        }

        /// <summary>
        /// Initializes the custom reorderable list.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected virtual void InitList(SerializedProperty property)
        {
            this.list = new ReorderableList(property.serializedObject, property.FindPropertyRelative("Items"), true, true, true, true);

            this.DrawHeaderCallback(property);
            this.DrawElementCallback(property);
            this.OnAddDropdownCallback(property);
            this.OnAddCallback(property);
            this.OnChangedCallback(property);
        }

        /// <summary>
        /// Draws the list's header.
        /// </summary>
        /// <param name="property"></param>
        protected virtual void DrawHeaderCallback(SerializedProperty property)
        {
            // Set list header.
            this.list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, property.displayName); };
        }

        /// <summary>
        /// Draws the list elements.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected virtual void DrawElementCallback(SerializedProperty property)
        { 
        }

        /// <summary>
        /// Executes when the elements changed.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected virtual void OnChangedCallback(SerializedProperty property)
        {
        }

        /// <summary>
        /// Sets the add drop down menu.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected virtual void OnAddDropdownCallback(SerializedProperty property)
        {
        }

        /// <summary>
        /// Executes when add new element.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected virtual void OnAddCallback(SerializedProperty property)
        {
        }
    }
}
