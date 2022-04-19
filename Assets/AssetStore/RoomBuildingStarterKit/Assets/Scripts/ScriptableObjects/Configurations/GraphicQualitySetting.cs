namespace RoomBuildingStarterKit.Configurations
{
    using RoomBuildingStarterKit.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The GraphicQualitySetting class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "GraphicQualitySetting", menuName = "GameSettings/GraphicQualitySetting", order = 1)]
    public class GraphicQualitySetting : ConfigurationItemBase
    {
        /// <summary>
        /// The playerrefs key.
        /// </summary>
        private const string PREFS_KEY = "GraphicQuality";

        /// <summary>
        /// The option texts.
        /// </summary>
        private readonly List<UIText> contents = new List<UIText>
        {
            UIText.GRAPHIC_QUALITY_VERY_LOW,
            UIText.GRAPHIC_QUALITY_LOW,
            UIText.GRAPHIC_QUALITY_MEDIUM,
            UIText.GRAPHIC_QUALITY_HIGH,
            UIText.GRAPHIC_QUALITY_VERY_HIGH,
            UIText.GRAPHIC_QUALITY_ULTRA,
        };

        /// <summary>
        /// The default value.
        /// </summary>
        [SerializeField]
        private int defaultValue = 5;

        /// <summary>
        /// Gets the contenst.
        /// </summary>
        public override List<UIText> Contents => this.contents;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public override UIText Value
        {
            get
            {
                return this.Contents[PlayerPrefs.GetInt(PREFS_KEY, this.defaultValue)];
            }
            set
            {
                var qualityLevel = this.Contents.FindIndex(c => c == value);
                QualitySettings.SetQualityLevel(qualityLevel);

                PlayerPrefs.SetInt(PREFS_KEY, qualityLevel);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Executes when reset setting.
        /// </summary>
        protected override void ResetInternal()
        {
            this.Value = this.contents[this.defaultValue];
        }
    }
}