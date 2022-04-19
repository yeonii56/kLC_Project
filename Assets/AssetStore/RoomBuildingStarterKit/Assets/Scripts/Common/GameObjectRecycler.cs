namespace RoomBuildingStarterKit.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// The GameObjectRecycler class used to instantiate and recycle game objects by object pool.
    /// ** Make sure when recycle a game object, all its references are set to null.
    /// </summary>
    public class GameObjectRecycler : Singleton<GameObjectRecycler>
    {
        /// <summary>
        /// The maximum pool size.
        /// </summary>
        public int MaxPoolSize = 100;

        /// <summary>
        /// The object pools.
        /// </summary>
        private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// The recycler's transform.
        /// </summary>
        private Transform selfTransform;

        /// <summary>
        /// Converts class to string for print.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in this.pools)
            {
                stringBuilder.AppendLine($"PoolName: {item.Key}, PoolSize: {item.Value.Count}");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Instantiates game object or gets a game object from pool.
        /// </summary>
        /// <param name="prefab">The prefab.</param>
        /// <param name="parent">The transform.</param>
        /// <returns>The game object.</returns>
        public GameObject Instantiate(GameObject prefab, Transform parent)
        {
            var prefabName = this.GetPrefabName(prefab);
            if (this.pools.TryGetValue(prefabName, out Queue<GameObject> pool) == false)
            {
                pool = new Queue<GameObject>();
                this.pools.Add(prefabName, pool);
            }

            GameObject item = null;
            if (pool.Count > 0)
            {
                item = pool.Dequeue();
                item.transform.SetParent(parent);
                item.SetActive(true);
            }
            else
            {
                item = GameObject.Instantiate(prefab, parent);
            }

            return item;
        }

        /// <summary>
        /// Recycles a game object or destroy it.
        /// </summary>
        /// <param name="item">The gameObject to be destroyed.</param>
        public void Destroy(GameObject item)
        {
            if (LayerMask.LayerToName(item.layer) == "Outline")
            {
                item.layer = LayerMask.NameToLayer("Selectable");
            }

            var prefabName = this.GetPrefabName(item);
            if (this.pools.TryGetValue(prefabName, out Queue<GameObject> pool) == false)
            {
                pool = new Queue<GameObject>();
                this.pools.Add(prefabName, pool);
            }

            if (pool.Count < MaxPoolSize)
            {
                item.SetActive(false);
                item.transform.SetParent(this.selfTransform);
                pool.Enqueue(item);
            }
            else
            {
                GameObject.Destroy(item);
            }
        }

        /// <summary>
        /// Destroys or recycle a game object.
        /// </summary>
        /// <param name="item">The item to be destroyed</param>
        /// <param name="onDestroy">The destroy callback.</param>
        public void Destroy(GameObject item, Action<GameObject> onDestroy)
        {
            onDestroy(item);
            this.Destroy(item);
        }

        /// <summary>
        /// Executes when game object instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            this.selfTransform = this.transform;
        }

        /// <summary>
        /// Gets the prefab name.
        /// </summary>
        /// <param name="item">The game object.</param>
        /// <returns>The prefab name.</returns>
        private string GetPrefabName(GameObject item)
        {
            var prefabName = item.name;
            if (prefabName.EndsWith("(Clone)"))
            {
                prefabName = prefabName.Substring(0, prefabName.Length - 7);
            }

            return prefabName;
        }
    }
}