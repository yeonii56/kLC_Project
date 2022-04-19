namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using UnityEngine.UI;

    /// <summary>
    /// The UI class.
    /// </summary>
    public class UI : Singleton<UI>
    {
        /// <summary>
        /// The add room floor button.
        /// </summary>
        public Button AddRoomFloorButton;

        /// <summary>
        /// The delete room floor button.
        /// </summary>
        public Button DeleteRoomFloorButton;

        /// <summary>
        /// The build room button.
        /// </summary>
        public Button buildRoomButton;

        /// <summary>
        /// The build room complete button.
        /// </summary>
        public Button BuildRoomCompleteButton;

        /// <summary>
        /// The blue print data.
        /// </summary>
        public BluePrintDataBase BluePrintData;

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            UIMouseEventDetector.Initialize();
        }
    }
}
