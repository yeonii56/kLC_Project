using UnityEngine;
using UnityEngine.UI;
#if VRCONCEPT_XR_INTERACTION
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace Epibyte.ConceptVR
{
    public class LaserPointer : MonoBehaviour
    {
        #region Public Variables
        [Header("VR Settings")]
#if VRCONCEPT_OCULUS
        public OVRInput.Controller controller;
        public OVRInput.Button trigger = OVRInput.Button.PrimaryIndexTrigger;
#endif

#if VRCONCEPT_XR_INTERACTION
        public XRController controller;
        public InputHelpers.Button triggerButton;
        bool isTriggerPreviousPressed = false;
#endif

        [Header("Laser Settings")]
        public LineRenderer lineRender;
        public Transform pointer;
        public float length = 0.5f;
        [Header("Joystick Controller")]
        public JoystickController joystickController;
        public static LaserPointer instance;
        #endregion

        #region Private Variables
        IInteractable currentHoverGo;
        IInteractable currentPressedGo;
        Transform currentHitGo;
        float dist;

        #endregion

        #region Properties
        public IInteractable Target
        {
            set
            {
                if (null != currentPressedGo)
                {
                    currentPressedGo.OnReleased();
                }
                currentPressedGo = value;
            }
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)

                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
        }

        void Start()
        {
            if (null != joystickController)
            {
                joystickController.OnJoystickMove += OperateTarget;
            }

            if (null == lineRender)
            {
                lineRender = GetComponent<LineRenderer>();
            }
        }

        void LateUpdate()
        {
            SetLineRenderPosition(0, transform.position);
            RaycastHit hit;
            Vector3 fwd = transform.TransformDirection(Vector3.forward);

            ProcessPress();

            if (Physics.Raycast(transform.position, fwd, out hit))
            {
                if (hit.collider)
                {
                    IInteractable interactable = hit.transform.GetComponent<IInteractable>();

                    if (null != currentPressedGo && null != interactable && currentPressedGo == interactable)
                    {
                        SetPointerForward(dist);
                        return;
                    }

                    currentHitGo = hit.transform;
                    if (Vector3.Distance(transform.position, hit.point) > length)
                    {
                        SetPointerForward(length);
                        return;
                    }
                    else
                    {
                        SetPointerPosition(hit.point);
                    }

                    dist = Vector3.Distance(transform.position, hit.point);

                    if (null != interactable)
                    {
                        if (null == currentHoverGo)
                        {
                            currentHoverGo = interactable;
                        }

                        if (currentHoverGo != interactable)
                        {
                            currentHoverGo.OnLeave();
                            currentHoverGo = interactable;
                        }

                        interactable.OnHovered();
                    }
                    else
                    {
                        if (null != currentHoverGo)
                        {
                            currentHoverGo.OnLeave();
                            currentHoverGo = null;
                        }
                    }
                }
                else
                {
                    SetPointerForward(length);
                }
            }
            else
            {
                currentHitGo = null;
                SetPointerForward(length);
                if (null != currentHoverGo)
                {
                    currentHoverGo.OnLeave();
                    currentHoverGo = null;
                }
            }
        }

        void OnEnable()
        {
            // Reset position for smooth transtation when enbale laser pointer
            if (null != lineRender && lineRender.positionCount > 1)
            {
                lineRender.SetPosition(0, Vector3.zero);
                lineRender.SetPosition(1, Vector3.zero);
            }

        }

        void OnDisable()
        {
            if (null != currentPressedGo)
            {
                currentPressedGo.OnReleased();
                currentPressedGo = null;
            }
        }
        #endregion

        void SetPointerForward(float length)
        {
            Vector3 temp = transform.forward * length + transform.position;
            SetPointerPosition(temp);
        }

        void SetPointerPosition(Vector3 position)
        {
            SetLineRenderPosition(1, position);
            if (null != pointer)
            {
                pointer.position = position;
            }
        }

        void ProcessPress()
        {
#if VRCONCEPT_OCULUS
            if (OVRInput.GetDown(trigger, controller))
#endif

#if VRCONCEPT_XR_INTERACTION
            ButtonState triggerButtonState = getTriggerButtonState();
            if (triggerButtonState == ButtonState.Pressed)
#endif

            {
                if (null != currentHoverGo)
                {
                    // Set the pointer position to the hit object position when clicking.
                    dist = Vector3.Distance(transform.position, currentHitGo.position);
                    SetPointerForward(dist);
                    currentPressedGo = currentHoverGo;
                    currentPressedGo.OnClicked();
                    currentHoverGo = null;
                }
            }

#if VRCONCEPT_OCULUS
            if (OVRInput.GetUp(trigger, controller))
#endif

#if VRCONCEPT_XR_INTERACTION
            if (triggerButtonState == ButtonState.Released)
#endif
            {
                if (null != currentPressedGo)
                {
                    currentPressedGo.OnReleased();
                    currentPressedGo = null;
                }
            }
        }

#if VRCONCEPT_XR_INTERACTION
        ButtonState getTriggerButtonState()
        {
            if (controller.inputDevice.IsPressed(triggerButton, out bool pressed))
            {
                if (isTriggerPreviousPressed != pressed)
                {
                    isTriggerPreviousPressed = pressed;

                    if (pressed)
                    {
                        return ButtonState.Pressed;
                    }
                    else
                    {
                        return ButtonState.Released;
                    }
                }
            }
            return ButtonState.Others;
        }
#endif

        void OperateTarget(ThumbstickStatus status)
        {
            if (null != currentPressedGo && currentPressedGo is Grabble)
            {
                switch (status)
                {
                    case ThumbstickStatus.Down:
                        ((Grabble)currentPressedGo).Scale(-1);
                        break;
                    case ThumbstickStatus.Up:
                        ((Grabble)currentPressedGo).Scale(1);
                        break;
                    case ThumbstickStatus.Left:
                        ((Grabble)currentPressedGo).Rotate(1);
                        break;
                    case ThumbstickStatus.Right:
                        ((Grabble)currentPressedGo).Rotate(-1);
                        break;
                    case ThumbstickStatus.Stayput:
                        ((Grabble)currentPressedGo).StopAll();
                        break;
                }
            }
        }

        void SetLineRenderPosition(int index, Vector3 position)
        {
            if (null != lineRender && lineRender.positionCount > index)
            {
                lineRender.SetPosition(index, position);
            }
        }
    }

#if VRCONCEPT_XR_INTERACTION
    public enum ButtonState { Pressed, Released, Others }
#endif
}
