using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    // Usable Function

    public float[] SetSmoothZoom
    (
        float[] oldTargetZoom,
        float zoomScrollA,
        float zoomScrollB,
        bool resetA,
        bool resetB,
        float zoomDelay
    )
    {
        float[] result =
        {
            0f,
            0f,
            oldTargetZoom[2],
            CalcZoomScroll(oldTargetZoom[3], zoomScrollA, zoomDelay),
            CalcZoomScroll(oldTargetZoom[4], zoomScrollB, zoomDelay)
        };

        if (resetA || resetB)
        {
            result[2] = 0f;
            result[3] = 0f;
            result[4] = 0f;
        }
        else if (result[3] != 0f || result[4] != 0f)
        {
            result[2] = 1f;
        }

        result[1] = oldTargetZoom[0] + result[3] + result[4];

        float buf = Mathf.Lerp(oldTargetZoom[0], result[1] * result[2], Time.deltaTime * zoomDelay);

        result[0] = UniversalFunction.ClearZeroValue(buf, 0);

        return result;
    }

    // Specific Function

    float CalcZoomScroll(float oldScrollZoom, float zoomScroll, float delay)
    {
        float result = Mathf.Lerp
        (
            oldScrollZoom,
            zoomScroll,
            Time.deltaTime * delay
        );

        return UniversalFunction.ClearZeroValue(result, 0);
    }

    float CalcZoomScroll(float oldScrollZoom, bool zoomUp, bool zoomDown, float delay)
    {
        float result = Mathf.Lerp
        (
            oldScrollZoom,
            UniversalFunction.ConvBoolToFloat(zoomUp) - UniversalFunction.ConvBoolToFloat(zoomDown),
            Time.deltaTime * delay
        );

        return UniversalFunction.ClearZeroValue(result, 0);
    }
}
