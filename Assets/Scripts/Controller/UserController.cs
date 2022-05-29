using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using TMPro;

public class UserController : MonoBehaviour
{
    public DisplayController DisplayController;
    public NotificationController NotificationController;
    public CameraMove CameraMove;
    public CameraZoom CameraZoom;

    [Header("Use Object")]
    [SerializeField] private Button addDisplayButton;
    [SerializeField] private Button removeDisplayButton;
    [SerializeField] private Button swapPrevDisplayButton;
    [SerializeField] private Button swapNextDisplayButton;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject plainObject;
    [SerializeField] public GameObject[] displayObjects;

    [HideInInspector] public int displayTargetNum = 0;
    [HideInInspector] public bool updateDisplayTargetNum = false;

    [Header("Interface Config")]
    [SerializeField] private GameObject[] interfaceObjects;
    [SerializeField] private Vector3[] interfaceMouseNormalPoses;
    [SerializeField] private Vector3[] interfaceMouseFocusPoses;

    [HideInInspector] public bool hideInterface = false;
    [HideInInspector] public bool updateHideInterface = false;

    [HideInInspector] private UniversalFunction.interfaceConfig[] individualInterfaceConfigsA;
    [HideInInspector] private interfaceConfig[] individualInterfaceConfigsB;

    private class interfaceConfig
    {
        public GameObject interfaceObject;
        public Vector3 mouseNormalPos = Vector3.zero;
        public Vector3 mouseFocusPos = Vector3.zero;
        public bool isFocus = false;
    }

    [Header("Control Config")]
    [SerializeField] private float adjustPointerMovement = 0.005f;
    [SerializeField] private float fixPointerMovement = 0.002f;
    [SerializeField] private float fixKeyMovement = 0.004f;
    [SerializeField] private float delayPointerMovement = 4f;
    [SerializeField] private float delayCameraZoom = 10f;
    [SerializeField] private float timesCameraZoom = 0.5f;
    [SerializeField] private float minCameraZoom = 6f;
    [SerializeField] private float maxCameraZoom = 1f;
    [SerializeField] private float smoothTimes = 15f;

    [Header("Display Config")]
    [SerializeField] private Vector2 displaySizeA = new Vector2(1920f, 1080f);
    [SerializeField] private Vector2 displaySizeB = new Vector2(1920f - 120f, 1080f - 120f);
    [SerializeField] private Vector2 displayLocalScale = new Vector2(0.005f, 0.005f);
    [SerializeField] private float delayChangeDisplayMovement = 4f;
    [SerializeField] private float displayDistanceA = 1.5f;
    [SerializeField] private float displayDistanceB = 3.5f;
    [SerializeField] private float cameraRotationMultiplyA = 1f;
    [SerializeField] private float cameraRotationMultiplyB = 2f;

    [HideInInspector] public Vector2 limitDisplaySizeA = Vector2.zero;
    [HideInInspector] public Vector2 limitDisplaySizeB = Vector2.zero;

    [HideInInspector] private operationManagement userOperationManagement;

    private class operationManagement
    {
        public Vector2 pointerPos = new Vector2(-1f, -1f);
        public Vector2[] pointerDelta = new Vector2[2];
        public bool keyMoveFaster = false;
        public bool modeTranslation = false;
        public float[] zoomScroll = new float[2];
        public bool[] zoomReset = new bool[2];
        public bool addDisplay = false;
        public bool removeDisplay = false;
        public int moveDisplay = 0;
        public bool closeApplication = false;
        public bool extendMode = false;
        public bool toggleInterface = false;
    }

    [HideInInspector] private Vector2 mousePos = Vector2.zero;
    [HideInInspector] public Vector2 targetTranslation = Vector2.zero;
    [HideInInspector] private Vector3 targetPos = Vector3.zero;
    [HideInInspector] private Vector3 targetRot = Vector3.zero;
    [HideInInspector] private float[] targetZoom = new float[5];

    // Unity

