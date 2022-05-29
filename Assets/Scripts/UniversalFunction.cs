using System;
using System.Collections;
using System.Collections.Generic;
using SD = System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TMPro;

/*

If (use System.Drawing)
{
    - Need "System.Drawing.dll" from "C:\Program Files\Unity\Hub\Editor\[Unity]\Editor\Data\MonoBleedingEdge\lib\mono\4.5\System.Drawing.dll"
    - Change ".NET 2.0" to ".NET 4.x" in "Project Settings > Player > Other Settings > Configuration > Api Compatibility Level"
}
else
{
    - Remove ReadImageResolution() and the functions that use it
}

*/

public class UniversalFunction : MonoBehaviour
{
    public class interfaceConfig
    {
        public GameObject interfaceObject;
        public bool activeCache;
    }

    public static int FixValueRange(int val, int mode, int min, int max)
    {
        if (mode < 0 || mode > 2) mode = 0;
        if (min > max) max = min;

        if (val < min && (mode == 0 || mode == 1))
        {
            return min;
        }
        else if (val > max && (mode == 0 || mode == 2))
        {
            return max;
        }
        else
        {
            return val;
        }
    }

    public static float FixValueRange(float val, int mode, float min, float max)
    {
        if (mode < 0 || mode > 2) mode = 0;
        if (min > max) max = min;

        if (val < min && (mode == 0 || mode == 1))
        {
            return min;
        }
        else if (val > max && (mode == 0 || mode == 2))
        {
            return max;
        }
        else
        {
            return val;
        }
    }

    public static Vector2 FixValueRange(Vector2 val, int mode, Vector2 min, Vector2 max)
    {
        return new Vector2
        (
            FixValueRange(val.x, mode, min.x, max.x),
            FixValueRange(val.y, mode, min.y, max.y)
        );
    }

    public static int CalcValeToClosePow(float valA, float valB)
    {
        if (valA <= valB) return 0;

        float n = valA;
        int i = 1;

        while (true)
        {
            n /= valB;
            i++;

            if (n <= valB) break;
        }

        return i;
    }

    public static int CeilDivideIntVale(int valA, int valB)
    {
        return (int)Mathf.Ceil((float)valA / (float)valB);
    }

    public static float FloorBasedOnZero(float val, float multiply)
    {
        return Mathf.Floor(val / multiply) * multiply + (val >= 0f ? 0f : multiply);
    }

    public static Vector2 MathfLerp(Vector2 valA, Vector2 valB, float time, bool clearZero)
    {
        return new Vector2
        (
            Mathf.Lerp(valA.x, valB.x, time),
            Mathf.Lerp(valA.y, valB.y, time)
        );
    }

    public static Vector3 MathfLerp(Vector3 valA, Vector3 valB, float time)
    {
        return new Vector3
        (
            Mathf.Lerp(valA.x, valB.x, time),
            Mathf.Lerp(valA.y, valB.y, time),
            Mathf.Lerp(valA.z, valB.z, time)
        );
    }

    public static Vector3 GenerateRandomRange(float min, float max)
    {
        return new Vector3
        (
            UnityEngine.Random.Range(min, max),
            UnityEngine.Random.Range(min, max),
            UnityEngine.Random.Range(min, max)
        );
    }

    public static int ConvBoolToInt(bool val)
    {
        return val ? 1 : 0;
    }

    public static float ConvBoolToFloat(bool val)
    {
        return val ? 1f : 0f;
    }

    public static float ConvDegToRad(float val)
    {
        return (val / 360f) * Mathf.PI * 2f;
    }

    public static float ClearZeroValue(float val, int mode)
    {
        if (mode < 0 || mode > 2) mode = 0;

        float res = 0f;

        switch (mode)
        {
            case 0:
                res = Mathf.Abs(val) >= 0.001f ? val : 0f;
                break;
            case 1:
                res = val >= 0.001f ? val : 0f;
                break;
            case 2:
                res = val <= -0.001f ? val : 0f;
                break;
        }

        return res;
    }

    public static float CorrectPositiveAngle(float val)
    {
        float res = val - FloorBasedOnZero(val, 360f);

        return res >= 0f ? res : 360f + res;
    }

    public static float FixTwistAngleA(float decideVal, float compareVal)
    {
        float newDecideVal = CorrectPositiveAngle(decideVal) - (decideVal >= 0f ? 0f : 360f);
        float newCompareVal = CorrectPositiveAngle(compareVal) - (compareVal >= 0f ? 0f : 360f);

        if (Mathf.Abs(newDecideVal - newCompareVal) < 180f)
        {
            return newDecideVal;
        }
        else
        {
            return newDecideVal - 360f;
        }
    }

