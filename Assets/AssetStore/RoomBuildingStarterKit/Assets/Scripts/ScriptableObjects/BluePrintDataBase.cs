namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Entity;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The BluePrintDataBase class used to share the blue print common data between other gameObjects.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "BluePrintDataBase", menuName = "BuildSystem/BluePrint/BluePrintDataBase", order = 1)]
    public class BluePrintDataBase : ScriptableObject
    {
        [Tooltip("Blue print floor prefab during add floor.")]
        public GameObject BluePrintAdditionFloorPrefab;

        [Tooltip("Blue print wall prefab during add floor.")]
        public GameObject BluePrintAdditionWallPrefab;

        [Tooltip("BLue print floor prefab during delete floor.")]
        public GameObject BluePrintDeletionFloorPrefab;

        [Tooltip("Blue print wall prefab during delete wall.")]
        public GameObject BluePrintDeletionWallPrefab;

        [Tooltip("Blue print window wall prefab.")]
        public GameObject BluePrintWindowWallPrefab;

        [Tooltip("Blue print door prefab.")]
        public GameObject BluePrintDoorPrefab;

        [Tooltip("Blue print office door prefab.")]
        public GameObject BluePrintOfficeDoorPrefab;

        [Tooltip("Blue print window prefab.")]
        public GameObject BluePrintWindowPrefab;

        [Tooltip("Blue print floor and wall material if the blue print design is valid.")]
        public Material BluePrintValidMaterial;

        [Tooltip("Blue print floor and wall material if the blue print design is invalid.")]
        public Material BluePrintInvalidMaterial;

        /// <summary>
        /// The real room layer offset along y direction.
        /// </summary>
        public readonly float REALROOM_LAYER_OFFSET_Y = 0.201f;

        /// <summary>
        /// The office editor floor layer offset along y direction.
        /// </summary>
        public readonly float REALROOM_OFFICEEDITOR_FLOOR_OFFSET_Y = 0.2f;

        /// <summary>
        /// The office editor layer offset along y direction.
        /// </summary>
        public readonly float REALROOM_OFFICEEDITOR_OFFSET_Y = 0.4f;

        /// <summary>
        /// The blue print layer offset along y direction.
        /// </summary>
        public readonly float BLUEPRINT_LAYER_OFFSET_Y = 0.205f;

        /// <summary>
        /// The blue print furniture layer offset.
        /// </summary>
        public readonly Vector3 BLUEPRINT_FURNITURE_LAYER_OFFSET = new Vector3(0f, 0.207f, 0f);

        public readonly List<short> DRAW_DIRECTIONS = new List<short> { 0, 1, 2, 3, };

        public readonly List<Quaternion> DRAW_QUATERNIONS = new List<Quaternion>
        {
            Quaternion.Euler(new Vector3(0, 0, 0)),
            Quaternion.Euler(new Vector3(0, 90, 0)),
            Quaternion.Euler(new Vector3(0, 180, 0)),
            Quaternion.Euler(new Vector3(0, -90, 0)),
        };

        public readonly List<Vector3> DRAW_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, 0.11f),
            new Vector3(0.11f, 0, 0),
            new Vector3(0, 0, -0.11f),
            new Vector3(-0.11f, 0, 0),
        };

        public readonly List<Quaternion> REALROOM_WINDOWWALL_QUATERNIONS = new List<Quaternion>
        {
            Quaternion.Euler(new Vector3(0, 0, 0)),
            Quaternion.Euler(new Vector3(0, 180, 0)),
            Quaternion.Euler(new Vector3(0, 90, 0)),
            Quaternion.Euler(new Vector3(0, -90, 0)),
        };

        public readonly List<Quaternion> REALROOM_NEIGHBOURWALL_QUATERNIONS = new List<Quaternion>
        {
            Quaternion.Euler(new Vector3(0, 180, 0)),
            Quaternion.Euler(new Vector3(0, -90, 0)),
            Quaternion.Euler(new Vector3(0, 0, 0)),
            Quaternion.Euler(new Vector3(0, 90, 0)),
        };

        public readonly List<Vector3> REALROOM_WINDOWWALL_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, 0.1f),
            new Vector3(0.1f, 0, 0),
            new Vector3(0, 0, -0.1f),
            new Vector3(-0.1f, 0, 0),
        };

        public readonly List<Vector3> REALROOM_NEIGHBOURWALL_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, -0.1f),
            new Vector3(-0.1f, 0, 0),
            new Vector3(0, 0, 0.1f),
            new Vector3(0.1f, 0, 0),
        };

        public readonly List<Quaternion> REALROOM_DOORWALL_QUATERNIONS = new List<Quaternion>
        {
            Quaternion.Euler(new Vector3(0, 0, 0)),
            Quaternion.Euler(new Vector3(0, 90, 0)),
            Quaternion.Euler(new Vector3(0, 180, 0)),
            Quaternion.Euler(new Vector3(0, -90, 0)),
        };

        public readonly List<Vector3> REALROOM_DOORWALL_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, 0.1f),
            new Vector3(0.1f, 0, 0),
            new Vector3(0, 0, -0.1f),
            new Vector3(-0.1f, 0, 0),
        };

        public readonly List<Quaternion> REALROOM_NEIGHBOUR_DOORWALL_QUATERNIONS = new List<Quaternion>
        {
            Quaternion.Euler(new Vector3(0, 180, 0)),
            Quaternion.Euler(new Vector3(0, -90, 0)),
            Quaternion.Euler(new Vector3(0, 0, 0)),
            Quaternion.Euler(new Vector3(0, 90, 0)),
        };

        public readonly List<Vector3> REALROOM_NEIGHBOUR_DOORWALL_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, -0.1f),
            new Vector3(-0.1f, 0, 0),
            new Vector3(0, 0, 0.1f),
            new Vector3(0.1f, 0, 0),
        };

        public readonly List<List<short>> DIRECTION_NEIGHBOURS_MAP = new List<List<short>>
        {
            new List<short> { 4, 5 },
            new List<short> { 5, 6 },
            new List<short> { 6, 7 },
            new List<short> { 7, 4 },
            new List<short> { 0, 3 },
            new List<short> { 0, 1 },
            new List<short> { 1, 2 },
            new List<short> { 2, 3 },
        };

        public readonly List<Vector3> REALROOM_WALL_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, 0.1f),
            new Vector3(0, 0, -0.1f),
            new Vector3(0.1f, 0, 0),
            new Vector3(-0.1f, 0, 0),
        };

        public readonly List<List<short>> DIRECTION_OPPOSITE_CORNERS_MAP = new List<List<short>>
        {
            new List<short> { 6, 7, },
            new List<short> { 7, 4, },
            new List<short> { 4, 5, },
            new List<short> { 5, 6, },
        };

        public readonly List<Vector3> REALROOM_OUTWALLCORNER_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, -0.1f),
            new Vector3(-0.1f, 0, 0),
            new Vector3(0, 0, 0.1f),
            new Vector3(0.1f, 0, 0),
        };

        public readonly List<Vector3> REALROOM_OFFICEWALL_OFFSETS = new List<Vector3>
        {
            new Vector3(0, 0, -0.3f),
            new Vector3(-0.3f, 0, 0),
            new Vector3(0, 0, 0.3f),
            new Vector3(0.3f, 0, 0),
        };

        public readonly List<short> OPPOSITE_DIRECTIONS = new List<short> { 2, 3, 0, 1, 6, 7, 4, 5, };

        /// <summary>
        /// Whether the put furniture button clicked.
        /// </summary>
        [HideInInspector]
        public int putFurnitureButtonClicked = -1;

        /// <summary>
        /// The cache.
        /// </summary>
        private FrameCache cache = new FrameCache();

        /// <summary>
        /// Whether the delete floor button clicked.
        /// </summary>
        private bool deleteFloorButtonClicked = false;

        /// <summary>
        /// Whether the add floor button clicked.
        /// </summary>
        private bool addFloorButtonClicked = false;

        /// <summary>
        /// Gets or sets the mouse selected floor entities.
        /// </summary>
        public List<FloorEntity> SelectedFloorEntities { get; set; } = new List<FloorEntity>();

        /// <summary>
        /// Gets or sets is in adding or deleting blue printing mode.
        /// </summary>
        public bool IsAddOrDeleteBluePrinting { get; set; } = false;

        /// <summary>
        /// Gets or sets is modifying furniture.
        /// </summary>
        public bool IsModifyFurniture { get; set; } = false;

        /// <summary>
        /// Gets or sets is moving blue print.
        /// </summary>
        public bool IsMovingBluePrint { get; set; } = false;

        /// <summary>
        /// Gets or sets the last frame combined floor entities.
        /// </summary>
        public List<FloorEntity> LastFrameCombinedFloorEntities { get; set; }

        /// <summary>
        /// Gets or sets the move blue print mouse first clicked floor.
        /// </summary>
        public FloorEntity MoveBluePrintRelativeFloor { get; set; }

        /// <summary>
        /// Gets is the add floor button clicked.
        /// </summary>
        public bool AddFloorButtonClicked
        {
            get
            {
                this.cache.Cache("AddFloorButtonClicked", 1, () =>
                {
                    this.addFloorButtonClicked = false;
                });

                return this.addFloorButtonClicked;
            }
        }

        /// <summary>
        /// Gets is the delete floor button clicked. 
        /// </summary>
        public bool DeleteFloorButtonClicked
        {
            get
            {
                this.cache.Cache("DeleteFloorButtonClicked", 1, () =>
                {
                    this.deleteFloorButtonClicked = false;
                });

                return this.deleteFloorButtonClicked;
            }
        }

        /// <summary>
        /// Gets is the put furniture button clicked.
        /// </summary>
        public int PutFurnitureButtonClicked
        {
            get
            {
                this.cache.Cache("PutFurnitureButtonClicked", 1, () =>
                {
                    this.putFurnitureButtonClicked = -1;
                });

                return this.putFurnitureButtonClicked;
            }
        }

        /// <summary>
        /// Executes when the add room floor button clicked.
        /// </summary>
        public void OnAddRoomFloorButtonClicked()
        {
            // Call this property to let the cache record this frame.
            var tmp = this.AddFloorButtonClicked;
            this.addFloorButtonClicked = true;
        }

        /// <summary>
        /// Executes when the remove room floor button clicked.
        /// </summary>
        public void OnRemoveRoomFloorButtonClicked()
        {
            // Call this property to let the cache record this frame.
            var tmp = this.DeleteFloorButtonClicked;
            this.deleteFloorButtonClicked = true;
        }

        /// <summary>
        /// Executes when the put furniture button clicked.
        /// </summary>
        /// <param name="furnitureType">The furniture type.</param>
        public void OnPutFurnitureButtonClicked(int furnitureType)
        {
            var tmp = this.PutFurnitureButtonClicked;
            this.putFurnitureButtonClicked = furnitureType;
        }

        /// <summary>
        /// Executes when the gameObject enable.
        /// </summary>
        private void OnEnable()
        {
            this.SelectedFloorEntities = new List<FloorEntity>();
            this.IsAddOrDeleteBluePrinting = false;
            this.IsModifyFurniture = false;
            this.LastFrameCombinedFloorEntities = null;
            this.cache = new FrameCache();
            this.deleteFloorButtonClicked = false;
            this.addFloorButtonClicked = false;
            this.putFurnitureButtonClicked = -1;
            this.MoveBluePrintRelativeFloor = null;
            this.IsMovingBluePrint = false;
        }
    }
}