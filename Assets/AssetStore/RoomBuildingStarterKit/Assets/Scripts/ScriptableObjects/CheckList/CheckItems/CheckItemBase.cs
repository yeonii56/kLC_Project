namespace RoomBuildingStarterKit.Common
{
    using System;
    using UnityEngine;
    using RoomBuildingStarterKit.UI;
    using System.Collections.Generic;

    /// <summary>
    /// The CheckItem element used by check list.
    /// </summary>
    [Serializable]
    public struct CheckItem
    {
        /// <summary>
        /// Only the enabled check item in check list can be validated. 
        /// </summary>
        public bool IsEnable;

        /// <summary>
        /// The fatal check item will lead the entire check result to false.
        /// </summary>
        public bool IsFatal;

        /// <summary>
        /// Whether the check item need to display on UI.
        /// </summary>
        public bool AddToUICheckList;

        /// <summary>
        /// The check item.
        /// </summary>
        public CheckItemBase Item;
    }

    /// <summary>
    /// The CheckItemBase class is the base class for check item.
    /// </summary>
    [Serializable]
    public abstract class CheckItemBase : ScriptableObject
    {
        /// <summary>
        /// The ui text.
        /// </summary>
        protected UIText uiText;

        /// <summary>
        /// The text string format arguments.
        /// </summary>
        protected string[] args;

        /// <summary>
        /// The context.
        /// </summary>
        protected Dictionary<string, object> context = new Dictionary<string, object>();

        /// <summary>
        /// Setups the check item.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Setup(Dictionary<string, object> context)
        {
            this.context = context;
            this.SetupInternal();
            this.SetupUI();
        }

        /// <summary>
        /// Checks the check item.
        /// </summary>
        /// <param name="addToUICheckList">Whether need display on ui.</param>
        /// <returns>The check result.</returns>
        public bool Check(bool addToUICheckList)
        {
            this.Prepare();
            var result = this.Validate();
            if (addToUICheckList)
            {
                CheckListUI.inst.Add(result, uiText, args);
            }

            return result;
        }

        /// <summary>
        /// Setups the check item.
        /// </summary>
        protected virtual void SetupInternal()
        {
        }

        /// <summary>
        /// Setups ui.
        /// </summary>
        protected virtual void SetupUI()
        {
        }

        /// <summary>
        /// Prepares data before validate check item.
        /// </summary>
        protected virtual void Prepare()
        {
        }

        /// <summary>
        /// Validates check item.
        /// </summary>
        /// <returns>The validate result.</returns>
        protected abstract bool Validate();
    }
}