namespace RoomBuildingStarterKit.BuildSystem
{
    using Newtonsoft.Json;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.Entity;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The WallFurnitureEntity class.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class WallFurnitureEntity : FurnitureEntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WallFurnitureEntity"/> class.
        /// </summary>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="floorEntities">The floor entities.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="furnitureType">The furniture type.</param>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        [JsonConstructor]
        public WallFurnitureEntity(FloorEntity floorEntity, List<FloorEntity> floorEntities, short direction, FurnitureType furnitureType, CustomVector3 position, CustomVector3 rotation, FurnitureCustomPersistentProperties customProperties) 
            : base(furnitureType, floorEntity, position, rotation, direction, customProperties)
        {
            this.FloorEntities = floorEntities;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WallFurnitureEntity"/> class.
        /// </summary>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="floorEntities">The floor entities.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="furnitureType">The furniture type.</param>
        /// <param name="furniture">The furniture gameObject.</param>
        public WallFurnitureEntity(FloorEntity floorEntity, List<FloorEntity> floorEntities, short direction, FurnitureType furnitureType, GameObject furniture) : base(furnitureType, furniture, direction, floorEntity)
        {
            this.FloorEntities = floorEntities;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WallFurnitureEntity"/> class.
        /// </summary>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="floorEntities">The floor entities.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="furnitureType">The furniture type.</param>
        /// <param name="furniture">The furniture gameObject.</param>
        /// <param name="customProperties">The custom properties.</param>
        public WallFurnitureEntity(FloorEntity floorEntity, List<FloorEntity> floorEntities, short direction, FurnitureType furnitureType, GameObject furniture, FurnitureCustomPersistentProperties customProperties) : base(furnitureType, furniture, direction, floorEntity, customProperties)
        {
            this.FloorEntities = floorEntities;
        }

        /// <summary>
        /// Gets or sets the floor entities.
        /// </summary>
        [JsonProperty]
        public List<FloorEntity> FloorEntities { get; set; }
    }
}