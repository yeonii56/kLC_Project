namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.UI;
    using RoomBuildingStarterKit.VisualizeDictionary.Implementations;
    using UnityEngine;

    /// <summary>
    /// The GlobalRoomManager class used to manage rooms.
    /// </summary>
    public class GlobalRoomManager : Singleton<GlobalRoomManager>
    {
        /// <summary>
        /// The room type to prefab mapping.
        /// </summary>
        public RoomTypeToPrefabMapping RoomTypeToPrefabMapping;

        /// <summary>
        /// The blue printing room.
        /// </summary>
        public BluePrintingRoom BluePrintingRoom;

        /// <summary>
        /// The blue printing room type.
        /// </summary>
        private int bluePrintingRoomType = -1;

        /// <summary>
        /// Gets or sets the focus on room.
        /// </summary>
        public GameObject FocusOnRoom { get; set; }

        /// <summary>
        /// Gets the room type to prefab mappings.
        /// </summary>
        public RoomTypeEnumGameObjectDict RoomTypeToPrefabs => this.RoomTypeToPrefabMapping.RoomTypeEnumGameObjectDict;

        /// <summary>
        /// Starts build room.
        /// </summary>
        /// <param name="roomType">The room type.</param>
        public void StartBuildRoom(int roomType)
        {
            this.bluePrintingRoomType = roomType;
            BluePrintCursor.inst.SetState(BluePrintCursorState.Normal);
        }

        /// <summary>
        /// Cancels build room.
        /// </summary>
        public void CancelBuildRoom()
        {
            this.BluePrintingRoom.Room = null;
            this.bluePrintingRoomType = -1;
            BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);
        }

        /// <summary>
        /// Cancels blue print mode.
        /// </summary>
        public void CancelBluePrintMode()
        {
            this.BluePrintingRoom.Room.GetComponent<BluePrint>().CancelBluePrintMode();
            this.CancelBuildRoom();
        }

        /// <summary>
        /// Copies the focus on room.
        /// </summary>
        public void CopyRoom()
        {
            if (this.FocusOnRoom != null)
            {
                var srcRoom = this.FocusOnRoom.GetComponent<BluePrint>();
                this.BluePrintingRoom.Room = srcRoom.Office.GetComponent<OfficeController>().CreateNewRoom(srcRoom.RoomType);
                var dstRoom = this.BluePrintingRoom.Room.GetComponent<BluePrint>();

                var dataString = srcRoom.Serialize();
                dstRoom.CopyFrom(dataString);
                InGameUI.inst.EnterBluePrintMode((int)dstRoom.RoomType);

                this.FocusOnRoom = null;
                BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);
            }
        }

        /// <summary>
        /// Updates focus on room.
        /// </summary>
        private void UpdateFocusOnRoom()
        {
            if (GlobalFurnitureManager.inst.IsBuildingOfficeFurniture)
            {
                return;
            }

            var focusOnFloorEntity = GlobalOfficeMouseEventManager.inst.MouseEventListener?.MouseClickedFloorEntity;
            var room = focusOnFloorEntity?.OccupiedRoom;
            if (this.BluePrintingRoom.Room == null && room != null && room.GetComponent<RealRoom>().RealRoomContainer.activeSelf == true)
            {
                InGameUI.inst.OnBluePrintStarted();
                this.FocusOnRoom = room;
                CameraController.inst.ResetToPoint(focusOnFloorEntity.FloorTransform.position);
            }
            else if (this.FocusOnRoom != null && InputWrapper.GetKeyUp(KeyCode.Mouse1))
            {
                this.FocusOnRoom = null;
                InGameUI.inst.OnBluePrintCompleted();
            }
        }

        /// <summary>
        /// Updates to create new blue print on a office.
        /// </summary>
        private void UpdateNewBluePrint()
        {
            var office = GlobalOfficeMouseEventManager.inst.MouseHoveredOffice;

            if (this.bluePrintingRoomType != -1 && InputWrapper.GetKeyDown(KeyCode.Mouse0) && this.BluePrintingRoom.Room == null && office != null)
            {
                this.BluePrintingRoom.Room = office.GetComponent<OfficeController>().CreateNewRoom((RoomType)this.bluePrintingRoomType);
                InGameUI.inst.EnterBluePrintMode(this.bluePrintingRoomType);
            }
        }

        /// <summary>
        /// On update.
        /// </summary>
        private void Update()
        {
            this.UpdateNewBluePrint();
            this.UpdateFocusOnRoom();
        }
    }
}