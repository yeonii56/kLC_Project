namespace RoomBuildingStarterKit.Common
{
    using System;
    using UnityEngine;

    /// <summary>
    /// The NotOverlapWithOfficeFurniture class is used to make sure the rooms will not overlap with office furnitures.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NotOverlapWithOfficeFurniture", menuName = "CheckList/CheckItems/NotOverlapWithOfficeFurniture", order = 1)]
    public class NotOverlapWithOfficeFurniture : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups ui.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_NOT_OVERLAP_WITH_OFFICE_FURNITURE;
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
                if (offsetFloor != null)
                {
                    var isBuildable = (offsetFloor.FloorEntity.OccupiedFurniture == null);
                    f.BluePrintFloor.GetComponent<MeshRenderer>().sharedMaterial = (isBuildable ? this.BluePrintData.BluePrintValidMaterial : this.BluePrintData.BluePrintInvalidMaterial);
                    result &= isBuildable;
                }
            });

            return result;
        }
    }
}