namespace RoomBuildingStarterKit.Components
{
    using UnityEngine;
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using RoomBuildingStarterKit.Common;
    using System.Collections.Generic;
    using RoomBuildingStarterKit.Entity;
    using System.Linq;
    using RoomBuildingStarterKit.Helpers;

    /// <summary>
    /// The ModifyFurnitureBase class used for PutFurniture/MoveFurniture states.
    /// </summary>
    [Serializable]
    public abstract class ModifyFurnitureBase : BluePrintState
    {
        /// <summary>
        /// The editing furniture type.
        /// </summary>
        protected int editingFurnitureType = -1;

        /// <summary>
        /// The editing furniture direction.
        /// </summary>
        protected short editingFurnitureDirection = 0;

        /// <summary>
        /// The editing furniture gameObject.
        /// </summary>
        protected GameObject editingFurniture;
        
        /// <summary>
        /// The door smooth follow velocity.
        /// </summary>
        protected Vector3 doorSmoothFollowVelocity;

        /// <summary>
        /// The window smooth follow velocity.
        /// </summary>
        protected Vector3 windowSmoothFollowVelocity;

        /// <summary>
        /// The furniture smooth follow velocity.
        /// </summary>
        protected Vector3 furnitureSmoothFollowVelocity;

        /// <summary>
        /// The office collection.
        /// </summary>
        protected OfficeFloorCollection officeCollection = new OfficeFloorCollection();

        /// <summary>
        /// The furniture occupied floor entrities.
        /// </summary>
        private List<FloorEntity> occupiedFloorEntities = new List<FloorEntity>();
        
        /// <summary>
        /// The border floor entities.
        /// </summary>
        private List<List<FloorEntity>> borderFloorEntities = new List<List<FloorEntity>>() { new List<FloorEntity>(), new List<FloorEntity>(), new List<FloorEntity>(), new List<FloorEntity>() };

        /// <summary>
        /// Updates build furniture.
        /// </summary>
        public void UpdateBuildFurnitureCommon()
        {
            if (this.editingFurniture != null && this.BluePrint.BluePrintFloorEntities.Any())
            {
                if (this.editingFurniture.name.Contains("Window"))
                {
                    this.UpdateBuildBluePrintWindow();
                }
                else if (this.editingFurniture.name.Contains("Door"))
                {
                    this.UpdateBuildBluePrintDoor();
                }
                else if ((this.editingFurniture.GetComponent<FurnitureController>()?.IsWallFurniture ?? false) == true)
                {
                    this.UpdateBuildBluePrintWallFurniture();
                }
                else
                {
                    this.UpdateBuildFurniture();
                }
            }
        }

        /// <summary>
        /// Tries transiting to add blue print floor state.
        /// * Mouse click on add room floor button.
        /// </summary>
        /// <returns>The state.</returns>
        public StateBase TryTransitToAddFloor()
        {
            return (this.BluePrintData.AddFloorButtonClicked || this.editingFurniture == null) ? this.GetStateByType(typeof(AddFloor)) : null;
        }

