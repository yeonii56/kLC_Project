namespace RoomBuildingStarterKit.BuildSystem
{
    using RoomBuildingStarterKit.Common;
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// The CameraController class used to control main camera.
    /// </summary>
    public class CameraController : Singleton<CameraController>
    {
        /// <summary>
        /// The world left down location.
        /// </summary>
        public Vector2 WorldLeftDownPoint;

        /// <summary>
        /// The world right up location.
        /// </summary>
        public Vector2 WorldRightUpPoint;

        /// <summary>
        /// The world position offset.
        /// </summary>
        public float WorldPosOffset;

        /// <summary>
        /// The hover speed by distance curve.
        /// </summary>
        public AnimationCurve HoverSpeedToDist;

        /// <summary>
        /// The mouse drag to move speed.
        /// </summary>
        public float DragSpeed = 0.05f;

        /// <summary>
        /// The camera follow position.
        /// </summary>
        private Vector3 followPos;

        /// <summary>
        /// The camera zoom drag factor.
        /// </summary>
        public float ZoomDrag = 10f;

        /// <summary>
        /// The camera zoom factor.
        /// </summary>
        public float ZoomFactor = 300f;

        /// <summary>
        /// The camera phi range.
        /// </summary>
        public MinMax PhiRange = new MinMax(50f, 85f);

        /// <summary>
        /// The camera zoom range.
        /// </summary>
        public MinMax ZoomRange = new MinMax(15f, 60f);

        /// <summary>
        /// The camera theta rotation speed.
        /// </summary>
        public float ThetaSpeed = 0.5f;

        /// <summary>
        /// The camera phi rotation speed.
        /// </summary>
        public float PhiSpeed = 0.5f;

        /// <summary>
        /// The camera follow smooth time.
        /// </summary>
        public float FollowSmoothTime = 0.2f;

        /// <summary>
        /// The target theta.
        /// </summary>
        public float TargetTheta = -90f;

        [Tooltip("The default camera follow target position.")]
        public Vector3 DefaultTargetPos = new Vector3(11f, 0f, 35f);

        /// <summary>
        /// The distance to phi offset curve.
        /// </summary>
        public AnimationCurve DistToPhiOffset;

        /// <summary>
        /// The camera to follow position distance.
        /// </summary>
        private float dist = 50f;

        /// <summary>
        /// The camera theta.
        /// </summary>
        private float theta = 0f;

        /// <summary>
        /// The camera phi.
        /// </summary>
        private float phi = 55f;

        /// <summary>
        /// The last mouse position.
        /// </summary>
        private Vector3 lastMousePos;

        /// <summary>
        /// The is dragging.
        /// </summary>
        private bool isDragging;

        /// <summary>
        /// The mouse hovered total time.
        /// </summary>
        private float mouseHoveredTime;

        /// <summary>
        /// The distance factor.
        /// </summary>
        private float distFactor;

        /// <summary>
        /// The target distance.
        /// </summary>
        private float targetDist = 40f;

        /// <summary>
        /// The target phi.
        /// </summary>
        private float targetPhi = 50f;

        /// <summary>
        /// The camera follow target position.
        /// </summary>
        private Vector3 targetPos = new Vector3(11f, 0f, 35f);

        /// <summary>
        /// The camera position.
        /// </summary>
        private Vector3 cameraPos;

        /// <summary>
        /// The drag threshold by pixel.
        /// </summary>
        private int dragThreshold;

        /// <summary>
        /// The start drag mouse position.
        /// </summary>
        private Vector3 startMousePosition;

        /// <summary>
        /// The can drag flag represents reach threshold or not.
        /// </summary>
        private bool canDrag;

        /// <summary>
        /// Whether the camera is interactable with mouse event.
        /// </summary>
        private bool interactable = true;

        /// <summary>
        /// The camera start follow position.
        /// </summary>
        private Vector3 startFollowPosition;

        /// <summary>
        /// The animator component.
        /// </summary>
        private Animator animator;

        /// <summary>
        /// The camera follow smooth damp velocity.
        /// </summary>
        private Vector3 smoothFollowVelocity;

        /// <summary>
        /// The theta smooth damp velocity.
        /// </summary>
        private float thetaSmoothChangeVelocity;

        /// <summary>
        /// The phi smooth damp velocity.
        /// </summary>
        private float phiSmoothChangeVelocity;

        /// <summary>
        /// The distance smooth damp velocity.
        /// </summary>
        private float distSmoothChangeVelocity;

        /// <summary>
        /// The wait for end of frame.
        /// </summary>
        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        /// <summary>
        /// Gets the main camera component.
        /// </summary>
        public Camera Cam { get; private set; }

        /// <summary>
        /// Gets the animator component.
        /// </summary>
        public Animator Animator => this.animator;

        /// <summary>
        /// Gets or sets the target distance.
        /// </summary>
        public float TargetDist
        {
            get => this.targetDist;
            set => this.targetDist = this.ZoomRange.Clamp(value);
        }

        /// <summary>
        /// Resets camera to focus on a point.
        /// </summary>
        /// <param name="point">The target point.</param>
        public void ResetToPoint(Vector3 point)
        {
            this.targetPos = point;
            this.interactable = false;
            StartCoroutine(this.UnblockInteractable());
        }

        /// <summary>
        /// Projects mouse position to ground.
        /// </summary>
        /// <param name="mousePos">The mouse position.</param>
        /// <returns>The mouse  projection on ground.</returns>
        public Vector3 ProjectToGround(Vector3 mousePos)
        {
            Ray ray = this.Cam.ScreenPointToRay(mousePos);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }

            return Vector3.zero;
        }

        /// <summary>
        /// Executes when the gameObject instantiates.
        /// </summary>
        protected override void AwakeInternal()
        {
            this.Cam = Camera.main;
            this.animator = this.GetComponent<Animator>();

            this.dragThreshold = (int)(((Screen.dpi == 0f) ? 72f : Screen.dpi) / 2f);
            this.dist = this.targetDist;
            this.theta = this.TargetTheta;
            this.phi = this.targetPhi;
            this.followPos = this.targetPos = this.DefaultTargetPos;
        }

        /// <summary>
        /// Unblocks camera interactable state.
        /// </summary>
        /// <returns>The coroutine.</returns>
        private IEnumerator UnblockInteractable()
        {
            while (Mathf.Abs(this.dist - this.targetDist) >= 0.2f
                || Mathf.Abs(this.phi - this.targetPhi) >= 0.2f
                || Mathf.Abs(this.theta - this.TargetTheta) >= 0.2f)
            {
                yield return this.waitForEndOfFrame;
            }

            this.interactable = true;
        }

        /// <summary>
        /// Updates mouse zoom.
        /// </summary>
        /// <param name="deltaTime">The delta time.</param>
        private void UpdateMouseZoom(float deltaTime)
        {
            if (!this.interactable)
            {
                return;
            }

            this.distFactor += -Input.GetAxis("Mouse ScrollWheel") * this.ZoomFactor;
            this.distFactor += -this.distFactor * this.ZoomDrag * deltaTime;
            this.TargetDist += this.distFactor * deltaTime;
        }

        /// <summary>
        /// Updates mouse rotate.
        /// </summary>
        private void UpdateMouseRotate()
        {
            if (!this.interactable)
            {
                return;
            }

            if (!Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(2))
                {
                    this.lastMousePos = Input.mousePosition;
                }
                else if (Input.GetMouseButton(2))
                {
                    var v = Input.mousePosition - this.lastMousePos;
                    this.lastMousePos = Input.mousePosition;
                    this.TargetTheta += v.x * this.ThetaSpeed;
                    this.targetPhi += -v.y * this.PhiSpeed;
                }
            }
        }

        /// <summary>
        /// Clamps the camera follow point inside world bounds.
        /// </summary>
        /// <param name="p"></param>
        private Vector3 ClampToWorldBounds(Vector3 p)
        {
            p.x = Mathf.Clamp(p.x, this.WorldLeftDownPoint.x - this.WorldPosOffset, this.WorldRightUpPoint.x + this.WorldPosOffset);
            p.z = Mathf.Clamp(p.z, this.WorldLeftDownPoint.y - this.WorldPosOffset, this.WorldRightUpPoint.y + this.WorldPosOffset);
            return p;
        }

        /// <summary>
        /// Tries to start drag.
        /// </summary>
        /// <param name="mousePosition">The mouse position.</param>
        private void TryStartDrag(Vector3 mousePosition)
        {
            if (!this.isDragging && this.interactable)
            {
                this.startMousePosition = mousePosition;
                this.isDragging = true;
                this.canDrag = false;
            }
        }

        /// <summary>
        /// Executes every frame.
        /// </summary>
        private void Update()
        {
            if (InputWrapper.IsBlocking)
            {
                return;
            }

            float deltaTime = Mathf.Clamp(Time.unscaledDeltaTime, 1e-05f, 0.03f);
            Vector3 mousePosition = Input.mousePosition;

            this.UpdateMouseZoom(deltaTime);
            this.UpdateMouseRotate();

            if (Input.GetMouseButtonDown(1))
            {
                this.TryStartDrag(mousePosition);
            }
            else if (Input.GetMouseButton(1) && this.isDragging)
            {
                if((this.startMousePosition - mousePosition).magnitude > (float)this.dragThreshold && !this.canDrag)
                {
                    this.canDrag = true;
                    this.startFollowPosition = this.targetPos;
                }

                if (this.canDrag)
                {
                    var dir = (this.startMousePosition - mousePosition) * this.DragSpeed;
                    var rightDir = this.transform.right;
                    var forwardDir = this.transform.forward;
                    rightDir.y = 0;
                    forwardDir.y = 0;
                    rightDir.Normalize();
                    forwardDir.Normalize();
                    this.targetPos = this.startFollowPosition + dir.x * rightDir + dir.y * forwardDir;
                }
            }
            else
            {
                this.isDragging = false;
            }

            this.targetPos.y = Mathf.Clamp(this.targetPos.y, 0f, 2f);
            this.targetPos = this.ClampToWorldBounds(this.targetPos);
            this.followPos = Vector3.SmoothDamp(this.followPos, this.targetPos, ref smoothFollowVelocity, this.FollowSmoothTime, float.MaxValue, deltaTime);

            this.theta = Mathf.SmoothDamp(this.theta, this.TargetTheta, ref thetaSmoothChangeVelocity, 0.05f, float.MaxValue, deltaTime);
            this.phi = Mathf.SmoothDamp(this.phi, this.targetPhi, ref phiSmoothChangeVelocity, 0.1f, float.MaxValue, deltaTime);
            this.dist = Mathf.SmoothDamp(this.dist, this.targetDist, ref distSmoothChangeVelocity, 0.1f, float.MaxValue, deltaTime);

            float offsetPhi = this.phi + this.DistToPhiOffset.Evaluate(this.dist);
            offsetPhi = Mathf.Clamp(offsetPhi, 45f, 85f);
            this.targetPhi = Mathf.Clamp(this.targetPhi, this.PhiRange.Min, this.PhiRange.Max);
            
            Vector3 vector = Quaternion.AngleAxis(offsetPhi, new Vector3(0f, 0f, 1f)) * new Vector3(1f, 0f, 0f);
            vector = Quaternion.AngleAxis(this.theta, new Vector3(0f, 1f, 0f)) * vector;
            vector.Normalize();

            this.transform.position = this.followPos + this.dist * vector;
            
            Vector3 forward = this.followPos - this.transform.position;
            forward.Normalize();
            
            this.transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}