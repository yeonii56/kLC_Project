namespace RoomBuildingStarterKit.Components
{
    using UnityEngine;
    using RoomBuildingStarterKit.BuildSystem;
    using System;
    using RoomBuildingStarterKit.Common;
    using System.Collections.Generic;
    using RoomBuildingStarterKit.Entity;
    using RoomBuildingStarterKit.Helpers;
    using RoomBuildingStarterKit.UI;

    /// <summary>
    /// The PutFurniture class.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "PutFurnitureState", menuName = "States/PutFurniture", order = 1)]
    public class PutFurniture : ModifyFurnitureBase
    {
        /// <summary>
        /// The putting furniture type.
        /// </summary>
        private int puttingFurnitureType = -1;

        /// <summary>
        /// Tries transiting to put furniture state.
        /// * Put down a furniture.
        /// </summary>
        /// <returns></returns>
        public StateBase TryTransitToPutFurniture()
        {
            this.puttingFurnitureType = this.BluePrintData.PutFurnitureButtonClicked;
            return (this.puttingFurnitureType != -1 && this.editingFurnitureType != -1 && this.puttingFurnitureType != this.editingFurnitureType) ? this.GetStateByType(typeof(PutFurniture)) : null;
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnterUIChange()
        {
            UI.inst.AddRoomFloorButton.interactable = true;
            UI.inst.DeleteRoomFloorButton.interactable = true;
        }

        /// <summary>
        /// Execute when enter state.
        /// </summary>
        protected override void OnEnter()
        {
            this.BluePrintData.IsModifyFurniture = true;

            this.officeCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);

            this.puttingFurnitureType = this.BluePrintData.PutFurnitureButtonClicked;

            if (this.puttingFurnitureType == 0)
            {
                this.CreatePutFurnitureCommon(this.BluePrintData.BluePrintWindowPrefab);
            }
            else if (this.puttingFurnitureType == 1)
            {
                if (this.BluePrint.SingleDoor == true)
                {
                    this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
                    {
                        if (d.CantBuildRealRoom == false)
                        {
                            d.OutRoomFloorEntity.OccupiedDoorEntities.Remove(d);
                        }

                        Destroy(d.Furniture);
                    });

                    this.BluePrint.BluePrintDoorFurnitureEntities.Clear();

                    // Rebuild door need to update checklist state.
                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }

                this.CreatePutFurnitureCommon(this.BluePrintData.BluePrintDoorPrefab);
            }
            else if (this.puttingFurnitureType != -1)
            {
                this.editingFurnitureDirection = 0;
                this.CreatePutFurnitureCommon(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[(FurnitureType)this.puttingFurnitureType]);
            }
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected override void OnExit()
        {
            this.OnExitCommon();
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        protected override void OnUpdate()
        {
            this.UpdatePutFurniture();
        }

        /// <summary>
        /// Setups state.
        /// </summary>
        protected override void SetupInternal()
        {
            this.AddTransition(this.TryTransitToAddFloor);
            this.AddTransition(this.TryTransitToDeleteFloor);
            this.AddTransition(this.TryTransitToPutFurniture);
        }

        /// <summary>
        /// Puts down furniture.
        /// </summary>
        /// <param name="floorEntity">The target floor entity.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        /// <param name="occupiedFloorEntities">The occupied floor entities.</param>
        protected override void PutDownFurniture(FloorEntity floorEntity, Vector3 targetPosition, Quaternion targetRotation, List<FloorEntity> occupiedFloorEntities)
        {
            var furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[(FurnitureType)this.editingFurnitureType], this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));
            furniture.transform.position = targetPosition + Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
            furniture.transform.localRotation = targetRotation;

            FurnitureHelper.ChangeFurnitureLayer(furniture, LayerMask.NameToLayer("Selectable"));
            furniture.GetComponent<BoxCollider>().enabled = false;

            this.BluePrint.BluePrintFurnitureEntities.Add(new GroundFurnitureEntity(floorEntity, occupiedFloorEntities, this.editingFurnitureDirection, furniture.GetComponent<FurnitureController>().FurnitureType, furniture));
        }

        /// <summary>
        /// Puts down window.
        /// </summary>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="dir">The window direction.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        protected override void PutDownWindow(FloorEntity floorEntity, short dir, Vector3 targetPosition, Quaternion targetRotation)
        {
            var window = Instantiate(this.BluePrintData.BluePrintWindowPrefab, this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));

            // Avoid outline.
            window.layer = LayerMask.NameToLayer("Selectable");
            window.GetComponent<BoxCollider>().enabled = false;

            window.transform.position = targetPosition + Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
            window.transform.localRotation = targetRotation;
            this.BluePrint.BluePrintWindowFurnitureEntities.Add(new WindowFurnitureEntity(floorEntity, dir, FurnitureType.Common_Window, window));
        }

        /// <summary>
        /// Puts down wall furniture.
        /// </summary>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="floorEntities">The floor entities.</param>
        /// <param name="dir">The window direction.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        protected override void PutDownWallFurniture(FloorEntity floorEntity, List<FloorEntity> floorEntities, short dir, Vector3 targetPosition, Quaternion targetRotation)
        {
            var furniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[(FurnitureType)this.editingFurnitureType], this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));

            furniture.transform.position = targetPosition + Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
            furniture.transform.localRotation = targetRotation;

            FurnitureHelper.ChangeFurnitureLayer(furniture, LayerMask.NameToLayer("Selectable"));
            furniture.GetComponent<BoxCollider>().enabled = false;

            this.BluePrint.BluePrintWallFurnitureEntities.Add(new WallFurnitureEntity(floorEntity, floorEntities, dir, furniture.GetComponent<FurnitureController>().FurnitureType, furniture));
        }

        /// <summary>
        /// Creates put furniture gameObject.
        /// </summary>
        /// <param name="furniturePrefab"></param>
        private void CreatePutFurnitureCommon(GameObject furniturePrefab)
        {
            BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);

            this.editingFurnitureType = this.puttingFurnitureType;
            this.editingFurniture = Instantiate(furniturePrefab, this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));
            this.editingFurniture.SetActive(false);

            // Keep outline.
            FurnitureHelper.ChangeFurnitureLayer(this.editingFurniture, LayerMask.NameToLayer("Outline"));
            this.editingFurniture.GetComponent<BoxCollider>().enabled = false;

            FurnitureHelper.DisableFurnituresSelectable(this.BluePrint);
        }
    }
}