    void Awake()
    {
        individualInterfaceConfigsA = UniversalFunction.CreateInterfaceConfigs(interfaceObjects);

        if (interfaceObjects.Length == interfaceMouseNormalPoses.Length && interfaceObjects.Length == interfaceMouseFocusPoses.Length)
        {
            int length = interfaceObjects.Length;

            individualInterfaceConfigsB = new interfaceConfig[length];

            for (int i = 0; i < length; i++) individualInterfaceConfigsB[i] = SetInterfaceConfig(interfaceObjects[i], i, interfaceMouseNormalPoses[i], interfaceMouseFocusPoses[i]);
        }

        userOperationManagement = new operationManagement();

        AddDisplay();
    }

    void Start()
    {
        SetMoveDisplayButton(displayTargetNum);
    }

    void Update()
    {
        if (userOperationManagement.toggleInterface)
        {
            hideInterface = !hideInterface;

            individualInterfaceConfigsA = UniversalFunction.SetInterfaceConfigs(individualInterfaceConfigsA, hideInterface);

            updateHideInterface = true;
        }

        SetInterface(individualInterfaceConfigsB, smoothTimes);

        bool isInvalidInput = UniversalFunction.CheckSelectInputField() || NotificationController.isActive != 0;

        GetUserOperation(false);

        CommandAddAndRemoveDisplay(isInvalidInput);

        CommandMoveAndSwapDisplay(displayTargetNum, isInvalidInput);

        CommandCloseApplication(isInvalidInput);

        SetCamera(isInvalidInput);
    }

    void LateUpdate()
    {
        if (updateDisplayTargetNum) updateDisplayTargetNum = false;

        if (updateHideInterface) updateHideInterface = false;
    }

    // Custom Function

    void SetCamera(bool selectInputField)
    {
        GameObject targetObject = displayObjects
        [
            UniversalFunction.FixValueRange(displayTargetNum, 0, 0, displayObjects.Length - 1)
        ];

        targetZoom = CameraZoom.SetSmoothZoom
        (
            targetZoom,
            userOperationManagement.zoomScroll[0] * 0.1f * (!selectInputField ? 1f : 0f),
            userOperationManagement.zoomScroll[1] * 0.5f * (!selectInputField ? 1f : 0f),
            !selectInputField ? userOperationManagement.zoomReset[0] : false,
            !selectInputField ? userOperationManagement.zoomReset[1] : false,
            delayCameraZoom
        );

        float minCameraResultZoom = (minCameraZoom - (displayDistanceA + displayDistanceB)) / timesCameraZoom * -1f;
        float maxCameraResultZoom = (maxCameraZoom - (displayDistanceA + displayDistanceB)) / timesCameraZoom * -1f;

        targetZoom[0] = UniversalFunction.FixValueRange(targetZoom[0], 0, minCameraResultZoom, maxCameraResultZoom);

        float zoomRatio = targetZoom[0] > 0f ? (maxCameraResultZoom - targetZoom[0]) / maxCameraResultZoom : 1f;

        limitDisplaySizeA = GetLimitDisplaySize(displaySizeA, maxCameraResultZoom);
        limitDisplaySizeB = GetLimitDisplaySize(displaySizeB, maxCameraResultZoom);

        targetTranslation = UniversalFunction.FixValueRange
        (
            GetTargetTranslation(minCameraResultZoom, maxCameraResultZoom, selectInputField),
            0,
            limitDisplaySizeA * -1f,
            limitDisplaySizeA
        );

        mousePos = CameraMove.CalcMousePos(mousePos, userOperationManagement.pointerPos, delayPointerMovement);
        targetPos = CameraMove.CalcTargetPos(targetPos, targetObject.transform.position, delayChangeDisplayMovement);
        targetRot = CameraMove.CalcTargetRot(targetRot, targetObject.transform.localEulerAngles, delayChangeDisplayMovement);

        Vector2 mouseRot = !hideInterface ? CameraMove.CalcMouseRot(mousePos, adjustPointerMovement * zoomRatio) : Vector2.zero;

        cameraObject.transform.position = UniversalFunction.MathfLerp
        (
            cameraObject.transform.position,
            CameraMove.SetTransformPosition
            (
                mouseRot,
                targetPos + CameraMove.AdjustPosAccordingRot
                (
                    (Vector3)targetTranslation * (targetZoom[0] > 0f ? targetZoom[0] : 0f),
                    Vector3.zero,
                    targetObject.transform.localEulerAngles
                ),
                targetRot,
                displayDistanceA - targetZoom[0] * timesCameraZoom,
                displayDistanceB,
                cameraRotationMultiplyA,
                cameraRotationMultiplyB
            ),
            Time.deltaTime * smoothTimes
        );
        cameraObject.transform.rotation = Quaternion.Euler(CameraMove.SetTransformRotation(mouseRot, targetRot));
    }

