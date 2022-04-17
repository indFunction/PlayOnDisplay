using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using SFB;

public class ImageManager : MonoBehaviour
{
    public UserController UserController;

    [Header("Use Object")]
    [SerializeField] private GameObject imageObject;
    [SerializeField] private GameObject backgroundObject;
    [SerializeField] private Button importDataButton;
    [SerializeField] private Button rejectDataButton;
    [SerializeField] private GameObject[] interfaceObjects;

    [HideInInspector] private UniversalFunction.interfaceConfig[] individualInterfaceConfigs;

    [Header("Graphic Config")]
    [SerializeField] private Vector2 screenSize = new Vector2(1920f, 1080f);

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
            new ExtensionFilter("Image File", "png", "jpg")
        };

        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensionList, true, (string[] paths) =>
        {
            byte[] bin = UniversalFunction.ReadFile(paths[0]);

            SpriteRenderer imageSpriteRenderer = imageObject.gameObject.GetComponent<SpriteRenderer>();
            imageSpriteRenderer.sprite = UniversalFunction.SetImageSprite(paths[0]);

            RectTransform imageRectTransform = imageObject.GetComponent<RectTransform>();
            imageRectTransform.localScale = UniversalFunction.ResizeRectResolution(UniversalFunction.ReadImageResolution(paths[0]), screenSize);

            backgroundObject.SetActive(false);
        });
    }

    public void RejectData()
    {
        SpriteRenderer imageSpriteRenderer = imageObject.gameObject.GetComponent<SpriteRenderer>();
        imageSpriteRenderer.sprite = null;

        backgroundObject.SetActive(true);
    }
}
