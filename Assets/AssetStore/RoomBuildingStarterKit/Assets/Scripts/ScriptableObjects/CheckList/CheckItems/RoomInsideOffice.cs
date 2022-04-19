namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using UnityEngine;

    /// <summary>
    /// The RoomInsideOffice class is used to make sure room build inside the offices.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "RoomInsideOffice", menuName = "CheckList/CheckItems/RoomInsideOffice", order = 1)]
    public class RoomInsideOffice : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups ui.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_ROOM_INSIDE_OFFICE;
        }

        /// <summary>
        /// Prepares before validate check item.
        /// </summary>
        protected override void Prepare()
        {
            this.officeFloorCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);
            this.officeFloorCollection.Reset(this.validateOffsetFloorEntities);
        }

        /// <summary>
        /// Validates check item.
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

                var isBuildable = (offsetFloor != null);
                f.BluePrintFloor.GetComponent<MeshRenderer>().sharedMaterial = (isBuildable ? this.BluePrintData.BluePrintValidMaterial : this.BluePrintData.BluePrintInvalidMaterial);
                result &= isBuildable;
            });

            return result;
        }
    }
}