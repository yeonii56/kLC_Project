namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The FurnitureValid class is used to make sure furnitures put in the correct position.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "FurnitureValid", menuName = "CheckList/CheckItems/FurnitureValid", order = 1)]
    public class FurnitureValid : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups ui.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_FURNITURE_VALID;
        }

        /// <summary>
        /// Prepares the check item before validate.
        /// </summary>
        protected override void Prepare()
        {
            this.officeFloorCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);
            this.officeFloorCollection.Reset(this.validateOffsetFloorEntities);
        }

        /// <summary>
        /// Validates the check item.
        /// </summary>
        /// <returns>The validate result.</returns>
        protected override bool Validate()
        {
            bool result = true;
            this.BluePrint.BluePrintFurnitureEntities.ForEach(f =>
            {
                var isBuildable = f.FloorEntities.All(ff => this.officeFloorCollection.GetOfficeFloor(this.pendingTransformAndRotate(ff)) != null && !ff.OccupiedDoorEntities.Any());
                f.Furniture.GetComponent<FurnitureController>().SetBuildableState(isBuildable);
                f.CantBuildRealRoom = !isBuildable;
                result &= isBuildable;
            });

            this.BluePrint.BluePrintWallFurnitureEntities.ForEach(f =>
            {
                var isBuildable = true;
                f.FloorEntities.ForEach(ff =>
                {
                    var offsetFloor = this.officeFloorCollection.GetOfficeFloor(this.pendingTransformAndRotate(ff)); // Gets floor after move or rotate.
                    isBuildable &= (offsetFloor != null &&
                                (offsetFloor.FloorEntity.GetOriginalWallByDir(this.pendingRotateFurniture(f.Direction))?.IsWindow ?? false) == false &&  // Overlaps with an office window. 
                                this.officeFloorCollection.GetOfficeFloorByDir(offsetFloor, this.pendingRotateFurniture(f.Direction)) == null &&   // Wall furniture must on the wall.
                                (offsetFloor.FloorEntity.GetWallByDir(this.pendingRotateFurniture(f.Direction))?.IsWindow ?? false) == false) &&   // Overlaps with a window of other room.
                                !offsetFloor.FloorEntity.OccupiedDoorEntities.Any(d => this.pendingRotateFurniture(d.Direction) == (this.pendingRotateFurniture(f.Direction) + 2) % 4);  // Overlap with a door of other room.
                });

                f.Furniture.GetComponent<FurnitureController>().SetBuildableState(isBuildable);
                f.CantBuildRealRoom = !isBuildable;
                result &= isBuildable;
            });

            return result;
        }
    }
}