namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The LeftRightSelectButton class is an UI gameObject used to choose options.
    /// </summary>
    public abstract class LeftRightSelectButton : MonoBehaviour
    {
        /// <summary>
        /// The left selection button.
        /// </summary>
        public Button LeftButton;

        /// <summary>
        /// The right selection button.
        /// </summary>
        public Button RightButton;

        /// <summary>
        /// The text inside the two buttons.
        /// </summary>
        public TextMeshProUGUIWrapper TextMeshPro;

        /// <summary>
        /// The options text.
        /// </summary>
        protected List<UIText> options;

        /// <summary>
        /// The selected option index.
        /// </summary>
        protected int index = 0;

        /// <summary>
        /// Initializes the options.
        /// </summary>
        /// <param name="contents">The option texts.</param>
        /// <param name="index">The option index.</param>
        protected abstract void InitOptions(ref List<UIText> options, ref int index);

        /// <summary>
        /// Executes when option changed.
        /// </summary>
        /// <param name="value">The selected option.</param>
        protected abstract void OnChanged(UIText value);

        /// <summary>
        /// Selects options when click left/right button.
        /// </summary>
        /// <param name="isLeft"></param>
        private void SelectContent(bool isLeft)
        {
            this.index = (this.index + this.options.Count + (isLeft ? -1 : 1)) % this.options.Count;
            this.TextMeshPro.SetGlobalText(this.options[this.index]);
            this.OnChanged(this.options[this.index]);
        }

        /// <summary>
        /// Executes when instantiates gameObject.
        /// </summary>
        private void Awake()
        {
            this.InitOptions(ref this.options, ref this.index);
            this.TextMeshPro.SetGlobalText(this.options[this.index]);

            this.LeftButton.onClick.AddListener(() => this.SelectContent(true));
            this.RightButton.onClick.AddListener(() => this.SelectContent(false));
        }
    }
}