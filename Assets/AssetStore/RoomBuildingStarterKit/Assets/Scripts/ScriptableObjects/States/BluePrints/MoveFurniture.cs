namespace RoomBuildingStarterKit.Components
{
    using UnityEngine;
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Entity;
    using System.Collections.Generic;
    using System;
    using RoomBuildingStarterKit.Helpers;
    using RoomBuildingStarterKit.UI;

    /// <summary>
    /// The MoveFurniture class used to move existing furniture.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "MoveFurniture", menuName = "States/MoveFurniture", order = 1)]
    public class MoveFurniture : ModifyFurnitureBase
    {
        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnterUIChange()
        {
            UI.inst.AddRoomFloorButton.interactable = true;
            UI.inst.DeleteRoomFloorButton.interactable = true;
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected override void OnEnter()
        {
            this.BluePrintData.IsModifyFurniture = true;

            this.officeCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);

            var furnitureEntity = this.BluePrint.MouseEventListener.MouseClickedFurnitureEntity;
            if (furnitureEntity != null)
            {
                BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);

                if (furnitureEntity.FurnitureType == FurnitureType.Common_Window)
                {
                    var screenPos = Camera.main.WorldToScreenPoint(furnitureEntity.FloorEntity.CenterWorldPosition);
                    this.BluePrint.MouseEventListener.SetMousePosition(screenPos);

                    this.editingFurniture = Instantiate(this.BluePrintData.BluePrintWindowPrefab, this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));
                    this.editingFurniture.transform.position = furnitureEntity.Furniture.transform.position - Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                    this.editingFurniture.transform.localRotation = furnitureEntity.Furniture.transform.localRotation;

                    Destroy(furnitureEntity.Furniture);
                    this.BluePrint.BluePrintWindowFurnitureEntities.Remove(furnitureEntity as WindowFurnitureEntity);

                    // Keep outline.
                    this.editingFurniture.layer = LayerMask.NameToLayer("Outline");
                    this.editingFurniture.GetComponent<BoxCollider>().enabled = false;

                    FurnitureHelper.DisableFurnituresSelectable(this.BluePrint);
                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }
                else if (furnitureEntity.FurnitureType == FurnitureType.Common_Door || furnitureEntity.FurnitureType == FurnitureType.Common_OfficeDoor)
                {
                    var doorEntity = (DoorFurnitureEntity)furnitureEntity;
                    var screenPos = Camera.main.WorldToScreenPoint(doorEntity.FloorEntity.CenterWorldPosition);
                    this.BluePrint.MouseEventListener.SetMousePosition(screenPos);

                    if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && this.BluePrint.RoomType == RoomType.Office)
                    {
                        this.editingFurniture = Instantiate(this.BluePrintData.BluePrintOfficeDoorPrefab, this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));
                    }
                    else
                    {
                        this.editingFurniture = Instantiate(this.BluePrintData.BluePrintDoorPrefab, this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));
                    }

                    this.editingFurniture.transform.position = doorEntity.Furniture.transform.position - Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                    this.editingFurniture.transform.localRotation = doorEntity.Furniture.transform.localRotation;

                    if (doorEntity.CantBuildRealRoom == false)
                    {
                        doorEntity.OutRoomFloorEntity.OccupiedDoorEntities.Remove(doorEntity);
                    }

                    Destroy(doorEntity.Furniture);
                    this.BluePrint.BluePrintDoorFurnitureEntities.Remove(doorEntity);

                    // Keep single door.
                    if (this.BluePrint.SingleDoor == true)
                    {
                        this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
                        {
                            if (d.CantBuildRealRoom == false)
                            {
                                d.OutRoomFloorEntity.OccupiedDoorEntities.Remove(doorEntity);
                            }

                            Destroy(d.Furniture);
                        });

                        this.BluePrint.BluePrintDoorFurnitureEntities.Clear();
                    }

                    // Keep outline.
                    this.editingFurniture.layer = LayerMask.NameToLayer("Outline");
                    this.editingFurniture.GetComponent<BoxCollider>().enabled = false;

                    FurnitureHelper.DisableFurnituresSelectable(this.BluePrint);
                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }
                else
                {
                    var screenPos = Camera.main.WorldToScreenPoint(furnitureEntity.FloorEntity.CenterWorldPosition);
                    this.BluePrint.MouseEventListener.SetMousePosition(screenPos);

                    this.editingFurnitureDirection = furnitureEntity.Direction;
                    this.editingFurniture = Instantiate(GlobalFurnitureManager.inst.FurnitureTypeToPrefabs[furnitureEntity.FurnitureType], this.BluePrint.BluePrintContainer.transform.Find("Furnitures"));
                    this.editingFurniture.transform.position = furnitureEntity.Furniture.transform.position - Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;
                    this.editingFurniture.transform.localRotation = furnitureEntity.Furniture.transform.localRotation;

                    // Bind furniture properties after move up it.
                    this.editingFurniture.GetComponent<FurniturePropertiesExample>()?.SetFurnitureEntity(furnitureEntity);

                    if (furnitureEntity is GroundFurnitureEntity)
                    {
                        (furnitureEntity as GroundFurnitureEntity).FloorEntities.ForEach(f => f.OccupiedFurniture = null);
                        Destroy(furnitureEntity.Furniture);
                        this.BluePrint.BluePrintFurnitureEntities.Remove(furnitureEntity as GroundFurnitureEntity);
                    }
                    else if (furnitureEntity is WallFurnitureEntity)
                    {
                        (furnitureEntity as WallFurnitureEntity).FloorEntities.ForEach(f => f.GetWallByDir(furnitureEntity.Direction)?.SetOccupiedFurniture(null));
                        Destroy(furnitureEntity.Furniture);
                        this.BluePrint.BluePrintWallFurnitureEntities.Remove(furnitureEntity as WallFurnitureEntity);
                    }

                    // Keep outline.
                    FurnitureHelper.ChangeFurnitureLayer(this.editingFurniture, LayerMask.NameToLayer("Outline"));
                    this.editingFurniture.GetComponent<BoxCollider>().enabled = false;

                    FurnitureHelper.DisableFurnituresSelectable(this.BluePrint);
                    this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
                }
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
        }

        /// <summary>
        /// Puts down furniture.
        /// </summary>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        /// <param name="occupiedFloorEntities">The occupied floor entities.</param>
        protected override void PutDownFurniture(FloorEntity floorEntity, Vector3 targetPosition, Quaternion targetRotation, List<FloorEntity> occupiedFloorEntities)
        {
            // Avoid outline.
            FurnitureHelper.ChangeFurnitureLayer(this.editingFurniture, LayerMask.NameToLayer("Selectable"));
            this.editingFurniture.GetComponent<BoxCollider>().enabled = false;
            this.editingFurniture.transform.position = targetPosition + Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;

            this.BluePrint.BluePrintFurnitureEntities.Add(new GroundFurnitureEntity(floorEntity, occupiedFloorEntities, this.editingFurnitureDirection, this.editingFurniture.GetComponent<FurnitureController>().FurnitureType, this.editingFurniture));
            this.editingFurniture = null;
            this.editingFurnitureType = -1;
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
            // Avoid outline.
            this.editingFurniture.layer = LayerMask.NameToLayer("Selectable");
            this.editingFurniture.GetComponent<BoxCollider>().enabled = false;
            this.editingFurniture.transform.position = targetPosition + Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;

            this.BluePrint.BluePrintWindowFurnitureEntities.Add(new WindowFurnitureEntity(floorEntity, dir, FurnitureType.Common_Window, this.editingFurniture));
            this.editingFurniture = null;
            this.editingFurnitureType = -1;
        }

        /// <summary>
        /// Puts down window.
        /// </summary>
        /// <param name="floorEntity">The floor entity.</param>
        /// <param name="dir">The window direction.</param>
        /// <param name="targetPosition">The position.</param>
        /// <param name="targetRotation">The rotation.</param>
        protected override void PutDownWallFurniture(FloorEntity floorEntity, List<FloorEntity> floorEntities, short dir, Vector3 targetPosition, Quaternion targetRotation)
        {
            // Avoid outline.
            FurnitureHelper.ChangeFurnitureLayer(this.editingFurniture, LayerMask.NameToLayer("Selectable"));
            this.editingFurniture.GetComponent<BoxCollider>().enabled = false;
            this.editingFurniture.transform.position = targetPosition + Vector3.up * this.BluePrintData.BLUEPRINT_LAYER_OFFSET_Y - this.BluePrintData.BLUEPRINT_FURNITURE_LAYER_OFFSET;

            this.BluePrint.BluePrintWallFurnitureEntities.Add(new WallFurnitureEntity(floorEntity, floorEntities, dir, this.editingFurniture.GetComponent<FurnitureController>().FurnitureType, this.editingFurniture));
            this.editingFurniture = null;
            this.editingFurnitureType = -1;
        }
    }
}