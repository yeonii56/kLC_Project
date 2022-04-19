namespace RoomBuildingStarterKit.Components
{
    using RoomBuildingStarterKit.Entity;
    using RoomBuildingStarterKit.Common;
    using RoomBuildingStarterKit.Common.Extensions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using RoomBuildingStarterKit.BuildSystem;
    using RoomBuildingStarterKit.UI;

    /// <summary>
    /// This component is used to handle mouse events for BuildSystem.
    /// </summary>
    [RequireComponent(typeof(FoundationManager))]
    public class OfficeMouseEventListener : MonoBehaviour
    {
        /// <summary>
        /// The mouse drag start floor entity.
        /// </summary>
        private FloorEntity mouseDragStartFloorEntity;

        /// <summary>
        /// The mouse drag end floor entity.
        /// </summary>
        private FloorEntity mouseDragEndFloorEntity;

        /// <summary>
        /// The mouse selected floor entities.
        /// </summary>
        private List<FloorEntity> mouseSelectedFloorEntities = new List<FloorEntity>();

        /// <summary>
        /// The mouse click start floor used to update mouse clicked floor.
        /// </summary>
        private GameObject mouseClickStartFloor;

        /// <summary>
        /// The mouse clicked floor.
        /// </summary>
        private GameObject mouseClickedFloor;

        /// <summary>
        /// The mouse click start gameobject with selectable or outline layer, used to update mouse clicked selectable gameobject.
        /// </summary>
        private GameObject mouseClickStartSelectableObject;

        /// <summary>
        /// The mouse clicked selectable object in current frame.
        /// </summary>
        private GameObject mouseClickedSelectableObject;

        /// <summary>
        /// The mouse hovered floor entity in current frame.
        /// </summary>
        private FloorEntity mouseHoveredFloorEntity;

        /// <summary>
        /// The mouse hovered selectable object.
        /// </summary>
        private GameObject mouseHoveredSelectableObject;

        /// <summary>
        /// The mouse pressed floor entity in current frame.
        /// </summary>
        private FloorEntity mousePressedFloorEntity;

        /// <summary>
        /// The mouse clicked floor entity in current frame.
        /// </summary>
        private FloorEntity mouseClickedFloorEntity;

        /// <summary>
        /// The mouse click start furniture gameobject used to update mouse clicked furniture.
        /// </summary>
        private GameObject mouseClickStartFurniture;

        /// <summary>
        /// The mouse clicked furniture gameobject in current frame.
        /// </summary>
        private GameObject mouseClickedFurniture;

        /// <summary>
        /// The click furniture mouse down frame.
        /// </summary>
        private int clickStartFurnitureFrame;

        /// <summary>
        /// The click furniture mouse up frame.
        /// </summary>
        private int clickedFurnitureFrame;

        /// <summary>
        /// The click selectable object mouse down frame.
        /// </summary>
        private int clickStartSelectableObjectFrame;

        /// <summary>
        /// The click selectable object mouse up frame.
        /// </summary>
        private int clickedSelectableObjectFrame;

        /// <summary>
        /// The click floor mouse down frame.
        /// </summary>
        private int clickStartFloorFrame;

        /// <summary>
        /// The click floor mouse up frame.
        /// </summary>
        private int clickedFloorFrame;

        /// <summary>
        /// The mouse hovered furniture entity in current frame.
        /// </summary>
        private FurnitureEntityBase mouseHoveredFurnitureEntity;

        /// <summary>
        /// The mouse clicked furniture entity in current frame.
        /// </summary>
        private FurnitureEntityBase mouseClickedFurnitureEntity;

        /// <summary>
        /// The foundation manager.
        /// </summary>
        private FoundationManager foundationManager;

        /// <summary>
        /// The cache.
        /// </summary>
        private FrameCache cache = new FrameCache();

        /// <summary>
        /// Gets the is mouse event listener working.
        /// </summary>
        public bool IsWorking => this.MouseHoveredFloorEntity != null;

        /// <summary>
        /// The mouse selected floor entities, will be cleared when the left mouse up.
        /// </summary>
        public List<FloorEntity> MouseSelectedFloorEntities { get => this.mouseSelectedFloorEntities; }

        /// <summary>
        /// Gets the mouse clicked furniture entity.
        /// </summary>
        public FurnitureEntityBase MouseClickedFurnitureEntity { get => this.mouseClickedFurnitureEntity; }

        /// <summary>
        /// Gets the mouse clicked furniture entity hold on frame count.
        /// </summary>
        public int MouseClickedFurnitureEntityHoldOnFrameCount { get => this.clickedFurnitureFrame - this.clickStartFloorFrame + 1; }

        /// <summary>
        /// Gets the last not null mouse hovered floor entity.
        /// </summary>
        public FloorEntity LastNotNullMouseHoveredFloorEntity { get; private set; }

        /// <summary>
        /// Gets the mouse hovered floor entity in current frame.
        /// </summary>
        public FloorEntity MouseHoveredFloorEntity
        {
            get
            {
                this.cache.Cache("MouseHoveredFloorEntity", 1, () =>
                {
                    this.mouseHoveredFloorEntity = null;
                    if (!InputWrapper.IsBlocking)
                    {
                        RaycastHit hit;
                        var ray = CameraController.inst.Cam.ScreenPointToRay(InputWrapper.MousePosition);
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
                        {
                            var hitFloor = hit.collider.gameObject;
                            this.foundationManager.FloorGoToFloorEntityMap.TryGetValue(hitFloor.GetInstanceID(), out this.mouseHoveredFloorEntity);

                            // Can't choose bound floor in office editor.
                            if (Global.inst.RuntimeMode == RuntimeMode.OfficeEditor && (this.mouseHoveredFloorEntity?.IsOfficeEditorBoundFloor ?? false) == true)
                            {
                                this.mouseHoveredFloorEntity = null;
                            }

                            this.LastNotNullMouseHoveredFloorEntity = this.mouseHoveredFloorEntity ?? this.LastNotNullMouseHoveredFloorEntity;
                        }
                    }
                });

                return this.mouseHoveredFloorEntity;
            }
        }

        /// <summary>
        /// Gets the mouse pressed floor entity in current frame.
        /// </summary>
        public FloorEntity MousePressedFloorEntity
        {
            get
            {
                this.cache.Cache("MousePressedFloorEntity", 1, () =>
                {
                    this.mousePressedFloorEntity = null;

                    if (InputWrapper.GetKey(KeyCode.Mouse0))
                    {
                        this.mousePressedFloorEntity = this.MouseHoveredFloorEntity;
                    }
                });

                return this.mousePressedFloorEntity;
            }
        }

        /// <summary>
        /// Gets the mouse clicked floor entity in current frame.
        /// </summary>
        public FloorEntity MouseClickedFloorEntity { get => this.mouseClickedFloorEntity; }

        /// <summary>
        /// Gets the mouse hovered furniture entity in current frame.
        /// </summary>
        public FurnitureEntityBase MouseHoveredFurnitureEntity
        {
            get
            {
                this.cache.Cache("MouseHoveredFurnitureEntity", 1, () =>
                {
                    this.mouseHoveredFurnitureEntity = null;

                    var obj = OfficeMouseEventListener.GetMouseHoveredObjectByLayer(LayerMask.GetMask("Furniture"));
                    if (obj != null)
                    {
                        GlobalFurnitureManager.inst.FurnitureGoToFurnitureEntityMaps.TryGetValue(obj.transform.parent.gameObject.GetInstanceID(), out this.mouseHoveredFurnitureEntity);
                    }
                });

                return this.mouseHoveredFurnitureEntity;
            }
        }

        /// <summary>
        /// Gets the mouse hovered furniture entity in current frame.
        /// </summary>
        public GameObject MouseHoveredSelectableObject
        {
            get
            {
                this.cache.Cache("MouseHoveredSelectableObject", 1, () =>
                {
                    this.mouseHoveredSelectableObject = OfficeMouseEventListener.GetMouseHoveredObjectByLayer(LayerMask.GetMask("Selectable") | LayerMask.GetMask("Outline"));
                });

                return this.mouseHoveredSelectableObject;
            }
        }

        /// <summary>
        /// Gets or sets the mouse clicked selectable object.
        /// </summary>
        public GameObject MouseClickSelectableObject { get; set; }

        /// <summary>
        /// Sets cursor position at system level.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns>Operation success or not.</returns>
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// Gets cursor position at system level.
        /// </summary>
        /// <param name="point">The cursor position represented by a point.</param>
        /// <returns>Operation success or not.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Vector2Int point);

        /// <summary>
        /// Gets mouse hoverd gameobject with the specified layer mask in current frame.
        /// </summary>
        /// <param name="layerMask">The layer mask.</param>
        /// <returns>The gameobject.</returns>
        public static GameObject GetMouseHoveredObjectByLayer(int layerMask)
        {
            RaycastHit hit;
            var ray = CameraController.inst.Cam.ScreenPointToRay(InputWrapper.MousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        /// <summary>
        /// Gets mouse hoverd gameobject with the specified layer mask in current frame.
        /// </summary>
        /// <param name="layerMask">The layer mask.</param>
        /// <returns>The gameobject.</returns>
        public static GameObject GetMouseHoveredUnLockableObject()
        {
            var obj = OfficeMouseEventListener.GetMouseHoveredObjectByLayer(LayerMask.GetMask("UnLockableObject"));
            return obj?.GetFather();
        }

        /// <summary>
        /// Sets the mouse to target position.
        /// </summary>
        /// <param name="targetPos">The target position.</param>
        public void SetMousePosition(Vector3 targetPos)
        {
            var currentPos = CameraController.inst.Cam.WorldToScreenPoint(CameraController.inst.ProjectToGround(Input.mousePosition));
            Vector2 offset = targetPos - currentPos;

            GetCursorPos(out Vector2Int point);
            SetCursorPos(point.x + (int)offset.x, point.y - (int)offset.y);
        }

        /// <summary>
        /// Updates mouse clicked furniture entity.
        /// Mouse left button down and up at the same furniture and cannot drag and move to other object at the interval.
        /// </summary>
        public void UpdateMouseClickedFurnitureEntity()
        {
            this.mouseClickedFurnitureEntity = null;
            this.UpdateMouseClickedObjectByLayer(LayerMask.GetMask("Furniture"), ref this.mouseClickStartFurniture, out this.mouseClickedFurniture, ref this.clickStartFurnitureFrame, ref this.clickedFurnitureFrame);

            // Additional layer.
            GlobalFurnitureManager.inst.FurnitureGoToFurnitureEntityMaps.TryGetValue(this.mouseClickedFurniture?.GetFather()?.GetInstanceID() ?? -1, out this.mouseClickedFurnitureEntity);
        }

        /// <summary>
        /// Updates the mouse clicked gameobject by the specified layer.
        /// </summary>
        /// <param name="layerMask">The layer mask.</param>
        /// <param name="mouseClickStartObject">The mouse click start gameobject.</param>
        /// <param name="mouseClickedObject">The mouse clicked gameobject.</param>
        private void UpdateMouseClickedObjectByLayer(int layerMask, ref GameObject mouseClickStartObject, out GameObject mouseClickedObject, ref int clickStartFrame, ref int clickedFrame)
        {
            mouseClickedObject = null;

            var mouseHoveredObject = OfficeMouseEventListener.GetMouseHoveredObjectByLayer(layerMask);

            if (Input.GetKey(KeyCode.Mouse0) && mouseClickStartObject != mouseHoveredObject)
            {
                mouseClickStartObject = null;
            }

            if (InputWrapper.GetKeyDown(KeyCode.Mouse0))
            {
                mouseClickStartObject = mouseHoveredObject;
                clickStartFrame = Time.frameCount;
            }
            else if (InputWrapper.GetKeyUp(KeyCode.Mouse0) && mouseClickStartObject == mouseHoveredObject)
            {
                mouseClickedObject = mouseClickStartObject;
                mouseClickStartObject = null;
                clickedFrame = Time.frameCount;
            }
        }

        /// <summary>
        /// Updates mouse clicked selectable object.
        /// </summary>
        private void UpdateMouseClickedSelectableObject()
        {
            this.UpdateMouseClickedObjectByLayer(LayerMask.GetMask("Selectable") | LayerMask.GetMask("Outline"), ref this.mouseClickStartSelectableObject, out this.mouseClickedSelectableObject, ref this.clickStartSelectableObjectFrame, ref this.clickedSelectableObjectFrame);
        }

        /// <summary>
        /// Updates mouse clicked floor entity.
        /// Mouse left button down and up at the same floor and cannot drag and move to other floor at the interval.
        /// </summary>
        private void UpdateMouseClickedFloorEntity()
        {
            this.mouseClickedFloorEntity = null;
            this.UpdateMouseClickedObjectByLayer(LayerMask.GetMask("Floor"), ref this.mouseClickStartFloor, out this.mouseClickedFloor, ref this.clickStartFloorFrame, ref this.clickedFloorFrame);
            this.foundationManager.FloorGoToFloorEntityMap.TryGetValue(this.mouseClickedFloor?.GetInstanceID() ?? -1, out this.mouseClickedFloorEntity);
        }

        /// <summary>
        /// Updates mouse selected floor entities.
        /// Press and drag left mouse to select floor entities. The selected floors will be cleared when left mouse key up.
        /// </summary>
        private void UpdateMouseSelectedFloorEntities()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                this.mouseDragStartFloorEntity = null;
                this.mouseDragEndFloorEntity = null;
                this.mouseSelectedFloorEntities.Clear();
            }

            if (this.mouseDragStartFloorEntity == null && this.MousePressedFloorEntity != null)
            {
                this.mouseDragStartFloorEntity = this.MousePressedFloorEntity;
            }
            else if (this.mouseDragStartFloorEntity != null && this.MousePressedFloorEntity != null)
            {
                this.mouseDragEndFloorEntity = this.MousePressedFloorEntity;
                this.mouseSelectedFloorEntities = this.foundationManager.OfficeFloorCollection.FloorEntities.Where(f => f.IsInsideRectangleRange(this.mouseDragStartFloorEntity, this.mouseDragEndFloorEntity)).ToList();
            }
        }

        /// <summary>
        /// Executes when gameObject instantiates.
        /// </summary>
        private void Awake()
        {
            this.foundationManager = this.GetComponent<FoundationManager>();
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            this.UpdateMouseClickedSelectableObject();
            this.UpdateMouseClickedFurnitureEntity();
            this.UpdateMouseClickedFloorEntity();
            this.UpdateMouseSelectedFloorEntities();
        }
    }
}