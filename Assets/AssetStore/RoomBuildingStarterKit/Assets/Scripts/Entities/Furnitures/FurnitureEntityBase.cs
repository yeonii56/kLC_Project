namespace RoomBuildingStarterKit.BuildSystem
{
    using Newtonsoft.Json;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.Entity;
    using System;
    using UnityEngine;
    
    /// <summary>
    /// The WallEntity class.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class FurnitureEntityBase
    {
        /// <summary>
        /// The position of the furniture entity.
        /// </summary>
        private CustomVector3 position;

        /// <summary>
        /// The rotation of the furniture entity.
        /// </summary>
        private CustomVector3 rotation;

        /// <summary>
        /// The furniture gameObject.
        /// </summary>
        private GameObject furniture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FurnitureEntityBase"/> class.
        /// </summary>
        /// <param name="furnitureType">The furniture type.</param>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="customProperties">The custom properties.</param>
        [JsonConstructor]
        public FurnitureEntityBase(FurnitureType furnitureType, FloorEntity floorEntity, CustomVector3 position, CustomVector3 rotation, short direction, FurnitureCustomPersistentProperties customProperties)
        {
            this.FurnitureType = furnitureType;
            this.Position = position;
            this.rotation = rotation;
            this.Direction = direction;
            this.FloorEntity = floorEntity;
            this.CustomProperties = customProperties;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurnitureEntityBase"/> class.
        /// </summary>
        /// <param name="furnitureType">The furniture type.</param>
        /// <param name="furniture">The furniture.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="customProperties">The custom properties.</param>
        public FurnitureEntityBase(FurnitureType furnitureType, GameObject furniture, short direction, FloorEntity floorEntity, FurnitureCustomPersistentProperties customProperties)
        {
            this.FurnitureType = furnitureType;
            this.Furniture = furniture;
            this.Transform = furniture.transform;
            this.Direction = direction;
            this.FloorEntity = floorEntity;
            this.CustomProperties = customProperties;

            var propertiesDisplay = furniture?.GetComponent<FurniturePropertiesExample>();
            if (propertiesDisplay != null)
            {
                if (customProperties.AssignedFromInspector == false)
                {
                    customProperties.Assign(propertiesDisplay);
                }

                propertiesDisplay.FurnitureEntity = this;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurnitureEntityBase"/> class.
        /// </summary>
        /// <param name="furnitureType">The furniture type.</param>
        /// <param name="furniture">The furniture.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="floorEntity">The floor entity.</param>
        public FurnitureEntityBase(FurnitureType furnitureType, GameObject furniture, short direction, FloorEntity floorEntity) : this(furnitureType, furniture, direction, floorEntity, new FurnitureCustomPersistentProperties())
        {
        }

        /// <summary>
        /// Gets or sets the furniture type.
        /// </summary>
        [JsonProperty]
        public FurnitureType FurnitureType{ get; set; }

        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Gets or sets the furniture gameObject.
        /// </summary>
        public GameObject Furniture
        {
            get => this.furniture;
            set
            {
                this.furniture = value;
                GlobalFurnitureManager.inst.FurnitureGoToFurnitureEntityMaps.Add(value.GetInstanceID(), this);
            }
        }

        /// <summary>
        /// Gets or sets the furniture position.
        /// </summary>
        [JsonProperty]
        public CustomVector3 Position
        {
            get
            {
                return this.Transform != null ? new CustomVector3(this.Transform.position) : this.position;
            }

            set => this.position = value;
        }

        /// <summary>
        /// Gets or sets the furniture rotation.
        /// </summary>
        [JsonProperty]
        public CustomVector3 Rotation
        {
            get
            {
                return this.Transform!= null ? new CustomVector3(this.Transform.rotation.eulerAngles) : this.rotation;
            }

            set => this.rotation = value;
        }

        /// <summary>
        /// Gets or sets the furniture direction.
        /// </summary>
        [JsonProperty]
        public short Direction { get; set; }

        /// <summary>
        /// Gets or sets the furniture occupied floor entity.
        /// </summary>
        [JsonProperty]
        public FloorEntity FloorEntity { get; set; }

        /// <summary>
        /// Gets or sets the furniture custom persistent properties.
        /// </summary>
        [JsonProperty]
        public FurnitureCustomPersistentProperties CustomProperties { get; set; }

        /// <summary>
        /// Gets or sets the furniture can build in real room or not.
        /// </summary>
        public bool CantBuildRealRoom { get; set; } = false;
    }
}