    Vector2 GetTargetTranslation(float minCameraResultZoom, float maxCameraResultZoom, bool selectInputField)
    {
        Vector2 res = Vector2.zero;

        if (targetZoom[0] > 0f)
        {
            if (userOperationManagement.modeTranslation)
            {
                res = targetTranslation - userOperationManagement.pointerDelta[0] * fixPointerMovement * (maxCameraResultZoom - targetZoom[0] * 0.75f) / maxCameraResultZoom;
            }
            else if (!selectInputField && userOperationManagement.pointerDelta[1].magnitude > 0f)
            {
                res = targetTranslation + userOperationManagement.pointerDelta[1] * fixKeyMovement * (maxCameraResultZoom - targetZoom[0] * 0.75f) / maxCameraResultZoom;
            }
            else
            {
                res = targetTranslation;
            }
        }

        return res;
    }

    Vector2 GetLimitDisplaySize(Vector2 displaySize, float maxCameraResultZoom)
    {
        return new Vector2
        (
            displayLocalScale.x * displaySize.x,
            displayLocalScale.y * displaySize.y
        ) / 2f / maxCameraResultZoom;
    }

    public void TrueFocusInterface(int num)
    {
        individualInterfaceConfigsB[num].isFocus = true;
    }

    public void FalseFocusInterface(int num)
    {
        individualInterfaceConfigsB[num].isFocus = false;
    }

    public GameObject GetDisplayObject(int val)
    {
        return displayObjects[AssumingMoveDisplayTargetNum(val)];
    }

    public void AddDisplay()
    {
        GameObject dummy = SetDisplay();
    }

    public GameObject AddDisplay(int defaultDisplayId)
    {
        GameObject cloneSomeObject = SetDisplay().GetComponentInChildren<PlainManager>().ReplaceAndTakeDisplay(defaultDisplayId);

        return cloneSomeObject;
    }

    GameObject SetDisplay()
    {
        GameObject clonePlainObject = UniversalFunction.SetCloneObject(plainObject, DisplayController.absoluteObject);

        int num = displayObjects.Length > 0 ? displayTargetNum : -1;

        displayObjects = UniversalFunction.InterruptAddArray(displayObjects, clonePlainObject, num);

        DisplayController.circleObjects = displayObjects;

        DisplayController.UpdateDisplayCircle(num + 1);

        updateDisplayTargetNum = true;

        return clonePlainObject;
    }

    public void ReadyRemoveDisplay()
    {
        Button[] go = NotificationController.SetWarningNotification("ディスプレイを削除しますか？");

        // Debug.Log("Do you want to remove the display?");

        go[0].onClick.AddListener(RemoveDisplay);

        updateDisplayTargetNum = true;
    }

    public void RemoveDisplay()
    {
        int num = displayObjects.Length > 1 ? displayTargetNum : -1;

        /*

        if (num == -1)
        {
            Button[] dummy = NotificationController.SetErrorNotification("一つ以下のディスプレイを削除することはできません！");

            // Debug.Log("You can't delete the display that has only one!");

            return;
        }

        */

        if (num != -1)
        {
            GameObject.Destroy(displayObjects[num]);

            displayObjects = UniversalFunction.InterruptRemoveArray(displayObjects, num);
        }
        else
        {
            GameObject clonePlainObject = UniversalFunction.SetCloneObject(plainObject, DisplayController.absoluteObject);
            GameObject destroyObject = displayObjects[0];

            displayObjects = UniversalFunction.ReplaceArray(displayObjects, clonePlainObject, destroyObject);

            GameObject.Destroy(destroyObject);
        }

        DisplayController.circleObjects = displayObjects;

        num = num > 0 ? displayTargetNum - 1 : displayObjects.Length - 1;

        DisplayController.UpdateDisplayCircle(num);

        updateDisplayTargetNum = true;
    }

