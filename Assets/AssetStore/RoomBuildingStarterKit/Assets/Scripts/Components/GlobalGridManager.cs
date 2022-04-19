namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Entity;
    using System.Collections.Generic;

    /// <summary>
    /// The global grid manager class used to 
    /// </summary>
    public class GlobalGridManager : Singleton<GlobalGridManager>
    {
        /// <summary>
        /// The floor gameObject instance to floor entity dictionary. 
        /// </summary>
        public Dictionary<int?, FloorEntity> FloorGoToFloorEntityMaps { get; } = new Dictionary<int?, FloorEntity>();
    }
}