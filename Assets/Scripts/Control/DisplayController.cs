using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    public UserController UserController;

    [Header("Use Object")]
    [SerializeField] public GameObject absoluteObject;
    [SerializeField] public GameObject[] circleObjects;

    // Custom Function

    public void UpdateDisplayCircle(int id)
    {
        UserController.displayObjects = circleObjects;

        if (id > -1)
        {
            UserController.displayTargetNum = id;

            UserController.SetMoveDisplayButton(id);
        }

        SetDisplayCircle(absoluteObject, circleObjects, (float)circleObjects.Length * 2f);
    }

    public void SwapPrevDisplay(int id)
    {
        int length = circleObjects.Length;
        int targetId = id == 0 ? length - 1 : id - 1;

        GameObject goBuffer = circleObjects[targetId];

        circleObjects[targetId] = circleObjects[id];
        circleObjects[id] = goBuffer;

        UpdateDisplayCircle(targetId);

        UserController.updateDisplayTargetNum = true;
    }

    public void SwapNextDisplay(int id)
    {
        int length = circleObjects.Length;
        int targetId = id == length - 1 ? 0 : id + 1;

        GameObject goBuffer = circleObjects[targetId];

        circleObjects[targetId] = circleObjects[id];
        circleObjects[id] = goBuffer;

        UpdateDisplayCircle(targetId);

        UserController.updateDisplayTargetNum = true;
    }

    // Specific Function

    void SetDisplayCircle(GameObject absoluteObject, GameObject[] circleObjects, float radius)
    {
        int length = circleObjects.Length;

        if (absoluteObject == null || circleObjects.Length == 0) return;

        for (int i = 0; i < length; i++)
        {
            circleObjects[i].transform.position = CalcDisplayCirclePos(absoluteObject.transform.position, i, length, radius, 90f, true);
            circleObjects[i].transform.rotation = Quaternion.Euler
            (
                CalcDisplayCircleRot(i, length, 0f, false)
            );
        }
    }

    Vector3 CalcDisplayCirclePos(Vector3 absPos, int num, int polygon, float radius, float addDeg, bool reverse)
    {
        float deg = UniversalFunction.CorrectPositiveAngle
        (
            (float)num / (float)polygon * 360f * (reverse ? -1f : 1f) + addDeg
        );

        float rad = UniversalFunction.ConvDegToRad(deg);

        return absPos + new Vector3
        (
            radius * Mathf.Cos(rad),
            0f,
            radius * Mathf.Sin(rad)
        );
    }

    Vector3 CalcDisplayCircleRot(int num, int polygon, float addDeg, bool reverse)
    {
        float deg = UniversalFunction.CorrectPositiveAngle
        (
            (float)num / (float)polygon * 360f * (reverse ? -1f : 1f) + addDeg
        );

        return new Vector3(0f, deg, 0f);
    }
}
