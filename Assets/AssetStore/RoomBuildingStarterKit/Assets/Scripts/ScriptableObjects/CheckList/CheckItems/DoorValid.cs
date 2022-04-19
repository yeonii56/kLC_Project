namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Components;
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The DoorValid class is used to make sure doors at right place.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "DoorValid", menuName = "CheckList/CheckItems/DoorValid", order = 1)]
    public class DoorValid : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups ui.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_DOOR_VALID;
        }

        /// <summary>
        /// Prepares before validate.
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
            if (this.BluePrint.BluePrintDoorFurnitureEntities.Any())
            {
                this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
                {
                    var inRoomDoorFloorEntity = d.InRoomFloorEntity;

                    var offsetInRoomDoorFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(this.pendingTransformAndRotate(inRoomDoorFloorEntity));

                    var offsetInRoomDoorFloorEntity = offsetInRoomDoorFloor?.FloorEntity;

                    var offsetOutRoomDoorFloorEntity = (offsetInRoomDoorFloor == null ? null :
                        this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(offsetInRoomDoorFloor, this.pendingRotateFurniture(d.Direction))?.FloorEntity);

                    var isBuildable = (offsetInRoomDoorFloorEntity != null &&
                                       offsetOutRoomDoorFloorEntity != null &&
                                       offsetOutRoomDoorFloorEntity.OccupiedFurniture == null &&
                                       this.officeFloorCollection.GetOfficeFloor(offsetInRoomDoorFloorEntity.X, offsetInRoomDoorFloorEntity.Z) != null &&
                                       this.officeFloorCollection.GetOfficeFloor(offsetOutRoomDoorFloorEntity.X, offsetOutRoomDoorFloorEntity.Z) == null &&
                                       (offsetOutRoomDoorFloorEntity.GetWallByDir((short)((this.pendingRotateFurniture(d.Direction) + 2) % 4))?.IsWindow ?? false) == false &&
                                       !offsetInRoomDoorFloorEntity.OccupiedDoorEntities.Any(dd => dd.OutRoomFloorEntity == offsetInRoomDoorFloorEntity && dd.InRoomFloorEntity == offsetOutRoomDoorFloorEntity) &&
                                       offsetOutRoomDoorFloorEntity.OccupiedFurniture == null &&
                                       (this.BluePrint.CanDoorEntranceOverlap || offsetInRoomDoorFloorEntity.OccupiedDoorEntities.Count == 0 && offsetOutRoomDoorFloorEntity.OccupiedDoorEntities.Count == 0 && (offsetOutRoomDoorFloorEntity.OccupiedRoom == null || !offsetOutRoomDoorFloorEntity.OccupiedRoom.GetComponent<BluePrint>().BluePrintDoorFurnitureEntities.Any(dd => dd.InRoomFloorEntity == offsetOutRoomDoorFloorEntity))));

                    d.CantBuildRealRoom = !isBuildable;
                    d.Furniture.GetComponent<FurnitureController>().SetBuildableState(isBuildable);
                    result &= isBuildable;
                });
            }
            else
            {
                result = false;
            }

            // Office can have no doors in office editor.
            if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && !this.BluePrint.BluePrintDoorFurnitureEntities.Any())
            {
                result = true;
            }

            return result;
        }
    }
}