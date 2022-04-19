#if UNITY_EDITOR
namespace RoomBuildingStarterKit.OfficeEditor
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Components;
    using RoomBuildingStarterKit.UI;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;
    using StateMachine = Components.StateMachine;

    /// <summary>
    /// The OfficeEditor class used to design office.
    /// </summary>
    public class OfficeEditor : MonoBehaviour
    {
        /// <summary>
        /// The button to save office as a prefab.
        /// </summary>
        public Button SaveOfficeButton;

        /// <summary>
        /// The button to enter office blue print mode.
        /// </summary>
        public Button OfficeBluePrintButton;

        /// <summary>
        /// The room container's transform.
        /// </summary>
        private Transform rooms;

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.rooms = this.transform.Find("Rooms");
        }

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            var bound = (this.GetComponent<MapGenerator>().MapSize * Global.inst.BuildSystemSettings.GridSize);
            CameraController.inst.WorldLeftDownPoint = new Vector2(-bound, -bound);
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            if (GlobalRoomManager.inst == null || GlobalRoomManager.inst.BluePrintingRoom.Room != null)
            {
                this.OfficeBluePrintButton.interactable = false;
            }
            else
            {
                this.OfficeBluePrintButton.interactable = true;
            }

            if (this.rooms.childCount < 3 || GlobalRoomManager.inst.BluePrintingRoom.Room != null)
            {
                this.SaveOfficeButton.interactable = false;
            }
            else
            {
                this.SaveOfficeButton.interactable = true;
            }
        }

        /// <summary>
        /// Executes when clicks OfficeBluePrintButton.
        /// </summary>
        public void OnOfficeBluePrintButtonClicked()
        {
            if (this.rooms.childCount == 3)
            {
                var office = this.rooms.GetChild(2).gameObject;

                InGameUI.inst.EnterBluePrintMode((int)RoomType.Office);
                office.GetComponent<BluePrint>().EnterBluePrintMode();
            }
            else if (GlobalRoomManager.inst != null)
            {
                GlobalRoomManager.inst.StartBuildRoom((int)RoomType.Office);
            }
        }

        /// <summary>
        /// Executes when clicks OfficePrefabButton.
        /// </summary>
        public void OnSaveOfficePrefabButtonClicked()
        {
            var office = this.rooms.GetChild(2).gameObject;
            if (office != null)
            {
                this.SavePrefab(office);
            }
        }

        /// <summary>
        /// Saves office as a prefab.
        /// </summary>
        /// <param name="office">The office gameObject.</param>
        private void SavePrefab(GameObject office)
        {
            Assert.IsTrue(office?.GetComponent<BluePrint>() != null &&
                office?.GetComponent<StateMachine>() != null &&
                office?.GetComponent<RealRoom>() != null &&
                office?.GetComponent<FoundationManager>() != null &&
                office?.GetComponent<OfficeMouseEventListener>() != null &&
                office?.GetComponent<OfficeController>() != null);

            office.GetComponent<OfficeController>().RoomsContainerTransform = office.transform.Find("Rooms");

            var prefabSource = PrefabUtility.SaveAsPrefabAsset(office, $"Assets/RoomBuildingStarterKit/Assets/Prefabs/OfficeEditor/Offices/Office_{DateTime.UtcNow.Year}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Day}_{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}_{DateTime.UtcNow.Second}.prefab");
            
            DestroyImmediate(prefabSource.GetComponent<RealRoom>(), true);
            DestroyImmediate(prefabSource.GetComponent<BluePrint>(), true);
            DestroyImmediate(prefabSource.GetComponent<StateMachine>(), true);

            prefabSource.GetComponent<FoundationManager>().enabled = true;
            prefabSource.GetComponent<OfficeMouseEventListener>().enabled = true;
            prefabSource.GetComponent<OfficeController>().enabled = true;

            PrefabUtility.SavePrefabAsset(prefabSource);
        }
    }
}
#endif