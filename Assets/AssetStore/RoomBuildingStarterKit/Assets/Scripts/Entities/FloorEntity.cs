namespace RoomBuildingStarterKit.Entity
{
    using Newtonsoft.Json;
    using RoomBuildingStarterKit.BuildSystem;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// The FloorEntity class.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class FloorEntity
    {
        /// <summary>
        /// The left wall entity.
        /// </summary>
        private WallEntity leftWall;

        /// <summary>
        /// The right wall entity.
        /// </summary>
        private WallEntity rightWall;

        /// <summary>
        /// The up wall entity.
        /// </summary>
        private WallEntity upWall;

        /// <summary>
        /// The down wall entity.
        /// </summary>
        private WallEntity downWall;

        /// <summary>
        /// The left up outside wall corner gameObject.
        /// </summary>
        private GameObject leftUpOutWallCorner;

        /// <summary>
        /// The right up outside wall corner gameObject.
        /// </summary>
        private GameObject rightUpOutWallCorner;

        /// <summary>
        /// The left down outside wall corner gameObject.
        /// </summary>
        private GameObject leftDownOutWallCorner;

        /// <summary>
        /// The right down outside wall corner gameObject.
        /// </summary>
        private GameObject rightDownOutWallCorner;

        /// <summary>
        /// The door entities take the floor as their outside door floor.
        /// </summary>
        private HashSet<DoorFurnitureEntity> occupiedDoors = new HashSet<DoorFurnitureEntity>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorEntity"/> class.
        /// </summary>
        /// <param name="x">The index x.</param>
        /// <param name="z">The index z.</param>
        [JsonConstructor]
        public FloorEntity(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorEntity"/> class.
        /// </summary>
        /// <param name="index">The floor index in grid.</param>
        /// <param name="x">The x dimension of the floor in grid.</param>
        /// <param name="z">The y dimension of the floor in grid.</param>
        /// <param name="floorTransform">The floor transform.</param>
        /// <param name="upWall">The wall in up direction of the floor.</param>
        /// <param name="downWall">The wall in down direction of the floor.</param>
        /// <param name="leftWall">The wall in left direction of the floor.</param>
        /// <param name="rightWall">The wall in right direction of the floor.</param>
        /// <param name="office">The office the floor belongs to.</param>
        public FloorEntity(int index, int x, int z, Transform floorTransform, WallEntity upWall, WallEntity downWall, WallEntity leftWall, WallEntity rightWall, GameObject office)
        {
            this.Index = index;
            this.X = x;
            this.Z = z;
            this.Floor = floorTransform.gameObject;
            this.FloorTransform = floorTransform;
            this.upWall = upWall;
            this.downWall = downWall;
            this.leftWall = leftWall;
            this.rightWall = rightWall;
            this.Office = office;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorEntity"/> class.
        /// </summary>
        /// <param name="floorTransform">The floor transform.</param>
        /// <param name="office">The office the floor belongs to.</param>
        public FloorEntity(Transform floorTransform, GameObject office) : this(0, 0, 0, floorTransform, null, null, null, null, office)
        {
        }

        /// <summary>
        /// Gets or sets the office gameObject.
        /// </summary>
        public GameObject Office { get; set; }

        /// <summary>
        /// Gets or sets the floor entity's index in floor entities list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the floor entity's index in x direction.
        /// </summary>
        [JsonProperty]
        public int X { get; set; }

        /// <summary>
        /// Gets the floor entity's index in z direction.
        /// </summary>
        [JsonProperty]
        public int Z { get; set; }

        /// <summary>
        /// Gets the floor gameobject.
        /// </summary>
        public GameObject Floor { get; private set; }

        /// <summary>
        /// Gets the floor transform.
        /// </summary>
        public Transform FloorTransform { get; private set; }

        /// <summary>
        /// Gets the floor's left down corner's local position.
        /// </summary>
        public Vector3 LeftDownLocalPosition { get => this.FloorTransform.localPosition; }

        /// <summary>
        /// Gets the floor's left down corner's world position.
        /// </summary>
        public Vector3 LeftDownWorldPosition { get => this.FloorTransform.position; }

        /// <summary>
        /// Gets the floor's right down corner's local position.
        /// </summary>
        public Vector3 RightDownLocalPosition { get => this.LeftDownLocalPosition - new Vector3(Global.inst.BuildSystemSettings.GridSize, 0f, 0f); }

        /// <summary>
        /// Gets the floor's right down corner's world position.
        /// </summary>
        public Vector3 RightDownWorldPosition { get => this.LeftDownWorldPosition - new Vector3(Global.inst.BuildSystemSettings.GridSize, 0f, 0f); }

        /// <summary>
        /// Gets the floor's left up corner's local position.
        /// </summary>
        public Vector3 LeftUpLocalPosition { get => this.LeftDownLocalPosition - new Vector3(0f, 0f, Global.inst.BuildSystemSettings.GridSize); }

        /// <summary>
        /// Gets the floor's left up corner's world position.
        /// </summary>
        public Vector3 LeftUpWorldPosition { get => this.LeftDownWorldPosition - new Vector3(0f, 0f, Global.inst.BuildSystemSettings.GridSize); }

        /// <summary>
        /// Gets the floor's right up corner's local position.
        /// </summary>
        public Vector3 RightUpLocalPosition { get => this.LeftDownLocalPosition - new Vector3(Global.inst.BuildSystemSettings.GridSize, 0f, Global.inst.BuildSystemSettings.GridSize); }

        /// <summary>
        /// Gets the floor's right up corner's world position.
        /// </summary>
        public Vector3 RightUpWorldPosition { get => this.LeftDownWorldPosition - new Vector3(Global.inst.BuildSystemSettings.GridSize, 0f, Global.inst.BuildSystemSettings.GridSize); }

        /// <summary>
        /// Gets the floor's center local position.
        /// </summary>
        public Vector3 CenterLocalPosition { get => (this.LeftUpLocalPosition + this.RightDownLocalPosition) / 2; }

        /// <summary>
        /// Gets the floor's center world position.
        /// </summary>
        public Vector3 CenterWorldPosition { get => (this.LeftUpWorldPosition + this.RightDownWorldPosition) / 2; }

        /// <summary>
        /// Gets or sets the occupied room.
        /// </summary>
        public GameObject OccupiedRoom { get; set; }

        /// <summary>
        /// Gets or sets the occupied furniture.
        /// </summary>
        public FurnitureEntityBase OccupiedFurniture { get; set; }

        /// <summary>
        /// Gets or sets the left wall. Shows original office inside wall if there had one when delete current room wall.
        /// </summary>
        public WallEntity LeftWall
        {
            get => this.leftWall;
            set
            {
                if (value == null && this.OriginalLeftWall != null)
                {
                    this.OriginalLeftWall.Wall.SetActive(true);
                    this.leftWall = this.OriginalLeftWall;
                }
                else
                {
                    this.leftWall = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the right wall. Shows original office inside wall if there had one when delete current room wall.
        /// </summary>
        public WallEntity RightWall
        {
            get => this.rightWall;
            set
            {
                if (value == null && this.OriginalRightWall != null)
                {
                    this.OriginalRightWall.Wall.SetActive(true);
                    this.rightWall = this.OriginalRightWall;
                }
                else
                {
                    this.rightWall = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the up wall. Shows original office inside wall if there had one when delete current room wall.
        /// </summary>
        public WallEntity UpWall
        {
            get => this.upWall;
            set
            {
                if (value == null && this.OriginalUpWall != null)
                {
                    this.OriginalUpWall.Wall.SetActive(true);
                    this.upWall = this.OriginalUpWall;
                }
                else
                {
                    this.upWall = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the down wall. Shows original office inside wall if there had one when delete current room wall.
        /// </summary>
        public WallEntity DownWall
        {
            get => this.downWall;
            set
            {
                if (value == null && this.OriginalDownWall != null)
                {
                    this.OriginalDownWall.Wall.SetActive(true);
                    this.downWall = this.OriginalDownWall;
                }
                else
                {
                    this.downWall = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the original left wall (office inside wall).
        /// </summary>
        public WallEntity OriginalLeftWall { get; set; }

        /// <summary>
        /// Gets or sets the original right wall (office inside wall).
        /// </summary>
        public WallEntity OriginalRightWall { get; set; }

        /// <summary>
        /// Gets or sets the original up wall (office inside wall).
        /// </summary>
        public WallEntity OriginalUpWall { get; set; }

        /// <summary>
        /// Gets or sets the original down wall (office inside wall).
        /// </summary>
        public WallEntity OriginalDownWall { get; set; }

        /// <summary>
        /// Gets or sets the left up wall corner.
        /// </summary>
        public GameObject LeftUpWallCorner { get; set; }

        /// <summary>
        /// Gets or sets the original left up wall corner (office inside wall corner).
        /// </summary>
        public GameObject OriginalLeftUpWallCorner { get; set; }

        /// <summary>
        /// Gets or sets the original right up wall corner (office inside wall corner).
        /// </summary>
        public GameObject OriginalRightUpWallCorner { get; set; }

        /// <summary>
        /// Gets or sets the original left down wall corner (office inside wall corner).
        /// </summary>
        public GameObject OriginalLeftDownWallCorner { get; set; }

        /// <summary>
        /// Gets or sets the original right down wall corner (office inside wall corner).
        /// </summary>
        public GameObject OriginalRightDownWallCorner { get; set; }

        /// <summary>
        /// Gets or sets whether this is an office door floor.
        /// </summary>
        public bool IsOfficeDoorFloor { get; set; }

        /// <summary>
        /// Gets or sets whether this is an office editor bound floor.
        /// </summary>
        public bool IsOfficeEditorBoundFloor { get; set; } = false;

        /// <summary>
        /// Gets or sets the blue print floor gameObject.
        /// </summary>
        public GameObject BluePrintFloor { get; set; }

        /// <summary>
        /// Gets or sets the occupied doors.
        /// </summary>
        public HashSet<DoorFurnitureEntity> OccupiedDoorEntities => this.occupiedDoors;

        /// <summary>
        /// Gets or sets the right up wall corner.
        /// </summary>
        public GameObject RightUpWallCorner { get; set; }

        /// <summary>
        /// Gets or sets the right down wall corner.
        /// </summary>
        public GameObject RightDownWallCorner { get; set; }

        /// <summary>
        /// Gets or sets the left down wall corner.
        /// </summary>
        public GameObject LeftDownWallCorner { get; set; }

        /// <summary>
        /// Gets or sets the left up outside wall corner.
        /// </summary>
        public GameObject LeftUpOutWallCorner
        {
            get => this.leftUpOutWallCorner;
            set
            {
                if (value == null && this.OriginalLeftUpWallCorner != null)
                {
                    this.OriginalLeftUpWallCorner.SetActive(true);
                    this.leftUpOutWallCorner = this.OriginalLeftUpWallCorner;
                }
                else
                {
                    this.leftUpOutWallCorner = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the right up outside wall corner.
        /// </summary>
        public GameObject RightUpOutWallCorner
        {
            get => this.rightUpOutWallCorner;
            set
            {
                if (value == null && this.OriginalRightUpWallCorner != null)
                {
                    this.OriginalRightUpWallCorner.SetActive(true);
                    this.rightUpOutWallCorner = this.OriginalRightUpWallCorner;
                }
                else
                {
                    this.rightUpOutWallCorner = value;
                }
            }
        }

        /// <summary>
        /// Get or sets the right down outside wall corner.
        /// </summary>
        public GameObject RightDownOutWallCorner
        {
            get => this.rightDownOutWallCorner;
            set
            {
                if (value == null && this.OriginalRightDownWallCorner != null)
                {
                    this.OriginalRightDownWallCorner.SetActive(true);
                    this.rightDownOutWallCorner = this.OriginalRightDownWallCorner;
                }
                else
                {
                    this.rightDownOutWallCorner = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the left down outside wall corner.
        /// </summary>
        public GameObject LeftDownOutWallCorner
        {
            get => this.leftDownOutWallCorner;
            set
            {
                if (value == null && this.OriginalLeftDownWallCorner != null)
                {
                    this.OriginalLeftDownWallCorner.SetActive(true);
                    this.leftDownOutWallCorner = this.OriginalLeftDownWallCorner;
                }
                else
                {
                    this.leftDownOutWallCorner = value;
                }
            }
        }

        /// <summary>
        /// Clears the real room info of the floor entity.
        /// </summary>
        public void ClearRealRoomInfo()
        {
            this.OccupiedFurniture = null;
            this.OccupiedRoom = null;
            this.LeftWall = null;
            this.RightWall = null;
            this.DownWall = null;
            this.UpWall = null;
            this.LeftUpWallCorner = null;
            this.LeftDownWallCorner = null;
            this.RightUpWallCorner = null;
            this.RightDownWallCorner = null;
            this.LeftUpOutWallCorner = null;
            this.LeftDownOutWallCorner = null;
            this.RightUpOutWallCorner = null;
            this.RightDownOutWallCorner = null;
        }

        /// <summary>
        /// Gets world position by direction.
        /// Directions and their relative values:
        /// 0  1
        /// 3  2
        /// </summary>
        /// <returns>The world position.</returns>
        public Vector3 GetWorldPositionByDir(short dir)
        {
            Assert.IsTrue(dir >= 0 && dir < 4);
            switch (dir)
            {
                case 0: return this.LeftUpWorldPosition;
                case 1: return this.RightUpWorldPosition;
                case 2: return this.RightDownWorldPosition;
                case 3: return this.LeftDownWorldPosition;
                default: return Vector3.zero;
            }
        }

        /// <summary>
        /// Gets local position by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The local position.</returns>
        public Vector3 GetLocalPositionByDir(short dir)
        {
            Assert.IsTrue(dir >= 0 && dir < 4);
            switch (dir)
            {
                case 0: return this.LeftUpLocalPosition;
                case 1: return this.RightUpLocalPosition;
                case 2: return this.RightDownLocalPosition;
                case 3: return this.LeftDownLocalPosition;
                default: return Vector3.zero;
            }
        }

        /// <summary>
        /// Gets the wall by direction.
        /// Directions and their relative values:
        ///   0
        /// 3   1
        ///   2
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The wall entity.</returns>
        public WallEntity GetWallByDir(short dir)
        {
            Assert.IsTrue(dir >= 0 && dir < 4, "dir is invalid");
            switch (dir)
            {
                case 0: return this.UpWall;
                case 1: return this.RightWall;
                case 2: return this.DownWall;
                case 3: return this.LeftWall;
                default: return null;
            }
        }

        /// <summary>
        /// Sets the wall by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <param name="value">The wall entity.</param>
        public void SetWallByDir(short dir, WallEntity value)
        {
            Assert.IsTrue(dir >= 0 && dir < 4, "dir is invalid");
            switch (dir)
            {
                case 0: this.UpWall = value; return;
                case 1: this.RightWall = value; return;
                case 2: this.DownWall = value; return;
                case 3: this.LeftWall = value; return;
            }
        }

        /// <summary>
        /// Gets the original wall by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The wall entity.</returns>
        public WallEntity GetOriginalWallByDir(short dir)
        {
            Assert.IsTrue(dir >= 0 && dir < 8, "dir is invalid");
            switch (dir)
            {
                case 0: return this.OriginalUpWall;
                case 1: return this.OriginalRightWall;
                case 2: return this.OriginalDownWall;
                case 3: return this.OriginalLeftWall;
                default: return null;
            }
        }

        /// <summary>
        /// Gets the original wall corner by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The wall corner gameObject.</returns>
        public GameObject GetOriginalWallCornerByDir(short dir)
        {
            Assert.IsTrue(dir >= 4 && dir < 8, "dir is invalid");
            switch (dir)
            {
                case 4: return this.OriginalLeftUpWallCorner;
                case 5: return this.OriginalRightUpWallCorner;
                case 6: return this.OriginalRightDownWallCorner;
                case 7: return this.OriginalLeftDownWallCorner;
                default: return null;
            }
        }

        /// <summary>
        /// Gets the wall corner by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The wall corner gameObject.</returns>
        public GameObject GetWallCornerByDir(short dir)
        {
            Assert.IsTrue(dir >= 4 && dir < 8, "dir is invalid");
            switch (dir)
            {
                case 4: return this.LeftUpWallCorner;
                case 5: return this.RightUpWallCorner;
                case 6: return this.RightDownWallCorner;
                case 7: return this.LeftDownWallCorner;
                default: return null;
            }
        }

        /// <summary>
        /// Sets the wall corner by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <param name="value">The wall corner gameObject.</param>
        public void SetWallCornerByDir(short dir, GameObject value)
        {
            Assert.IsTrue(dir >= 4 && dir < 8, "dir is invalid");
            switch (dir)
            {
                case 4: this.LeftUpWallCorner = value; return;
                case 5: this.RightUpWallCorner = value; return;
                case 6: this.RightDownWallCorner = value; return;
                case 7: this.LeftDownWallCorner = value; return;
            }
        }

        /// <summary>
        /// Gets outside wall corner by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <returns>The wall corner gameObject.</returns>
        public GameObject GetOutWallCornerByDir(short dir)
        {
            Assert.IsTrue(dir >= 4 && dir < 8, "dir is invalid");
            switch (dir)
            {
                case 4: return this.LeftUpOutWallCorner;
                case 5: return this.RightUpOutWallCorner;
                case 6: return this.RightDownOutWallCorner;
                case 7: return this.LeftDownOutWallCorner;
                default: return null;
            }
        }

        /// <summary>
        /// Sets outside wall corner by direction.
        /// </summary>
        /// <param name="dir">The direction.</param>
        /// <param name="value">The wall corner gameObject.</param>
        public void SetOutWallCornerByDir(short dir, GameObject value)
        {
            Assert.IsTrue(dir >= 4 && dir < 8, "dir is invalid");
            switch (dir)
            {
                case 4: this.LeftUpOutWallCorner = value; return;
                case 5: this.RightUpOutWallCorner = value; return;
                case 6: this.RightDownOutWallCorner = value; return;
                case 7: this.LeftDownOutWallCorner = value; return;
            }
        }

        /// <summary>
        /// Checks whether the floor is inside the rectangle range.
        /// </summary>
        /// <param name="leftDownFloorEntity">The left down floor entity.</param>
        /// <param name="rightUpFloorEntity">The right up floor entity.</param>
        /// <returns></returns>
        public bool IsInsideRectangleRange(FloorEntity leftDownFloorEntity, FloorEntity rightUpFloorEntity)
        {
            return this.X >= Mathf.Min(leftDownFloorEntity.X, rightUpFloorEntity.X)
                && this.X <= Mathf.Max(leftDownFloorEntity.X, rightUpFloorEntity.X)
                && this.Z >= Mathf.Min(leftDownFloorEntity.Z, rightUpFloorEntity.Z)
                && this.Z <= Mathf.Max(leftDownFloorEntity.Z, rightUpFloorEntity.Z);
        }

        /// <summary>
        /// Converts the floor entity instance to string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return $"FloorEntity #{this.Index}, Row:{this.Z}, Column:{this.X} LeftDownLocalPosition: {this.LeftDownLocalPosition}{this.GetWallsInfo()}\r\n";
        }

        /// <summary>
        /// Gets the wall infos.
        /// </summary>
        /// <returns>The wall info string.</returns>
        private string GetWallsInfo()
        {
            return (this.LeftWall == null ? string.Empty : $" LeftWall [Object: {this.LeftWall.Wall}, IsOut: {this.LeftWall.IsOut}, IsWindow:{this.LeftWall.IsWindow}]")
                + (this.RightWall == null ? string.Empty : $" RightWall [Object: {this.RightWall.Wall}, IsOut: {this.RightWall.IsOut}, IsWindow:{this.RightWall.IsWindow}]")
                + (this.UpWall == null ? string.Empty : $" UpWall [Object: {this.UpWall.Wall}, IsOut: {this.UpWall.IsOut}, IsWindow:{this.UpWall.IsWindow}]")
                + (this.DownWall == null ? string.Empty : $" DownWall [Object: {this.DownWall.Wall}, IsOut: {this.DownWall.IsOut}, IsWindow:{this.DownWall.IsWindow}]")
                + (this.LeftDownWallCorner == null ? string.Empty : $" LeftDownWallCorner [Object: {this.LeftDownWallCorner}")
                + (this.LeftUpWallCorner == null ? string.Empty : $" LeftUpWallCorner [Object: {this.LeftUpWallCorner}")
                + (this.RightDownWallCorner == null ? string.Empty : $" RightDownWallCorner [Object: {this.RightDownWallCorner}")
                + (this.RightUpWallCorner == null ? string.Empty : $" RightUpWallCorner [Object: {this.RightUpWallCorner}")
                + (this.LeftDownOutWallCorner == null ? string.Empty : $" LeftDownOutWallCorner [Object: {this.LeftDownOutWallCorner}")
                + (this.LeftUpOutWallCorner == null ? string.Empty : $" LeftUpOutWallCorner [Object: {this.LeftUpOutWallCorner}")
                + (this.RightDownOutWallCorner == null ? string.Empty : $" RightDownOutWallCorner [Object: {this.RightDownOutWallCorner}")
                + (this.RightUpOutWallCorner == null ? string.Empty : $" RightUpOutWallCorner [Object: {this.RightUpOutWallCorner}");
        }
    }
}