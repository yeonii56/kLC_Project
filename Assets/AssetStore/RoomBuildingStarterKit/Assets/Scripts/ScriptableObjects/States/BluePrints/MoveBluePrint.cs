namespace RoomBuildingStarterKit.Components
{
    using UnityEngine;
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.Common;
    using System.Linq;
    using RoomBuildingStarterKit.Entity;
    using System;
    using RoomBuildingStarterKit.Helpers;
    using RoomBuildingStarterKit.UI;
    using System.Threading;

    /// <summary>
    /// The MoveBluePrint class used to control move blue print state.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "MoveBluePrintState", menuName = "States/MoveBluePrint", order = 1)]
    public class MoveBluePrint : BluePrintState
    {
        /// <summary>
        /// The check list.
        /// </summary>
        public CheckListBase CheckList;

        /// <summary>
        /// The blue print original position before move up.
        /// </summary>
        private Vector3 bluePrintOriginPosition;

        /// <summary>
        /// The move blue print offset in y direction.
        /// </summary>
        private const float MOVE_BLUEPRINT_OFFSET_Y = 0.2f;

        /// <summary>
        /// The move blue print smooth time.
        /// </summary>
        private const float MOVE_BLUEPRINT_FOLLOW_SMOOTH_TIME = 0.02f;

        /// <summary>
        /// The last update floor entity.
        /// </summary>
        private FloorEntity lastUpdateFloorEntity;

        /// <summary>
        /// The office floor collection.
        /// </summary>
        private OfficeFloorCollection officeFloorCollection = new OfficeFloorCollection();

        /// <summary>
        /// The blue print smooth follow velocity.
        /// </summary>
        private Vector3 smoothFollowVelocity;

        /// <summary>
        /// The room direction.
        /// </summary>
        private short direction;

        /// <summary>
        /// The move container is used to help move and rotate blue print.
        /// </summary>
        private Transform moveContainerTransform;

        /// <summary>
        /// 
        /// </summary>
        private Transform afterRotateFurnituresContainerTransform;

        /// <summary>
        /// Transform and rotates floor entity. 
        /// </summary>
        /// <param name="offsetX">The offset x.</param>
        /// <param name="offsetZ">The offset z.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="targetFloorEntity">The target floor entity.</param>
        /// <param name="relativeFloorEntity">The relative floor entity.</param>
        public static Vector2Int TransformAndRotate(int offsetX, int offsetZ, short direction, FloorEntity targetFloorEntity, FloorEntity relativeFloorEntity)
        {
            var xx = targetFloorEntity.X - relativeFloorEntity.X;
            var zz = targetFloorEntity.Z - relativeFloorEntity.Z;

            // Consider rotation.
            for (short i = 0; i < direction; ++i)
            {
                var tmp = xx;
                xx = zz;
                zz = -tmp;
            }

            return new Vector2Int(relativeFloorEntity.X + xx + offsetX, relativeFloorEntity.Z + zz + offsetZ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetDirection"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static short RotateFurniture(short targetDirection, short direction)
        {
            return (short)((targetDirection + direction) % 4);
        }

        /// <summary>
        /// Tries transit to add blue print floor state.
        /// * Put down blue print.
        /// </summary>
        /// <returns>The state.</returns>
        public StateBase TryTransitToAddFloor()
        {
            var floorEntity = this.BluePrint.MouseEventListener.MouseHoveredFloorEntity;
            return (floorEntity != null && InputWrapper.GetKeyDown(KeyCode.Mouse0) && this.CanPutDownBluePrint()) ? this.GetStateByType(typeof(AddFloor)) : null;
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnterUIChange()
        {
            UI.inst.AddRoomFloorButton.interactable = false;
            UI.inst.DeleteRoomFloorButton.interactable = false;
            UI.inst.BuildRoomCompleteButton.interactable = false;
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected override void OnExitUIChange()
        {
        }

        /// <summary>
        /// Executes when enter state.
        /// </summary>
        protected override void OnEnter()
        {
            this.BluePrintData.IsMovingBluePrint = true;

            this.DisableFurnituresSelectable();
            this.lastUpdateFloorEntity = null;
            this.CheckList.Context["BluePrint"] = this.BluePrint;
            this.BluePrint.Draw(this.BluePrint.BluePrintFloorEntities, ignoreOutsideWindow: true);
            this.BluePrintData.MoveBluePrintRelativeFloor = this.BluePrintData.MoveBluePrintRelativeFloor ?? this.BluePrint.MouseEventListener?.MouseHoveredFloorEntity;
            this.officeFloorCollection.Resize(this.BluePrint.FoundationManager.OfficeFloorCollection);
            BluePrintCursor.inst.SetState(BluePrintCursorState.Invisible);
            this.direction = 0;
            this.CanPutDownBluePrint();

            this.afterRotateFurnituresContainerTransform = this.BluePrint.BluePrintContainer.transform.Find("AfterRotateFurnituresContainer");

            this.moveContainerTransform = this.BluePrint.transform.parent.Find("MoveContainer");
            this.moveContainerTransform.position = this.BluePrintData.MoveBluePrintRelativeFloor.CenterWorldPosition;
            this.BluePrint.transform.parent = this.moveContainerTransform;
            this.moveContainerTransform.position += Vector3.up * MOVE_BLUEPRINT_OFFSET_Y;
            this.bluePrintOriginPosition = this.moveContainerTransform.position;
        }

        /// <summary>
        /// Executes when exit state.
        /// </summary>
        protected override void OnExit()
        {
            this.BluePrintData.IsMovingBluePrint = false;

            var floorEntity = this.BluePrint.MouseEventListener.MouseHoveredFloorEntity;
            if (floorEntity != null)
            {
                var offsetX = floorEntity.X - this.BluePrintData.MoveBluePrintRelativeFloor.X;
                var offsetZ = floorEntity.Z - this.BluePrintData.MoveBluePrintRelativeFloor.Z;

                // Offsets floors.
                this.BluePrint.BluePrintFloorEntities = this.BluePrint.BluePrintFloorEntities.Select(f => this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, f, this.BluePrintData.MoveBluePrintRelativeFloor)).FloorEntity).ToList();
                
                // Offsets windows.
                this.BluePrint.BluePrintWindowFurnitureEntities.ForEach(w =>
                {
                    w.Direction = (short)((w.Direction + this.direction) % 4);
                    w.FloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, w.FloorEntity, this.BluePrintData.MoveBluePrintRelativeFloor)).FloorEntity;
                });

                // Offsets door.
                this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
                {
                    d.Direction = (short)((d.Direction + this.direction) % 4);
                    var offsetInRoomDoorOfficeFloor = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, d.InRoomFloorEntity, this.BluePrintData.MoveBluePrintRelativeFloor));
                    d.InRoomFloorEntity = d.FloorEntity = offsetInRoomDoorOfficeFloor.FloorEntity;
                    d.OutRoomFloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloorByDir(offsetInRoomDoorOfficeFloor, d.Direction)?.FloorEntity;
                });

                // Offsets furnitures.
                this.BluePrint.BluePrintFurnitureEntities.ForEach(f =>
                {
                    f.Direction = (short)((f.Direction + this.direction) % 4);
                    f.FloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, f.FloorEntity, this.BluePrintData.MoveBluePrintRelativeFloor)).FloorEntity;
                    f.FloorEntities = f.FloorEntities.Select(ff => this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, ff, this.BluePrintData.MoveBluePrintRelativeFloor)).FloorEntity).ToList();
                });

                // Offsets wall furnitures.
                this.BluePrint.BluePrintWallFurnitureEntities.ForEach(f =>
                {
                    f.Direction = (short)((f.Direction + this.direction) % 4);
                    f.FloorEntity = this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, f.FloorEntity, this.BluePrintData.MoveBluePrintRelativeFloor)).FloorEntity;
                    f.FloorEntities = f.FloorEntities.Select(ff => this.BluePrint.FoundationManager.OfficeFloorCollection.GetOfficeFloor(MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, ff, this.BluePrintData.MoveBluePrintRelativeFloor)).FloorEntity).ToList();
                });

                // Offsets blueprint container.
                this.moveContainerTransform.position = this.bluePrintOriginPosition + (floorEntity.LeftDownLocalPosition - this.BluePrintData.MoveBluePrintRelativeFloor.LeftDownLocalPosition) - Vector3.up * MOVE_BLUEPRINT_OFFSET_Y;
                this.moveContainerTransform.localRotation = Quaternion.Euler(new Vector3(0, 90 * this.direction, 0));

                // Detach furnitures before rotate back.
                this.BluePrint.BluePrintWindowFurnitureEntities.ForEach(w => w.Furniture.transform.parent = this.afterRotateFurnituresContainerTransform);
                this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d => d.Furniture.transform.parent = this.afterRotateFurnituresContainerTransform);
                this.BluePrint.BluePrintFurnitureEntities.ForEach(f => f.Furniture.transform.parent = this.afterRotateFurnituresContainerTransform);
                this.BluePrint.BluePrintWallFurnitureEntities.ForEach(f => f.Furniture.transform.parent = this.afterRotateFurnituresContainerTransform);

                // Detach the blue print.
                for (int i = 0; i < this.direction; ++i)
                {
                    this.moveContainerTransform.Rotate(Vector3.up, -90);
                }

                this.BluePrint.transform.parent = this.moveContainerTransform.parent;

                // Reattach furnitures after rotate back.
                var furnituresParent = this.BluePrint.BluePrintContainer.transform.Find("Furnitures");
                this.BluePrint.BluePrintWindowFurnitureEntities.ForEach(w => w.Furniture.transform.parent = furnituresParent);
                this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d => d.Furniture.transform.parent = furnituresParent);
                this.BluePrint.BluePrintFurnitureEntities.ForEach(f => f.Furniture.transform.parent = furnituresParent);
                this.BluePrint.BluePrintWallFurnitureEntities.ForEach(f => f.Furniture.transform.parent = furnituresParent);

                // Put down blue print.
                this.BluePrint.Draw(this.BluePrint.BluePrintFloorEntities);
            }
            else
            {
                // Detach the blue print.
                for (int i = 0; i < this.direction; ++i)
                {
                    this.moveContainerTransform.Rotate(Vector3.up, -90);
                }

                this.BluePrint.transform.parent = this.moveContainerTransform.parent;
            }

            this.BluePrintData.MoveBluePrintRelativeFloor = null;
            this.lastUpdateFloorEntity = null;
            this.EnableFurnituresSelectable();

            this.BluePrint.CanBuildRealRoom(this.BluePrint.BluePrintFloorEntities);
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        protected override void OnUpdate()
        {
            var mouseEventListener = this.BluePrint.MouseEventListener;
            if (this.BluePrintData.MoveBluePrintRelativeFloor != null)
            {
                var floorEntity = mouseEventListener.MouseHoveredFloorEntity ?? mouseEventListener.LastNotNullMouseHoveredFloorEntity;
                if (floorEntity != null)
                {
                    this.moveContainerTransform.position = Vector3.SmoothDamp(
                        this.moveContainerTransform.position,
                        this.bluePrintOriginPosition + (floorEntity.LeftDownLocalPosition - this.BluePrintData.MoveBluePrintRelativeFloor.LeftDownLocalPosition),
                        ref this.smoothFollowVelocity,
                        MOVE_BLUEPRINT_FOLLOW_SMOOTH_TIME,
                        float.MaxValue,
                        Time.deltaTime);

                    // Rotates room.
                    if (InputWrapper.GetKeyDown(KeyCode.R))
                    {
                        this.moveContainerTransform.localRotation = Quaternion.Euler(new Vector3(0, 90 * this.direction, 0));
                        this.moveContainerTransform.position = this.bluePrintOriginPosition + (floorEntity.LeftDownLocalPosition - this.BluePrintData.MoveBluePrintRelativeFloor.LeftDownLocalPosition);
                        this.direction = (short)((this.direction + 1) % 4);
                    }

                    this.moveContainerTransform.localRotation = Quaternion.Lerp(this.moveContainerTransform.localRotation, Quaternion.Euler(new Vector3(0, 90 * this.direction, 0)), Time.unscaledDeltaTime * 30f);
                }
                else
                {
                    this.moveContainerTransform.localRotation = Quaternion.Euler(new Vector3(0, 90 * this.direction, 0));
                }

                // Updates CanPutDownBluePrint state only if mouse moved to another floor.
                if (floorEntity != this.lastUpdateFloorEntity || InputWrapper.GetKeyDown(KeyCode.R))
                {
                    this.CanPutDownBluePrint();
                    this.lastUpdateFloorEntity = floorEntity;
                }
            }
        }

        /// <summary>
        /// Setups state.
        /// </summary>
        protected override void SetupInternal()
        {
            this.AddTransition(this.TryTransitToAddFloor);
        }

        /// <summary>
        /// Disables furnitures selectable.
        /// </summary>
        private void DisableFurnituresSelectable()
        {
            // Windows
            this.BluePrint.BluePrintWindowFurnitureEntities.ForEach(w =>
            {
                w.Furniture.GetComponent<BoxCollider>().enabled = false;
            });

            // Doors
            this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
            {
                d.Furniture.GetComponent<BoxCollider>().enabled = false;
            });

            // Furnitures
            this.BluePrint.BluePrintFurnitureEntities.ForEach(f =>
            {
                f.Furniture.GetComponent<BoxCollider>().enabled = false;
            });

            // Wall Furnitures
            this.BluePrint.BluePrintWallFurnitureEntities.ForEach(f =>
            {
                f.Furniture.GetComponent<BoxCollider>().enabled = false;
            });
        }

        /// <summary>
        /// Enables furniture selectable.
        /// </summary>
        private void EnableFurnituresSelectable()
        {
            // Windows
            this.BluePrint.BluePrintWindowFurnitureEntities.ForEach(w =>
            {
                w.Furniture.layer = LayerMask.NameToLayer("Selectable");
                w.Furniture.GetComponent<BoxCollider>().enabled = true;
            });

            // Doors
            this.BluePrint.BluePrintDoorFurnitureEntities.ForEach(d =>
            {
                d.Furniture.layer = LayerMask.NameToLayer("Selectable");
                d.Furniture.GetComponent<BoxCollider>().enabled = true;
            });

            // Furnitures
            this.BluePrint.BluePrintFurnitureEntities.ForEach(f =>
            {
                FurnitureHelper.ChangeFurnitureLayer(f.Furniture, LayerMask.NameToLayer("Selectable"));
                f.Furniture.GetComponent<BoxCollider>().enabled = true;
            });

            // Wall furnitures
            this.BluePrint.BluePrintWallFurnitureEntities.ForEach(f =>
            {
                FurnitureHelper.ChangeFurnitureLayer(f.Furniture, LayerMask.NameToLayer("Selectable"));
                f.Furniture.GetComponent<BoxCollider>().enabled = true;
            });
        }

        /// <summary>
        /// Can put down blue print or not.
        /// </summary>
        /// <returns>True or false.</returns>
        private bool CanPutDownBluePrint()
        {
            var mouseEventListener = this.BluePrint.MouseEventListener;
            if (this.BluePrintData.MoveBluePrintRelativeFloor != null)
            {
                var floorEntity = mouseEventListener.MouseHoveredFloorEntity ?? mouseEventListener.LastNotNullMouseHoveredFloorEntity;
                if (floorEntity != null)
                {
                    var offsetX = floorEntity.X - this.BluePrintData.MoveBluePrintRelativeFloor.X;
                    var offsetZ = floorEntity.Z - this.BluePrintData.MoveBluePrintRelativeFloor.Z;

                    this.CheckList.PendingValidateFloorOffset.x = offsetX;
                    this.CheckList.PendingValidateFloorOffset.z = offsetZ;
                    this.CheckList.PendingValidateFloorDirection = this.direction;
                    this.CheckList.PendingTransformAndRotate = (f) => MoveBluePrint.TransformAndRotate(offsetX, offsetZ, this.direction, f, this.BluePrintData.MoveBluePrintRelativeFloor);
                    this.CheckList.PendingRotateFurniture = (dir) => MoveBluePrint.RotateFurniture(dir, this.direction);
                    this.CheckList.PendingValidateFloorEntities = this.BluePrint.BluePrintFloorEntities;
                    
                    var result = this.CheckList.Validate();
                    return result;
                }
            }

            return true;
        }
    }
}