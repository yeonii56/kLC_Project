namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Entity;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The BuildSystem helper class.
    /// </summary>
    public class BuildSystemHelper
    {
        /// <summary>
        /// The office floor collection.
        /// </summary>
        private static OfficeFloorCollection OfficeFloorCollection = new OfficeFloorCollection();

        /// <summary>
        /// Checks the equality of the floor entities.
        /// </summary>
        /// <param name="lhs">The left hand side floor entities list.</param>
        /// <param name="rhs">The right hand side floor entities list.</param>
        /// <returns>The bool value indicates the floor entities equal or not.</returns>
        public static bool CheckSelectFloorEntitiesEquality(List<FloorEntity> lhs, List<FloorEntity> rhs)
        {
            if(lhs == null && rhs == null)
            {
                return true;
            }
            else if(lhs == null || rhs == null)
            {
                return false;
            }

            if(lhs.Count != rhs.Count)
            {
                return false;
            }

            for (int i = 0; i < lhs.Count; ++i)
            {
                if (lhs[i] != rhs[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks selected floors connectivity.
        /// </summary>
        /// <param name="floorEntities">The floorEntities need to validation.</param>
        /// <param name="X">The size in x direction.</param>
        public static bool CheckSelectedFloorsConnectivity(List<FloorEntity> floorEntities)
        {
            if (!floorEntities.Any())
            {
                return true;
            }

            OfficeFloorCollection.Reset(floorEntities);
            return OfficeFloorCollection.CheckConnect();
        }

        /// <summary>
        /// Gets the border floor entities.
        /// </summary>
        /// <param name="floorEntities">The floor entities.</param>
        /// <param name="borderFloorEntities">The border floor entities.</param>
        public static void GetBorderFloorEntities(List<FloorEntity> floorEntities, ref List<List<FloorEntity>> borderFloorEntities)
        {
            OfficeFloorCollection.Reset(floorEntities);

            for (short i = 0; i < 4; ++i)
            {
                borderFloorEntities[i].Clear();
                foreach (var officeFloor in OfficeFloorCollection.OfficeFloors)
                {
                    if (OfficeFloorCollection.GetOfficeFloorByDir(officeFloor, i) == null)
                    {
                        borderFloorEntities[i].Add(officeFloor.FloorEntity);
                    }
                }
            }
        }
    }
}