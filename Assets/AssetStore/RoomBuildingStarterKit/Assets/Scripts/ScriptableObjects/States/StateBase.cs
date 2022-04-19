namespace RoomBuildingStarterKit.Common
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using System.Linq;

    /// <summary>
    /// The StateItem used by states list.
    /// </summary>
    [Serializable]
    public struct StateItem
    {
        /// <summary>
        /// The state is enabled.
        /// </summary>
        public bool IsEnable;

        /// <summary>
        /// The state item.
        /// </summary>
        public StateBase Item;
    }

    /// <summary>
    /// The state base class.
    /// </summary>
    [Serializable]
    public abstract class StateBase : ScriptableObject
    {
        /// <summary>
        /// The state item.
        /// </summary>
        [HideInInspector]
        public StateItem StateItem;

        /// <summary>
        /// Whether is default state.
        /// </summary>
        public bool IsDefault = false;

        /// <summary>
        /// The frame count.
        /// </summary>
        protected int frameCount = 0;

        /// <summary>
        /// The transitions.
        /// </summary>
        private List<Func<StateBase>> transitions = new List<Func<StateBase>>();

        /// <summary>
        /// Gets whether the state is enable.
        /// </summary>
        public bool IsEnable { get => this.StateItem.IsEnable; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public Dictionary<string, object> Context { get; private set; }

        /// <summary>
        /// Gets the states.
        /// </summary>
        protected List<StateItem> States { get => this.Context["States"] as List<StateItem>; }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        public void Enter()
        {
            this.frameCount = 0;
            this.OnEnter();
            this.OnEnterUIChange();
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        public void Exit()
        {
            this.OnExit();
            this.OnExitUIChange();
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        public void Update()
        {
            ++this.frameCount;
            this.OnUpdate();
        }

        /// <summary>
        /// Transits to other state.
        /// </summary>
        /// <returns></returns>
        public StateBase Transition()
        {
            StateBase state = null;
            foreach (var transit in this.transitions)
            {
                state = transit();
                if (state != null)
                {
                    this.Exit();
                    state.Enter();
                    return state;
                }
            }

            return this;
        }

        /// <summary>
        /// Setups state.
        /// </summary>
        /// <param name="context"></param>
        public virtual void Setup(Dictionary<string, object> context)
        {
            this.Context = context;
            this.SetupInternal();
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected abstract void OnEnter();

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected abstract void OnExit();

        /// <summary>
        /// Executes when enter state. Prepares ui here.
        /// </summary>
        protected virtual void OnEnterUIChange()
        {
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected virtual void OnExitUIChange()
        {
        }

        /// <summary>
        /// Executes every frame after enter state.
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// Gets state by type.
        /// </summary>
        /// <param name="stateType">The state type.</param>
        /// <returns>The state.</returns>
        protected StateBase GetStateByType(Type stateType)
        {
            var item = this.States.First(s => s.Item.GetType() == stateType);
            return (item.IsEnable ? item.Item : null);
        }

        /// <summary>
        /// Setups state.
        /// </summary>
        protected abstract void SetupInternal();

        /// <summary>
        /// Adds transition into transition list.
        /// </summary>
        /// <param name="transition"></param>
        protected void AddTransition(Func<StateBase> transition)
        {
            this.transitions.Add(transition);
        }
    }
}