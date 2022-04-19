namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using UnityEngine;

    /// <summary>
    /// The blue print cursor state definitions.
    /// </summary>
    public enum BluePrintCursorState
    {
        Invisible,
        Normal,
        AddFloor,
        DeleteFloor,
        Invalid,
    }

    /// <summary>
    /// The BluePrintCursor class used to control the cursor.
    /// </summary>
    public class BluePrintCursor : Singleton<BluePrintCursor>
    {
        /// <summary>
        /// The blue print cursor offset.
        /// </summary>
        public const float BLUEPRINT_CURSOR_OFFSET = 0.21f;

        /// <summary>
        /// The blue print cursor offset during add floor.
        /// </summary>
        public const float ADD_FLOOR_BLUEPRINT_CURSOR_OFFSET = 0.203f;

        /// <summary>
        /// The blue print cursor offset during delete floor.
        /// </summary>
        public const float DELETE_FLOOR_BLUEPRINT_CURSOR_OFFSET = 0.209f;

        /// <summary>
        /// The blue print cursor material during add floor.
        /// </summary>
        public Material AddFloorMaterial;

        /// <summary>
        /// The blue print cursor material during delete floor.
        /// </summary>
        public Material DeleteFloorMaterial;

        /// <summary>
        /// The cursor move smooth time.
        /// </summary>
        public const float MOVE_SMOOTH_TIME = 0.02f;

        /// <summary>
        /// The blue print invisible position.
        /// </summary>
        private readonly Vector3 BLUEPRINT_INVISIBLE_POSITION = new Vector3(-1000f, -1000f, -1000f);

        /// <summary>
        /// The cursor state.
        /// </summary>
        private BluePrintCursorState state = BluePrintCursorState.Invisible;

        /// <summary>
        /// The cursor state in last frame.
        /// </summary>
        private BluePrintCursorState lastState = BluePrintCursorState.Invisible;

        /// <summary>
        /// The mesh renderer of the cursor.
        /// </summary>
        private MeshRenderer meshRenderer;

        /// <summary>
        /// Gets the cursor position offset.
        /// </summary>
        private Vector3 Offset
        {
            get
            {
                float offset = 0f;
                if (this.state == BluePrintCursorState.Normal)
                {
                    offset = BLUEPRINT_CURSOR_OFFSET;
                }
                else if (this.state == BluePrintCursorState.AddFloor)
                {
                    offset = ADD_FLOOR_BLUEPRINT_CURSOR_OFFSET;
                }
                else if (this.state == BluePrintCursorState.DeleteFloor)
                {
                    offset = DELETE_FLOOR_BLUEPRINT_CURSOR_OFFSET;
                }

                return Vector3.up * offset;
            }
        }

        /// <summary>
        /// The cursor follow smooth damp velocity.
        /// </summary>
        private Vector3 cursorSmoothFollowVelocity;

        /// <summary>
        /// Sets the blue print state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void SetState(BluePrintCursorState state = BluePrintCursorState.Invisible)
        {
            this.lastState = this.state;
            this.state = state;

            if (state == BluePrintCursorState.DeleteFloor)
            {
                this.meshRenderer.sharedMaterial = this.DeleteFloorMaterial; 
            }
            else
            {
                this.meshRenderer.sharedMaterial = this.AddFloorMaterial;
            }

            this.UpdatePosition();
        }

        /// <summary>
        /// Executes when the gameObject instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }

        /// <summary>
        /// Updates cursor position.
        /// </summary>
        /// <param name="state">The cursor state.</param>
        private void UpdatePosition()
        {
            var mouseEventListener = GlobalOfficeMouseEventManager.inst.MouseEventListener;
            var floorEntity = mouseEventListener?.MouseHoveredFloorEntity;

            // 1. Cursor invisible when mouse event listeners not work.
            // 2. Cursor invisible when mouse not hovered on floor.
            // 3. Cursor invisible when mouse hovered on furniture.
            if (mouseEventListener == null || 
                mouseEventListener.MouseHoveredFloorEntity == null || 
                mouseEventListener.MouseHoveredFurnitureEntity != null ||
                this.state == BluePrintCursorState.Invisible)
            {
                this.transform.position = BLUEPRINT_INVISIBLE_POSITION;
                this.lastState = BluePrintCursorState.Invisible;
                return;
            }

            if (this.state != BluePrintCursorState.Invisible)
            {
                var targetPosition = floorEntity.LeftDownWorldPosition;
                if (this.lastState != this.state)
                {
                    this.transform.position = targetPosition + this.Offset;
                    this.lastState = this.state;
                }
                else
                {
                    this.transform.position = Vector3.SmoothDamp(
                        this.transform.position,
                        targetPosition + this.Offset,
                        ref cursorSmoothFollowVelocity,
                        MOVE_SMOOTH_TIME,
                        float.MaxValue,
                        Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            this.UpdatePosition();
        }
    }
}