        /// <summary>
        /// Tries transiting to delete floor state.
        /// </summary>
        /// <returns>The state.</returns>
        public StateBase TryTransitToDeleteFloor()
        {
            return this.BluePrintData.DeleteFloorButtonClicked ? this.GetStateByType(typeof(DeleteFloor)) : null;
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected void OnExitCommon()
        {
            this.BluePrintData.IsModifyFurniture = false;

            if (this.editingFurniture != null)
            {
                Destroy(this.editingFurniture);
                this.editingFurniture = null;
                this.editingFurnitureType = -1;
            }

            FurnitureHelper.EnableFurnituresSelectable(this.BluePrint);

            this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        protected void UpdatePutFurniture()
        {
            this.UpdateBuildFurnitureCommon();

            if (Input.GetKeyUp(KeyCode.E))
            {
                Destroy(this.editingFurniture);
                this.editingFurniture = null;
                this.editingFurnitureType = -1;
            }
        }

        /// <summary>
        /// Puts down window.
        /// </summary>
        /// <param name="floorEntity">The window occupied floor entity.</param>
        /// <param name="dir">The window direction.</param>
        /// <param name="targetPosition">The target position.</param>
        /// <param name="targetRotation">The target rotation.</param>
        protected abstract void PutDownWindow(FloorEntity floorEntity, short dir, Vector3 targetPosition, Quaternion targetRotation);

        /// <summary>
        /// Puts down wall furniture.
        /// </summary>
        /// <param name="floorEntity">The target floor entity.</param>
        /// <param name="floorEntities">The target floor entities.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        protected abstract void PutDownWallFurniture(FloorEntity floorEntity, List<FloorEntity> floorEntities, short dir, Vector3 targetPosition, Quaternion targetRotation);

        /// <summary>
        /// Puts down furniture.
        /// </summary>
        /// <param name="floorEntity">The target floor entity.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        /// <param name="occupiedFloorEntities">The occupied floor entities.</param>
        protected abstract void PutDownFurniture(FloorEntity floorEntity, Vector3 targetPosition, Quaternion targetRotation, List<FloorEntity> occupiedFloorEntities);

        /// <summary>
        /// Checks whether can put room door.
        /// </summary>
        /// <param name="targetFloorEntity">The target floor entity.</param>
        /// <param name="neighbourFloorEntity">The neighbour floor entity.</param>
        /// <param name="globalOfficeFloor">The global office floor.</param>
        /// <param name="dir">The door direction.</param>
        /// <returns>Can put door or not.</returns>
        private bool CheckCanPutDoor(FloorEntity targetFloorEntity, FloorEntity neighbourFloorEntity, OfficeFloor globalOfficeFloor, short dir)
        {
            return !(neighbourFloorEntity == null ||
                    (neighbourFloorEntity.GetWallByDir((short)((dir + 2) % 4))?.IsWindow ?? false) == true ||    // Overlaps with window of another room.
                    globalOfficeFloor.FloorEntity.OccupiedDoorEntities.Any(d => d.OutRoomFloorEntity == globalOfficeFloor.FloorEntity && d.InRoomFloorEntity == neighbourFloorEntity) ||     // Overlaps with another door of self room.
                    this.BluePrint.BluePrintDoorFurnitureEntities.Any(d => d.InRoomFloorEntity == globalOfficeFloor.FloorEntity && (dir == d.Direction || !this.BluePrint.CanDoorEntranceOverlap) || d.OutRoomFloorEntity == neighbourFloorEntity && !this.BluePrint.CanDoorEntranceOverlap) ||    // Entrance overlaps with another door of self room.
                    this.BluePrint.BluePrintWindowFurnitureEntities.Any(w => w.FloorEntity == targetFloorEntity && dir == w.Direction) ||    // Overlaps with window of self room.
                    this.BluePrint.BluePrintFurnitureEntities.Any(f => f.FloorEntities.Any(ff => ff == targetFloorEntity)) ||    // Overlaps with a furniture of self room.
                    neighbourFloorEntity.OccupiedFurniture != null ||    // Overlaps with a furniture of another room.
                    this.BluePrint.BluePrintWallFurnitureEntities.Any(f => dir == f.Direction && f.FloorEntities.Any(ff => ff == targetFloorEntity)) || // Overlaps with a wall furniture of self room.
                    neighbourFloorEntity.GetWallByDir((short)((dir + 2) % 4))?.OccupiedFurniture != null || // Overlaps with a wall furniture of another room.
                    !(this.BluePrint.CanDoorEntranceOverlap || globalOfficeFloor.FloorEntity.OccupiedDoorEntities.Count == 0 && neighbourFloorEntity.OccupiedDoorEntities.Count == 0 && (neighbourFloorEntity.OccupiedRoom == null || !neighbourFloorEntity.OccupiedRoom.GetComponent<BluePrint>().BluePrintDoorFurnitureEntities.Any(dd => dd.InRoomFloorEntity == neighbourFloorEntity))));    // Entrance overlaps with another door of other room.
        }

        /// <summary>
        /// Checks whether can put office door.
        /// </summary>
        /// <param name="targetFloorEntity">The target floor entity.</param>
        /// <param name="neighbourFloorEntity">The neighbour floor entity.</param>
        /// <param name="globalOfficeFloor">The global office floor.</param>
        /// <param name="dir">The door direction.</param>
        /// <returns>Can put door or not.</returns>
        private bool CheckCanPutOfficeDoor(FloorEntity targetFloorEntity, FloorEntity neighbourFloorEntity, OfficeFloor globalOfficeFloor, short dir, bool checkWindow = true)
        {
            return !(neighbourFloorEntity == null ||
                globalOfficeFloor.FloorEntity.OccupiedDoorEntities.Any(d => d.OutRoomFloorEntity == globalOfficeFloor.FloorEntity && d.InRoomFloorEntity == neighbourFloorEntity) ||     // Overlaps with another door of self room.
                this.BluePrint.BluePrintDoorFurnitureEntities.Any(d => d.InRoomFloorEntity == globalOfficeFloor.FloorEntity && (dir == d.Direction || !this.BluePrint.CanDoorEntranceOverlap) || d.OutRoomFloorEntity == neighbourFloorEntity && !this.BluePrint.CanDoorEntranceOverlap) ||    // Entrance overlaps with another door of self room.
                checkWindow && this.BluePrint.BluePrintWindowFurnitureEntities.Any(w => w.FloorEntity == targetFloorEntity && dir == w.Direction));    // Overlaps with window of self room.
        }

        /// <summary>
        /// Executes every frame to udpate blue print door.
        /// </summary>
        private void UpdateBuildBluePrintDoor()
        {
            this.GetMouseNearestBorderFloorEntityWithDirection(this.BluePrint.BluePrintFloorEntities, out FloorEntity targetFloorEntity, out short dir);
            if (targetFloorEntity != null)
            {
                var furnitureTransform = this.editingFurniture.transform;
                Vector3 targetPosition = Vector3.zero;
                Quaternion targetRotation = Quaternion.Euler(Vector3.zero);

                var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(targetFloorEntity.X, targetFloorEntity.Z);
                bool canPutDoor = true;

                targetPosition = (targetFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[dir][0] - 4)) + targetFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[dir][1] - 4))) / 2f + this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;

