namespace RoomBuildingStarterKit.BuildSystem
{
    using System;
    using UnityEngine;

    /// <summary>
    /// The BuildSystemSettings class used to control the basic settings of the build system.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "BuildSystemSettings", menuName = "BuildSystem/BuildSystemSettings", order = 1)]
    public class BuildSystemSettings : ScriptableObject
    {
        [Tooltip("Unit: meter")]
        public float GridSize = 2f;
    }
}