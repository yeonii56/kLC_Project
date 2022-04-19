namespace RoomBuildingStarterKit.UI
{
    using RoomBuildingStarterKit.Common;
    using UnityEngine;
    
    /// <summary>
    /// The BlockMouseEventUI base class. UI which need to block mouse event need to derive from this class.
    /// </summary>
    public class BlockMouseEventUIBase : MonoBehaviour
    {
        /// <summary>
        /// Executes when gameObject enable.
        /// </summary>
        protected virtual void OnEnable()
        {
            InputWrapper.IsBlocking = true;
            EventManager.TriggerEvent(EventManager.Event.UIBlockMouse, true);
        }

        /// <summary>
        /// Executes when gameObject disable.
        /// </summary>
        protected virtual void OnDisable()
        {
            InputWrapper.IsBlocking = false;
            EventManager.TriggerEvent(EventManager.Event.UIBlockMouse, false);
        }
    }
}
