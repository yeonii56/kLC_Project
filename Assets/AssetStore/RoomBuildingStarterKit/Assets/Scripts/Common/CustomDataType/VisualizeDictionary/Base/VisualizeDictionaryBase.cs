namespace RoomBuildingStarterKit.VisualizeDictionary
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The visualize dictionary entry.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    [Serializable]
    public class DictEntry<TKey, TValue>
    {
        /// <summary>
        /// The key.
        /// </summary>
        public TKey Key;

        /// <summary>
        /// The value.
        /// </summary>
        public TValue Value;
    }

    /// <summary>
    /// The visualize dictionary class.
    /// </summary>
    /// <typeparam name="T">The entry type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    [Serializable]
    public class VisualizeDictionary<T, TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver where T : DictEntry<TKey, TValue>, new()
    {
        /// <summary>
        /// The items used for reorderable list.
        /// </summary>
        public List<T> Items = new List<T>();

        /// <summary>
        /// Executes after deserialize.
        /// </summary>
        public void OnAfterDeserialize()
        {
            this.Clear();
            this.Items.ForEach(e => this[e.Key] = e.Value);
        }

        /// <summary>
        /// Executes before serialize.
        /// </summary>
        public void OnBeforeSerialize()
        {
            this.Items.Clear();

            foreach(var pair in this)
            {
                this.Items.Add(new T { Key = pair.Key, Value = pair.Value });
            }
        }
    }
}