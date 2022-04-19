namespace RoomBuildingStarterKit.BuildSystem
{
    using UnityEngine;
    using RoomBuildingStarterKit.UI;
    using System.Linq;
    using RoomBuildingStarterKit.Common;
    using System;

    /// <summary>
    /// The UnLockableObject class used to unlock office.
    /// </summary>
    public class UnLockableObject : MonoBehaviour
    {
        /// <summary>
        /// The buy panel position.
        /// </summary>
        public Vector3 BuyPanelPosition;

        /// <summary>
        /// The price.
        /// </summary>
        public int Price;

        /// <summary>
        /// The office gameObject.
        /// </summary>
        public GameObject Office;

        /// <summary>
        /// The building name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The foundation manager.
        /// </summary>
        private FoundationManager foundationManager;

        /// <summary>
        /// Shows the buy panel.
        /// </summary>
        public void ShowBuyPanel()
        {
            BuyNewOffice.inst.BuyPanel.SetActive(true);
            BuyNewOffice.inst.UnLockableObject = this;
            BuyNewOffice.inst.Position = this.BuyPanelPosition;
            BuyNewOffice.inst.LandName.SetGlobalText((UIText)Enum.Parse(typeof(UIText), this.Name.Substring(7)));
            BuyNewOffice.inst.Price.text = $"${this.Price}";
        }

        /// <summary>
        /// Unlocks the office.
        /// </summary>
        public void UnLockOffice()
        {
            this.UnLockOfficeInternal();

            this.Office.GetComponent<FoundationManager>().JustUnLock = true;

            // Auto save after unlock an office.
            SaveLoader.inst.Save("AutoSave");
        }

        /// <summary>
        /// Unlocks the office.
        /// </summary>
        private void UnLockOfficeInternal()
        {
            this.gameObject.SetActive(false);
            this.Office.SetActive(true);
        }

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            if (SaveLoader.inst.OfficeDatas.Any(o => o.OfficeType == this.Office.GetComponent<OfficeController>().OfficeType))
            {
                this.UnLockOfficeInternal();
            }
        }
    }
} 
