namespace RoomBuildingStarterKit.Common
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// The UIMouseEventDetector class is used to detect mouse event on UI.
    /// </summary>
    public class UIMouseEventDetector
    {
        /// <summary>
        /// The graphic raycaster.
        /// </summary>
        private static GraphicRaycaster UIGraphicRaycaster;

        /// <summary>
        /// The event system.
        /// </summary>
        private static EventSystem UIEventSystem;

        /// <summary>
        /// Initializes the detector.
        /// </summary>
        public static void Initialize()
        {
            UIMouseEventDetector.UIGraphicRaycaster = GameObject.Find("UI").GetComponent<GraphicRaycaster>();
            UIMouseEventDetector.UIEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        /// <summary>
        /// Checks the mouse event on UI.
        /// </summary>
        /// <returns>Whether mouse event on UI or not.</returns>
        public static bool CheckMouseEventOnUI()
        {
            PointerEventData eventData = new PointerEventData(UIMouseEventDetector.UIEventSystem);
            eventData.pressPosition = Input.mousePosition;
            eventData.position = Input.mousePosition;

            List<RaycastResult> list = new List<RaycastResult>();
            UIMouseEventDetector.UIGraphicRaycaster?.GetComponent<GraphicRaycaster>()?.Raycast(eventData, list);
            return list.Count > 0;
        }
    }
}