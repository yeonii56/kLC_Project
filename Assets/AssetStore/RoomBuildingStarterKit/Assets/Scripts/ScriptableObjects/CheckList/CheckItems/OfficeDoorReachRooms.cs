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
    /// The OfficeDoorReachRooms class is used to make sure office doors could reach rooms.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "OfficeDoorReachRooms", menuName = "CheckList/CheckItems/OfficeDoorReachRooms", order = 1)]
    public class OfficeDoorReachRooms : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups ui.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_ROOM_REACHABLE;
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
        /// <returns></returns>
        protected override bool Validate()
        {
            List<Tuple<FloorEntity, short>> doorParams = new List<Tuple<FloorEntity, short>>();

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
                                       this.officeFloorCollection.GetOfficeFloor(offsetInRoomDoorFloorEntity.X, offsetInRoomDoorFloorEntity.Z) != null &&
                                       this.officeFloorCollection.GetOfficeFloor(offsetOutRoomDoorFloorEntity.X, offsetOutRoomDoorFloorEntity.Z) == null &&
                                       (offsetOutRoomDoorFloorEntity.GetWallByDir((short)((this.pendingRotateFurniture(d.Direction) + 2) % 4))?.IsWindow ?? false) == false &&
                                       !offsetInRoomDoorFloorEntity.OccupiedDoorEntities.Any(dd => dd.OutRoomFloorEntity == offsetInRoomDoorFloorEntity && dd.InRoomFloorEntity == offsetOutRoomDoorFloorEntity) &&
                                       offsetOutRoomDoorFloorEntity.OccupiedFurniture == null &&
                                       (this.BluePrint.CanDoorEntranceOverlap || offsetInRoomDoorFloorEntity.OccupiedDoorEntities.Count == 0 && offsetOutRoomDoorFloorEntity.OccupiedDoorEntities.Count == 0 && (offsetOutRoomDoorFloorEntity.OccupiedRoom == null || !offsetOutRoomDoorFloorEntity.OccupiedRoom.GetComponent<BluePrint>().BluePrintDoorFurnitureEntities.Any(dd => dd.InRoomFloorEntity == offsetOutRoomDoorFloorEntity))));

                    d.CantBuildRealRoom = !isBuildable;
                    d.Furniture.GetComponent<FurnitureController>().SetBuildableState(isBuildable);

                    // result &= isBuildable;
                    doorParams.Add(Tuple.Create<FloorEntity, short>(offsetOutRoomDoorFloorEntity, this.pendingRotateFurniture(d.Direction)));
                });

                var isConnect = this.BluePrint.FoundationManager.OfficeFloorCollection.CheckDoorsUnion(doorParams, this.validateOffsetFloorEntities);
                this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
                {
                    d.CantBuildRealRoom |= !isConnect;
                    d.Furniture.GetComponent<FurnitureController>().SetBuildableState(!d.CantBuildRealRoom);
                });

                this.BluePrint.Office.GetComponent<OfficeController>().Rooms.ForEach(r => r.Room.GetComponent<RealRoom>().RealRoomDoors.Where(d => d.activeSelf == true).ToList().ForEach(d => d.GetComponent<FurnitureController>().SetBuildableState(isConnect)));
                this.BluePrint.FoundationManager.OfficeDoors.ForEach(d => d.SetBuildableState(isConnect));
                result &= isConnect;
            }
            else
            {
                var isBuildable = this.BluePrint.FoundationManager.OfficeFloorCollection.CheckDoorsUnion(null, this.validateOffsetFloorEntities);
                this.BluePrint.Office.GetComponent<OfficeController>().Rooms.ForEach(r => r.Room.GetComponent<RealRoom>().RealRoomDoors.Where(d => d.activeSelf == true).ToList().ForEach(d => d.GetComponent<FurnitureController>().SetBuildableState(isBuildable)));
                this.BluePrint.FoundationManager.OfficeDoors.ForEach(d => d.SetBuildableState(isBuildable));
                result &= isBuildable;
            }

            return result;
        }
    }
}