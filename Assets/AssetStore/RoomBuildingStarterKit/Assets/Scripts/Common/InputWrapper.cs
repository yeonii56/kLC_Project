namespace RoomBuildingStarterKit.Common
{
    using UnityEngine;

    /// <summary>
    /// The Input wrapper class for handle UI block mouse event.
    /// </summary>
    public class InputWrapper
    {
        /// <summary>
        /// Whether the Input event is blocking.
        /// </summary>
        public static bool IsBlocking = false;

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        public static Vector3 MousePosition { get => Input.mousePosition; }

        /// <summary>
        /// Gets button key hold on.
        /// </summary>
        /// <param name="key">The button key.</param>
        /// <returns>True or false.</returns>
        public static bool GetKey(KeyCode key)
        {
            return InputWrapper.IsBlocking || UIMouseEventDetector.CheckMouseEventOnUI() ? false : Input.GetKey(key);
        }

        /// <summary>
        /// Gets the button key down.
        /// </summary>
        /// <param name="key">The button key.</param>
        /// <returns>True or false.</returns>
        public static bool GetKeyDown(KeyCode key)
        {
            return InputWrapper.IsBlocking || UIMouseEventDetector.CheckMouseEventOnUI() ? false : Input.GetKeyDown(key);
        }

        /// <summary>
        /// Gets the button key up.
        /// </summary>
        /// <param name="key">The button key.</param>
        /// <returns>True or false.</returns>
        public static bool GetKeyUp(KeyCode key)
        {
            return InputWrapper.IsBlocking || UIMouseEventDetector.CheckMouseEventOnUI() ? false : Input.GetKeyUp(key);
        }

        /// <summary>
        /// Gets axis.
        /// </summary>
        /// <param name="axisName">The axis name.</param>
        /// <returns>The axis input.</returns>
        public static float GetAxis(string axisName)
        {
            return InputWrapper.IsBlocking ? 0 : Input.GetAxis(axisName);
        }
    }
}