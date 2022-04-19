namespace RoomBuildingStarterKit.Components
{
    using UnityEngine;
    using System.Collections.Generic;
    using RoomBuildingStarterKit.Entity;
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using System.Linq;
    using RoomBuildingStarterKit.Common.Extensions;
    using System.Threading;

    /// <summary>
    /// The RealRoom class used to handle build real room operations.
    /// </summary>
    [RequireComponent(typeof(BluePrint))]
    public class RealRoom : MonoBehaviour
    {
        /// <summary>
        /// The floor prefab.
        /// </summary>
        public GameObject FloorPrefab;

        /// <summary>
        /// The door prefab.
        /// </summary>
        public GameObject DoorPrefab;

        /// <summary>
        /// The inside wall prefab.
        /// </summary>
        public GameObject InsideWallPrefab;

        /// <summary>
        /// The inside window wall prefab.
        /// </summary>
        public GameObject InsideWindowWallPrefab;

        /// <summary>
        /// The inside door wall prefab.
        /// </summary>
        public GameObject InsideDoorWallPrefab;

        /// <summary>
        /// The inside wall corner prefab.
        /// </summary>
        public GameObject InsideWallCornerPrefab;

        /// <summary>
        /// The outside wall prefab.
        /// </summary>
        public GameObject OutsideWallPrefab;

        /// <summary>
        /// The outside window wall prefab.
        /// </summary>
        public GameObject OutsideWindowWallPrefab;

        /// <summary>
        /// The outside door wall prefab.
        /// </summary>
        public GameObject OutsideDoorWallPrefab;

        /// <summary>
        /// The outside wall corner prefab.
        /// </summary>
        public GameObject OutsideWallCornerPrefab;

        /// <summary>
        /// The child floors.
        /// </summary>
        private List<GameObject> childFloors = new List<GameObject>();

        /// <summary>
        /// The child walls.
        /// </summary>
        private List<GameObject> childWalls = new List<GameObject>();

        /// <summary>
        /// The child furnitures.
        /// </summary>
        private List<GameObject> childFurnitures = new List<GameObject>();

        /// <summary>
        /// The real room floor container transform.
        /// </summary>
        private Transform realRoomFloorsContainerTransform;

        /// <summary>
        /// The real room wall container transform.
        /// </summary>
        private Transform realRoomWallsContainerTransform;

        /// <summary>
        /// The real room furniture container transform.
        /// </summary>
        private Transform realRoomFurnitureContainerTransform;

        /// <summary>
        /// The office floor collection.
        /// </summary>
        private OfficeFloorCollection officeFloorCollection = new OfficeFloorCollection();

        /// <summary>
        /// The real room doors.
        /// </summary>
        private List<GameObject> realRoomDoors = new List<GameObject>();

        /// <summary>
        /// Gets the blue print component.
        /// </summary>
        public BluePrint BluePrint { get; private set; }

        /// <summary>
        /// Gets the blue print data.
        /// </summary>
        public BluePrintDataBase BluePrintData { get => this.BluePrint.BluePrintData; }

        /// <summary>
        /// Gets the real room container gameObject.
        /// </summary>
        public GameObject RealRoomContainer { get; private set; }

        /// <summary>
        /// Gets the real room doors.
        /// </summary>
        public List<GameObject> RealRoomDoors
        {
            get
            {
                return this.realRoomDoors;
            }

            private set
            {
                this.realRoomDoors = value;
            }
        }

        /// <summary>
        /// Cleans up and disable real room container.
        /// </summary>
        public void CleanupAndDisableContainer()
        {
            var floorContainer = this.RealRoomContainer.transform.Find("Floors");
            var wallContainer = this.RealRoomContainer.transform.Find("Walls");
            floorContainer.GetChilds(ref childFloors);
            wallContainer.GetChilds(ref childWalls);
            this.RealRoomContainer.transform.Find("Furnitures").GetChilds(ref childFurnitures);

            childFloors.ForEach(f => GameObjectRecycler.inst.Destroy(f));
            childWalls.ForEach(w => GameObjectRecycler.inst.Destroy(w));
            childFurnitures.ForEach(f => Destroy(f));

            // ** Recover outline state.
            if (floorContainer.gameObject.layer == LayerMask.NameToLayer("Outline"))
            {
                floorContainer.gameObject.layer = LayerMask.NameToLayer("Selectable");
            }

            if (wallContainer.gameObject.layer == LayerMask.NameToLayer("Outline"))
            {
                wallContainer.gameObject.layer = LayerMask.NameToLayer("Selectable");
            }

            if (this.RealRoomContainer.gameObject.layer == LayerMask.NameToLayer("Outline"))
            {
                this.RealRoomContainer.gameObject.layer = LayerMask.NameToLayer("Selectable");
            }

            this.RealRoomContainer.SetActive(false);
        }

        /// <summary>
        /// Cleans up real room infos for rebuild it.
        /// </summary>
        public void CleanRealRoomInfoForRebuild()
        {
            foreach (var floorEntity in this.BluePrint.BluePrintFloorEntities)
            {
                floorEntity.ClearRealRoomInfo();
                var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);

                for (short i = 0; i < 4; ++i)
                {
                    var neighbourFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, i)?.FloorEntity;
                    if (neighbourFloorEntity != null && neighbourFloorEntity.OccupiedRoom == null)
                    {
                        neighbourFloorEntity.SetWallByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[i], null);
                        foreach (var j in this.BluePrintData.DIRECTION_OPPOSITE_CORNERS_MAP[i])
                        {
                            neighbourFloorEntity.SetOutWallCornerByDir(j, null);
                        }
                    }
                }

                for (short i = 4; i < 8; ++i)
                {
                    var neighbourCornerFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, i)?.FloorEntity;
                    if (neighbourCornerFloorEntity != null && neighbourCornerFloorEntity.OccupiedRoom == null)
                    {
                        neighbourCornerFloorEntity.SetOutWallCornerByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[i], null);
                    }
                }
            }

            this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
            {
                if (d.CantBuildRealRoom == false)
                {
                    d.OutRoomFloorEntity.OccupiedDoorEntities.Remove(d);
                }
            });

            this.BluePrint.BluePrintFurnitureEntities.ForEach(f => f.FloorEntities.ForEach(ff => ff.OccupiedFurniture = null));
            this.RealRoomDoors.Clear();
        }

        /// <summary>
        /// Rebuilds real room. 
        /// </summary>
        public void RebuildRealRoom()
        {
            this.CleanupAndDisableContainer();
            this.CleanRealRoomInfoForRebuild();
            this.BuildRealRoom();
        }

        /// <summary>
        /// Builds real room.
        /// </summary>
        public void BuildRealRoom()
        {
            this.BuildRealRoomBase(this.BluePrint.BluePrintFloorEntities);
            this.BuildRealRoomWindows();
            this.BuildRealRoomDoors();
            this.BuildRealRoomFurnitures();
            this.BuildRealRoomWallFurnitures();
        }

        /// <summary>
        /// Builds real room windows.
        /// </summary>
        private void BuildRealRoomWindows()
        {
            foreach (var furnitureEntity in this.BluePrint.BluePrintWindowFurnitureEntities)
            {
                if (furnitureEntity.FurnitureType == FurnitureType.Common_Window)
                {
                    var floorEntity = furnitureEntity.FloorEntity;
                    var direction = furnitureEntity.Direction;
                    var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);

                    if (floorEntity.GetWallByDir(direction).IsWindow == false)
                    {
                        GameObjectRecycler.inst.Destroy(floorEntity.GetWallByDir(direction).Wall);
                        var wall = GameObjectRecycler.inst.Instantiate(this.InsideWindowWallPrefab, this.realRoomWallsContainerTransform);
                        wall.transform.localRotation = this.BluePrintData.DRAW_QUATERNIONS[direction];

                        if(Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                        {
                            wall.transform.position = floorEntity.GetWorldPositionByDir(direction) + Vector3.up * this.BluePrintData.REALROOM_OFFICEEDITOR_OFFSET_Y + this.BluePrintData.REALROOM_WINDOWWALL_OFFSETS[direction];
                        }
                        else
                        { 
                            wall.transform.position = floorEntity.GetWorldPositionByDir(direction) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y + this.BluePrintData.REALROOM_WINDOWWALL_OFFSETS[direction];
                        }

                        floorEntity.GetWallByDir(direction).IsWindow = true;
                        floorEntity.GetWallByDir(direction).Wall = wall;

                        var neighbourFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, direction)?.FloorEntity;
                        var neighbourWallEntity = neighbourFloorEntity?.GetWallByDir((short)((direction + 2) % 4));

                        if ((neighbourWallEntity?.IsWindow ?? true) == false)
                        {
                            // If the floor belong to another room, the new created wall should belong to that room.
                            var originalParent = neighbourWallEntity.Wall.transform.parent;
                            GameObjectRecycler.inst.Destroy(neighbourWallEntity.Wall);
                            wall = GameObjectRecycler.inst.Instantiate(neighbourFloorEntity.OccupiedRoom != null ? neighbourFloorEntity.OccupiedRoom.GetComponent<RealRoom>().InsideWindowWallPrefab : this.OutsideWindowWallPrefab, originalParent);
                            wall.transform.localRotation = this.BluePrintData.REALROOM_NEIGHBOURWALL_QUATERNIONS[direction];

                            if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                            {
                                wall.transform.position = neighbourFloorEntity.GetWorldPositionByDir((short)((direction + 2) % 4)) + Vector3.up * this.BluePrintData.REALROOM_OFFICEEDITOR_OFFSET_Y + this.BluePrintData.REALROOM_OFFICEWALL_OFFSETS[direction];
                            }
                            else
                            {
                                wall.transform.position = neighbourFloorEntity.GetWorldPositionByDir((short)((direction + 2) % 4)) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y + this.BluePrintData.REALROOM_NEIGHBOURWALL_OFFSETS[direction];
                            }

                            neighbourWallEntity.IsWindow = true;
                            neighbourWallEntity.Wall = wall;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds real room doors.
        /// </summary>
        private void BuildRealRoomDoors()
        {
            foreach (var furnitureEntity in this.BluePrint.BluePrintDoorFurnitureEntities)
            {
                if (furnitureEntity?.FurnitureType == FurnitureType.Common_Door)
                {
                    var door = GameObjectRecycler.inst.Instantiate(this.DoorPrefab, this.realRoomFurnitureContainerTransform);
                    door.transform.position = furnitureEntity.Transform.position;
                    door.transform.localRotation = furnitureEntity.Transform.localRotation;
                    door.GetComponent<FurnitureController>().DisableBuildablePanel();
                    this.RealRoomDoors.Add(door);

                    var floorEntity = furnitureEntity.FloorEntity;
                    var direction = furnitureEntity.Direction;
                    furnitureEntity.OutRoomFloorEntity.OccupiedDoorEntities.Add(furnitureEntity);

                    var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);
                    GameObjectRecycler.inst.Destroy(floorEntity.GetWallByDir(direction).Wall);

                    if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                    {
                        door.transform.position = new Vector3(door.transform.position.x, floorEntity.LeftDownWorldPosition.y + this.BluePrintData.REALROOM_OFFICEEDITOR_OFFSET_Y, door.transform.position.z);
                        var neighbourFloorEntityRight = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, (short)((direction + 1) % 4))?.FloorEntity;
                        GameObjectRecycler.inst.Destroy(neighbourFloorEntityRight.GetWallByDir(direction).Wall);
                    }
                    else
                    {
                        var wall = GameObjectRecycler.inst.Instantiate(this.InsideDoorWallPrefab, this.realRoomWallsContainerTransform);
                        wall.transform.localRotation = this.BluePrintData.REALROOM_DOORWALL_QUATERNIONS[direction];
                        wall.transform.position = floorEntity.GetWorldPositionByDir(direction) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y + this.BluePrintData.REALROOM_DOORWALL_OFFSETS[direction];
                        floorEntity.GetWallByDir(direction).Wall = wall;
                    }

                    var neighbourFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, direction)?.FloorEntity;
                    var neighbourWallEntity = neighbourFloorEntity?.GetWallByDir((short)((direction + 2) % 4));

                    if (neighbourWallEntity != null)
                    {
                        var originalParent = neighbourWallEntity.Wall.transform.parent;
                        GameObjectRecycler.inst.Destroy(neighbourWallEntity.Wall);

                        if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                        {
                            var neighbourOfficeFloorRight = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, (short)((direction + 1) % 4));
                            var neighbourFloorEntityRightUp = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(neighbourOfficeFloorRight, direction)?.FloorEntity;
                            var neighbourWallEntityRightUp = neighbourFloorEntityRightUp?.GetWallByDir((short)((direction + 2) % 4));

                            if (neighbourWallEntityRightUp != null)
                            {
                                GameObjectRecycler.inst.Destroy(neighbourWallEntityRightUp.Wall);
                            }
                        }
                        else
                        {
                            var wall = GameObjectRecycler.inst.Instantiate(neighbourFloorEntity.OccupiedRoom != null ? neighbourFloorEntity.OccupiedRoom.GetComponent<RealRoom>().InsideDoorWallPrefab : this.OutsideDoorWallPrefab, originalParent);
                            wall.transform.localRotation = this.BluePrintData.REALROOM_NEIGHBOUR_DOORWALL_QUATERNIONS[direction];
                            wall.transform.position = neighbourFloorEntity.GetWorldPositionByDir((short)((direction + 2) % 4)) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y + this.BluePrintData.REALROOM_NEIGHBOUR_DOORWALL_OFFSETS[direction];
                            neighbourWallEntity.Wall = wall;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds real room furnitures. 
        /// </summary>
        private void BuildRealRoomFurnitures()
        {
            foreach (var furnitureEntity in this.BluePrint.BluePrintFurnitureEntities)
            {
                var furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[furnitureEntity.Furniture.GetComponent<FurnitureController>().FurnitureType], this.realRoomFurnitureContainerTransform);
                furniture.transform.position = furnitureEntity.Transform.position;
                furniture.transform.localRotation = Quaternion.Euler(Vector3.up * furnitureEntity.Direction * 90);
                furnitureEntity.FloorEntities.ForEach(f => f.OccupiedFurniture = furnitureEntity);
                furniture.GetComponent<FurnitureController>().DisableBuildablePanel();

                // Bind the new furniture with its entity.
                furniture.GetComponent<FurniturePropertiesExample>()?.SetFurnitureEntity(furnitureEntity);
            }
        }

        /// <summary>
        /// Builds real room wall furnitures. 
        /// </summary>
        private void BuildRealRoomWallFurnitures()
        {
            foreach (var furnitureEntity in this.BluePrint.BluePrintWallFurnitureEntities)
            {
                var furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[furnitureEntity.Furniture.GetComponent<FurnitureController>().FurnitureType], this.realRoomFurnitureContainerTransform);
                furniture.transform.position = furnitureEntity.Transform.position;
                furniture.transform.localRotation = Quaternion.Euler(Vector3.up * furnitureEntity.Direction * 90);
                furnitureEntity.FloorEntities.ForEach(f => f.GetWallByDir(furnitureEntity.Direction)?.SetOccupiedFurniture(furnitureEntity));
                furniture.GetComponent<FurnitureController>().DisableBuildablePanel();

                // Bind the new furniture with its entity.
                furniture.GetComponent<FurniturePropertiesExample>()?.SetFurnitureEntity(furnitureEntity);
            }
        }

        /// <summary>
        /// Builds real room floors.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        private void BuildRealRoomFloors(List<FloorEntity> selectedFloorEntities)
        {
            this.officeFloorCollection.Reset(selectedFloorEntities);

            // Build base walls around floor.
            foreach (var officeFloor in this.officeFloorCollection.OfficeFloors)
            {
                // Set occupied room.
                var floorEntity = officeFloor.FloorEntity;
                floorEntity.OccupiedRoom = this.gameObject;

                // Create real room floor.
                var floor = GameObjectRecycler.inst.Instantiate(this.FloorPrefab, this.realRoomFloorsContainerTransform);

                if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                {
                    floor.transform.position = floorEntity.LeftDownWorldPosition + Vector3.up * this.BluePrintData.REALROOM_OFFICEEDITOR_FLOOR_OFFSET_Y;
                }
                else
                {
                    floor.transform.position = floorEntity.LeftDownWorldPosition + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y;
                }
            }
        }

        /// <summary>
        /// Builds real room inside walls around floor.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        private void BuildRealRoomInnerWallsAroundFloor(List<FloorEntity> selectedFloorEntities)
        {
            this.officeFloorCollection.Reset(selectedFloorEntities);

            // Build base walls around floor.
            foreach (var officeFloor in this.officeFloorCollection.OfficeFloors)
            {
                var floorEntity = officeFloor.FloorEntity;

                GameObject wall;
                for (short i = 0; i < 4; ++i)
                {
                    // Need to build wall if there is no neighbour floor.
                    if (this.officeFloorCollection.GetOfficeFloorByDir(officeFloor, i) == null)
                    {
                        // Destroy wall corners on current floor.
                        foreach (short j in this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[i])
                        {
                            if (floorEntity.GetOutWallCornerByDir(j) != null)
                            {
                                if (floorEntity.GetOriginalWallCornerByDir(j) != null)
                                {
                                    floorEntity.GetOutWallCornerByDir(j).SetActive(false);
                                }
                                else
                                {
                                    GameObjectRecycler.inst.Destroy(floorEntity.GetOutWallCornerByDir(j));
                                    floorEntity.SetOutWallCornerByDir(j, null);
                                }
                            }
                        }

                        // Destroy wall on current floor.
                        if (floorEntity.GetWallByDir(i) != null)
                        {
                            if (floorEntity.GetOriginalWallByDir(i) != null)
                            {
                                floorEntity.GetWallByDir(i).Wall.SetActive(false);
                            }
                            else
                            {
                                GameObjectRecycler.inst.Destroy(floorEntity.GetWallByDir(i).Wall);
                            }
                        }

                        // 1. Neighbour floor's inside wall has window, new wall must has window.
                        // 2. Neighbour room's outside wall has window, new wall must has window.
                        var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);
                        var neighbourFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, i)?.FloorEntity;

                        if ((floorEntity.GetWallByDir(i)?.IsWindow ?? false) == true || neighbourFloorEntity?.OccupiedRoom != null && (neighbourFloorEntity.GetWallByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[i])?.IsWindow ?? false) == true)
                        {
                            wall = GameObjectRecycler.inst.Instantiate(this.InsideWindowWallPrefab, this.realRoomWallsContainerTransform);
                        }
                        else if (floorEntity.OccupiedDoorEntities.Any(d => this.BluePrintData.OPPOSITE_DIRECTIONS[i] == d.Direction))
                        {
                            wall = GameObjectRecycler.inst.Instantiate(this.InsideDoorWallPrefab, this.realRoomWallsContainerTransform);
                        }
                        else
                        {
                            wall = GameObjectRecycler.inst.Instantiate(this.InsideWallPrefab, this.realRoomWallsContainerTransform);
                        }

                        wall.transform.localRotation = this.BluePrintData.DRAW_QUATERNIONS[i];

                        if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                        {
                            wall.transform.position = floorEntity.GetWorldPositionByDir(i) + Vector3.up * this.BluePrintData.REALROOM_OFFICEEDITOR_OFFSET_Y + this.BluePrintData.REALROOM_WINDOWWALL_OFFSETS[i];
                        }
                        else
                        {
                            wall.transform.position = floorEntity.GetWorldPositionByDir(i) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y + this.BluePrintData.REALROOM_WINDOWWALL_OFFSETS[i];
                        }

                        floorEntity.SetWallByDir(i, new WallEntity(wall, false, (floorEntity.GetWallByDir(i)?.IsWindow ?? false), floorEntity));
                    }
                }
            }
        }

        /// <summary>
        /// Build real room inside wall corner around floor.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        private void BuildRealRoomInnerWallCornersAroundFloor(List<FloorEntity> selectedFloorEntities)
        {
            this.officeFloorCollection.Reset(selectedFloorEntities);

            foreach (var officeFloor in this.officeFloorCollection.OfficeFloors)
            {
                var floorEntity = officeFloor.FloorEntity;

                // Build base wall corners on current floor.
                for (short i = 0; i < 4; ++i)
                {
                    // For example:
                    // No left up wall corner but has left floor and up floor, need to build a left up wall corner.
                    if (this.officeFloorCollection.GetOfficeFloorByDir(officeFloor, (short)(i + 4)) == null &&
                        this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[i + 4].All(j => this.officeFloorCollection.GetOfficeFloorByDir(officeFloor, j) != null))
                    {
                        // Destroy other room's out wall corner on current floor.
                        if (floorEntity.GetOutWallCornerByDir((short)(i + 4)) != null)
                        {
                            if (floorEntity.GetOriginalWallCornerByDir((short)(i + 4)) != null)
                            {
                                floorEntity.GetOutWallCornerByDir((short)(i + 4)).SetActive(false);
                            }
                            else
                            {
                                GameObjectRecycler.inst.Destroy(floorEntity.GetOutWallCornerByDir((short)(i + 4)));
                                floorEntity.SetOutWallCornerByDir((short)(i + 4), null);
                            }
                        }

                        var wallCorner = GameObjectRecycler.inst.Instantiate(this.InsideWallCornerPrefab, this.realRoomWallsContainerTransform);
                        wallCorner.transform.localRotation = this.BluePrintData.DRAW_QUATERNIONS[i];

                        if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                        {
                            wallCorner.transform.position = floorEntity.GetWorldPositionByDir(i) + Vector3.up * this.BluePrintData.REALROOM_OFFICEEDITOR_OFFSET_Y;
                        }
                        else
                        {
                            wallCorner.transform.position = floorEntity.GetWorldPositionByDir(i) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y;
                        }

                        floorEntity.SetWallCornerByDir((short)(i + 4), wallCorner);
                    }
                }
            }
        }

        /// <summary>
        /// Builds real room outside walls around floor.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        private void BuildRealRoomOutWallsAroundFloor(List<FloorEntity> selectedFloorEntities)
        {
            foreach (var floorEntity in selectedFloorEntities)
            {
                var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);
                GameObject wall;
                for (short i = 0; i < 4; ++i)
                {
                    var neighbourFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, i)?.FloorEntity;
                    if (neighbourFloorEntity != null && neighbourFloorEntity.OccupiedRoom == null)
                    {
                        foreach (var j in this.BluePrintData.DIRECTION_OPPOSITE_CORNERS_MAP[i])
                        {
                            if (neighbourFloorEntity.GetOutWallCornerByDir(j) != null)
                            {
                                if (neighbourFloorEntity.GetOriginalWallCornerByDir(j) != null)
                                {
                                    neighbourFloorEntity.GetOutWallCornerByDir(j).SetActive(false);
                                }
                                else
                                {
                                    GameObjectRecycler.inst.Destroy(neighbourFloorEntity.GetOutWallCornerByDir(j));
                                    neighbourFloorEntity.SetOutWallCornerByDir(j, null);
                                }
                            }
                        }

                        wall = GameObjectRecycler.inst.Instantiate(this.OutsideWallPrefab, this.realRoomWallsContainerTransform);
                        wall.transform.localRotation = this.BluePrintData.REALROOM_NEIGHBOURWALL_QUATERNIONS[i];
                        
                        if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                        {
                            wall.transform.position = neighbourFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_OPPOSITE_CORNERS_MAP[i][0] - 4)) + Vector3.up * this.BluePrintData.REALROOM_OFFICEEDITOR_OFFSET_Y + this.BluePrintData.REALROOM_OFFICEWALL_OFFSETS[i];
                        }
                        else
                        {
                            wall.transform.position = neighbourFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_OPPOSITE_CORNERS_MAP[i][0] - 4)) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y + this.BluePrintData.REALROOM_OUTWALLCORNER_OFFSETS[i];
                        }

                        neighbourFloorEntity.SetWallByDir((short)((i + 2) % 4), new WallEntity(wall, true, false, neighbourFloorEntity));
                    }
                }
            }
        }

        /// <summary>
        /// Builds real room outside wall corners around floor.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        private void BuildRealRoomOutWallCornersAroundFloor(List<FloorEntity> selectedFloorEntities)
        {
            foreach (var floorEntity in selectedFloorEntities)
            {
                var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);
                for (int i = 0; i < 4; ++i)
                {
                    var neighbourCornerFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, (short)(i + 4))?.FloorEntity;
                    if (this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[i + 4].All(j => this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, j)?.FloorEntity?.GetWallByDir((short)((j + 2) % 4))?.IsOut ?? false) == true &&
                        neighbourCornerFloor != null &&
                        neighbourCornerFloor.OccupiedRoom == null &&
                        neighbourCornerFloor.GetOutWallCornerByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[i + 4]) == null)
                    {
                        var wallCorner = GameObjectRecycler.inst.Instantiate(this.OutsideWallCornerPrefab, this.realRoomWallsContainerTransform);
                        wallCorner.transform.localRotation = this.BluePrintData.REALROOM_NEIGHBOUR_DOORWALL_QUATERNIONS[i];
                        
                        if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                        {
                            wallCorner.transform.position = neighbourCornerFloor.GetWorldPositionByDir((short)(this.BluePrintData.OPPOSITE_DIRECTIONS[i + 4] - 4)) + Vector3.up * this.BluePrintData.REALROOM_OFFICEEDITOR_OFFSET_Y;
                        }
                        else
                        {
                            wallCorner.transform.position = neighbourCornerFloor.GetWorldPositionByDir((short)(this.BluePrintData.OPPOSITE_DIRECTIONS[i + 4] - 4)) + Vector3.up * this.BluePrintData.REALROOM_LAYER_OFFSET_Y;
                        }

                        neighbourCornerFloor.SetOutWallCornerByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[i + 4], wallCorner);
                    }
                }
            }
        }

        /// <summary>
        /// Builds base floors and walls for real room. The walls type could be modify in follow furniture build steps.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        private void BuildRealRoomBase(List<FloorEntity> selectedFloorEntities)
        {
            if (!selectedFloorEntities.Any())
            {
                return;
            }

            this.RealRoomContainer.SetActive(true);
            this.BluePrint.BluePrintContainer.SetActive(false);

            this.BuildRealRoomFloors(selectedFloorEntities);
            this.BuildRealRoomInnerWallsAroundFloor(selectedFloorEntities);
            this.BuildRealRoomInnerWallCornersAroundFloor(selectedFloorEntities);
            this.BuildRealRoomOutWallsAroundFloor(selectedFloorEntities);
            this.BuildRealRoomOutWallCornersAroundFloor(selectedFloorEntities);
        }

        /// <summary>
        /// Executes after Awake.
        /// </summary>
        private void OnEnable()
        {
            this.BluePrint = this.GetComponent<BluePrint>();

            this.RealRoomContainer = this.transform.Find("RealRoom").gameObject;
            this.realRoomFloorsContainerTransform = this.RealRoomContainer.transform.Find("Floors");
            this.realRoomWallsContainerTransform = this.RealRoomContainer.transform.Find("Walls");
            this.realRoomFurnitureContainerTransform = this.RealRoomContainer.transform.Find("Furnitures");

            this.officeFloorCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);
        }
    }
}
