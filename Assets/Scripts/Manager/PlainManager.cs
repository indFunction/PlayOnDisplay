using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlainManager : MonoBehaviour
{
    public UserController UserController;
    public DisplayController DisplayController;

    [Header("Use Object")]
    [SerializeField] private GameObject plainDisplay;

    [Header("Original Object")]
    [SerializeField] private Button[] cloneDisplayButtons;
    [SerializeField] private GameObject[] originalDisplayObjects;

    // Unity

    void Start()
    {
        foreach (GameObject go in originalDisplayObjects) go.SetActive(false);
    }

    // Custom Function

    public void ReplaceDisplay(int id)
    {
        GameObject dummy = SetDisplay(id);
    }

    public GameObject ReplaceAndTakeDisplay(int id)
    {
        GameObject cloneDisplayObject = SetDisplay(id);

        return cloneDisplayObject;
    }

    GameObject SetDisplay(int id)
    {
        GameObject cloneDisplayObject = UniversalFunction.SetCloneObject(originalDisplayObjects[id], DisplayController.absoluteObject);

        GameObject[] newDisplayObjects = UniversalFunction.ReplaceArray(UserController.displayObjects, cloneDisplayObject, plainDisplay);

        UserController.displayObjects = newDisplayObjects;
        DisplayController.circleObjects = newDisplayObjects;

        DisplayController.UpdateDisplayCircle(-1);

        GameObject.Destroy(plainDisplay);

        return cloneDisplayObject;
    }
}
