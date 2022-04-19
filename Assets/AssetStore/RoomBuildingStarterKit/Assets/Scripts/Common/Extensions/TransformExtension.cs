namespace RoomBuildingStarterKit.Common.Extensions
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The Unity built-in Transform extension methods.
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// Gets father gameobject of the specified gameobject.
        /// </summary>
        /// <param name="obj">The gameobject.</param>
        /// <returns>The father gameobject.</returns>
        public static void GetChilds(this Transform trans, ref List<GameObject> childs)
        {
            childs.Clear();
            for (int i = 0; i < trans.childCount; ++i)
            {
                childs.Add(trans.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Gets the child transforms.
        /// </summary>
        /// <param name="trans">The transform.</param>
        /// <param name="childs">The child transform list.</param>
        public static void GetChilds(this Transform trans, ref List<Transform> childs)
        {
            childs.Clear();
            for (int i = 0; i < trans.childCount; ++i)
            {
                childs.Add(trans.GetChild(i));
            }
        }

        /// <summary>
        /// Gets the child components.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="trans">The transform.</param>
        /// <param name="childs">The child component list.</param>
        public static void GetChilds<T>(this Transform trans, ref List<T> childs)
        {
            childs.Clear();
            for (int i = 0; i < trans.childCount; ++i)
            {
                childs.Add(trans.GetChild(i).GetComponent<T>());
            }
        }

        /// <summary>
        /// Gets the active child components.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="trans">The transform.</param>
        /// <param name="childs">The child component list.</param>
        public static void GetActiveChilds<T>(this Transform trans, ref List<T> childs)
        {
            childs.Clear();
            for (int i = 0; i < trans.childCount; ++i)
            {
                var child = trans.GetChild(i);
                if (child.gameObject.activeSelf == true)
                {
                    childs.Add(trans.GetChild(i).GetComponent<T>());
                }
            }
        }

        /// <summary>
        /// Gets the child components by layer.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="trans">The transform.</param>
        /// <param name="layerMask">The layer mask.</param>
        /// <param name="childs">The child component list.</param>
        public static void GetChildsByLayer<T>(this Transform trans, int layerMask, ref List<T> childs)
        {
            childs.Clear();
            for (int i = 0; i < trans.childCount; ++i)
            {
                var child = trans.GetChild(i);
                if (child.gameObject.layer == layerMask)
                {
                    childs.Add(child.GetComponent<T>());
                }
            }
        }

        /// <summary>
        /// Gets the child transform by name.
        /// </summary>
        /// <param name="trans">The transform.</param>
        /// <param name="name">The target's name.</param>
        /// <returns>The target transform.</returns>
        public static Transform GetChildByName(this Transform trans, string name)
        {
            for (int i = 0; i < trans.childCount; ++i)
            {
                var child = trans.GetChild(i);
                if(child.name == name)
                {
                    return child;
                }
            }

            return null;
        }
    }
}