    public void SetDisplayTargetNum(int val)
    {
        if (val > 0 && val < displayObjects.Length) displayTargetNum = val;

        updateDisplayTargetNum = true;
    }

    public void MoveDisplayTargetNum(int val)
    {
        displayTargetNum = AssumingMoveDisplayTargetNum(val);

        SetMoveDisplayButton(displayTargetNum);

        updateDisplayTargetNum = true;
    }

    int AssumingMoveDisplayTargetNum(int val)
    {
        int res = displayTargetNum;

        if (val < displayObjects.Length && val > displayObjects.Length * -1)
        {
            if (val > 0)
            {
                res += val;
                res = res >= displayObjects.Length ? res - displayObjects.Length : res;
            }
            else if (val < 0)
            {
                res += val;
                res = res < 0 ? res + displayObjects.Length : res;
            }
            else
            {
                res = 0;
            }
        }

        return res;
    }

    public void SetMoveDisplayButton(int num)
    {
        swapPrevDisplayButton.onClick.RemoveAllListeners();
        swapPrevDisplayButton.onClick.AddListener(() => { DisplayController.SwapPrevDisplay(num); });

        swapNextDisplayButton.onClick.RemoveAllListeners();
        swapNextDisplayButton.onClick.AddListener(() => { DisplayController.SwapNextDisplay(num); });
    }

    void CommandAddAndRemoveDisplay(bool isInvalidInput)
    {
        if (!isInvalidInput && userOperationManagement.extendMode)
        {
            if (userOperationManagement.addDisplay) AddDisplay();

            if (userOperationManagement.removeDisplay) ReadyRemoveDisplay();
        }
    }

    void CommandMoveAndSwapDisplay(int num, bool isInvalidInput)
    {
        if (!isInvalidInput && userOperationManagement.moveDisplay != 0)
        {
            if (userOperationManagement.extendMode)
            {
                if (userOperationManagement.moveDisplay == 1)
                {
                    DisplayController.SwapNextDisplay(num);
                }
                else
                {
                    DisplayController.SwapPrevDisplay(num);
                }
            }
            else
            {
                MoveDisplayTargetNum(userOperationManagement.moveDisplay);
            }

            updateDisplayTargetNum = true;
        }
    }

    public void CommandCloseApplication(bool isInvalidInput)
    {
        if (!isInvalidInput && userOperationManagement.extendMode && userOperationManagement.closeApplication) ReadyCloseApplication();
    }

    public void ReadyCloseApplication()
    {
        Button[] go = NotificationController.SetWarningNotification("ソフトウェアを終了しますか？");

        // Debug.Log("Do you want to close this application?");

        go[0].onClick.AddListener(CloseApplication);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }

    public GameObject GetCurrentDisplay()
    {
        return displayObjects[displayTargetNum];
    }

    // Specific Function

