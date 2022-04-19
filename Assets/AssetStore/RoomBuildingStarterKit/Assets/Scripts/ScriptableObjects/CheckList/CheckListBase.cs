namespace RoomBuildingStarterKit.Components
{
    using RoomBuildingStarterKit.Common;
    using UnityEngine;
    using RoomBuildingStarterKit.UI;
    using System.Collections.Generic;
    using RoomBuildingStarterKit.Entity;
    using System;

    /// <summary>
    /// The check list used for reorderable list..
    /// </summary>
    [Serializable]
    public class CheckList : ReorderableList<CheckItem>
    {
    }

    /// <summary>
    /// The check list base class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "CheckListBase", menuName = "CheckList/CheckListBase", order = 1)]
    public class CheckListBase : ScriptableObject
    {
        /// <summary>
        /// The check items.
        /// </summary>
        [CheckList]
        [SerializeField]
        private CheckList checkItems = new CheckList();

        /// <summary>
        /// Gets the check items.
        /// </summary>
        public List<CheckItem> CheckItems => this.checkItems.Items;

        /// <summary>
        /// The pending validate floor entities.
        /// </summary>
        [HideInInspector]
        public List<FloorEntity> PendingValidateFloorEntities = new List<FloorEntity>();

        /// <summary>
        /// The pending validate floor offset.
        /// </summary>
        [HideInInspector]
        public Vector3Int PendingValidateFloorOffset = Vector3Int.zero;

        /// <summary>
        /// The pending validate floor direction.
        /// </summary>
        [HideInInspector]
        public short PendingValidateFloorDirection = 0;

        /// <summary>
        /// The pending transform and rotation.
        /// </summary>
        public Func<FloorEntity, Vector2Int> PendingTransformAndRotate = (f) => new Vector2Int(f.X, f.Z);

        /// <summary>
        /// The pending rotate furniture.
        /// </summary>
        public Func<short, short> PendingRotateFurniture = (dir) => dir;

        /// <summary>
        /// Gets the context.
        /// </summary>
        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Initializes the check list.
        /// </summary>
        private void Init()
        {
            this.Context["CheckList"] = this;
            this.CheckItems.ForEach(i => i.Item.Setup(this.Context));
        }

        /// <summary>
        /// Validates the check items.
        /// </summary>
        /// <returns>The validate result.</returns>
        public bool Validate()
        {
            this.Init();
            CheckListUI.inst.Clear();

            bool result = true;
            foreach(var item in this.CheckItems)
            {
                if (item.IsEnable)
                {
                    var flag = item.Item.Check(item.AddToUICheckList);
                    if (item.IsFatal)
                    {
                        result &= flag;
                    }
                }
            }

            return result;
        }
    }
}