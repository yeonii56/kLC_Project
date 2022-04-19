namespace RoomBuildingStarterKit.BuildSystem
{
    using System.Linq;
    using UnityEngine;
    using RoomBuildingStarterKit.UI;
    using RoomBuildingStarterKit.Common;

    /// <summary>
    /// The NoticeBoardController class.
    /// </summary>
    public class NoticeBoardController : MonoBehaviour
    {
        /// <summary>
        /// The room grid size text.
        /// </summary>
        public TextMeshProUGUIWrapper Text;

        /// <summary>
        /// The notice board gameObject.
        /// </summary>
        public GameObject NoticeBoardUI;

        /// <summary>
        /// Updates the billboard position.
        /// </summary>
        /// <param name="floorEntities">The floorEntities.</param>
        public void UpdateText()
        {
            var selectedFloorEntities = UI.inst.BluePrintData.SelectedFloorEntities;
            if (UI.inst.BluePrintData.IsAddOrDeleteBluePrinting && selectedFloorEntities.Any())
            {
                if (this.NoticeBoardUI.activeSelf == false)
                {
                    this.NoticeBoardUI.SetActive(true);
                }

                int minX = selectedFloorEntities.Min(f => f.X);
                int maxX = selectedFloorEntities.Max(f => f.X);
                int minZ = selectedFloorEntities.Min(f => f.Z);
                int maxZ = selectedFloorEntities.Max(f => f.Z);

                var rows = maxZ - minZ + 1;
                var columns = maxX - minX + 1;
                this.Text.SetGlobalText(UIText.BLUE_PRINT_SIZE_GUIDE, rows.ToString(), columns.ToString());
            }
            else if (UI.inst.BluePrintData.IsModifyFurniture)
            {
                if (this.NoticeBoardUI.activeSelf == false)
                {
                    this.NoticeBoardUI.SetActive(true);
                }

                this.Text.SetGlobalText(UIText.PUT_FURNITURE_GUIDE);
            }
            else if (UI.inst.BluePrintData.IsMovingBluePrint)
            {
                if (this.NoticeBoardUI.activeSelf == false)
                {
                    this.NoticeBoardUI.SetActive(true);
                }

                this.Text.SetGlobalText(UIText.MOVE_BLUEPRINT_GUIDE);
            }
            else if (GlobalFurnitureManager.inst.IsBuildingOfficeFurniture)
            {
                if (this.NoticeBoardUI.activeSelf == false)
                {
                    this.NoticeBoardUI.SetActive(true);
                }

                this.Text.SetGlobalText(UIText.PUT_FURNITURE_GUIDE);
            }
            else if (this.NoticeBoardUI.activeSelf == true)
            {
                this.NoticeBoardUI.SetActive(false);
            }
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            this.UpdateText();
        }
    }
}