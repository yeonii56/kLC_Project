using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.Extras;

public class VRInputModule_LGW : BaseInputModule
{
    public PointerEventData data;
    public Camera cam;

    public SteamVR_Input_Sources sources;
    public SteamVR_Action_Boolean trigger;

    public float x;
    public float y;
    protected override void Awake()
    {
        //Debug.Log(cam.pixelWidth);
        //Debug.Log(cam.pixelHeight);
        x = 910;
        y = 920;
        base.Awake();

        data = new PointerEventData(eventSystem);
        //data.position = new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f);
        data.position = new Vector2(x, y);
    }
    private void Update()
    {
        //Debug.Log(data.position);
        //data.position = new Vector2(x, y);
    }
    public override void Process()
    {
        // eventsystem의 레이캐스트 사용
        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);

        HandlePointerExitAndEnter(data, data.pointerCurrentRaycast.gameObject);

        ExecuteEvents.Execute(data.pointerDrag, data, ExecuteEvents.dragHandler);

        if (trigger.GetStateDown(sources))
        {
            Press();
        }
        else if (trigger.GetStateUp(sources))
        {
            Release();
        }
    }
    private void Press()
    {
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        GameObject pressObj = ExecuteEvents.GetEventHandler<IPointerDownHandler>(data.pointerCurrentRaycast.gameObject);

        if (pressObj == null)
            ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject);

        data.pointerPress = pressObj;
        data.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(data.pointerCurrentRaycast.gameObject);

        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerDownHandler);

        SendUpdateEventToSelectObj();
    }

    private void Release()
    {
        GameObject pointerRelease = ExecuteEvents.GetEventHandler<IPointerClickHandler>(data.pointerCurrentRaycast.gameObject);
        if (data.pointerPress == pointerRelease)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.deselectHandler);

        data.pointerPress = null;
        data.pointerDrag = null;

        data.pointerCurrentRaycast.Clear();
    }

    public void SendUpdateEventToSelectObj()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return;

        BaseEventData baseData = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, baseData, ExecuteEvents.updateSelectedHandler);
    }
}
