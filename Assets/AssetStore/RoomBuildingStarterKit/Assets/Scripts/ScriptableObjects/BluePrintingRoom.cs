namespace RoomBuildingStarterKit.BuildSystem
{
    using System;
    using UnityEngine;

    /// <summary>
    /// The BluePrintingRoom class is used for room in blue print mode.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "BluePrintingRoom", menuName = "Common/BluePrintingRoom", order = 1)]
    public class BluePrintingRoom : ScriptableObject
    {
        /// <summary>
        /// The blue printing gameObject.
        /// </summary>
        [HideInInspector]
        public GameObject Room;

        /// <summary>
        /// Executes when gameObject enabled.
        /// </summary>
        private void OnEnable()
        {
            this.Room = null;
        }
    }
}
