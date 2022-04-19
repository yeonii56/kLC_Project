namespace RoomBuildingStarterKit.Common.Extensions
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The Unity built-in GameObject extension methods.
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        /// Gets father gameobject of the specified gameobject.
        /// </summary>
        /// <param name="obj">The gameobject.</param>
        /// <returns>The father gameobject.</returns>
        public static GameObject GetFather(this GameObject obj)
        {
            return obj.transform.parent.gameObject;
        }

        /// <summary>
        /// Gets the root gameobject with the same layer of the specified gameobject.
        /// </summary>
        /// <param name="obj">The gameobject.</param>
        /// <returns>The root gameobject.</returns>
        public static GameObject GetSameLayerRootObject(this GameObject obj)
        {
            while (obj.GetFather().layer == obj.layer)
            {
                obj = obj.GetFather();
            }

            return obj;
        }

        /// <summary>
        /// Gets child GameObjects.
        /// </summary>
        /// <param name="obj">The game object.</param>
        /// <param name="childs">The child game object list.</param>
        public static void GetChilds(this GameObject obj, ref List<GameObject> childs)
        {
            obj.transform.GetChilds(ref childs);
        }

        /// <summary>
        /// Gets child transforms.
        /// </summary>
        /// <param name="obj">The game object.</param>
        /// <param name="childs">The child transform list.</param>
        public static void GetChilds(this GameObject obj, ref List<Transform> childs)
        {
            obj.transform.GetChilds(ref childs);
        }

        /// <summary>
        /// Gets child components.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="obj">The game object.</param>
        /// <param name="childs">The child component list.</param>
        public static void GetChilds<T>(this GameObject obj, ref List<T> childs)
        {
            obj.transform.GetChilds(ref childs);
        }
    }
}