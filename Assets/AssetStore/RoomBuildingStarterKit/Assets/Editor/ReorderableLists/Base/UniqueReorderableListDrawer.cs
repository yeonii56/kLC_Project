namespace RoomBuildingStarterKit.CustomReorderableList
{
    using UnityEditorInternal;
    using UnityEditor;
    using System.Collections.Generic;

    /// <summary>
    /// The UniqueReorderableList class is used to make sure the reorderable list has no same keys.
    /// </summary>
    public abstract class UniqueReorderableList : ReorderableListDrawerBase
    {
        /// <summary>
        /// The dictionary is used to check whether the element already exists.
        /// </summary>
        private Dictionary<string, bool> listElementExist = new Dictionary<string, bool>();

        /// <summary>
        /// Executes when the elements changed.
        /// </summary>
        /// <param name="property">The serialized property.</param>
        protected override void OnChangedCallback(SerializedProperty property)
        {
            this.list.onChangedCallback = (ReorderableList l) =>
            {
                this.listElementExist.Clear();
                var count = this.list.serializedProperty.arraySize;
                for (int i = count - 1; i >= 0; --i)
                {
                    var element = this.list.serializedProperty.GetArrayElementAtIndex(i);
                    var uniqueKey = this.GetUniqueKey(element);
                    if (this.listElementExist.TryGetValue(uniqueKey, out bool exists))
                    {
                        this.list.serializedProperty.DeleteArrayElementAtIndex(i);
                    }
                    else
                    {
                        this.listElementExist.Add(uniqueKey, true);
                    }
                }
            };
        }

        /// <summary>
        /// Gets the unique key.
        /// </summary>
        /// <param name="element">The serialized property.</param>
        /// <returns>The unique key.</returns>
        protected abstract string GetUniqueKey(SerializedProperty element);

        /// <summary>
        /// Checks whether the element has already existed.
        /// </summary>
        /// <param name="key">The unique key.</param>
        /// <returns>The element exists or not.</returns>
        protected bool DoesElementAlreadyExist(string key)
        {
            var count = this.list.serializedProperty.arraySize;
            for (int i = 0; i < count; ++i)
            {
                var element = this.list.serializedProperty.GetArrayElementAtIndex(i);
                var uniqueKey = this.GetUniqueKey(element);
                if (uniqueKey == key)
                {
                    if (EditorUtility.DisplayDialog("Error!", $"Already has {key} in list, could add another one.", "Cancel"))
                    {
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
