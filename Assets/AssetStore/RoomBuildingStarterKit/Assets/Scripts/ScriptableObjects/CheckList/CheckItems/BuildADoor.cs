namespace RoomBuildingStarterKit.Common
{
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The BuildADoor class is used to make sure there has a door before build real room.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "BuildADoor", menuName = "CheckList/CheckItems/BuildADoor", order = 1)]
    public class BuildADoor : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups the ui content.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_BUILD_A_DOOR;
        }

        /// <summary>
        /// Does validate.
        /// </summary>
        /// <returns>True or false.</returns>
        protected override bool Validate()
        {
            return this.BluePrint.BluePrintDoorFurnitureEntities.Any();
        }
    }
}