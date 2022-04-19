namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The states list.
    /// </summary>
    [Serializable]
    public class StateList : ReorderableList<StateItem>
    {
    }

    /// <summary>
    /// The StateMachineDataBase class is common data used by state machine. 
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "StateMachineDataBase", menuName = "States/StateMachineDataBase", order = 1)]
    public class StateMachineDataBase : ScriptableObject
    {
        /// <summary>
        /// The state list.
        /// </summary>
        [StateList]
        [SerializeField]
        private StateList states = new StateList();

        /// <summary>
        /// Gets the states.
        /// </summary>
        public List<StateItem> States  => this.states.Items;
    }
}