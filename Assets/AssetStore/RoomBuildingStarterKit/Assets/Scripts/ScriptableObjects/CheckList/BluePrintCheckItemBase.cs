namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.Entity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The BluePrintCheckItemBase class.
    /// </summary>
    [Serializable]
    public abstract class BluePrintCheckItemBase : CheckItemBase
    {
        /// <summary>
        /// Gets the blue print component.
        /// </summary>
        protected BluePrint BluePrint { get => this.context["BluePrint"] as BluePrint; }

        /// <summary>
        /// Gets the blue print data.
        /// </summary>
        protected BluePrintDataBase BluePrintData { get => this.BluePrint.BluePrintData; }

        /// <summary>
        /// The office collection.
        /// </summary>
        protected OfficeFloorCollection officeFloorCollection = new OfficeFloorCollection();

        /// <summary>
        /// Gets the check list.
        /// </summary>
        protected CheckListBase CheckList { get => this.context["CheckList"] as CheckListBase; }

        /// <summary>
        /// Gets the pending validate floor entities.
        /// </summary>
        protected List<FloorEntity> validateFloorEntities { get => this.CheckList.PendingValidateFloorEntities; }

        /// <summary>
        /// Gets the pending validate floor offsets (move blueprint).
        /// </summary>
        protected Vector3Int validateFloorOffset { get => this.CheckList.PendingValidateFloorOffset; }

        /// <summary>
        /// Gets the pending transform and rotation function.
        /// </summary>
        protected Func<FloorEntity, Vector2Int> pendingTransformAndRotate { get => this.CheckList.PendingTransformAndRotate; }

        /// <summary>
        /// Gets the pending rotate furniture function.
        /// </summary>
        protected Func<short, short> pendingRotateFurniture { get => this.CheckList.PendingRotateFurniture; }

        /// <summary>
        /// Gets the pending direction.
        /// </summary>
        protected short pendingDirection { get => this.CheckList.PendingValidateFloorDirection; }

        /// <summary>
        /// Gets the pending validate floor entities after offset.
        /// </summary>
        protected List<FloorEntity> validateOffsetFloorEntities { get => this.validateFloorEntities.Select(
            f => this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(this.pendingTransformAndRotate(f))?.FloorEntity).ToList(); }
    }
}