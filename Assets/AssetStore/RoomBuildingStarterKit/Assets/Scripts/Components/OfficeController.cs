namespace RoomBuildingStarterKit.BuildSystem
{
    using Newtonsoft.Json;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The office types.
    /// </summary>
    public enum OfficeType
    {
        CenterOffice,
        LeftDownOffice,
        LeftUpOffice,
        RightUpOffice,

        TestOffice,
    }

    /// <summary>
    /// The room data.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RoomData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomData"/> class.
        /// </summary>
        /// <param name="roomID">The room guid.</param>
        /// <param name="roomType">The room type.</param>
        [JsonConstructor]
        public RoomData(Guid roomID, RoomType roomType)
        {
            this.RoomID = roomID;
            this.RoomType = roomType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomData"/> class.
        /// </summary>
        /// <param name="roomType">The room type.</param>
        /// <param name="room">The room gameObject.</param>
        public RoomData(RoomType roomType, GameObject room)
        {
            this.RoomType = roomType;
            this.Room = room;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomData"/> class.
        /// </summary>
        /// <param name="roomType">The room type.</param>
        /// <param name="room">The room gameObject.</param>
        /// <param name="roomID">The room guid.</param>
        public RoomData(RoomType roomType, GameObject room, Guid roomID) : this(roomType, room)
        {
            this.RoomID = roomID;
        }

        /// <summary>
        /// Gets or sets the room guid.
        /// </summary>
        [JsonProperty]
        public Guid RoomID { get; set; }

        /// <summary>
        /// Gets or sets the room type.
        /// </summary>
        [JsonProperty]
        public RoomType RoomType { get; set; }

        /// <summary>
        /// Gets or sets the room gameObject.
        /// </summary>
        public GameObject Room { get; set; }
    }

    /// <summary>
    /// The OfficeControllerSerializableData is used to store serializable data in OfficeController class.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class OfficeControllerSerializableData
    {
        /// <summary>
        /// Gets or sets the office type.
        /// </summary>
        [JsonProperty]
        public OfficeType OfficeType { get; set; }

        /// <summary>
        /// Gets or sets the room datas.
        /// </summary>
        [JsonProperty]
        public List<RoomData> Rooms { get; set; } = new List<RoomData>();
    }

    /// <summary>
    /// The OfficeController class.
    /// </summary>
    public class OfficeController : MonoBehaviour
    {
        /// <summary>
        /// The office type.
        /// </summary>
        public OfficeType OfficeType;

        /// <summary>
        /// The rooms container transform.
        /// </summary>
        public Transform RoomsContainerTransform;

        /// <summary>
        /// The serializable data.
        /// </summary>
        private OfficeControllerSerializableData serializableData = new OfficeControllerSerializableData();

        /// <summary>
        /// Gets or sets the room datas.
        /// </summary>
        public List<RoomData> Rooms { get => this.serializableData.Rooms; set => this.serializableData.Rooms = value; }

        /// <summary>
        /// Gets the real room doors.
        /// </summary>
        public List<GameObject> RealRoomDoors 
        {
            get
            {
                var doors = new List<GameObject>();
                this.Rooms.ForEach(r =>
                {
                    r.Room.GetComponent<RealRoom>().RealRoomDoors.Where(d => d.activeSelf == true).ToList().ForEach(d => doors.Add(d));
                });

                return doors;
            }
        }

        /// <summary>
        /// Creates a new room.
        /// </summary>
        /// <param name="roomType">The room type.</param>
        /// <returns></returns>
        public GameObject CreateNewRoom(RoomType roomType)
        {
            var room = Instantiate(GlobalRoomManager.inst.RoomTypeToPrefabs[roomType], this.RoomsContainerTransform);
            this.Rooms.Add(new RoomData(roomType, room, room.GetComponent<BluePrint>().RoomID));
            return room;
        }

        /// <summary>
        /// Saves the office data.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void Save(string fileName)
        {
            SaveLoader.inst.OfficeDatas.Add(this.serializableData);
        }

        /// <summary>
        /// Assigns another office controller instance to this one. 
        /// </summary>
        /// <param name="officeController"></param>
        public void Assign(OfficeControllerSerializableData officeController)
        {
            this.Rooms = officeController.Rooms.Select(r =>
            {
                var room = Instantiate(GlobalRoomManager.inst.RoomTypeToPrefabs[r.RoomType], this.RoomsContainerTransform);
                room.GetComponent<BluePrint>().RoomID = r.RoomID;
                return new RoomData(r.RoomType, room, r.RoomID);
            }).ToList();
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.serializableData.OfficeType = this.OfficeType;
        }

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            if (SaveLoader.inst.OfficeDatas.Any(d => d.OfficeType == this.OfficeType))
            {
                this.Assign(SaveLoader.inst.OfficeDatas.First(d => d.OfficeType == this.OfficeType));
            }
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