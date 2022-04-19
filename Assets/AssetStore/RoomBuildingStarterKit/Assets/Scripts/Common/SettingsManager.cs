namespace RoomBuildingStarterKit.Common
{
    using RoomBuildingStarterKit.Configurations;
    using System.Collections.Generic;

    /// <summary>
    /// The SettingsManager class used to control settings.
    /// </summary>
    public class SettingsManager : Singleton<SettingsManager>
    {
        /// <summary>
        /// The settings.
        /// </summary>
        public List<ConfigurationItemBase> Settings = new List<ConfigurationItemBase>();

        /// <summary>
        /// Resets settings to default.
        /// </summary>
        public void ResetSettings()
        {
            this.Settings.ForEach(s => s.Reset());
        }

        /// <summary>
        /// Executes after OnEnable.
        /// </summary>
        private void Start()
        {
            this.Settings.ForEach(s => s.Value = s.Value);
        }
    }
}
