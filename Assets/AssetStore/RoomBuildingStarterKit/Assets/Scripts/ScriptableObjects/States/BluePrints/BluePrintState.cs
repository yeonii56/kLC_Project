namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using System;

    /// <summary>
    /// The BluePrintState class.
    /// </summary>
    [Serializable]
    public abstract class BluePrintState : StateBase
    {
        /// <summary>
        /// Gets the blue print.
        /// </summary>
        protected BluePrint BluePrint { get => this.Context["BluePrint"] as BluePrint; }

        /// <summary>
        /// Gets the blue print data.
        /// </summary>
        protected BluePrintDataBase BluePrintData { get => this.BluePrint.BluePrintData; }
    }
}