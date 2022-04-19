namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using UnityEngine;

    /// <summary>
    /// The NotOverlapWithOtherRoom class is used to make sure the rooms will not overlap with each other.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NotOverlapWithOtherRoom", menuName = "CheckList/CheckItems/NotOverlapWithOtherRoom", order = 1)]
    public class NotOverlapWithOtherRoom : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups ui.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_NOT_OVERLAP_WITH_OTHER_ROOM;
        }

        /// <summary>
        /// Prepares the check item before validates.
        /// </summary>
        protected override void Prepare()
        {
            this.officeFloorCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);
            this.officeFloorCollection.Reset(this.validateOffsetFloorEntities);
        }

        /// <summary>
        /// Validates the check item.
        /// </summary>
        /// <returns>The validate result.</returns>
        protected override bool Validate()
        {
            bool result = true;
            this.validateFloorEntities.ForEach(f =>
            {
                var offsetFloor = this.officeFloorCollection.GetOfficeFloor(this.pendingTransformAndRotate(f));

                // Can't put office on office editor bound floor.
                if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office && (offsetFloor?.FloorEntity?.IsOfficeEditorBoundFloor ?? false) == true)
                {
                    offsetFloor = null;
                }

                if (offsetFloor != null)
                {
                    var isBuildable = (offsetFloor.FloorEntity.OccupiedRoom == null);
                    f.BluePrintFloor.GetComponent<MeshRenderer>().sharedMaterial = (isBuildable ? this.BluePrintData.BluePrintValidMaterial : this.BluePrintData.BluePrintInvalidMaterial);
                    result &= isBuildable;
                }
            });

            return result;
        }
    }
}