using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
    [Header("Use Warning Object")]
    [SerializeField] private GameObject warningNotification;
    [SerializeField] private GameObject warningMessageText;
    [SerializeField] private Button warningAllowActionButton;
    [SerializeField] private Button warningDenyActionButton;

    [Header("Use Error Object")]
    [SerializeField] private GameObject errorNotification;
    [SerializeField] private GameObject errorMessageText;
    [SerializeField] private Button errorRogerButton;

    [HideInInspector] public int isActive = 0;

    // Unity

    void Start()
    {
        SetWarningDisplay(2);
        SetErrorDisplay(2);
    }

    void Update()
    {
        if (isActive != 0)
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CommandNotification();

                SetWarningDisplay(2);
                SetErrorDisplay(2);
            }

            if (isActive == 1 && Keyboard.current.yKey.wasPressedThisFrame) warningAllowActionButton.onClick.Invoke();

            if (isActive == 1 && Keyboard.current.nKey.wasPressedThisFrame) warningDenyActionButton.onClick.Invoke();

            if (isActive == 2 && Keyboard.current.enterKey.wasPressedThisFrame) errorRogerButton.onClick.Invoke();
        }
    }

    // Custom Function

    public Button[] SetWarningNotification(string message)
    {
        UniversalFunction.SetText(warningMessageText, message);

        Button[] sub = new Button[2];

        sub[0] = CallButton(1, true);
        sub[1] = CallButton(2, true);

        SetWarningDisplay(1);

        isActive = 1;

        return sub;
    }

    public Button[] SetErrorNotification(string message)
    {
        UniversalFunction.SetText(errorMessageText, message);

        Button[] sub = new Button[1];

        sub[0] = CallButton(3, true);

        SetErrorDisplay(1);

        isActive = 2;

        return sub;
    }

    public void SetWarningDisplay(int force)
    {
        GameObject go = warningNotification;

        switch (force)
        {
            case 0:
                go.SetActive(!go.activeSelf);
                break;
            case 1:
                go.SetActive(true);
                break;
            case 2:
                go.SetActive(false);
                break;
        }
    }

    public void SetErrorDisplay(int force)
    {
        GameObject go = errorNotification;

        switch (force)
        {
            case 0:
                go.SetActive(!go.activeSelf);
                break;
            case 1:
                go.SetActive(true);
                break;
            case 2:
                go.SetActive(false);
                break;
        }
    }

    public void CommandNotification()
    {
        isActive = 0;
    }

    Button CallButton(int num, bool resetEvent)
    {
        Button go = null;

        switch (num)
        {
            case 1:
                go = warningAllowActionButton;

                if (resetEvent)
                {
                    go.onClick.RemoveAllListeners();
                    go.onClick.AddListener(() => { SetWarningDisplay(2); });
                }
                break;
            case 2:
                go = warningDenyActionButton;

                if (resetEvent)
                {
                    go.onClick.RemoveAllListeners();
                    go.onClick.AddListener(() => { SetWarningDisplay(2); });
                }
                break;
            case 3:
                go = errorRogerButton;

                if (resetEvent)
                {
                    go.onClick.RemoveAllListeners();
                    go.onClick.AddListener(() => { SetErrorDisplay(2); });
                }
                break;
        }

        if (resetEvent) go.onClick.AddListener(CommandNotification);

        return go;
    }
}