    void GetUserOperation(bool isInvalidInput)
    {
        if (Mouse.current != null && !isInvalidInput)
        {
            userOperationManagement.pointerPos = Mouse.current.position.ReadValue();
            userOperationManagement.pointerDelta[0] = Mouse.current.delta.ReadValue();
            userOperationManagement.modeTranslation = Mouse.current.rightButton.isPressed;
            userOperationManagement.zoomScroll[0] = Mouse.current.scroll.ReadValue().y;
            userOperationManagement.zoomReset[0] = Mouse.current.middleButton.wasPressedThisFrame;
        }
        else
        {
            userOperationManagement.pointerPos = new Vector2(-1f, -1f);
            userOperationManagement.pointerDelta[0] = Vector2.zero;
            userOperationManagement.modeTranslation = false;
            userOperationManagement.zoomScroll[0] = 0f;
            userOperationManagement.zoomReset[0] = false;
        }

        if (Keyboard.current != null && !isInvalidInput)
        {
            userOperationManagement.keyMoveFaster = Keyboard.current.leftShiftKey.isPressed;
            userOperationManagement.pointerDelta[1] = GetKeyMove(userOperationManagement.pointerDelta[1], userOperationManagement.keyMoveFaster);
            userOperationManagement.zoomScroll[1] = GetKeyZoom(userOperationManagement.keyMoveFaster);
            userOperationManagement.zoomReset[1] = Keyboard.current.minusKey.wasPressedThisFrame;
            userOperationManagement.addDisplay = Keyboard.current.aKey.wasPressedThisFrame;
            userOperationManagement.removeDisplay = Keyboard.current.rKey.wasPressedThisFrame;
            userOperationManagement.moveDisplay = UniversalFunction.ConvBoolToInt(Keyboard.current.rightArrowKey.wasPressedThisFrame) - UniversalFunction.ConvBoolToInt(Keyboard.current.leftArrowKey.wasPressedThisFrame);
            userOperationManagement.closeApplication = Keyboard.current.qKey.wasPressedThisFrame;
            userOperationManagement.extendMode = Keyboard.current.leftCtrlKey.isPressed;
            userOperationManagement.toggleInterface = Keyboard.current.f1Key.wasPressedThisFrame;
        }
        else
        {
            userOperationManagement.keyMoveFaster = false;
            userOperationManagement.pointerDelta[1] = Vector2.zero;
            userOperationManagement.zoomScroll[1] = 0f;
            userOperationManagement.zoomReset[1] = false;
            userOperationManagement.addDisplay = false;
            userOperationManagement.removeDisplay = false;
            userOperationManagement.moveDisplay = 0;
            userOperationManagement.closeApplication = false;
            userOperationManagement.extendMode = false;
            userOperationManagement.toggleInterface = false;
        }
    }

    Vector2 GetKeyMove(Vector2 oldVector, bool dash)
    {
        float moveX = UniversalFunction.SmoothKeyValueTransition
        (
            oldVector.x,
            Keyboard.current.dKey.isPressed,
            Keyboard.current.aKey.isPressed,
            dash ? 2f : 1f,
            Time.deltaTime * 15f
        );

        float moveY = UniversalFunction.SmoothKeyValueTransition
        (
            oldVector.y,
            Keyboard.current.wKey.isPressed,
            Keyboard.current.sKey.isPressed,
            dash ? 2f : 1f,
            Time.deltaTime * 15f
        );

        return new Vector2(moveX, moveY);
    }

    float GetKeyZoom(bool dash)
    {
        return
        (
            UniversalFunction.ConvBoolToFloat(Keyboard.current.qKey.isPressed) - UniversalFunction.ConvBoolToFloat(Keyboard.current.eKey.isPressed)
        ) * (dash ? 2f : 1f);
    }

    interfaceConfig SetInterfaceConfig(GameObject go, int num, Vector3 normalPos, Vector3 focusPos)
    {
        interfaceConfig res = new interfaceConfig();

        res.interfaceObject = go;
        res.mouseNormalPos = normalPos;
        res.mouseFocusPos = focusPos;

        AttachEventTrigger(go, num);

        return res;
    }

    void AttachEventTrigger(GameObject go, int num)
    {
        go.AddComponent<EventTrigger>();
        var trigger = go.GetComponent<EventTrigger>();

        var entryPointerEnter = new EventTrigger.Entry();
        entryPointerEnter.eventID = EventTriggerType.PointerEnter;
        entryPointerEnter.callback.AddListener((data) => { TrueFocusInterface(num); });

        var entryPointerExit = new EventTrigger.Entry();
        entryPointerExit.eventID = EventTriggerType.PointerExit;
        entryPointerExit.callback.AddListener((data) => { FalseFocusInterface(num); });

        trigger.triggers.Add(entryPointerEnter);
        trigger.triggers.Add(entryPointerExit);
    }

    void SetInterface(interfaceConfig[] individualInterfaceConfigs, float smoothTimes)
    {
        foreach (interfaceConfig individualInterfaceConfig in individualInterfaceConfigs)
        {
            GameObject go = individualInterfaceConfig.interfaceObject;
            Vector3 pos =
            (
                individualInterfaceConfig.isFocus ?
                individualInterfaceConfig.mouseFocusPos :
                individualInterfaceConfig.mouseNormalPos
            );

            RectTransform goRectTransform = go.GetComponent<RectTransform>();
            goRectTransform.anchoredPosition = UniversalFunction.MathfLerp(goRectTransform.anchoredPosition, pos, Time.deltaTime * smoothTimes);
        }
    }
}
