using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Usable Function

    public Vector3 CalcTargetPos(Vector3 oldTargetPos, Vector3 newTargetPos, float delay)
    {
        return UniversalFunction.MathfLerp(oldTargetPos, newTargetPos, Time.deltaTime * delay);
    }

    public Vector3 CalcTargetRot(Vector3 oldTargetRot, Vector3 newTargetRot, float delay)
    {
        Vector3 resultTargetRot = UniversalFunction.ConvLessMovementAngleB(oldTargetRot, newTargetRot, 60f);

        return UniversalFunction.MathfLerp
        (
            resultTargetRot,
            UniversalFunction.ConvLessMovementAngleA(newTargetRot, resultTargetRot),
            Time.deltaTime * delay
        );
    }

    public Vector2 CalcMousePos(Vector2 oldMousePos, Vector2 newMousePos, float delay)
    {
        return UniversalFunction.MathfLerp
        (
            oldMousePos,
            UniversalFunction.ConvPointerCentralReference
            (
                UniversalFunction.FixPointerOutsideWindow
                (
                    newMousePos,
                    UniversalFunction.ConvPointerCentralReference(Vector2.zero, true)
                ),
                false
            ),
            Time.deltaTime * delay
        );
    }

    public Vector2 CalcMouseRot(Vector3 mousePos, float fix)
    {
        return CalcAngle(mousePos, fix, 90f);
    }

    public Vector3 SetTransformPosition
    (
        Vector2 mouseRot,
        Vector3 targetPos,
        Vector3 targetRot,
        float screenDistanceA,
        float screenDistanceB,
        float cameraRotationMultiplyA,
        float cameraRotationMultiplyB
    )
    {
        return CalcCenteredOnPoint
        (
            CalcCenteredOnPoint
            (
                targetPos,
                (Vector2)targetRot + mouseRot * cameraRotationMultiplyA,
                screenDistanceA
            ),
            (Vector2)targetRot + mouseRot * cameraRotationMultiplyB,
            screenDistanceB
        );
    }

    public Vector3 SetTransformRotation(Vector2 mouseRot, Vector3 targetRot)
    {
        return new Vector3(mouseRot.x, mouseRot.y, 0f) + targetRot;
    }

    public Vector3 AdjustPosAccordingRot(Vector3 pointPos, Vector3 targetPos, Vector3 targetRot) // Not perfect :(
    {
        Vector3 distance = pointPos - targetPos;

        Vector3 axisX = new Vector3
        (
            0f,
            distance.y * Mathf.Sin(UniversalFunction.ConvDegToRad(90f - targetRot.x)),
            distance.y * Mathf.Cos(UniversalFunction.ConvDegToRad(90f - targetRot.x))
        );

        Vector3 axisY = new Vector3
        (
            distance.x * Mathf.Cos(UniversalFunction.ConvDegToRad(targetRot.y)),
            0f,
            distance.x * Mathf.Sin(UniversalFunction.ConvDegToRad(targetRot.y)) * -1f
        );

        Vector3 axisZ = new Vector3
        (
            distance.y * Mathf.Sin(UniversalFunction.ConvDegToRad(targetRot.z)) * -1f + distance.x * Mathf.Cos(UniversalFunction.ConvDegToRad(targetRot.z)),
            distance.x * Mathf.Sin(UniversalFunction.ConvDegToRad(targetRot.z)) + distance.y * Mathf.Cos(UniversalFunction.ConvDegToRad(targetRot.z)),
            0f
        );

        return axisX + axisY; // + axisZ
    }

    // Specific Function

    Vector3 CalcAngle(Vector2 angle, float fixMove, float limitAngleX)
    {
        Vector2 newAngle = new Vector2(angle.y, angle.x * -1f) * fixMove;

        newAngle.x = UniversalFunction.CorrectPositiveAngle(newAngle.x);
        newAngle.y = UniversalFunction.CorrectPositiveAngle(newAngle.y);

        if (newAngle.x > limitAngleX && newAngle.x < 360f - limitAngleX)
        {
            newAngle.x = newAngle.x <= 180f ? limitAngleX : 360f - limitAngleX;
        }

        return newAngle;
    }

    Vector3 CalcCenteredOnPoint(Vector3 point, Vector2 angle, float distance)
    {
        Vector2 radX = new Vector2
        (
            Mathf.Sin(UniversalFunction.ConvDegToRad(angle.x)),
            Mathf.Cos(UniversalFunction.ConvDegToRad(angle.x))
        );

        Vector2 radY = new Vector2
        (
            Mathf.Sin(UniversalFunction.ConvDegToRad(angle.y)),
            Mathf.Cos(UniversalFunction.ConvDegToRad(angle.y))
        );

        return new Vector3
        (
            point.x - distance * radY.x + distance * (1f - radX.y) * radY.x,
            point.y + distance * radX.x,
            point.z - distance * radY.y + distance * (1f - radX.y) * radY.y
        );
    }
}
