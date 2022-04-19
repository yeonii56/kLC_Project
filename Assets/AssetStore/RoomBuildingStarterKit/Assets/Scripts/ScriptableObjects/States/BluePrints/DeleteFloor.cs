namespace RoomBuildingStarterKit.Components
{
    using UnityEngine;
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.UI;

    /// <summary>
    /// The DeleteFloor class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "DeleteFloorState", menuName = "States/DeleteFloor", order = 1)]
    public class DeleteFloor : BluePrintState
    {
        /// <summary>
        /// Tries transiting to add blue print floor state.
        /// * Mouse clicked on add room floor button.
        /// </summary>
        /// <returns>The state.</returns>
        public StateBase TryTransitToAddFloor()
        {
            return this.BluePrintData.AddFloorButtonClicked ? this.GetStateByType(typeof(AddFloor)) : null;
        }

        /// <summary>
        /// Tries transiting to move furniture state.
        /// * Mouse clicked on a furniture.
        /// </summary>
        /// <returns>The state.</returns>
        public StateBase TryTransitToMoveFurniture()
        {
            // Mouse clicked start and end event must happen in current state.
            return (this.BluePrint.MouseEventListener.MouseClickedFurnitureEntity != null &&
                this.BluePrint.MouseEventListener.MouseClickedFurnitureEntity.FurnitureType.ToString().StartsWith("Office_") == false &&
                this.BluePrint.MouseEventListener.MouseClickedFurnitureEntityHoldOnFrameCount <= this.frameCount) ? this.GetStateByType(typeof(MoveFurniture)) : null;
        }

        /// <summary>
        /// Tries transiting to put furniture state.
        /// * Mouse clicked on the buy furniture button in furniture list.
        /// </summary>
        /// <returns>The state.</returns>
        public StateBase TryTransitToPutFurniture()
        {
            return this.BluePrintData.PutFurnitureButtonClicked != -1 ? this.GetStateByType(typeof(PutFurniture)) : null;
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnterUIChange()
        {
            UI.inst.DeleteRoomFloorButton.interactable = false;
            UI.inst.AddRoomFloorButton.interactable = true;
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnter()
        {
            BluePrintCursor.inst.SetState(BluePrintCursorState.DeleteFloor);
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected override void OnExit()
        {
            BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);
            this.BluePrintData.LastFrameCombinedFloorEntities = null;
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        protected override void OnUpdate()
        {
            this.BluePrint.UpdateSize(false);
        }

        /// <summary>
        /// Setups state.
        /// </summary>
        protected override void SetupInternal()
        {
            this.AddTransition(this.TryTransitToAddFloor);
            this.AddTransition(this.TryTransitToMoveFurniture);
            this.AddTransition(this.TryTransitToPutFurniture);
        }
    }
}