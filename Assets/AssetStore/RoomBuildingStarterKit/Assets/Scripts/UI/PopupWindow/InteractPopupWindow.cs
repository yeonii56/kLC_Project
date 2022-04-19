namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// The InteractPopupWindow class.
    /// </summary>
    public class InteractPopupWindow : Singleton<InteractPopupWindow>
    {
        /// <summary>
        /// The popup window gameObject.
        /// </summary>
        public GameObject Window;

        /// <summary>
        /// The confirm button.
        /// </summary>
        public Button confirmButton;

        /// <summary>
        /// The cancel button.
        /// </summary>
        public Button cancelButton;

        /// <summary>
        /// The text mesh pro ugui.
        /// </summary>
        public TextMeshProUGUIWrapper Text;

        /// <summary>
        /// Should clouse window before confirm callback.
        /// </summary>
        private bool closeWindowBeforeConfirmCallback;

        /// <summary>
        /// Gets or sets the confirm callback.
        /// </summary>
        public Action ConfirmCallback { get; set; }

        /// <summary>
        /// Displays window.
        /// </summary>
        /// <param name="closeWindowBeforeConfirmCallback">Should clouse window before confirm callback.</param>
        public void Show(bool closeWindowBeforeConfirmCallback = false)
        {
            this.closeWindowBeforeConfirmCallback = closeWindowBeforeConfirmCallback;

            if (this.Window.activeSelf == false)
            {
                this.Window.SetActive(true);
            }
        }

        /// <summary>
        /// Hides window.
        /// </summary>
        public void Hide()
        {
            if (this.Window.activeSelf == true)
            {
                this.Window.SetActive(false);
            }
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetText(UIText text, params string[] args)
        {
            this.Text.SetGlobalText(text, args);
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            this.confirmButton.onClick.AddListener(this.OnConfirmButtonClicked);
            this.cancelButton.onClick.AddListener(() => this.Hide());
        }

        /// <summary>
        /// Executes when confirm button clicked.
        /// </summary>
        private void OnConfirmButtonClicked()
        {
            if (this.closeWindowBeforeConfirmCallback == true)
            {
                this.Hide();
                this.ConfirmCallback();
            }
            else
            {
                this.ConfirmCallback();
                this.Hide();
            }
        }
    }
}