namespace RoomBuildingStarterKit.Components
{
    using UnityEngine;
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.UI;
    using System.Linq;
    using System;

    /// <summary>
    /// The AddFloor class used to handle add blue print floor state.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "AddFloorState", menuName = "States/AddFloor", order = 1)]
    public class AddFloor : BluePrintState
    {
        /// <summary>
        /// Tries transiting to move furniture.
        /// * Mouse clicked on a furniture.
        /// </summary>
        /// <returns>The state instance.</returns>
        public StateBase TryTransitToMoveFurniture()
        {
            // Mouse clicked start and end event must happen in current state.
            return (this.BluePrint.MouseEventListener.MouseClickedFurnitureEntity != null &&
                this.BluePrint.MouseEventListener.MouseClickedFurnitureEntity.FurnitureType.ToString().StartsWith("Office_") == false &&
                this.BluePrint.MouseEventListener.MouseClickedFurnitureEntityHoldOnFrameCount <= this.frameCount) ? this.GetStateByType(typeof(MoveFurniture)) : null;
        }

        /// <summary>
        /// Tries transiting to put furniture state.
        /// * Mouse clicked the buy furniture button in furniture list.
        /// </summary>
        /// <returns>The state instance.</returns>
        public StateBase TryTransitToPutFurniture()
        {
            return this.BluePrintData.PutFurnitureButtonClicked != -1 ? this.GetStateByType(typeof(PutFurniture)) : null;
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnterUIChange()
        {
            UI.inst.AddRoomFloorButton.interactable = false;
            UI.inst.DeleteRoomFloorButton.interactable = true;
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnter()
        {
            BluePrintCursor.inst.SetState(BluePrintCursorState.AddFloor);
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
            this.BluePrint.UpdateSize(true);
        }

        /// <summary>
        /// Setups state.
        /// </summary>
        protected override void SetupInternal()
        {
            this.AddTransition(this.TryTransitToDeleteFloor);
            this.AddTransition(this.TryTransitToMoveBluePrint);
            this.AddTransition(this.TryTransitToPutFurniture);
            this.AddTransition(this.TryTransitToMoveFurniture);
        }

        /// <summary>
        /// Tries transiting to delete floor state.
        /// * Whenever the delete floor button is clicked
        /// </summary>
        /// <returns>The state instance.</returns>
        private StateBase TryTransitToDeleteFloor()
        {
            return this.BluePrintData.DeleteFloorButtonClicked ? this.GetStateByType(typeof(DeleteFloor)) : null;
        }

        /// <summary>
        /// Tries transiting to move blue print state.
        /// * Left mouse key down on a floor entity and the floor entity must be in the blue print floor entities.
        /// * Currently not working on add/delete floor.
        /// * Mouse not hovered on a furniture.
        /// </summary>
        /// <returns>The state instance.</returns>
        private StateBase TryTransitToMoveBluePrint()
        {
            var mouseEventListener = this.BluePrint.MouseEventListener;
            var floorEntity = mouseEventListener?.MouseHoveredFloorEntity;
            return (
                this.BluePrint.BluePrintFloorEntities.FirstOrDefault(f => f == floorEntity) != null
                && InputWrapper.GetKeyDown(KeyCode.Mouse0)
                && !this.BluePrintData.IsAddOrDeleteBluePrinting
                && mouseEventListener?.MouseHoveredFurnitureEntity == null) ? this.GetStateByType(typeof(MoveBluePrint)) : null;
        }
    }
}