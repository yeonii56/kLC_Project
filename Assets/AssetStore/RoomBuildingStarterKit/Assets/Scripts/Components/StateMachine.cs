namespace RoomBuildingStarterKit.Components
{
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// The StateMachine class.
    /// </summary>
    public class StateMachine : MonoBehaviour
    {
        /// <summary>
        /// The state machine data.
        /// </summary>
        public StateMachineDataBase StateMachineData;

        /// <summary>
        /// The current state.
        /// </summary>
        private StateBase currentState;

        /// <summary>
        /// Whether the state machine is stop or not.
        /// </summary>
        private bool isStopped = true;

        /// <summary>
        /// The entry state used to override the default state.
        /// </summary>
        private StateBase entryState;

        /// <summary>
        /// Gets the context.
        /// </summary>
        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Runs state machine.
        /// </summary>
        public void Run()
        {
            Assert.IsTrue(this.isStopped, "StateMachine is running.");

            this.isStopped = false;
            this.StateMachineData.States.ForEach(s => s.Item.Setup(this.Context));
            this.currentState = this.entryState ?? this.StateMachineData.States.First(s => s.Item.IsDefault).Item;
            this.currentState.Enter();
            this.entryState = null;
        }

        /// <summary>
        /// Stops state machine.
        /// </summary>
        /// <param name="stateShouldExit"></param>
        public void Stop(bool stateShouldExit = true)
        {
            Assert.IsFalse(this.isStopped, "StateMachine can't stop repeatly.");

            if (stateShouldExit == true)
            {
                this.currentState.Exit();
            }

            this.currentState = null;
            this.isStopped = true;
        }

        /// <summary>
        /// Sets state.
        /// </summary>
        /// <param name="entryState">The entry state.</param>
        public void SetState(Type entryState)
        {
            this.entryState = this.StateMachineData.States.First(s => s.Item.GetType() == entryState).Item;

            if (this.isStopped == false)
            {
                this.Stop();
                this.Run();
            }
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.Context["States"] = this.StateMachineData.States;
            this.StateMachineData.States.ForEach(s => s.Item.StateItem = s);
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            if (this.isStopped)
            {
                return;
            }

            this.currentState.Update();
            this.currentState = this.currentState.Transition();
        }
    }
}