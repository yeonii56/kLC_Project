namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Components;
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;
    using RoomBuildingStarterKit.Common.Extensions;

    /// <summary>
    /// The GlobalOfficeMouseEventManager class.
    /// </summary>
    public class GlobalOfficeMouseEventManager : Singleton<GlobalOfficeMouseEventManager>
    {
        /// <summary>
        /// The mouse event listener.
        /// </summary>
        private OfficeMouseEventListener mouseEventListener;

        /// <summary>
        /// The cache.
        /// </summary>
        private FrameCache cache = new FrameCache();

        /// <summary>
        /// Gets the offices.
        /// </summary>
        public List<GameObject> Offices { get; } = new List<GameObject>();

        /// <summary>
        /// Gets the working mouse event listener in current frame.
        /// </summary>
        public OfficeMouseEventListener MouseEventListener
        {
            get
            {
                this.cache.Cache("MouseEventListener", 1, () =>
                {
                    this.mouseEventListener = null;
                    var mouseEventListeners = this.Offices.Where(o => o.activeSelf == true).Select(o => o.GetComponent<OfficeMouseEventListener>()).ToList();
                    foreach (var mouseEventListener in mouseEventListeners)
                    {
                        if (mouseEventListener != null && mouseEventListener.IsWorking == true)
                        {
                            this.mouseEventListener = mouseEventListener;
                        }
                    }
                });

                return this.mouseEventListener;
            }
        }

        /// <summary>
        /// Gets the mouse hovered office.
        /// </summary>
        public GameObject MouseHoveredOffice => this.MouseEventListener?.MouseHoveredFloorEntity?.Office;

        /// <summary>
        /// Executes when the gameObject instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            var childCount = this.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                this.Offices.Add(this.transform.GetChild(i).gameObject);
            }
        }
    }
}