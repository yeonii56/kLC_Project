namespace RoomBuildingStarterKit.Configurations
{
    using RoomBuildingStarterKit.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The VSyncSetting class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "VSyncSetting", menuName = "GameSettings/VSyncSetting", order = 1)]
    public class VSyncSetting : ConfigurationItemBase
    {
        /// <summary>
        /// The playerrefs key.
        /// </summary>
        private const string PREFS_KEY = "VSyncCount";

        /// <summary>
        /// The option texts.
        /// </summary>
        private readonly List<UIText> contents = new List<UIText> { UIText.OFF, UIText.ON };

        /// <summary>
        /// The default vsync count.
        /// </summary>
        [SerializeField]
        private int defaultVSyncCount = 1;

        /// <summary>
        /// Gets the context.
        /// </summary>
        public override List<UIText> Contents => this.contents;

        /// <summary>
        /// Gets o sets the value.
        /// </summary>
        public override UIText Value
        {
            get
            {
                return PlayerPrefs.GetInt(PREFS_KEY, this.defaultVSyncCount) == 0 ? this.Contents[0] : this.Contents[1];
            }
            set
            {
                var vSyncCount = this.Contents.FindIndex(c => c == value);
                QualitySettings.vSyncCount = vSyncCount;

                PlayerPrefs.SetInt(PREFS_KEY, vSyncCount);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Executes when reset setting.
        /// </summary>
        protected override void ResetInternal()
        {
            this.Value = (this.defaultVSyncCount == 0 ? this.Contents[0] : this.Contents[1]);
        }
    }
}