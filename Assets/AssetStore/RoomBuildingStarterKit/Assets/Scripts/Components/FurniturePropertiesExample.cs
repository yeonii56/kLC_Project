namespace RoomBuildingStarterKit.Components
{
    using Newtonsoft.Json;
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using UnityEngine;

    /// <summary>
    /// The furniture properties serializable data.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class FurnitureCustomPersistentProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurnitureCustomPersistentProperties"/> class.
        /// </summary>
        public FurnitureCustomPersistentProperties()
        {
            this.ID = Guid.NewGuid();
            this.AssignedFromInspector = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurnitureCustomPersistentProperties"/> class.
        /// </summary>
        /// <param name="customProperties">The custom properties.</param>
        public FurnitureCustomPersistentProperties(FurnitureCustomPersistentProperties customProperties)
        {
            this.ID = Guid.NewGuid();
            this.AssignedFromInspector = customProperties.AssignedFromInspector;
            this.Name = customProperties.Name;
            this.Durality = customProperties.Durality;
        }

        /// <summary>
        /// Gets or sets if the properties are assigned from inspector.
        /// </summary>
        [JsonProperty]
        public bool AssignedFromInspector { get; set; } = false;

        /// <summary>
        /// Gets or sets the furniture id.
        /// </summary>
        [JsonProperty]
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets the furniture name.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the furniture durality.
        /// </summary>
        [JsonProperty]
        public float Durality { get; set; }

        /// <summary>
        /// Converts the instance to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"CustomProperties: {this.ID}, {this.Name}, {this.Durality}, {this.AssignedFromInspector}";
        }

        /// <summary>
        /// Assigns example values to the new created instance.
        /// </summary>
        /// <param name="example">The property examples.</param>
        public void Assign(FurniturePropertiesExample example)
        {
            this.AssignedFromInspector = true;
            this.Name = example.Name;
            this.Durality = example.Durality;
        }
    }

    /// <summary>
    /// The furniture properties example.
    /// </summary>
    public class FurniturePropertiesExample : MonoBehaviour
    {
        /// <summary>
        /// The furniture name.
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// The furniture durality.
        /// </summary>
        [Range(0, 100)]
        public float Durality = 100;

        /// <summary>
        /// The furniture entity.
        /// </summary>
        private FurnitureEntityBase furnitureEntity;

        /// <summary>
        /// Gets or sets the furniture entity.
        /// </summary>
        public FurnitureEntityBase FurnitureEntity 
        {
            get => this.furnitureEntity;

            set
            {
                this.furnitureEntity = value;
                this.DisplayProperties();
            }
        }

        /// <summary>
        /// Sets furniture entity.
        /// </summary>
        /// <param name="furnitureEntity">The furniture entity.</param>
        public void SetFurnitureEntity(FurnitureEntityBase furnitureEntity)
        {
            this.FurnitureEntity = furnitureEntity;
        }

        /// <summary>
        /// Display furniture properties.
        /// </summary>
        private void DisplayProperties()
        {
            if (this.FurnitureEntity != null)
            {
                this.Name = this.FurnitureEntity.CustomProperties.Name;
                this.Durality = this.FurnitureEntity.CustomProperties.Durality;
            }
        }

        private void OnValidate()
        {
            if (this.FurnitureEntity != null)
            {
                this.FurnitureEntity.CustomProperties.Name = this.Name;
                this.FurnitureEntity.CustomProperties.Durality = this.Durality;
            }
        }
    }
}