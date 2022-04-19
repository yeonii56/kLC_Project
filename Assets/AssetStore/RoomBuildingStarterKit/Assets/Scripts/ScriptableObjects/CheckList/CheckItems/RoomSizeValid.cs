namespace RoomBuildingStarterKit.Common
{
    using System;
    using UnityEngine;

    /// <summary>
    /// The RoomSizeValid class is used to make sure the room size larger than the minimum size.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "RoomSizeValid", menuName = "CheckList/CheckItems/RoomSizeValid", order = 1)]
    public class RoomSizeValid : BluePrintCheckItemBase
    {
        /// <summary>
        /// Setups ui.
        /// </summary>
        protected override void SetupUI()
        {
            this.uiText = UIText.CHECKLIST_ROOM_SIZE_VALID;
            this.args = new string[] { $"{this.BluePrint.MinimumRoomSize.x}", $"{this.BluePrint.MinimumRoomSize.y}", };
        }

        /// <summary>
        /// Prepares the check item before validate.
        /// </summary>
        protected override void Prepare()
        {
            this.officeFloorCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);
            this.officeFloorCollection.Reset(this.validateFloorEntities);
        }

        /// <summary>
        /// Validates the check item.
        /// </summary>
        /// <returns>The validate result.</returns>
        protected override bool Validate()
        {
            bool result = false;
            if (this.validateFloorEntities.Count >= this.BluePrint.MinimumRoomSize.x * this.BluePrint.MinimumRoomSize.y)
            {
                foreach (var officeFloor in officeFloorCollection.OfficeFloors)
                {
                    bool flag = true;
                    for (int i = 0; i < this.BluePrint.MinimumRoomSize.x; ++i)
                    {
                        for (int j = 0; j < this.BluePrint.MinimumRoomSize.y; ++j)
                        {
                            if (this.officeFloorCollection.GetOfficeFloor(officeFloor.X + i, officeFloor.Z + j) == null)
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (!flag)
                        {
                            break;
                        }
                    }

                    if (flag)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
    }
}