    public static float FixTwistAngleB(float decideVal, float compareVal, float judgeAngle)
    {
        judgeAngle = FixValueRange(judgeAngle, 0, 180f, 360f);

        if (Mathf.Abs(decideVal - compareVal) > judgeAngle)
        {
            if (decideVal > compareVal)
            {
                return decideVal - 360f;
            }
            else
            {
                return decideVal + 360f;
            }
        }

        return decideVal;
    }

    public static Vector3 ConvLessMovementAngleA(Vector3 decideAngle, Vector3 compareAngle)
    {
        return new Vector3
        (
            FixTwistAngleA(decideAngle.x, compareAngle.x),
            FixTwistAngleA(decideAngle.y, compareAngle.y),
            FixTwistAngleA(decideAngle.z, compareAngle.z)
        );
    }

    public static Vector3 ConvLessMovementAngleB(Vector3 decideAngle, Vector3 compareAngle, float judgeAngle)
    {
        return new Vector3
        (
            FixTwistAngleB(decideAngle.x, compareAngle.x, judgeAngle),
            FixTwistAngleB(decideAngle.y, compareAngle.y, judgeAngle),
            FixTwistAngleB(decideAngle.z, compareAngle.z, judgeAngle)
        );
    }

    public static float ConvScreenCentralReference(float point, float screen, bool reverse)
    {
        return reverse ? (point + screen / 2f) / screen : point * screen - (screen / 2f);
    }

    public static Vector2 ConvPointerCentralReference(Vector2 rawMousePos, bool reverse)
    {
        Vector2 calcPos = Vector2.zero;

        if (!reverse)
        {
            calcPos.x = rawMousePos.x - Screen.width / 2f;
            calcPos.y = rawMousePos.y - Screen.height / 2f;
        }
        else
        {
            calcPos.x = rawMousePos.x + Screen.width / 2f;
            calcPos.y = rawMousePos.y + Screen.height / 2f;
        }

        return calcPos;
    }

    public static Vector2 FixPointerOutsideWindow(Vector2 rawPos, Vector2 defaultPos)
    {
        Vector2 fixPos = Vector2.zero;

        fixPos.x = FixValueRange(rawPos.x, 0, 0f, Screen.width);
        fixPos.y = FixValueRange(rawPos.y, 0, 0f, Screen.height);

        return rawPos == fixPos ? fixPos : defaultPos;
    }

    public static Vector2 ResizeRectResolution(Vector2 targetSize, Vector2 screenSize)
    {
        float targetAspectRatio = targetSize.x / targetSize.y;
        float screenAspectRatio = screenSize.x / screenSize.y;

        Vector2 fitSize = Vector2.zero;

        if (targetAspectRatio < screenAspectRatio)
        {
            fitSize.y = screenSize.y / targetSize.y;
            fitSize.x = fitSize.y;
        }
        else if (targetAspectRatio > screenAspectRatio)
        {
            fitSize.x = screenSize.x / targetSize.x;
            fitSize.y = fitSize.x;
        }
        else
        {
            fitSize = screenSize / targetSize;
        }

        return fitSize;
    }

    public static string AlignDigitsToMaxNum(int num, int max)
    {
        string optionZeroFill = "D" + CalcValeToClosePow((float)max, 10f);

        return num.ToString(optionZeroFill);
    }

    public static void DestroyChildObject(GameObject go)
    {
        foreach (Transform child in go.transform) GameObject.Destroy(child.gameObject);
    }

    public static void SetActiveObjects(GameObject[] gos, bool isActive)
    {
        foreach (GameObject go in gos) go.SetActive(isActive);
    }

    public static GameObject SetCloneObject(GameObject originalObject, GameObject cloneContainer)
    {
        GameObject cloneObject = Instantiate(originalObject) as GameObject;
        cloneObject.SetActive(true);
        cloneObject.transform.SetParent(cloneContainer.transform);

        return cloneObject;
    }

    public static Vector2 GetLineRendererPos(LineRenderer lineRenderer, int i, Vector2 screenSize)
    {
        Vector3 lineRendererPos = lineRenderer.GetPosition(i);

        Vector2 pos = new Vector2
        (
            ConvScreenCentralReference(lineRendererPos.x, screenSize.x, true),
            ConvScreenCentralReference(lineRendererPos.y, screenSize.y, true)
        );

        return pos;
    }

