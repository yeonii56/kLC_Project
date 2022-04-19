namespace RoomBuildingStarterKit.Configurations
{
    using RoomBuildingStarterKit.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The game settings base class.
    /// </summary>
    public abstract class ConfigurationItemBase : ScriptableObject
    {
        /// <summary>
        /// The reset settings to default event.
        /// </summary>
        public Action OnReset;

        /// <summary>
        /// Gets the context.
        /// </summary>
        public abstract List<UIText> Contents { get; }

        /// <summary>
        /// Gets the ui text.
        /// </summary>
        public abstract UIText Value { get; set; }

        /// <summary>
        /// Gets the option index.
        /// </summary>
        public int Index => this.Contents.FindIndex(c => c == this.Value);

        /// <summary>
        /// Executes when reset setting.
        /// </summary>
        protected virtual void ResetInternal()
        {
        }

        /// <summary>
        /// Executes when reset setting.
        /// </summary>
        public void Reset()
        {
            this.ResetInternal();
            this.OnReset();
        }
    }
}