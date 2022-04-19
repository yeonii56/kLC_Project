namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Effect;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The InGameUI class.
    /// </summary>
    public class InGameUI : Singleton<InGameUI>
    {
        /// <summary>
        /// The blue printing room.
        /// </summary>
        public BluePrintingRoom BluePrintingRoom;

        /// <summary>
        /// The room list panel.
        /// </summary>
        public GameObject RoomListPanel;

        /// <summary>
        /// The furniture list panel.
        /// </summary>
        public GameObject FurnitureListPanel;
        
        /// <summary>
        /// The blue print tool bar.
        /// </summary>
        public GameObject BluePrintToolBar;

        /// <summary>
        /// The build room tool bar.
        /// </summary>
        public GameObject BuildRoomToolBar;

        /// <summary>
        /// The animator component.
        /// </summary>
        private Animator animator;

        /// <summary>
        /// Executes when blue print mode started.
        /// </summary>
        public void OnBluePrintStarted()
        {
            this.BluePrintToolBar.SetActive(true);
            this.RoomListPanel.SetActive(false);
        }

        /// <summary>
        /// Executes when blue print state completes.
        /// </summary>
        public void OnBluePrintCompleted()
        {
            this.BluePrintToolBar.SetActive(false);
        }

        /// <summary>
        /// Executes when blue print button clicked.
        /// </summary>
        public void OnBluePrintButtonClicked()
        {
            this.EnterBluePrintMode((int)GlobalRoomManager.inst.FocusOnRoom.GetComponent<BluePrint>().RoomType);
            GlobalRoomManager.inst.FocusOnRoom.GetComponent<BluePrint>().EnterBluePrintMode();
            GlobalRoomManager.inst.FocusOnRoom = null;
        }

        /// <summary>
        /// Executes when copy blue print button clicked.
        /// </summary>
        public void OnCopyBluePrintButtonClicked()
        {
            GlobalRoomManager.inst.CopyRoom();
            this.BluePrintToolBar.SetActive(false);
        }

        /// <summary>
        /// Enters blue print mode.
        /// </summary>
        /// <param name="roomType">The room type.</param>
        public void EnterBluePrintMode(int roomType)
        {
            this.FurnitureListPanel.SetActive(true);
            this.FurnitureListPanel.GetComponent<FurnitureListController>().Refresh(roomType);
            this.RoomListPanel.SetActive(false);
            this.BuildRoomToolBar.SetActive(true);
            this.BluePrintToolBar.SetActive(false);
        }

        /// <summary>
        /// Enters build office furniture mode.
        /// </summary>
        public void EnterBuildOfficeFurnitureMode()
        {
            this.FurnitureListPanel.SetActive(true);
            this.FurnitureListPanel.GetComponent<FurnitureListController>().Refresh((int)RoomType.OfficeFurniture);
            this.RoomListPanel.SetActive(false);
            this.BluePrintToolBar.SetActive(false);
            GlobalRoomManager.inst.CancelBuildRoom();
            GlobalFurnitureManager.inst.BeginBuildOfficeFurniture();
        }

        /// <summary>
        /// Executes when build room completed.
        /// </summary>
        public void OnBuildRoomCompleted()
        {
            this.RoomListPanel.SetActive(false);
            this.FurnitureListPanel.SetActive(false);
            this.BuildRoomToolBar.SetActive(false);
            CheckListUI.inst.Hide();
        }

        /// <summary>
        /// Executes when build office furniture completed.
        /// </summary>
        public void OnBuildOfficeFurnitureCompleted()
        {
            this.FurnitureListPanel.SetActive(false);
        }

        /// <summary>
        /// Executes when build room button clicked.
        /// </summary>
        public void OnBuildRoomButtonClicked()
        {
            if (GlobalFurnitureManager.inst.IsBuildingOfficeFurniture == true)
            {
                this.FurnitureListPanel.SetActive(false);
                GlobalFurnitureManager.inst.IsBuildingOfficeFurniture = false;
                GlobalFurnitureManager.inst.CancelBuildOfficeFurniture();
            }

            this.RoomListPanel.SetActive(true);
            this.BluePrintToolBar.SetActive(false);
            GlobalRoomManager.inst.FocusOnRoom = null;
        }

        /// <summary>
        /// Executes when build room complete button clicked.
        /// </summary>
        public void OnBuildRoomCompleteButtonClicked()
        {
            this.BluePrintingRoom.Room.GetComponent<BluePrint>().OnBuildRoomCompleteButtonClicked(this.OnBuildRoomCompleted);
        }

        /// <summary>
        /// Executes when sell room button clicked.
        /// </summary>
        public void OnSellRoomButtonClicked()
        {
            InteractPopupWindow.inst.Show();
            InteractPopupWindow.inst.SetText(UIText.CONFIRM_TO_DELETE_ROOM);
            InteractPopupWindow.inst.ConfirmCallback = () =>
            {
                GlobalRoomManager.inst.FocusOnRoom.GetComponent<BluePrint>().DeleteRoom();
                this.BluePrintToolBar.SetActive(false);
                GlobalRoomManager.inst.FocusOnRoom = null;
            };
        }

        /// <summary>
        /// Executes when settings button clicked.
        /// </summary>
        public void OnSettingsButtonClicked()
        {
            if (GlobalRoomManager.inst.BluePrintingRoom.Room != null)
            {
                InteractPopupWindow.inst.Show(closeWindowBeforeConfirmCallback: true);
                InteractPopupWindow.inst.SetText(UIText.CONFIRM_TO_CANCEL_BLUEPRINT);
                InteractPopupWindow.inst.ConfirmCallback = () =>
                {
                    GlobalRoomManager.inst.CancelBluePrintMode();
                    this.OnBuildRoomCompleted();

                    this.OpenPauseMenu();
                };
            }
            else
            {
                if (this.RoomListPanel.activeSelf == true)
                {
                    GlobalRoomManager.inst.CancelBuildRoom();
                    this.RoomListPanel.SetActive(false);
                }

                if (GlobalFurnitureManager.inst.IsBuildingOfficeFurniture == true)
                {
                    this.FurnitureListPanel.SetActive(false);
                    GlobalFurnitureManager.inst.IsBuildingOfficeFurniture = false;
                    GlobalFurnitureManager.inst.CancelBuildOfficeFurniture();
                }

                this.OpenPauseMenu();
            }
        }

        /// <summary>
        /// Executes when deactive ingame ui.
        /// </summary>
        public void OnDeActiveInGameUI()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            this.animator = this.GetComponent<Animator>();
        }

        /// <summary>
        /// Opens pause menu.
        /// </summary>
        private void OpenPauseMenu()
        {
            this.animator.SetTrigger("InGameUIExit");

            MenuManager.inst.Menus[Menus.PauseMenu].SetActive(true);
            GaussianBlurController.inst.Animator.SetTrigger("StartGaussianBlur");
        }
    }
}