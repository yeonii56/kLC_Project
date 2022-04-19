namespace RoomBuildingStarterKit.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The ReorderableList base class.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    [Serializable]
    public class ReorderableList<T>
    {
        public List<T> Items = new List<T>();
    }
}
