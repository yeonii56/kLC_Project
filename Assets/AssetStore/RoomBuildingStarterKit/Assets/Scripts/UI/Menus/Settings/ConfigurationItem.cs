namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Configurations;
    using System.Collections.Generic;

    /// <summary>
    /// The ConfigurationItem class represents game settings.
    /// </summary>
    public class ConfigurationItem : LeftRightSelectButton
    {
        /// <summary>
        /// The settings item data.
        /// </summary>
        public ConfigurationItemBase ItemData;

        /// <summary>
        /// Executs when reset game settings.
        /// </summary>
        public void OnReset()
        {
            this.index = this.ItemData.Index;
            this.TextMeshPro.SetGlobalText(this.options[this.index]);
        }

        /// <summary>
        /// Initializes options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="index">The option index.</param>
        protected override void InitOptions(ref List<UIText> options, ref int index)
        {
            options = this.ItemData.Contents;
            index = this.ItemData.Index;
        }

        /// <summary>
        /// Executes when option changed.
        /// </summary>
        /// <param name="value"></param>
        protected override void OnChanged(UIText value)
        {
            this.ItemData.Value = value;
        }

        /// <summary>
        /// Executes after Awake when Instantiates gameObject.
        /// </summary>
        private void OnEnable()
        {
            this.ItemData.OnReset += this.OnReset;
        }

        /// <summary>
        /// Executes when this component disabled.
        /// </summary>
        private void OnDisable()
        {
            this.ItemData.OnReset -= this.OnReset;
        }
    }
}