                if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                {
                    targetPosition = targetFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[dir][1] - 4)) + this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                }
                
                targetRotation = this.BluePrintData.DRAW_QUATERNIONS[dir];

                if (this.editingFurniture.activeSelf == false)
                {
                    this.editingFurniture.SetActive(true);
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                }

                furnitureTransform.position = Vector3.SmoothDamp(furnitureTransform.position, targetPosition, ref this.doorSmoothFollowVelocity, BluePrintCursor.MOVE_SMOOTH_TIME, float.MaxValue, Time.unscaledDeltaTime);
                furnitureTransform.localRotation = Quaternion.Lerp(furnitureTransform.localRotation, targetRotation, Time.unscaledDeltaTime * 20f);

                var neighbourFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, dir)?.FloorEntity;
                canPutDoor = this.CheckCanPutDoor(targetFloorEntity, neighbourFloorEntity, globalOfficeFloor, dir);

                if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                {
                    var globalOfficeFloorRight = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, (short)((dir + 1) % 4));
                    var neighbourFloorEntityRightUp = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloorRight, dir)?.FloorEntity;

                    canPutDoor &= this.BluePrint.BluePrintFloorEntities.Any(f => f == globalOfficeFloorRight.FloorEntity) && 
                        !this.BluePrint.BluePrintFloorEntities.Any(f => f == neighbourFloorEntityRightUp) && 
                        this.CheckCanPutOfficeDoor(globalOfficeFloorRight?.FloorEntity, neighbourFloorEntityRightUp, globalOfficeFloorRight, dir);

                    var globalOfficeFloorLeft = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, (short)((dir + 3) % 4));
                    var neighbourFloorEntityLeftUp = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloorLeft, dir)?.FloorEntity;
                    canPutDoor &= this.CheckCanPutOfficeDoor(globalOfficeFloorLeft?.FloorEntity, neighbourFloorEntityLeftUp, globalOfficeFloorLeft, dir, checkWindow: false);
                }

                if (canPutDoor)
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowBuildablePanel();
                }
                else
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowNonBuildablePanel();
                }

                if (InputWrapper.GetKeyUp(KeyCode.Mouse0) && dir >= 0 && dir < 4 && canPutDoor)
                {
                    furnitureTransform.position = targetPosition + Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                    furnitureTransform.localRotation = targetRotation;

                    this.editingFurniture.layer = LayerMask.NameToLayer("Selectable");
                    this.editingFurniture.GetComponent<BoxCollider>().enabled = true;

                    this.BluePrint.BluePrintDoorFurnitureEntities.Add(new DoorFurnitureEntity(targetFloorEntity, this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, dir).FloorEntity, dir, FurnitureType.Common_Door, this.editingFurniture));
                    
                    this.editingFurniture = null;
                    this.editingFurnitureType = -1;

                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }
            }
        }

        /// <summary>
        /// Checks whether can put room window.
        /// </summary>
        /// <param name="neighbourFloorEntity">The neighbour floor entity.</param>
        /// <param name="targetFloorEntity">The target floor entity.</param>
        /// <param name="dir">The direction.</param>
        /// <returns>Can put window or not.</returns>
        private bool CheckCanPutWindow(FloorEntity neighbourFloorEntity, FloorEntity targetFloorEntity, short dir)
        {
            return !(neighbourFloorEntity == null ||
                targetFloorEntity.OccupiedRoom == null && neighbourFloorEntity.OccupiedRoom != null && neighbourFloorEntity.GetWallByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[dir]).OccupiedFurniture != null || // Overlaps with a wall furniture of other room.
                this.BluePrint.BluePrintWallFurnitureEntities.Any(f => f.Direction == dir && f.FloorEntities.Any(ff => ff == targetFloorEntity)) ||  // Overlaps with a wall furniture of self room.
                targetFloorEntity.OccupiedDoorEntities.Any(d => d.Direction == (dir + 2) % 4) ||  // Overlaps with a door of other room.
                targetFloorEntity.OccupiedRoom == null && neighbourFloorEntity.OccupiedRoom != null && neighbourFloorEntity.GetWallByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[dir]).IsWindow ||  // Overlaps with a window of other room.
                this.BluePrint.BluePrintDoorFurnitureEntities.Any(d => d.FloorEntity == targetFloorEntity && d.Direction == dir) ||   // Overlaps with a door of self room.
                this.BluePrint.BluePrintWindowFurnitureEntities.Any(w => w.FloorEntity == targetFloorEntity && w.Direction == dir));  // Overlaps with a window of self room.
        }

        /// <summary>
        /// Checks whether can put office window.
        /// </summary>
        /// <param name="neighbourFloorEntity">The neighbour floor entity.</param>
        /// <param name="targetFloorEntity">The target floor entity.</param>
        /// <param name="dir">The direction.</param>
        /// <returns>Can put window or not.</returns>
        private bool CheckCanPutOfficeWindow(FloorEntity neighbourFloorEntity, FloorEntity targetFloorEntity, short dir, bool checkWindow = true)
        {
            return !(neighbourFloorEntity == null ||
                this.BluePrint.BluePrintDoorFurnitureEntities.Any(d => d.FloorEntity == targetFloorEntity && d.Direction == dir) ||   // Overlaps with a door of self room.
                checkWindow && this.BluePrint.BluePrintWindowFurnitureEntities.Any(w => w.FloorEntity == targetFloorEntity && w.Direction == dir));  // Overlaps with a window of self room.
        }

        /// <summary>
        /// Executes every frame to update blue print window.
        /// </summary>
        private void UpdateBuildBluePrintWindow()
        {
            this.GetMouseNearestBorderFloorEntityWithDirection(this.BluePrint.BluePrintFloorEntities, out FloorEntity targetFloorEntity, out short dir);
            if (targetFloorEntity != null)
            {
                var furnitureTransform = this.editingFurniture.transform;
                Vector3 targetPosition = Vector3.zero;
                Quaternion targetRotation = Quaternion.Euler(Vector3.zero);

                var globalOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(targetFloorEntity.X, targetFloorEntity.Z);
                bool canPutWindow = true;

                targetPosition = (targetFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[dir][0] - 4)) + targetFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[dir][1] - 4))) / 2f + this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                targetRotation = this.BluePrintData.DRAW_QUATERNIONS[dir];

                if (this.editingFurniture.activeSelf == false)
                {
                    this.editingFurniture.SetActive(true);
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                }

                furnitureTransform.position = Vector3.SmoothDamp(furnitureTransform.position, targetPosition, ref this.windowSmoothFollowVelocity, BluePrintCursor.MOVE_SMOOTH_TIME, float.MaxValue, Time.unscaledDeltaTime);
                furnitureTransform.localRotation = Quaternion.Lerp(furnitureTransform.localRotation, targetRotation, Time.unscaledDeltaTime * 20f);

                var neighbourFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, dir)?.FloorEntity;
                canPutWindow = this.CheckCanPutWindow(neighbourFloorEntity, targetFloorEntity, dir);

                if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                {
                    var neighbourOfficeFloorLeft = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(globalOfficeFloor, (short)((dir + 3) % 4));
                    var neighbourFloorEntityLeftUp = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(neighbourOfficeFloorLeft, dir)?.FloorEntity;

                    canPutWindow &= this.CheckCanPutOfficeWindow(neighbourFloorEntityLeftUp, neighbourOfficeFloorLeft?.FloorEntity, dir, checkWindow: false);
                }

                if (canPutWindow)
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowBuildablePanel();
                }
                else
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowNonBuildablePanel();
                }

                if (InputWrapper.GetKeyUp(KeyCode.Mouse0) && canPutWindow)
                {
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                    this.PutDownWindow(targetFloorEntity, dir, targetPosition, targetRotation);
                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }
            }
        }

        /// <summary>
        /// Checks whether can put wall furniture.
        /// </summary>
        /// <param name="floorEntities">The target floor entities in even index and neighbour floor entities in odd index.</param>
        /// <param name="dir">The direction.</param>
        /// <returns>Can put window or not.</returns>
        private bool CheckCanPutWallFurniture(List<FloorEntity> floorEntities, short dir)
        {
            var targetFloorEntities = floorEntities.Where((f, i) => i % 2 == 0).ToList();
            var neighbourFloorEntities = floorEntities.Where((f, i) => i % 2 == 1 && f != null).ToList();

            return !(targetFloorEntities.Any(f => !this.BluePrint.BluePrintFloorEntities.Any(ff => ff == f)) || // Inside room and on wall.
                neighbourFloorEntities.Any(f => this.BluePrint.BluePrintFloorEntities.Any(ff => ff == f)) || // Inside room and on wall.
                targetFloorEntities.Any(f => f.GetOriginalWallByDir(dir)?.IsWindow ?? false) ||  // Overlaps with an office window.
                targetFloorEntities.Any(f => f.OccupiedDoorEntities.Any(d => d.Direction == (dir + 2) % 4)) ||  // Overlaps with a door of other room.
                neighbourFloorEntities.Any(f => f.OccupiedRoom != null && f.GetWallByDir(this.BluePrintData.OPPOSITE_DIRECTIONS[dir]).IsWindow) ||  // Overlaps with a window of other room.
                this.BluePrint.BluePrintDoorFurnitureEntities.Any(d => d.Direction == dir && targetFloorEntities.Any(f => f == d.FloorEntity)) ||   // Overlaps with a door of self room.
                this.BluePrint.BluePrintWindowFurnitureEntities.Any(w => w.Direction == dir && targetFloorEntities.Any(f => f == w.FloorEntity)) ||  // Overlaps with a window of self room.
                this.BluePrint.BluePrintWallFurnitureEntities.Any(w => w.Direction == dir && w.FloorEntities.Any(f => targetFloorEntities.Any(ff => ff == f))));  // Overlaps with a wall furniture of self room.
        }

        /// <summary>
        /// Executes every frame to update blue print wall furniture.
        /// </summary>
        private void UpdateBuildBluePrintWallFurniture()
        {
            this.GetMouseNearestBorderFloorEntityWithDirection(this.BluePrint.BluePrintFloorEntities, out FloorEntity targetFloorEntity, out short dir);
            if (targetFloorEntity != null)
            {
                var furnitureTransform = this.editingFurniture.transform;
                Vector3 targetPosition = Vector3.zero;
                Quaternion targetRotation = Quaternion.Euler(Vector3.zero);

                bool canPutWallFurniture = true;

                targetPosition = targetFloorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[dir][0] - 4)) + this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                targetRotation = this.BluePrintData.DRAW_QUATERNIONS[dir];

                if (this.editingFurniture.activeSelf == false)
                {
                    this.editingFurniture.SetActive(true);
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                }

                furnitureTransform.position = Vector3.SmoothDamp(furnitureTransform.position, targetPosition, ref this.windowSmoothFollowVelocity, BluePrintCursor.MOVE_SMOOTH_TIME, float.MaxValue, Time.unscaledDeltaTime);
                furnitureTransform.localRotation = Quaternion.Lerp(furnitureTransform.localRotation, targetRotation, Time.unscaledDeltaTime * 20f);

                var dimension = this.editingFurniture.GetComponent<FurnitureController>().Dimension;
                var targetOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(targetFloorEntity.X, targetFloorEntity.Z);
                this.occupiedFloorEntities.Clear();

                int dimensionX = (dir == 0 || dir == 2) ? dimension.x : dimension.y;
                int dimensionY = (dir == 0 || dir == 2) ? dimension.y : dimension.x;
                int rowMultiplier = ((dir == 0 || dir == 1) ? 1 : -1);
                int colMultiplier = ((dir == 0 || dir == 3) ? 1 : -1);

                for (int i = 0; i < dimensionX; ++i)
                {
                    for (int j = 0; j < dimensionY; ++j)
                    {
                        var officeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(targetOfficeFloor.X + i * rowMultiplier, targetOfficeFloor.Z + j * colMultiplier);
                        if (officeFloor?.FloorEntity != null)
                        {
                            this.occupiedFloorEntities.Add(officeFloor.FloorEntity);
                            var neighbourOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(officeFloor, dir);
                            this.occupiedFloorEntities.Add(neighbourOfficeFloor?.FloorEntity);
                        }
                        else
                        {
                            canPutWallFurniture = false;
                            break;
                        }
                    }

                    if (canPutWallFurniture == false)
                    {
                        break;
                    }
                }

                if (canPutWallFurniture)
                {
                    canPutWallFurniture = this.CheckCanPutWallFurniture(this.occupiedFloorEntities, dir);
                }

                if (canPutWallFurniture)
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowBuildablePanel();
                }
                else
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowNonBuildablePanel();
                }

                if (InputWrapper.GetKeyUp(KeyCode.Mouse0) && canPutWallFurniture)
                {
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                    this.PutDownWallFurniture(targetFloorEntity, this.occupiedFloorEntities.Where((f, i) => i % 2 == 0).ToList(), dir, targetPosition, targetRotation);
                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }
            }
        }

        /// <summary>
        /// Executes every frame to update furniture.
        /// </summary>
        private void UpdateBuildFurniture()
        {
            var floorEntity = this.BluePrint.MouseEventListener.MouseHoveredFloorEntity ?? this.BluePrint.MouseEventListener.LastNotNullMouseHoveredFloorEntity;
            if (floorEntity != null)
            {
                // Calculate the position and rotation of the editing furniture.
                var furnitureTransform = this.editingFurniture.transform;

                if (Input.GetKeyUp(KeyCode.R))
                {
                    this.editingFurnitureDirection = (short)((this.editingFurnitureDirection + 1) % 4);
                    furnitureTransform.position = floorEntity.GetWorldPositionByDir((short)((this.editingFurnitureDirection + 3) % 4)) + this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                }

                var targetPosition = floorEntity.GetWorldPositionByDir((short)((this.editingFurnitureDirection + 3) % 4)) + this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                var targetRotation = Quaternion.Euler(this.editingFurnitureDirection * 90 * Vector3.up);

                if (this.editingFurniture.activeSelf == false)
                {
                    this.editingFurniture.SetActive(true);
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                }

                furnitureTransform.position = Vector3.SmoothDamp(furnitureTransform.position, targetPosition, ref this.furnitureSmoothFollowVelocity, BluePrintCursor.MOVE_SMOOTH_TIME, float.MaxValue, Time.unscaledDeltaTime);
                furnitureTransform.localRotation = Quaternion.Lerp(furnitureTransform.localRotation, targetRotation, Time.unscaledDeltaTime * 20f);

                bool canPutFurniture = true;
                var dimension = this.editingFurniture.GetComponent<FurnitureController>().Dimension;
                var officeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);
                this.occupiedFloorEntities.Clear();

                int dimensionX = (this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 2) ? dimension.x : dimension.y;
                int dimensionY = (this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 2) ? dimension.y : dimension.x;
                int rowMultiplier = ((this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 1) ? 1 : -1);
                int colMultiplier = ((this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 3) ? 1 : -1);

                for (int i = 0; i < dimensionX; ++i)
                {
                    for (int j = 0; j < dimensionY; ++j)
                    {
                        var targetFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(officeFloor.X + i * rowMultiplier, officeFloor.Z + j * colMultiplier)?.FloorEntity;

                        // The occupied floor is invalid.
                        if (targetFloorEntity == null ||
                            this.BluePrint.BluePrintFloorEntities.Any(f => f == targetFloorEntity) == false ||
                            targetFloorEntity.OccupiedRoom != null && targetFloorEntity.OccupiedRoom != this.BluePrint.gameObject ||
                            targetFloorEntity.OccupiedDoorEntities.Any())
                        {
                            canPutFurniture = false;
                        }
                        else
                        {
                            this.occupiedFloorEntities.Add(targetFloorEntity);
                        }
                    }
                }

                // Overlap with door.
                if (this.occupiedFloorEntities.Any(f => this.BluePrint.BluePrintDoorFurnitureEntities.Any(d => d.InRoomFloorEntity == f)))
                {
                    canPutFurniture = false;
                    this.BluePrint.BluePrintDoorFurnitureEntities.Where(d => this.occupiedFloorEntities.Any(f => f == d.InRoomFloorEntity)).ToList().ForEach(d => d.Furniture.GetComponent<FurnitureController>().ShowNonBuildablePanel(false));
                }
                else if (this.BluePrint.BluePrintDoorFurnitureEntities.Any(d => d.CantBuildRealRoom == false))
                {
                    this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d => d.Furniture.GetComponent<FurnitureController>().DisableBuildablePanel());
                }

                // Overlap with other furnitures.
                this.BluePrint.BluePrintFurnitureEntities.ForEach(f =>
                {
                    this.officeCollection.Reset(f.FloorEntities);

                    if (this.occupiedFloorEntities.Any(ff => this.officeCollection.GetOfficeFloor(ff.X, ff.Z) != null))
                    {
                        canPutFurniture = false;
                        f.Furniture.GetComponent<FurnitureController>().ShowNonBuildablePanel(false);
                    }
                    else if (!f.CantBuildRealRoom)
                    {
                        f.Furniture.GetComponent<FurnitureController>().DisableBuildablePanel();
                    }
                });

                if (canPutFurniture)
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowBuildablePanel();
                }
                else
                {
                    this.editingFurniture.GetComponent<FurnitureController>().ShowNonBuildablePanel();
                }

                if (InputWrapper.GetKeyUp(KeyCode.Mouse0) && canPutFurniture)
                {
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                    this.PutDownFurniture(floorEntity, targetPosition, targetRotation, this.occupiedFloorEntities.ToList());
                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }
            }
        }

        /// <summary>
        /// Gets the mouse nearest border floor entity with direction.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        /// <param name="targetFloorEntity">The target floor entity.</param>
        /// <param name="dir">The direction.</param>
        private void GetMouseNearestBorderFloorEntityWithDirection(List<FloorEntity> selectedFloorEntities, out FloorEntity targetFloorEntity, out short dir)
        {
            BuildSystemHelper.GetBorderFloorEntities(this.BluePrint.BluePrintFloorEntities, ref this.borderFloorEntities);

            var point = CameraController.inst.ProjectToGround(InputWrapper.MousePosition);
            float maxDistance = -1f;
            targetFloorEntity = null;
            dir = 0;

            for (short i = 0; i < 4; ++i)
            {
                foreach (var floorEntity in this.borderFloorEntities[i])
                {
                    var middlePoint = (floorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[i][0] - 4)) + floorEntity.GetWorldPositionByDir((short)(this.BluePrintData.DIRECTION_NEIGHBOURS_MAP[i][1] - 4))) / 2f;
                    var distance = Vector3.Distance(point, middlePoint);
                    if (maxDistance < 0 || distance <= maxDistance)
                    {
                        maxDistance = distance;
                        targetFloorEntity = floorEntity;
                        dir = i;
                    }
                }
            }
        }
    }
}