    public static void ConnectRectParent(GameObject originalObject, GameObject cloneObject)
    {
        cloneObject.transform.localScale = originalObject.transform.localScale;

        RectTransform originalRectTransform = originalObject.GetComponent<RectTransform>();
        RectTransform cloneRectTransform = cloneObject.GetComponent<RectTransform>();

        if (originalRectTransform != null && cloneRectTransform != null)
        {
            cloneRectTransform.position = originalRectTransform.position;
            cloneRectTransform.anchoredPosition = originalRectTransform.anchoredPosition;
            cloneRectTransform.rotation = originalRectTransform.rotation;
            cloneRectTransform.sizeDelta = originalRectTransform.sizeDelta;
        }
        else
        {
            Debug.LogError("Component: RectTransform does not found!");
        }
    }

    public static GameObject[] InterruptAddArray(GameObject[] gos, GameObject go, int num)
    {
        int mode = 1;

        int newArrayLength = gos.Length + mode;

        if (mode != -1 && mode != 1 && newArrayLength < 1) return null;

        GameObject[] newArray = new GameObject[newArrayLength];

        for (int i = 0; i < newArrayLength; i++)
        {
            if (i < num + (mode == 1 ? 1 : 0))
            {
                newArray[i] = gos[i];
            }
            else if (i == num + 1 && mode == 1)
            {
                newArray[i] = go;
            }
            else
            {
                newArray[i] = gos[i - mode];
            }
        }

        return newArray;
    }

    public static GameObject[] InterruptRemoveArray(GameObject[] gos, int num)
    {
        int mode = -1;

        int newArrayLength = gos.Length + mode;

        if (mode != -1 && mode != 1 && newArrayLength < 1) return null;

        GameObject[] newArray = new GameObject[newArrayLength];

        for (int i = 0; i < newArrayLength; i++)
        {
            if (i < num + (mode == 1 ? 1 : 0))
            {
                newArray[i] = gos[i];
            }
            else if (i == num + 1 && mode == 1)
            {
                //
            }
            else
            {
                newArray[i] = gos[i - mode];
            }
        }

        return newArray;
    }

