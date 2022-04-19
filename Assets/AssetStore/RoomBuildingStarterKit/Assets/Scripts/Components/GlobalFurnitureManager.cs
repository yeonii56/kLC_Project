namespace RoomBuildingStarterKit.BuildSystem
{
    using Newtonsoft.Json;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Common.Extensions;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.Entity;
    using RoomBuildingStarterKit.Helpers;
    using RoomBuildingStarterKit.VisualizeDictionary.Implementations;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The GlobalFurnitureManagerSerializableData class is used to store the serializable data in BluePrint class.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class GlobalFurnitureManagerSerializableData
    {
        /// <summary>
        /// Gets or sets the blue print furniture entities.
        /// </summary>
        [JsonProperty]
        public List<GroundFurnitureEntity> OfficeFurnitureEntities { get; set; } = new List<GroundFurnitureEntity>();
    }

    /// <summary>
    /// The GlobalFurnitureManager class.
    /// </summary>
    public class GlobalFurnitureManager : Singleton<GlobalFurnitureManager>
    {
        /// <summary>
        /// The furniture opaque material.
        /// </summary>
        public Material FurnitureOpaqueMaterial;

        /// <summary>
        /// The furniture transparent material.
        /// </summary>
        public Material FurnitureTransparentMaterial;

        /// <summary>
        /// The furniture type to prefab mapping.
        /// </summary>
        public FurnitureTypeToPrefabMapping FurnitureTypeToPrefabMapping;

        /// <summary>
        /// The editing furniture direction.
        /// </summary>
        private short editingFurnitureDirection = 0;

        /// <summary>
        /// The editing furniture gameObject.
        /// </summary>
        private GameObject editingFurniture;

        /// <summary>
        /// The furniture smooth follow velocity.
        /// </summary>
        private Vector3 furnitureSmoothFollowVelocity;

        /// <summary>
        /// The global furniture manager serializable data.
        /// </summary>
        private GlobalFurnitureManagerSerializableData globalFurnitureManagerSerializableData = new GlobalFurnitureManagerSerializableData();

        /// <summary>
        /// The office furniture entities.
        /// </summary>
        private List<GroundFurnitureEntity> OfficeFurnitureEntities { get => this.globalFurnitureManagerSerializableData.OfficeFurnitureEntities; }

        /// <summary>
        /// The office furniture occupied floor entities.
        /// </summary>
        private List<FloorEntity> occupiedFloorEntities = new List<FloorEntity>();

        /// <summary>
        /// The editing furniture type.
        /// </summary>
        private int editingFurnitureType = -1;

        /// <summary>
        /// The blue print furniture layer offset.
        /// </summary>
        private readonly Vector3 BLUEPRINT_FURNITURE_LAYER_OFFSET = new Vector3(0f, 0.201f, 0f);

        /// <summary>
        /// The last not null floor entity.
        /// </summary>
        private FloorEntity lastNotNullFloorEntity;

        /// <summary>
        /// The is moving office furniture state.
        /// </summary>
        private bool isMovingOfficeFurniture;

        /// <summary>
        /// The office list.
        /// </summary>
        private List<OfficeController> offices = new List<OfficeController>();

        /// <summary>
        /// Gets the furniture gameObject instance Id to furniture entity dictionary.
        /// </summary>
        public Dictionary<int?, FurnitureEntityBase> FurnitureGoToFurnitureEntityMaps { get; } = new Dictionary<int?, FurnitureEntityBase>();

        /// <summary>
        /// Gets or sets is building office furniture state.
        /// </summary>
        public bool IsBuildingOfficeFurniture { get; set; } = false;

        /// <summary>
        /// Gets the furniture type to prefab mappings.
        /// </summary>
        public FurnitureTypeEnumGameObjectDict FurnitureTypeToPrefabs
        {
            get => this.FurnitureTypeToPrefabMapping.FurnitureTypeEnumGameObjectDict;
        }

        /// <summary>
        /// Executes when put furniture button be clicked.
        /// </summary>
        /// <param name="furnitureType">The furniture type.</param>
        public void OnPutFurnitureButtonClicked(int furnitureType)
        {
            if (this.editingFurniture != null)
            {
                Destroy(this.editingFurniture);
                this.editingFurniture = null;
            }

            this.editingFurnitureType = furnitureType;
            this.editingFurnitureDirection = 0;
            var furniturePrefab = this.FurnitureTypeToPrefabs[(FurnitureType)furnitureType];
            this.CreatePutFurniture(furniturePrefab);
        }

        /// <summary>
        /// Creates put furniture gameObject.
        /// </summary>
        /// <param name="furniturePrefab">The furniture prefab.</param>
        private void CreatePutFurniture(GameObject furniturePrefab)
        {
            BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);

            this.editingFurniture = Instantiate(furniturePrefab, this.transform);
            this.editingFurniture.SetActive(false);

            // Keep outline.
            FurnitureHelper.ChangeFurnitureLayer(this.editingFurniture, LayerMask.NameToLayer("Outline"));
            this.editingFurniture.GetComponent<BoxCollider>().enabled = false;

            this.OfficeFurnitureEntities.ForEach(f => f.Furniture.GetComponent<BoxCollider>().enabled = false);
        }

        /// <summary>
        /// Puts down furniture.
        /// </summary>
        /// <param name="floorEntity">The target floor entity.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        /// <param name="occupiedFloorEntities">The occupied floor entities.</param>
        private void PutDownFurniture(FloorEntity floorEntity, Vector3 targetPosition, Quaternion targetRotation, List<FloorEntity> occupiedFloorEntities)
        {
            var furniture = this.editingFurniture;

            if (this.isMovingOfficeFurniture == false)
            {
                furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[(FurnitureType)this.editingFurnitureType], floorEntity.Office.transform.GetChildByName("RealRoom")?.GetChildByName("Furnitures"));
                furniture.GetComponent<BoxCollider>().enabled = false;
            }

            furniture.transform.position = targetPosition - Vector3.up * 0.05f;
            furniture.transform.localRotation = targetRotation;

            FurnitureHelper.ChangeFurnitureLayer(furniture, LayerMask.NameToLayer("Selectable"));
            furniture.GetComponent<FurnitureController>().DisableBuildablePanel();

            var furnitureEntity = new GroundFurnitureEntity(floorEntity, occupiedFloorEntities, this.editingFurnitureDirection, furniture.GetComponent<FurnitureController>().FurnitureType, furniture);
            furnitureEntity.OfficeType = floorEntity.Office.GetComponent<OfficeController>().OfficeType;
            this.OfficeFurnitureEntities.Add(furnitureEntity);
            occupiedFloorEntities.ForEach(f => f.OccupiedFurniture = furnitureEntity);

            this.OfficeFurnitureEntities.ForEach(f => f.Furniture.GetComponent<BoxCollider>().enabled = this.isMovingOfficeFurniture);
            
            if (this.isMovingOfficeFurniture == true)
            {
                this.editingFurniture = null;
                this.editingFurnitureType = -1;
                this.isMovingOfficeFurniture = false;
            }
        }

        /// <summary>
        /// Executes every frame to update build office furniture.
        /// </summary>
        private void UpdateBuildOfficeFurniture()
        {
            var mouseEventListener = GlobalOfficeMouseEventManager.inst.MouseEventListener;
            var floorEntity = mouseEventListener?.MouseHoveredFloorEntity ?? mouseEventListener?.LastNotNullMouseHoveredFloorEntity ?? this.lastNotNullFloorEntity;

            if (floorEntity != null)
            {
                this.lastNotNullFloorEntity = floorEntity;

                // Calculate the position and rotation of the editing furniture.
                var furnitureTransform = this.editingFurniture.transform;

                if (Input.GetKeyUp(KeyCode.R))
                {
                    this.editingFurnitureDirection = (short)((this.editingFurnitureDirection + 1) % 4);
                    furnitureTransform.position = floorEntity.GetWorldPositionByDir((short)((this.editingFurnitureDirection + 3) % 4));
                }

                var targetPosition = floorEntity.GetWorldPositionByDir((short)((this.editingFurnitureDirection + 3) % 4)) + this.BLUEPRINT_FURNITURE_LAYER_OFFSET + Vector3.up * 0.05f;
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
                var officeFloor = floorEntity.Office.GetComponent<FoundationManager>().OfficeFloorCollection.GetOfficeFloor(floorEntity.X, floorEntity.Z);
                this.occupiedFloorEntities.Clear();

                int dimensionX = (this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 2) ? dimension.x : dimension.y;
                int dimensionY = (this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 2) ? dimension.y : dimension.x;
                int rowMultiplier = ((this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 1) ? 1 : -1);
                int colMultiplier = ((this.editingFurnitureDirection == 0 || this.editingFurnitureDirection == 3) ? 1 : -1);

                for (int i = 0; i < dimensionX; ++i)
                {
                    for (int j = 0; j < dimensionY; ++j)
                    {
                        var targetFloorEntity = floorEntity.Office.GetComponent<FoundationManager>().OfficeFloorCollection.GetOfficeFloor(officeFloor.X + i * rowMultiplier, officeFloor.Z + j * colMultiplier)?.FloorEntity;

                        // The occupied floor is invalid.
                        if (targetFloorEntity == null ||
                            targetFloorEntity.IsOfficeDoorFloor == true ||   // Overlaps with office door.
                            targetFloorEntity.OccupiedFurniture != null ||   // Overlaps with a furniture.
                            targetFloorEntity.OccupiedRoom != null ||   // Overlaps with a room.
                            targetFloorEntity.OccupiedDoorEntities.Any())   // Overlaps with a door.
                        {
                            canPutFurniture = false;
                        }

                        if (targetFloorEntity != null)
                        {
                            this.occupiedFloorEntities.Add(targetFloorEntity);
                        }
                    }
                }

                // Overlap with other furnitures.
                this.OfficeFurnitureEntities.ForEach(f =>
                {
                    if (this.occupiedFloorEntities.Any(ff => f.FloorEntities.Any(fff => fff == ff)))
                    {
                        canPutFurniture = false;
                        f.Furniture.GetComponent<FurnitureController>().ShowNonBuildablePanel(false);
                    }
                    else
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

                if (InputWrapper.GetKeyUp(KeyCode.Mouse0) && canPutFurniture && mouseEventListener?.MouseHoveredFloorEntity != null)
                {
                    furnitureTransform.position = targetPosition;
                    furnitureTransform.localRotation = targetRotation;
                    this.PutDownFurniture(floorEntity, targetPosition, targetRotation, this.occupiedFloorEntities.ToList());
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                this.CancelBuildOfficeFurniture();
                return;
            }
        }

        /// <summary>
        /// Updates move office furniture.
        /// </summary>
        private void UpdateMoveOfficeFurniture()
        {
            var mouseEventListener = GlobalOfficeMouseEventManager.inst.MouseEventListener;
            var furnitureEntity = mouseEventListener?.MouseClickedFurnitureEntity;

            if (furnitureEntity != null && this.editingFurnitureType == -1 && furnitureEntity.FurnitureType.ToString().StartsWith("Office_") == true)
            {
                this.isMovingOfficeFurniture = true;
                BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);

                var screenPos = Camera.main.WorldToScreenPoint(furnitureEntity.FloorEntity.CenterWorldPosition);
                mouseEventListener.SetMousePosition(screenPos);

                this.editingFurnitureType = (int)furnitureEntity.FurnitureType;
                this.editingFurnitureDirection = furnitureEntity.Direction;
                this.editingFurniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[furnitureEntity.FurnitureType], furnitureEntity.Furniture.transform.parent);
                this.editingFurniture.transform.position = furnitureEntity.Furniture.transform.position;
                this.editingFurniture.transform.localRotation = furnitureEntity.Furniture.transform.localRotation;

                // Bind furniture properties after move up it.
                this.editingFurniture.GetComponent<FurniturePropertiesExample>()?.SetFurnitureEntity(furnitureEntity);

                (furnitureEntity as GroundFurnitureEntity).FloorEntities.ForEach(f => f.OccupiedFurniture = null);
                Destroy(furnitureEntity.Furniture);
                this.OfficeFurnitureEntities.Remove(furnitureEntity as GroundFurnitureEntity);

                // Keep outline.
                FurnitureHelper.ChangeFurnitureLayer(this.editingFurniture, LayerMask.NameToLayer("Outline"));
                this.editingFurniture.GetComponent<BoxCollider>().enabled = false;

                this.OfficeFurnitureEntities.ForEach(f => f.Furniture.GetComponent<BoxCollider>().enabled = false);
            }
        }

        /// <summary>
        /// Begin to build office furniture.
        /// </summary>
        public void BeginBuildOfficeFurniture()
        {
            this.IsBuildingOfficeFurniture = true;
            this.OfficeFurnitureEntities.ForEach(f => f.Furniture.GetComponent<BoxCollider>().enabled = this.IsBuildingOfficeFurniture);
        }

        /// <summary>
        /// Cancel build office furniture.
        /// </summary>
        public void CancelBuildOfficeFurniture()
        {
            this.editingFurnitureType = -1;
            this.editingFurnitureDirection = 0;
            Destroy(this.editingFurniture);
            this.editingFurniture = null;

            this.OfficeFurnitureEntities.ForEach(f =>
            {
                f.Furniture.GetComponent<FurnitureController>().DisableBuildablePanel();
                f.Furniture.GetComponent<BoxCollider>().enabled = this.IsBuildingOfficeFurniture;
            });
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            if (this.IsBuildingOfficeFurniture)
            {
                if (this.editingFurnitureType != -1)
                {
                    this.UpdateBuildOfficeFurniture();
                }

                this.UpdateMoveOfficeFurniture();
            }
        }

        /// <summary>
        /// Assigns another office controller instance to this one. 
        /// </summary>
        /// <param name="officeController">The office controller.</param>
        public void Assign(GlobalFurnitureManagerSerializableData globalFurnitureManagerSerializableData)
        {
            globalFurnitureManagerSerializableData.OfficeFurnitureEntities.ForEach(f =>
            {
                var office = this.offices.First(o => o.OfficeType == f.OfficeType);
                var foundationManager = office.GetComponent<FoundationManager>();
                var floorEntity = foundationManager.OfficeFloorCollection.GetOfficeFloor(f.FloorEntity.X, f.FloorEntity.Z).FloorEntity;
                var floorEntities = f.FloorEntities.Select(ff => foundationManager.OfficeFloorCollection.GetOfficeFloor(ff.X, ff.Z).FloorEntity).ToList();

                var furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[f.FurnitureType], office.transform.GetChildByName("RealRoom")?.GetChildByName("Furnitures"));
                furniture.transform.position = f.Position.ToVector3();
                furniture.transform.rotation = Quaternion.Euler(f.Rotation.ToVector3());
                furniture.GetComponent<FurnitureController>().DisableBuildablePanel();

                FurnitureHelper.ChangeFurnitureLayer(furniture, LayerMask.NameToLayer("Selectable"));
                var furnitureEntity = new GroundFurnitureEntity(floorEntity, floorEntities, f.Direction, f.FurnitureType, furniture, f.CustomProperties);
                furnitureEntity.OfficeType = f.OfficeType;

                floorEntities.ForEach(ff => ff.OccupiedFurniture = furnitureEntity);
                this.OfficeFurnitureEntities.Add(furnitureEntity);
            });

            this.OfficeFurnitureEntities.ForEach(f => f.Furniture.GetComponent<BoxCollider>().enabled = false);
        }

        /// <summary>
        /// Save game data into file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void Save(string fileName)
        {
            SaveLoader.inst.FurnitureManagerData = this.globalFurnitureManagerSerializableData;
        }

        /// <summary>
        /// Executes when instantiates gameObject.
        /// </summary>
        protected override void AwakeInternal()
        {
            base.AwakeInternal();
        }

        /// <summary>
        /// Assigns data internally.
        /// </summary>
        private void AssignInternal()
        {
            this.Assign(SaveLoader.inst.FurnitureManagerData);
        }

        private void Start()
        {
            this.transform.GetChilds<OfficeController>(ref this.offices);
            Invoke(nameof(this.AssignInternal), 0.3f);
        }

        /// <summary>
        /// Executes after Awake.
        /// </summary>
        private void OnEnable()
        {
            EventManager.RegisterEvent(EventManager.Event.Save, this, nameof(Save));
        }

        /// <summary>
        /// Executes when gameObject disable.
        /// </summary>
        private void OnDisable()
        {
            EventManager.UnRegisterEvent(EventManager.Event.Save, this, nameof(Save));
        }
    }
}