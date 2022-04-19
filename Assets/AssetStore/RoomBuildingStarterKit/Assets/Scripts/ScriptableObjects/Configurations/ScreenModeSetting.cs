namespace RoomBuildingStarterKit.Configurations
{
    using RoomBuildingStarterKit.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The ScreenModeSetting class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "ScreenModeSetting", menuName = "GameSettings/ScreenModeSetting", order = 1)]
    public class ScreenModeSetting : ConfigurationItemBase
    {
        /// <summary>
        /// The resolution setting.
        /// </summary>
        public ResolutionSetting ResolutionSetting;

        /// <summary>
        /// The playerrefs key.
        /// </summary>
        private const string PREFS_KEY = "ScreenMode";

        /// <summary>
        /// The option texts.
        /// </summary>
        private readonly List<UIText> contents = new List<UIText>
        {
            UIText.SCREEN_MODE_WINDOW,
            UIText.SCREEN_MODE_FULL_SCREEN,
        };

        /// <summary>
        /// The default screen mode.
        /// </summary>
        [SerializeField]
        private FullScreenMode defaultScreenMode = FullScreenMode.Windowed;

        /// <summary>
        /// Gets the content.
        /// </summary>
        public override List<UIText> Contents => this.contents;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public override UIText Value
        {
            get
            {
                return ((FullScreenMode)PlayerPrefs.GetInt(PREFS_KEY, (int)this.defaultScreenMode)) == FullScreenMode.Windowed ? this.contents[0] : this.contents[1];
            }
            set
            {
                var screenMode = this.GetScreenMode(value);
                var resolution = this.ResolutionSetting.GetResolution(this.ResolutionSetting.Value);
                Screen.SetResolution(resolution[0], resolution[1], screenMode);

                PlayerPrefs.SetInt(PREFS_KEY, (int)screenMode);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Gets screen mode.
        /// </summary>
        /// <param name="screenMode">The screen mode ui text.</param>
        /// <returns>The full screen mode enum.</returns>
        public FullScreenMode GetScreenMode(UIText screenMode)
        {
            return (screenMode == this.contents[0] ? FullScreenMode.Windowed : FullScreenMode.ExclusiveFullScreen);
        }

        /// <summary>
        /// Executes when reset setting.
        /// </summary>
        protected override void ResetInternal()
        {
            this.Value = (this.defaultScreenMode == FullScreenMode.Windowed ? this.contents[0] : this.contents[1]);
        }
    }
}