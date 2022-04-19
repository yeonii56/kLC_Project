namespace RoomBuildingStarterKit.BuildSystem
{
    using Newtonsoft.Json;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.Entity;
    using RoomBuildingStarterKit.Helpers;
    using RoomBuildingStarterKit.UI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// The BluePrintSerializableData class is used to store the serializable data in BluePrint class.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class BluePrintSerializableData
    {
        /// <summary>
        /// Gets or sets the blue print floor entities.
        /// </summary>
        [JsonProperty]
        public List<FloorEntity> BluePrintFloorEntities { get; set; } = new List<FloorEntity>();

        /// <summary>
        /// Gets or sets the blue print window furniture entities.
        /// </summary>
        [JsonProperty]
        public List<WindowFurnitureEntity> BluePrintWindowFurnitureEntities { get; set; } = new List<WindowFurnitureEntity>();

        /// <summary>
        /// Gets or sets the blue print wall furniture entities.
        /// </summary>
        [JsonProperty]
        public List<WallFurnitureEntity> BluePrintWallFurnitureEntities { get; set; } = new List<WallFurnitureEntity>();

        /// <summary>
        /// Gets or sets the blue print furniture entities.
        /// </summary>
        [JsonProperty]
        public List<GroundFurnitureEntity> BluePrintFurnitureEntities { get; set; } = new List<GroundFurnitureEntity>();

        /// <summary>
        /// Gets or sets the blue print door furniture entities.
        /// </summary>
        [JsonProperty]
        public List<DoorFurnitureEntity> BluePrintDoorFurnitureEntities { get; set; } = new List<DoorFurnitureEntity>();

        /// <summary>
        /// Gets or sets the room id.
        /// </summary>
        [JsonProperty]
        public Guid RoomID { get; set; } = Guid.NewGuid();
    }

    /// <summary>
    /// The BluePrint class for blue print mode.
    /// </summary>
    [RequireComponent(typeof(StateMachine))]
    public class BluePrint : MonoBehaviour
    {
        /// <summary>
        /// The room type.
        /// </summary>
        public RoomType RoomType;

        /// <summary>
        /// The blue print data.
        /// </summary>
        public BluePrintDataBase BluePrintData;

        /// <summary>
        /// The check list.
        /// </summary>
        public CheckListBase CheckList;

        /// <summary>
        /// The minimum room size.
        /// </summary>
        public Vector2Int MinimumRoomSize;

        /// <summary>
        /// The single door flag used to determine whether one room can have more than one door.
        /// </summary>
        public bool SingleDoor = true;

        /// <summary>
        /// The door entrance can overlap with other door or not.
        /// </summary>
        public bool CanDoorEntranceOverlap = true;

        /// <summary>
        /// The serializable data.
        /// </summary>
        private BluePrintSerializableData serializableData = new BluePrintSerializableData();

        /// <summary>
        /// The serialized data used for copy room.
        /// </summary>
        private string copiedSerializedData;

        /// <summary>
        /// The blue print floors used for add room floor.
        /// </summary>
        private List<GameObject> bluePrintAdditionFloors = new List<GameObject>();

        /// <summary>
        /// The blue print walls used for add room walls.
        /// </summary>
        private List<GameObject> bluePrintAdditionWalls = new List<GameObject>();

        /// <summary>
        /// The blue print floors used for delete room floors.
        /// </summary>
        private List<GameObject> bluePrintDeletionFloors = new List<GameObject>();

        /// <summary>
        /// The blue print walls used for delete room walls.
        /// </summary>
        private List<GameObject> bluePrintDeletionWalls = new List<GameObject>();

        /// <summary>
        /// The state machine.
        /// </summary>
        private StateMachine stateMachine;

        /// <summary>
        /// The combined floor entities used to draw blue print in every frame.
        /// </summary>
        private List<FloorEntity> combineFloorEntities = new List<FloorEntity>();

        /// <summary>
        /// The office floor collection with the entire office size.
        /// </summary>
        private OfficeFloorCollection officeFloorCollection = new OfficeFloorCollection();

        /// <summary>
        /// The office controller.
        /// </summary>
        private OfficeController officeController;

        /// <summary>
        /// The blue print floors container transform.
        /// </summary>
        private Transform bluePrintFloorsContainerTransform;

        /// <summary>
        /// The blue print walls container transform.
        /// </summary>
        private Transform bluePrintWallsContainerTransform;

        /// <summary>
        /// Gets the foundation manager.
        /// </summary>
        public FoundationManager FoundationManager { get; private set; }

        /// <summary>
        /// Gets the blue print should be saved or not.
        /// </summary>
        public bool ShouldBeSaved => string.IsNullOrEmpty(this.copiedSerializedData) == false;

        /// <summary>
        /// Gets or sets the blue print floor entities.
        /// </summary>
        public List<FloorEntity> BluePrintFloorEntities { get => this.serializableData.BluePrintFloorEntities; set => this.serializableData.BluePrintFloorEntities = value; }

        /// <summary>
        /// Gets or sets the blue print window furniture entities.
        /// </summary>
        public List<WindowFurnitureEntity> BluePrintWindowFurnitureEntities { get => this.serializableData.BluePrintWindowFurnitureEntities; set => this.serializableData.BluePrintWindowFurnitureEntities = value; }

        /// <summary>
        /// Gets or sets the blue print wall furniture entities.
        /// </summary>
        public List<WallFurnitureEntity> BluePrintWallFurnitureEntities { get => this.serializableData.BluePrintWallFurnitureEntities; set => this.serializableData.BluePrintWallFurnitureEntities = value; }

        /// <summary>
        /// Gets or sets the blue print furniture entities.
        /// </summary>
        public List<GroundFurnitureEntity> BluePrintFurnitureEntities { get => this.serializableData.BluePrintFurnitureEntities; set => this.serializableData.BluePrintFurnitureEntities = value; }

        /// <summary>
        /// Gets or sets the blue print door furniture entity.
        /// </summary>
        public List<DoorFurnitureEntity> BluePrintDoorFurnitureEntities { get => this.serializableData.BluePrintDoorFurnitureEntities; set => this.serializableData.BluePrintDoorFurnitureEntities = value; }

        /// <summary>
        /// Gets the office gameObject.
        /// </summary>
        public GameObject Office { get; private set; }

        /// <summary>
        /// Gets the real room component.
        /// </summary>
        public RealRoom RealRoom { get; private set; }

        /// <summary>
        /// Gets the blue print container.
        /// </summary>
        public GameObject BluePrintContainer { get; private set; }

        /// <summary>
        /// Gets the mouse event listener.
        /// </summary>
        public OfficeMouseEventListener MouseEventListener { get; private set; }

        /// <summary>
        /// Gets or sets the room id.
        /// </summary>
        public Guid RoomID { get => this.serializableData.RoomID; set => this.serializableData.RoomID = value; }

        /// <summary>
        /// Checks whether can build real room in current blue print state.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        /// <returns>True or false.</returns>
        public bool CanBuildRealRoom(List<FloorEntity> selectedFloorEntities)
        {
            this.CheckList.PendingValidateFloorEntities = selectedFloorEntities;
            this.CheckList.PendingValidateFloorOffset = Vector3Int.zero;
            this.CheckList.Context["BluePrint"] = this;
            var result = this.CheckList.Validate();
            UI.inst.BuildRoomCompleteButton.interactable = result;

            return result;
        }

        /// <summary>
        /// Draw blue print every frame.
        /// </summary>
        /// <param name="selectedFloorEntities">The selected floor entities.</param>
        /// <param name="isAdd">Is add floor or delete floor.</param>
        /// <param name="ignoreOutsideWindow">Whether to ignore outside window or not.</param>
        /// <param name="moveRoomPositionOffsetY">The move room position offset in y direction.</param>
        public void Draw(List<FloorEntity> selectedFloorEntities, bool isAdd = true, bool ignoreOutsideWindow = false, float moveRoomPositionOffsetY = 0)
        {
            var bluePrintFloors = (isAdd ? this.bluePrintAdditionFloors : this.bluePrintDeletionFloors);
            var bluePrintWalls = (isAdd ? this.bluePrintAdditionWalls : this.bluePrintDeletionWalls);

            bluePrintFloors.ForEach(f =>
            {
                if (isAdd)
                {
                    f.GetComponent<MeshRenderer>().sharedMaterial = this.BluePrintData.BluePrintValidMaterial;  // Restore before recycle
                }

                GameObjectRecycler.inst.Destroy(f);
            });

            bluePrintFloors.Clear();

            bluePrintWalls.ForEach(w => GameObjectRecycler.inst.Destroy(w));
            bluePrintWalls.Clear();

            if (!selectedFloorEntities.Any())
            {
                return;
            }

            var floorPrefab = (isAdd ? this.BluePrintData.BluePrintAdditionFloorPrefab : this.BluePrintData.BluePrintDeletionFloorPrefab);
            this.officeFloorCollection.Reset(selectedFloorEntities);
            foreach (var officeFloor in this.officeFloorCollection.OfficeFloors)
            {
                var floorEntity = officeFloor.FloorEntity;

                var floor = GameObjectRecycler.inst.Instantiate(floorPrefab, this.bluePrintFloorsContainerTransform);
                floor.transform.position = floorEntity.LeftDownWorldPosition + Vector3.up * (this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y + moveRoomPositionOffsetY);
                bluePrintFloors.Add(floor);
                floorEntity.BluePrintFloor = floor;

                for (short i = 0; i < 4; ++i)
                {
                    this.CheckAndCreateBluePrintWall(
                        officeFloor,
                        isAdd,
                        ignoreOutsideWindow,
                        this.BluePrintData.DRAW_DIRECTIONS[i],
                        this.BluePrintData.DRAW_QUATERNIONS[i],
                        floorEntity.GetWorldPositionByDir(i) + Vector3.up * (this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y + moveRoomPositionOffsetY) + this.BluePrintData.DRAW_OFFSETS[i]);
                }
            }
        }

        /// <summary>
        /// Updates the blue print size.
        /// </summary>
        /// <param name="isAddFloor">Is add floor or delete floor.</param>
        public void UpdateSize(bool isAddFloor)
        {
            var mouseEventListener = this.MouseEventListener;
            if (mouseEventListener == null || mouseEventListener?.MouseHoveredFurnitureEntity != null && mouseEventListener.MouseSelectedFloorEntities.Count > 0)
            {
                this.UpdateBluePrintFloorEntities();
                return;
            }

            this.BluePrintData.SelectedFloorEntities = mouseEventListener?.MouseSelectedFloorEntities;
            this.combineFloorEntities.Clear();

            // The selected floor entities will be cleared when the left mouse up.
            if (this.BluePrintData.SelectedFloorEntities.Any())
            {
                this.combineFloorEntities.AddRange(this.BluePrintFloorEntities);
                var diffFloorEntities = this.BluePrintData.SelectedFloorEntities.Where(f => !this.combineFloorEntities.Any(e => e.Index == f.Index)).ToList();

                if (isAddFloor)
                {
                    this.combineFloorEntities.AddRange(diffFloorEntities);
                }
                else
                {
                    this.combineFloorEntities.RemoveAll(f => this.BluePrintData.SelectedFloorEntities.Any(e => e.Index == f.Index));
                }

                this.Draw(this.combineFloorEntities);

                // Transfer from add floor state to move room state. The IsAddOrDeleteBluePrinting can only be set to ture when mouse select floors outside the blue print floor entities.
                this.BluePrintData.IsAddOrDeleteBluePrinting = diffFloorEntities.Any();
                this.BluePrintData.LastFrameCombinedFloorEntities = this.combineFloorEntities.ToList();

                // Check before mouse0 up, during add floor and delete floor.
                this.CanBuildRealRoom(this.combineFloorEntities);
            }
            else
            {
                this.BluePrintData.IsAddOrDeleteBluePrinting = false;
            }

            if (!isAddFloor)
            {
                this.Draw(this.BluePrintData.SelectedFloorEntities, isAdd: false);
                this.BluePrintData.IsAddOrDeleteBluePrinting = true;
            }

            this.UpdateBluePrintFloorEntities();
        }


        private void UpdateBluePrintFloorEntities()
        {
            // lastFrameCombinedFloorEntities != null means mouse select floor entities in AddFloor or DeleteFloor state.
            if (Input.GetKeyUp(KeyCode.Mouse0) && this.BluePrintData.LastFrameCombinedFloorEntities != null)
            {
                if (!this.BluePrintData.LastFrameCombinedFloorEntities.Any() || this.officeFloorCollection.Reset(this.BluePrintData.LastFrameCombinedFloorEntities).CheckConnect())
                {
                    this.BluePrintFloorEntities = this.BluePrintData.LastFrameCombinedFloorEntities.ToList();
                }

                this.BluePrintData.LastFrameCombinedFloorEntities = null;

                this.Draw(this.BluePrintFloorEntities);
                this.BluePrintData.IsAddOrDeleteBluePrinting = false;

                // Need to check after mouse0 up.
                this.CanBuildRealRoom(this.BluePrintFloorEntities);
            }
        }

        /// <summary>
        /// Undos the blue print change.
        /// </summary>
        /// <returns>Undo success or not.</returns>
        public bool UndoBluePrint()
        {
            if (!string.IsNullOrEmpty(this.copiedSerializedData))
            {
                this.ClearBluePrintFurnitures();
                this.DeSerialize(this.copiedSerializedData);
                this.Draw(this.BluePrintFloorEntities);
                Assert.IsTrue(this.CanBuildRealRoom(this.BluePrintFloorEntities), "Can't build realroom during UndoBluePrint");
                this.RealRoom.BuildRealRoom();
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Deletes blue print.
        /// </summary>
        public void DeleteRoom()
        {
            this.RealRoom.CleanRealRoomInfoForRebuild();
            this.officeController.Rooms.ForEach(r =>
            {
                if (r.RoomID != this.RoomID)
                {
                    r.Room.GetComponent<RealRoom>().RebuildRealRoom();
                }
            });

            this.DeleteRoomCommon();
            GlobalParticleEffectsManager.inst.PlayDustsEffectByFloorEntities(this.BluePrintFloorEntities, 0, DustEffectType.DeleteRoom);
        }

        /// <summary>
        /// Copies from other blue print.
        /// </summary>
        /// <param name="data">The serialized data.</param>
        public void CopyFrom(string data)
        {
            this.DeSerialize(data, true);
            this.BluePrintContainer.SetActive(true);
            GlobalRoomManager.inst.BluePrintingRoom.Room = this.gameObject;
            this.Draw(this.BluePrintFloorEntities, ignoreOutsideWindow: true);
            this.BluePrintData.MoveBluePrintRelativeFloor = this.BluePrintFloorEntities.First();
            this.stateMachine.SetState(typeof(MoveBluePrint));
        }

        /// <summary>
        /// Cancels blue print mode.
        /// </summary>
        public void CancelBluePrintMode()
        {
            // 1. Stop state machine, make sure state run OnExit. Such as put/move furniture state.
            this.stateMachine.Stop();

            // 2. Undo changes or delete room.
            if (!this.UndoBluePrint())
            {
                this.DeleteRoomCommon();
            }

            // 3. Hide blue print container.
            this.BluePrintContainer.SetActive(false);

            // 4. One room leave blue print mode, other real room doors and office doors should back to normal.
            this.Office.GetComponent<OfficeController>().Rooms.ForEach(r => r.Room.GetComponent<RealRoom>().RealRoomDoors.Where(d => d.activeSelf == true).ToList().ForEach(d => d.GetComponent<FurnitureController>().SetBuildableState(true)));

            this.FoundationManager.OfficeDoors.ForEach(d => d.SetBuildableState(true));
        }

        /// <summary>
        /// Enters the blue print mode.
        /// </summary>
        public void EnterBluePrintMode()
        {
            this.RealRoom.CleanupAndDisableContainer();
            this.RealRoom.CleanRealRoomInfoForRebuild();

            this.Office.GetComponent<OfficeController>().Rooms.ForEach(r =>
            {
                if (r.RoomID != this.RoomID)
                {
                    r.Room.GetComponent<RealRoom>().RebuildRealRoom();
                }
            });

            this.stateMachine.Run();
            this.BluePrintContainer.SetActive(true);
            GlobalRoomManager.inst.BluePrintingRoom.Room = this.gameObject;
            this.Draw(this.BluePrintFloorEntities);
            this.CanBuildRealRoom(this.BluePrintFloorEntities);

            // Refresh inspector window.
            this.BluePrintFurnitureEntities.ForEach(f => f.Furniture?.GetComponent<FurniturePropertiesExample>()?.SetFurnitureEntity(f));
            this.BluePrintWallFurnitureEntities.ForEach(f => f.Furniture?.GetComponent<FurniturePropertiesExample>()?.SetFurnitureEntity(f));
        }

        /// <summary>
        /// Clicks the build room completed button.
        /// </summary>
        public void OnBuildRoomCompleteButtonClicked(Action callback)
        {
            if (!this.CanBuildRealRoom(this.BluePrintFloorEntities))
            {
                return;
            }

            this.OnBuildRoomCompleted();
            callback();
        }

        /// <summary>
        /// Executes when build room completed event triggered.
        /// </summary>
        public void OnBuildRoomCompleted()
        {
            // 1. Stop state machine. Make sure current state run OnExit. Such as put/move furniture state.
            this.stateMachine.Stop();

            // 2. Hide blue print container.
            this.BluePrintContainer.SetActive(false);
            
            // 3. Build real room.
            this.RealRoom.BuildRealRoom();

            // Serialize data when click build real room button.
            this.copiedSerializedData = this.Serialize();

            GlobalParticleEffectsManager.inst.PlayDustsEffectByFloorEntities(this.BluePrintFloorEntities, 0, DustEffectType.BuildRoom);

            GlobalRoomManager.inst.CancelBuildRoom();

            // Auto save after build a room.
            SaveLoader.inst.Save("AutoSave");
        }

        /// <summary>
        /// Serializes this blue print instance.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this.serializableData);
        }

        /// <summary>
        /// Deserializes a blue print instance.
        /// </summary>
        /// <param name="data">The string data.</param>
        /// <param name="isCopy">Whether is a copy action.</param>
        public void DeSerialize(string data, bool isCopy = false)
        {
            this.Assign(JsonConvert.DeserializeObject<BluePrintSerializableData>(data), isCopy);
        }

        /// <summary>
        /// Saves this blue print.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void Save(string fileName)
        {
            if (this.ShouldBeSaved)
            {
                SaveLoader.inst.RoomDatas.Add(this.serializableData);
            }
        }

        /// <summary>
        /// Destroy all furnitures then regenerate by the new furniture.
        /// </summary>
        public void ReGenerateFurnitures()
        {
            this.Assign(this.serializableData);
        }

        /// <summary>
        /// Checks and creates blue print walls.
        /// </summary>
        /// <param name="officeFloorCollection">The office collection.</param>
        /// <param name="officeFloor">The office floor.</param>
        /// <param name="ignoreOutsideWindow">Whether to ignore outside window or not.</param>
        /// <param name="dir">The direction.</param>
        /// <param name="quaternion">The quaternion.</param>
        /// <param name="position">The position.</param>
        private void CheckAndCreateBluePrintWall(OfficeFloor officeFloor, bool isAdd, bool ignoreOutsideWindow, short dir, Quaternion quaternion, Vector3 position)
        {
            GameObject wall;
            var wallPrefab = (isAdd ? this.BluePrintData.BluePrintAdditionWallPrefab : this.BluePrintData.BluePrintDeletionWallPrefab);
            if (this.officeFloorCollection.GetOfficeFloorByDir(officeFloor, dir) == null)
            {
                // Only add floor need to consider window wall.
                if (isAdd && !ignoreOutsideWindow && (officeFloor.FloorEntity.GetWallByDir(dir)?.IsWindow ?? false))
                {
                    wall = GameObjectRecycler.inst.Instantiate(this.BluePrintData.BluePrintWindowWallPrefab, this.bluePrintWallsContainerTransform);
                }
                else
                {
                    wall = GameObjectRecycler.inst.Instantiate(wallPrefab, this.bluePrintWallsContainerTransform);
                }

                wall.transform.localRotation = quaternion;
                wall.transform.position = position;

                var bluePrintWalls = (isAdd ? this.bluePrintAdditionWalls : this.bluePrintDeletionWalls);
                bluePrintWalls.Add(wall);
            }
        }

        /// <summary>
        /// Clears the blue print furnitures.
        /// </summary>
        private void ClearBluePrintFurnitures()
        {
            // Windows.
            this.BluePrintWindowFurnitureEntities.ForEach(w => Destroy(w.Furniture));
            this.BluePrintWindowFurnitureEntities.Clear();

            // Wall Furnitures.
            this.BluePrintWallFurnitureEntities.ForEach(f => Destroy(f.Furniture));
            this.BluePrintWallFurnitureEntities.Clear();

            // Door.
            this.BluePrintDoorFurnitureEntities.ForEach(d => Destroy(d.Furniture));
            this.BluePrintDoorFurnitureEntities.Clear();

            // Furnitures.
            this.BluePrintFurnitureEntities.ForEach(f => Destroy(f.Furniture));
            this.BluePrintFurnitureEntities.Clear();
        }

        /// <summary>
        /// Deletes the room.
        /// </summary>
        private void DeleteRoomCommon()
        {
            GlobalRoomManager.inst.BluePrintingRoom.Room = null;
            this.officeController.Rooms.RemoveAll(r => r.RoomID == this.RoomID);
            InGameUI.inst.OnBuildRoomCompleted();
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Assigns the blue print properties by another blue print.
        /// </summary>
        /// <param name="bluePrint">The blue print.</param>
        /// <param name="isCopy">Whether is a copy action.</param>
        private void Assign(BluePrintSerializableData bluePrint, bool isCopy = false)
        {
            // Floors.
            this.BluePrintFloorEntities = bluePrint.BluePrintFloorEntities.Select(f => this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(f.X, f.Z).FloorEntity).ToList();

            // Windows.
            if (bluePrint.BluePrintWindowFurnitureEntities.Any())
            {
                this.BluePrintWindowFurnitureEntities = bluePrint.BluePrintWindowFurnitureEntities.Select(w =>
                {
                    var floorEntity = this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(w.FloorEntity.X, w.FloorEntity.Z).FloorEntity;
                    var window = Instantiate(this.BluePrintData.BluePrintWindowPrefab, this.BluePrintContainer.transform.Find("Furnitures"));
                    window.transform.position = w.Position.ToVector3();
                    window.transform.rotation = Quaternion.Euler(w.Rotation.ToVector3());
                    window.layer = LayerMask.NameToLayer("Selectable");
                    window.GetComponent<FurnitureController>().DisableBuildablePanel();
                    return new WindowFurnitureEntity(floorEntity, w.Direction, w.FurnitureType, window);
                }).ToList();
            }

            // Wall Furnitures.
            if (bluePrint.BluePrintWallFurnitureEntities.Any())
            {
                this.BluePrintWallFurnitureEntities = bluePrint.BluePrintWallFurnitureEntities.Select(w =>
                {
                    var floorEntity = this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(w.FloorEntity.X, w.FloorEntity.Z).FloorEntity;
                    var floorEntities = w.FloorEntities.Select(ff => this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(ff.X, ff.Z).FloorEntity).ToList();

                    var furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[w.FurnitureType], this.BluePrintContainer.transform.Find("Furnitures"));
                    furniture.transform.position = w.Position.ToVector3();
                    furniture.transform.rotation = Quaternion.Euler(w.Rotation.ToVector3());
                    furniture.GetComponent<FurnitureController>().DisableBuildablePanel();

                    FurnitureHelper.ChangeFurnitureLayer(furniture, LayerMask.NameToLayer("Selectable"));
                    return new WallFurnitureEntity(floorEntity, floorEntities, w.Direction, w.FurnitureType, furniture, isCopy ? new FurnitureCustomPersistentProperties(w.CustomProperties) : w.CustomProperties);
                }).ToList();
            }

            // Doors.
            if (bluePrint.BluePrintDoorFurnitureEntities.Any())
            {
                this.BluePrintDoorFurnitureEntities = bluePrint.BluePrintDoorFurnitureEntities.Select(d =>
                {
                    var door = Instantiate(this.BluePrintData.BluePrintDoorPrefab, this.BluePrintContainer.transform.Find("Furnitures"));
                    door.transform.position = d.Position.ToVector3();
                    door.transform.rotation = Quaternion.Euler(d.Rotation.ToVector3());
                    door.layer = LayerMask.NameToLayer("Selectable");
                    door.GetComponent<FurnitureController>().DisableBuildablePanel();
                    return new DoorFurnitureEntity(
                        this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(d.InRoomFloorEntity.X, d.InRoomFloorEntity.Z).FloorEntity,
                        this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(d.OutRoomFloorEntity.X, d.OutRoomFloorEntity.Z).FloorEntity,
                        d.Direction,
                        d.FurnitureType,
                        door);
                }).ToList();
            }

            // Furnitures.
            if (bluePrint.BluePrintFurnitureEntities.Any())
            {
                this.BluePrintFurnitureEntities = bluePrint.BluePrintFurnitureEntities.Select(f =>
                {
                    var floorEntity = this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(f.FloorEntity.X, f.FloorEntity.Z).FloorEntity;
                    var floorEntities = f.FloorEntities.Select(ff => this.FoundationManager.OfficeFloorCollection.GetOfficeFloor(ff.X, ff.Z).FloorEntity).ToList();

                    var furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[f.FurnitureType], this.BluePrintContainer.transform.Find("Furnitures"));
                    furniture.transform.position = f.Position.ToVector3();
                    furniture.transform.rotation = Quaternion.Euler(f.Rotation.ToVector3());
                    furniture.GetComponent<FurnitureController>().DisableBuildablePanel();

                    FurnitureHelper.ChangeFurnitureLayer(furniture, LayerMask.NameToLayer("Selectable"));
                    return new GroundFurnitureEntity(floorEntity, floorEntities, f.Direction, f.FurnitureType, furniture, isCopy ? new FurnitureCustomPersistentProperties(f.CustomProperties) : f.CustomProperties);
                }).ToList();
            }
        }

        /// <summary>
        /// Initializes blue print data.
        /// </summary>
        private void InitData()
        {
            this.BluePrintContainer = this.transform.Find("BluePrint").gameObject;
            this.bluePrintFloorsContainerTransform = this.BluePrintContainer.transform.Find("Floors");
            this.bluePrintWallsContainerTransform = this.BluePrintContainer.transform.Find("Walls");
            this.Office = this.transform.parent.parent.gameObject;

            this.officeController = this.Office.GetComponent<OfficeController>();
            this.MouseEventListener = this.Office.GetComponent<OfficeMouseEventListener>();
            this.FoundationManager = this.Office.GetComponent<FoundationManager>();
            this.stateMachine = this.GetComponent<StateMachine>();
            this.RealRoom = this.GetComponent<RealRoom>();

            this.officeFloorCollection.Resize(this.FoundationManager.OfficeFloorCollection);
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.InitData();
        }

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            // Build room from save.
            if (SaveLoader.inst.RoomDatas.Any(d => d.RoomID == this.RoomID))
            {
                this.Assign(SaveLoader.inst.RoomDatas.First(d => d.RoomID == this.RoomID));
                this.copiedSerializedData = this.Serialize();
                this.RealRoom.BuildRealRoom();
            }
            // Create new room.
            else
            {
                this.stateMachine.Run();
            }
        }

        /// <summary>
        /// Executes when enable gameObject.
        /// </summary>
        private void OnEnable()
        {
            EventManager.RegisterEvent(EventManager.Event.Save, this, nameof(Save));
            this.stateMachine.Context["BluePrint"] = this;
        }

        /// <summary>
        /// Executes when disable gameObject.
        /// </summary>
        private void OnDisable()
        {
            EventManager.UnRegisterEvent(EventManager.Event.Save, this, nameof(Save));
        }
    }
}