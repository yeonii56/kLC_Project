namespace RoomBuildingStarterKit.Common.Extensions
{
    using System;
    using UnityEditor;

    /// <summary>
    /// The SerializedPropertyExtension class.
    /// </summary>
    public static class SerializedPropertyExtension
    {
        /// <summary>
        /// Gets value by type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>The value object.</returns>
        public static object GetValue<T>(this SerializedProperty property)
        {
            var type = typeof(T);
            if (type == typeof(int))
            {
                return property.intValue;
            }
            else if (type.IsEnum)
            {
                return Enum.GetName(typeof(T), property.enumValueIndex);
            }
            else if (type == typeof(string))
            {
                return property.stringValue;
            }

            return null;
        }
    }
}