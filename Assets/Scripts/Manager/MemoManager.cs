using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using SFB;

public class MemoManager : MonoBehaviour
{
    public UserController UserController;

    [Header("Use Object")]
    [SerializeField] private GameObject memoInputField;
    [SerializeField] private Button importDataButton;
    [SerializeField] private Button saveDataButton;
    [SerializeField] private GameObject[] interfaceObjects;

    [HideInInspector] private UniversalFunction.interfaceConfig[] individualInterfaceConfigs;

    // Unity

    void Awake()
    {
        individualInterfaceConfigs = UniversalFunction.CreateInterfaceConfigs(interfaceObjects);
    }

    void Update()
    {
        if (UserController.updateHideInterface) individualInterfaceConfigs = UniversalFunction.SetInterfaceConfigs(individualInterfaceConfigs, UserController.hideInterface);
    }

    // Custom Function

    public void ImportData()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("Text File", "txt")
        };

        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensionList, true, (string[] paths) =>
        {
            string res = "";

            using (var sr = new StreamReader(paths[0], System.Text.Encoding.GetEncoding("UTF-8")))
            {
                res = sr.ReadToEnd();
            }

            UniversalFunction.SetInputFieldText(memoInputField, res);
        });
    }

    public void SaveData()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("Text File", "txt")
        };

        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "Memo", extensionList, (string path) =>
        {
            string res = UniversalFunction.GetInputFieldText(memoInputField);

            StreamWriter sw = new StreamWriter(@path, false, System.Text.Encoding.GetEncoding("UTF-8"));

            sw.Write(res);

            sw.Flush();
            sw.Close();
        });
    }
}