    public static GameObject[] ReplaceArray(GameObject[] gos, GameObject replaceObject, GameObject targetObject)
    {
        GameObject[] newArray = gos;

        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i] == targetObject) gos[i] = replaceObject;
        }

        return newArray;
    }

    public static void SetText(GameObject go, string text)
    {
        TextMeshProUGUI goTMP = go.GetComponent<TextMeshProUGUI>();
        if (goTMP != null)
        {
            goTMP.SetText(text);
        }
        else
        {
            Text goText = go.GetComponent<Text>();
            if (goText != null)
            {
                goText.text = text;
            }
            else
            {
                Debug.LogError("Component: Text or TextMeshProUGUI does not found!");
            }
        }
    }

    public static void SetText(GameObject go, int mode, string text, Color32 color)
    {
        if (mode < 0 || mode > 3) mode = 3;
        if (mode == 3) return;

        TextMeshProUGUI goTMP = go.GetComponent<TextMeshProUGUI>();
        if (goTMP != null)
        {
            if (mode == 0 || mode == 1) goTMP.SetText(text);
            if (mode == 0 || mode == 2) goTMP.color = color;
        }
        else
        {
            Text goText = go.GetComponent<Text>();
            if (goText != null)
            {
                if (mode == 0 || mode == 1) goText.text = text;
                if (mode == 0 || mode == 2) goText.color = color;
            }
            else
            {
                Debug.LogError("Component: Text or TextMeshProUGUI does not found!");
            }
        }
    }

    public static void SetButtonText(GameObject go, string text)
    {
        TextMeshProUGUI goTMP = go.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (goTMP != null)
        {
            goTMP.SetText(text);
        }
        else
        {
            Text goText = go.transform.GetChild(0).gameObject.GetComponent<Text>();
            if (goText != null)
            {
                goText.text = text;
            }
            else
            {
                Debug.LogError("Component: Text or TextMeshProUGUI does not found!");
            }
        }
    }

    public static void SetButtonText(GameObject go, int mode, string text, Color32 color)
    {
        if (mode < 0 || mode > 3) mode = 3;
        if (mode == 3) return;

        TextMeshProUGUI goTMP = go.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (goTMP != null)
        {
            if (mode == 0 || mode == 1) goTMP.SetText(text);
            if (mode == 0 || mode == 2) goTMP.color = color;
        }
        else
        {
            Text goText = go.transform.GetChild(0).gameObject.GetComponent<Text>();
            if (goText != null)
            {
                if (mode == 0 || mode == 1) goText.text = text;
                if (mode == 0 || mode == 2) goText.color = color;
            }
            else
            {
                Debug.LogError("Component: Text or TextMeshProUGUI does not found!");
            }
        }
    }

    public static void SetButtonColor(Button button, int mode, Color32 color)
    {
        ColorBlock colorBlock = button.colors;

        switch (mode)
        {
            case 0:
                colorBlock.normalColor = color;
                break;
            case 1:
                colorBlock.highlightedColor = color;
                break;
            case 2:
                colorBlock.pressedColor = color;
                break;
            case 3:
                colorBlock.selectedColor = color;
                break;
            case 4:
                colorBlock.disabledColor = color;
                break;
        }

        button.colors = colorBlock;
    }

    public static void SetInteractableButton(Button button, bool interactable)
    {
        button.interactable = interactable;
    }

    public static string GetInputFieldText(GameObject go)
    {
        TMP_InputField goTMP_InputField = go.GetComponent<TMP_InputField>();
        if (goTMP_InputField != null)
        {
            return goTMP_InputField.text;
        }
        else
        {
            InputField goInputField = go.GetComponent<InputField>();
            if (goInputField != null)
            {
                return goInputField.text;
            }
            else
            {
                Debug.LogError("Component: InputField or TMP_InputField does not found!");

                return "";
            }
        }
    }

    public static void SetInputFieldText(GameObject go, string text)
    {
        TMP_InputField goTMP_InputField = go.GetComponent<TMP_InputField>();
        if (goTMP_InputField != null)
        {
            goTMP_InputField.text = text;
        }
        else
        {
            InputField goInputField = go.GetComponent<InputField>();
            if (goInputField != null)
            {
                goInputField.text = text;
            }
            else
            {
                Debug.LogError("Component: InputField or TMP_InputField does not found!");
            }
        }
    }

    public static void SetInteractableInputField(GameObject go, bool interactable)
    {
        TMP_InputField goTMP_InputField = go.GetComponent<TMP_InputField>();
        if (goTMP_InputField != null)
        {
            goTMP_InputField.interactable = interactable;
        }
        else
        {
            InputField goInputField = go.GetComponent<InputField>();
            if (goInputField != null)
            {
                goInputField.interactable = interactable;
            }
            else
            {
                Debug.LogError("Component: InputField or TMP_InputField does not found!");
            }
        }
    }

    public static void MoveLineRenderer(LineRenderer lineRenderer, Vector2 startPos, Vector2 endPos, Vector2 screenSize)
    {
        Vector3[] pos = new Vector3[2]
        {
            new Vector3
            (
                ConvScreenCentralReference(startPos.x, screenSize.x, false),
                ConvScreenCentralReference(startPos.y, screenSize.y, false),
                0f
            ),
            new Vector3
            (
                ConvScreenCentralReference(endPos.x, screenSize.x, false),
                ConvScreenCentralReference(endPos.y, screenSize.y, false),
                0f
            )
        };

        lineRenderer.SetPosition(0, pos[0]);
        lineRenderer.SetPosition(1, pos[1]);
    }

    public static void MoveLineRenderer(LineRenderer lineRenderer, Vector2[] pos, Vector2 screenSize)
    {
        int sumPos = pos.Length;

        for (int i = 0; i < sumPos; i++)
        {
            Vector3 sub = new Vector3
            (
                ConvScreenCentralReference(pos[i].x, screenSize.x, false),
                ConvScreenCentralReference(pos[i].y, screenSize.y, false),
                0f
            );

            lineRenderer.SetPosition(i, sub);
        }
    }

    public static Color32 GetColor(float valA, float valB, float valC, float valD)
    {
        Color32 res = new Color32();

        if (valD < 0f)
        {
            valA = FixValueRange(valA, 2, 0f, 1f);
            valB = FixValueRange(valB, 2, 0f, 1f);
            valC = FixValueRange(valC, 2, 0f, 1f);

            res = UnityEngine.Color.HSVToRGB
            (
                valA < 0f ? UnityEngine.Random.Range(0f, 1f) : valA,
                valB < 0f ? UnityEngine.Random.Range(0f, 1f) : valB,
                valC < 0f ? UnityEngine.Random.Range(0f, 1f) : valC
            );
        }
        else
        {
            bool isRandom = valC < 0f;

            valA = FixValueRange(valA, 0, 0f, 255f);
            valB = FixValueRange(valB, 0, 0f, 255f);
            valC = FixValueRange(valC, 0, 0f, 255f);
            valD = FixValueRange(valD, 0, 0f, 255f);

            res = new Color32
            (
                isRandom ? (byte)UnityEngine.Random.Range(valA, valB) : (byte)valA,
                isRandom ? (byte)UnityEngine.Random.Range(valA, valB) : (byte)valB,
                isRandom ? (byte)UnityEngine.Random.Range(valA, valB) : (byte)valC,
                (byte)valD
            );
        }

        return res;
    }

    public static float SmoothKeyValueTransition(float valCurrent, bool valPositive, bool valNegative, float dash, float delay)
    {
        float res = Mathf.Lerp
        (
            valCurrent,
            (ConvBoolToFloat(valPositive) - ConvBoolToFloat(valNegative)) * dash,
            delay
        );

        return ClearZeroValue(res, 0);
    }

    public static string GenerateDateTimeString(DateTime now)
    {
        return now.Year.ToString("D4") + "." + now.Month.ToString("D2") + "." + now.Day.ToString("D2") + "-" + now.Hour.ToString("D2") + ":" + now.Minute.ToString("D2") + ":" + now.Second.ToString("D2");
    }

    public static string GenerateRandomString(string charset, int length)
    {
        var str = new System.Text.StringBuilder(length);
        var rnd = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int pos = rnd.Next(charset.Length);
            char sed = charset[pos];

            str.Append(sed);
        }

        return str.ToString();
    }

    public static void SetInitState(string random)
    {
        int hash = 0;

        if (random == "")
        {
            DateTime dt = DateTime.Now;

            hash = dt.Millisecond + dt.Second * 10000 + dt.Minute * 100 * 10000 + dt.Hour * 100 * 100 * 10000;
        }
        else
        {
            var csp = new MD5CryptoServiceProvider();

            var hashBytes = csp.ComputeHash(Encoding.UTF8.GetBytes(random));

            var hashString = new StringBuilder();

            foreach (var hashByte in hashBytes)
            {
                // hashString.Append(hashByte.ToString("x2"));

                hashString.Append(hashByte);
            }

            hash = Convert.ToInt32(hashString.ToString().Substring(0, 9));
        }

        UnityEngine.Random.InitState(hash);
    }

    public static byte[] ReadFile(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fs);

        byte[] res = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return res;
    }

    public static Vector2Int ReadImageResolution(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

        Vector2Int res = Vector2Int.zero;

        using (SD.Image image = SD.Image.FromStream(fs))
        {
            res.x = image.Width;
            res.y = image.Height;
        }

        return res;
    }

    /*

    public static Vector2 ReadPngResolution(byte[] bin)
    {
        int pos = 16;
        int width = 0;
        int height = 0;

        for (int i = 0; i < 4; i++) width = width * 256 + bin[pos++];
        for (int i = 0; i < 4; i++) height = height * 256 + bin[pos++];

        return new Vector2(width, height);
    }

    */

    public static Sprite SetImageSprite(string path)
    {
        byte[] bin = ReadFile(path);

        Vector2Int size = ReadImageResolution(path);
        Texture2D tex = SetImageTexture(bin, size);

        tex.filterMode = FilterMode.Point;

        return Sprite.Create
        (
            tex,
            new Rect(0f, 0f, size.x, size.y),
            new Vector2(0.5f, 0.5f),
            1
        );
    }

    public static Texture2D SetImageTexture(byte[] bin, Vector2Int size)
    {
        Texture2D tex = new Texture2D(size.x, size.y);

        tex.LoadImage(bin);

        return tex;
    }

    public static bool CheckSelectInputField()
    {
        bool res =
        (
            EventSystem.current?.currentSelectedGameObject != null ?
            EventSystem.current?.currentSelectedGameObject?.GetComponent<InputField>() != null || EventSystem.current?.currentSelectedGameObject?.GetComponent<TMP_InputField>() != null :
            false
        );

        return res;
    }

    public static interfaceConfig[] CreateInterfaceConfigs(GameObject[] gos)
    {
        interfaceConfig[] sub = new interfaceConfig[gos.Length];

        for (int i = 0; i < sub.Length; i++)
        {
            sub[i] = new interfaceConfig();

            sub[i].interfaceObject = gos[i];
            sub[i].activeCache = gos[i].activeSelf;
        }

        return sub;
    }

    public static interfaceConfig[] SetInterfaceConfigs(interfaceConfig[] interfaceConfigs, bool isHide)
    {
        interfaceConfig[] sub = interfaceConfigs;

        foreach (interfaceConfig interfaceConfig in sub)
        {
            interfaceConfig.activeCache = isHide ? interfaceConfig.interfaceObject.activeSelf : interfaceConfig.activeCache;
            interfaceConfig.interfaceObject.SetActive(isHide ? false : interfaceConfig.activeCache);
        }

        return sub;
    }
}
