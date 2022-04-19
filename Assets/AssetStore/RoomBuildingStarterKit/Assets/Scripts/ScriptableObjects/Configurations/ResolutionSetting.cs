namespace RoomBuildingStarterKit.Configurations
{
    using RoomBuildingStarterKit.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// The ResolutionSetting class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "ResolutionSetting", menuName = "GameSettings/ResolutionSetting", order = 1)]
    public class ResolutionSetting : ConfigurationItemBase
    {
        /// <summary>
        /// The screen mode setting.
        /// </summary>
        public ScreenModeSetting ScreenModeSetting;

        /// <summary>
        /// The playerrefs key.
        /// </summary>
        private const string PREFS_KEY = "Resolution";

        /// <summary>
        /// The option texts.
        /// </summary>
        public readonly List<UIText> contents = new List<UIText>
        {
            UIText.RESOLUTION_1920_1080,
            UIText.RESOLUTION_3840_2160,
        };

        /// <summary>
        /// The default value.
        /// </summary>
        [SerializeField]
        private string defaultValue = "1920_1080";

        /// <summary>
        /// Gets the contents.
        /// </summary>
        public override List<UIText> Contents => this.contents;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public override UIText Value
        {
            get
            {
                return (UIText)Enum.Parse(typeof(UIText), $"RESOLUTION_{PlayerPrefs.GetString(PREFS_KEY, this.defaultValue)}");
            }
            set
            {
                this.Contents.First(r => r == value);
                var resolution = this.GetResolution(value);
                Screen.SetResolution(resolution[0], resolution[1], this.ScreenModeSetting.GetScreenMode(this.ScreenModeSetting.Value));

                PlayerPrefs.SetString(PREFS_KEY, $"{resolution[0]}_{resolution[1]}");
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Gets the resolution.
        /// </summary>
        /// <param name="resolution">The resolution ui text.</param>
        /// <returns>A list includes resolution width and height.</returns>
        public List<int> GetResolution(UIText resolution)
        {
            return LanguageManager.inst.GetText(resolution).Split('x').Select(x => int.Parse(x)).ToList<int>();
        }

        /// <summary>
        /// Executes when reset setting.
        /// </summary>
        protected override void ResetInternal()
        {
            this.Value = (UIText)Enum.Parse(typeof(UIText), $"RESOLUTION_{this.defaultValue}");
        }
    }
}