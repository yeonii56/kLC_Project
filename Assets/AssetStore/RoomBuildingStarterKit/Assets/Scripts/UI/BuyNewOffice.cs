namespace RoomBuildingStarterKit.UI
{
    using UnityEngine;
    using TMPro;
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Components;

    /// <summary>
    /// The BuyNewOffice class used to control the unlock panel.
    /// </summary>
    public class BuyNewOffice : Singleton<BuyNewOffice>
    {
        /// <summary>
        /// The blueprinting room.
        /// </summary>
        public BluePrintingRoom BluePrintingRoom;

        /// <summary>
        /// The buy panel.
        /// </summary>
        public GameObject BuyPanel;

        /// <summary>
        /// The price text.
        /// </summary>
        public TextMeshProUGUI Price;

        /// <summary>
        /// The land name.
        /// </summary>
        public TextMeshProUGUIWrapper LandName;

        /// <summary>
        /// Gets or sets the unlockable object.
        /// </summary>
        public UnLockableObject UnLockableObject { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Executes when buy office button clicked.
        /// </summary>
        public void OnBuyOfficeButtonClicked()
        {
            this.UnLockableObject?.UnLockOffice();
            this.BuyPanel.SetActive(false);
        }

        /// <summary>
        /// Updates billboard position.
        /// </summary>
        private void UpdateBillboardPosition()
        {
            var screenCenter = CameraController.inst.Cam.WorldToScreenPoint(this.Position);
            this.BuyPanel.transform.position = new Vector3(screenCenter.x, screenCenter.y, 0);
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            if (InputWrapper.IsBlocking == true || UIMouseEventDetector.CheckMouseEventOnUI() == true || this.BluePrintingRoom.Room != null || GlobalFurnitureManager.inst.IsBuildingOfficeFurniture)
            {
                if (this.BuyPanel.activeSelf == true)
                {
                    this.BuyPanel.SetActive(false);
                }

                return;
            }

            var obj = OfficeMouseEventListener.GetMouseHoveredUnLockableObject();
            if (obj != null)
            {
                obj.GetComponent<UnLockableObject>().ShowBuyPanel();
                this.UpdateBillboardPosition();
            }
            else if(this.BuyPanel.activeSelf == true)
            {
                this.BuyPanel.SetActive(false);
            }
        }
    }
}