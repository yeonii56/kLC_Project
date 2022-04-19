namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Entity;
    using UnityEngine;

    /// <summary>
    /// The WallEntity class.
    /// </summary>
    public class WallEntity
    {
        /// <summary>
        /// Initializes a new instance of ths <see cref="WallEntity"/> class.
        /// </summary>
        /// <param name="wall"></param>
        /// <param name="isOut"></param>
        /// <param name="isWindow"></param>
        /// <param name="floorEntity"></param>
        public WallEntity(GameObject wall, bool isOut, bool isWindow, FloorEntity floorEntity)
        {
            this.Wall = wall;
            this.IsOut = isOut;
            this.IsWindow = isWindow;
            this.FloorEntity = floorEntity;
        }

        /// <summary>
        /// Gets or sets whether the wall is a window wall.
        /// </summary>
        public bool IsWindow { get; set; }

        /// <summary>
        /// Gets or sets the wall gameobject.
        /// </summary>
        public GameObject Wall { get; set; }

        /// <summary>
        /// Gets or sets whether the wall is a outside wall.
        /// </summary>
        public bool IsOut { get; set; }

        /// <summary>
        /// Gets or sets the window occupied floor entity.
        /// </summary>
        public FloorEntity FloorEntity { get; set; }

        /// <summary>
        /// Gets or sets the occupied wall furniture entity.
        /// </summary>
        public WallFurnitureEntity OccupiedFurniture { get; set; }

        /// <summary>
        /// Sets the occupied furniture.
        /// </summary>
        /// <param name="furnitureEntity">The furniture entity.</param>
        public void SetOccupiedFurniture(WallFurnitureEntity furnitureEntity)
        {
            this.OccupiedFurniture = furnitureEntity;
        }
    }
}