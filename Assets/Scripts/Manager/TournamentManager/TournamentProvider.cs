using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using SFB;

public class TournamentProvider : MonoBehaviour
{
    public UserController UserController;
    public NotificationController NotificationController;
    public TournamentMaker TournamentMaker;
    public TournamentPainter TournamentPainter;
    public TournamentEditor TournamentEditor;
    public TournamentReader TournamentReader;
    public TournamentWriter TournamentWriter;

    [Header("Layer Object")]
    [SerializeField] private GameObject setupLayer;
    [SerializeField] private GameObject mainLayer;
    [SerializeField] private GameObject menuLayer;
    [SerializeField] private GameObject[] interfaceObjects;

    [HideInInspector] private UniversalFunction.interfaceConfig[] individualInterfaceConfigs;

    [Header("Container Object")]
    [SerializeField] private GameObject RootObject;
    [SerializeField] private GameObject canvasObject;
    [SerializeField] private GameObject baseLineContainer;
    [SerializeField] private GameObject resultLineContainer;
    [SerializeField] private GameObject playerContainer;

    [Header("Original Object")]
    [SerializeField] private GameObject baseLineObject;
    [SerializeField] private GameObject resultLineObject;
    [SerializeField] private GameObject playerObject;

    [Header("Setup Object")]
    [SerializeField] private Button createButton;
    [SerializeField] private GameObject sumPlayerInput;
    [SerializeField] private GameObject numGroupInput;
    [SerializeField] private GameObject dataStatusText;
    [SerializeField] private Button importDataButton;
    [SerializeField] private Button rejectDataButton;

    [Header("Main Object")]
    [SerializeField] private Button importDataSubButton;
    [SerializeField] private Button saveDataButton;
    [SerializeField] private Button exitGameButton;
    // [SerializeField] private GameObject sumGameText;
    [SerializeField] private GameObject finishGameText;
    [SerializeField] private GameObject remainingGameText;

    [Header("Menu Object")]
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private Image playerImage;
    [SerializeField] private GameObject playerNameInput;
    [SerializeField] private GameObject playerColorInput;
    [SerializeField] private GameObject playerNumberText;
    [SerializeField] private Button changePlayerNameButton;
    [SerializeField] private Button changePlayerColorButton;
    [SerializeField] private Button winActionButton;
    [SerializeField] private Button cancelActionButton;

    [Header("Tournament Config")]
    [SerializeField] private int maxSumPlayer = 100;

    [Header("Graphic Config")]
    [SerializeField] private float smoothTimes = 5f;
    [SerializeField] private Vector2 screenSize = new Vector2(1920f - 120f, 1080f - 120f);

    [HideInInspector] private string loadFilePath = "";
    [HideInInspector] private int loadFileType = 0;
    [HideInInspector] private int sumPlayer = 0;
    [HideInInspector] private int numGroup = 0;
    [HideInInspector] private int selectLayerNum = -1;
    [HideInInspector] private int selectPlayerId = -1;

    [HideInInspector] public tournamentData individualTournamentData;

    public class tournamentData
    {
        public int isUpdate = 0;
        public bool allowWalkover = false;
        public int defaultNumGroup = 0;
        public stageRoot[] stageRoots;
        public stageCollider[,] stageColliders;
        public stageProperty[] stageProperties;
    }

    public class stageRoot
    {
        public GameObject playerObject;
        public string playerName;
        public Color32 playerColor;
        public GameObject branchObject;
        public Vector2 startDrawRatioBranchPos = Vector2.zero;
        public Vector2 endDrawRatioBranchPos = Vector2.zero;
        public Vector2 currentPos = Vector2.zero;

        public stageRoot Clone()
        {
            return (stageRoot)MemberwiseClone();
        }
    }

    public class stageCollider
    {
        public int type = 0;
        public int[] connectBranch;
        public int winner = -1;
        public GameObject baseBranchObject;
        public GameObject resultBranchObject;
        public Vector2 startDrawRatioBranchPos = Vector2.zero;
        public Vector2 endDrawRatioBranchPos = Vector2.zero;
        public GameObject baseJointObject;
        public GameObject resultJointObject;
        public Vector2 startDrawRatioJointPos = Vector2.zero;
        public Vector2 endDrawRatioJointPos = Vector2.zero;
    }

    public class stageProperty
    {
        public bool reverseOrder = false;
        public int incompleteGroup = -1;
        public int numGroup = -1;
        public int sumGame = -1;
        public int sumBranch = -1;
    }

    [HideInInspector] private tournamentCounter individualTournamentCounter;

    public class tournamentCounter
    {
        public int sumGame = 0;
        public int finishGame = 0;
        public int remainingGame = 0;
        public int[] nextGamePos = new int[2];
    }

    [HideInInspector] private GameObject[] individualTournamentLayerObjects;

    [HideInInspector] private tournamentContainerObject individualTournamentContainerObjects;

    public class tournamentContainerObject
    {
        public GameObject canvasObject;
        public GameObject baseLineContainer;
        public GameObject resultLineContainer;
        public GameObject playerContainer;
    }

    [HideInInspector] private tournamentOriginalObject individualTournamentOriginalObjects;

    public class tournamentOriginalObject
    {
        public GameObject baseLineObject;
        public GameObject resultLineObject;
        public GameObject playerObject;
    }

    [HideInInspector] public tournamentMenuObject individualTournamentMenuObjects;

    public class tournamentMenuObject
    {
        public Button closeMenuButton;
        public Image playerImage;
        public GameObject playerNameInput;
        public GameObject playerColorInput;
        public GameObject playerNumberText;
        public Button changePlayerNameButton;
        public Button changePlayerColorButton;
        public Button winActionButton;
        public Button cancelActionButton;
    }

    // Unity

    void Awake()
    {
        individualInterfaceConfigs = UniversalFunction.CreateInterfaceConfigs(interfaceObjects);

        individualTournamentLayerObjects = new GameObject[]
        {
            setupLayer,
            mainLayer,
            menuLayer
        };

        individualTournamentContainerObjects = SetTournamentContainerObjects
        (
            canvasObject,
            baseLineContainer,
            resultLineContainer,
            playerContainer
        );

        individualTournamentOriginalObjects = SetTournamentOriginalObjects
        (
            baseLineObject,
            resultLineObject,
            playerObject
        );

        individualTournamentMenuObjects = SetTournamentMenuObjects
        (
            playerImage,
            closeMenuButton,
            playerNameInput,
            playerColorInput,
            playerNumberText,
            changePlayerNameButton,
            changePlayerColorButton,
            winActionButton,
            cancelActionButton
        );

        selectLayerNum = TournamentPainter.ChangeLayer(individualTournamentLayerObjects, 0, -1);
    }

    void Update()
    {
        if (selectLayerNum > 0) UpdateMainScreen();
    }

    // Custom Function

    void UpdateMainScreen()
    {
        if (individualTournamentData.isUpdate != 0)
        {
            switch (individualTournamentData.isUpdate)
            {
                case 1:
                    TournamentPainter.UpdateTournament(individualTournamentData, 2, screenSize);
                    break;
                case 2:
                    individualTournamentData = TournamentPainter.GenerareInitialTournament
                    (
                        individualTournamentContainerObjects,
                        individualTournamentOriginalObjects,
                        individualTournamentData,
                        false,
                        screenSize,
                        this
                    );
                    break;
            }

            individualTournamentData.isUpdate = 0;

            individualTournamentCounter = GetTournamentCounter(individualTournamentData);

            SetTournamentInformation(individualTournamentCounter);

            // Debug.Log(TournamentPainter.DebugTournamentData(individualTournamentData));

            // Debug.Log(TournamentPainter.DebugTournamentCounter(individualTournamentCounter));
        }

        TournamentPainter.SmoothUpdateTournament(individualTournamentData, smoothTimes, screenSize);

        bool isInvalidInput = UniversalFunction.CheckSelectInputField() || NotificationController.isActive != 0 || RootObject != UserController.GetCurrentDisplay();

        if (!isInvalidInput && selectLayerNum == 1 && Keyboard.current.fKey.wasPressedThisFrame) UserController.targetTranslation = FocusTournamentGame(individualTournamentData, individualTournamentCounter, UserController.limitDisplaySizeB);

        if (UserController.updateDisplayTargetNum || (!isInvalidInput && Keyboard.current.escapeKey.wasPressedThisFrame)) CloseMenu();

        if (UserController.updateHideInterface) individualInterfaceConfigs = UniversalFunction.SetInterfaceConfigs(individualInterfaceConfigs, UserController.hideInterface);
    }

    public void CreateTournament()
    {
        string sumPlayerStringBuffer = UniversalFunction.GetInputFieldText(sumPlayerInput);
        string numGroupStringBuffer = UniversalFunction.GetInputFieldText(numGroupInput);

        int sumPlayerIntBuffer = sumPlayerStringBuffer != "" ? int.Parse(sumPlayerStringBuffer) : 50;
        int numGroupIntBuffer = numGroupStringBuffer != "" ? int.Parse(numGroupStringBuffer) : 2;

        if (sumPlayerIntBuffer != UniversalFunction.FixValueRange(sumPlayerIntBuffer, 0, 2, maxSumPlayer))
        {
            Button[] go = NotificationController.SetErrorNotification("参加人数を1人以下、もしくは" + (maxSumPlayer + 1).ToString() + "人以上にすることはできません！");

            // Debug.Log("The number of players cannot be less than 1 or more than" + (maxSumPlayer + 1).ToString() + "!");

            return;
        }

        if (numGroupIntBuffer != UniversalFunction.FixValueRange(numGroupIntBuffer, 0, 2, maxSumPlayer))
        {
            Button[] go = NotificationController.SetErrorNotification("対戦人数を1人以下、もしくは" + (maxSumPlayer + 1).ToString() + "人以上にすることはできません！");

            // Debug.Log("The number of opponents cannot be less than 1 or more than" + (maxSumPlayer + 1).ToString() + "!");

            return;
        }

        sumPlayer = sumPlayerIntBuffer;
        numGroup = numGroupIntBuffer;

        tournamentData tournamentData = null;

        switch (loadFileType)
        {
            case 0:
                tournamentData = TournamentMaker.SetInitialTournamentData(sumPlayer, numGroup);
                break;
            case 1:
                tournamentData = TournamentReader.ImportList(loadFilePath, numGroup, maxSumPlayer, TournamentMaker);
                break;
            case 2:
                tournamentData = TournamentReader.ImportData(loadFilePath, TournamentMaker);
                break;
        }

        if (loadFileType == 0 || tournamentData != null)
        {
            individualTournamentData = tournamentData;
        }
        else
        {
            return;
        }

        individualTournamentData = TournamentPainter.GenerareInitialTournament
        (
            individualTournamentContainerObjects,
            individualTournamentOriginalObjects,
            individualTournamentData,
            loadFileType == 2 ? false : true,
            screenSize,
            this
        );

        // Debug.Log(TournamentPainter.DebugTournamentData(individualTournamentData));

        // Debug.Log(TournamentPainter.DebugTournamentCounter(individualTournamentCounter));

        individualTournamentData.isUpdate = 2;

        selectLayerNum = TournamentPainter.ChangeLayer(individualTournamentLayerObjects, 1, -1);
    }

    public void OpenMenu(int id)
    {
        selectPlayerId = TournamentPainter.SetMenu
        (
            individualTournamentMenuObjects,
            individualTournamentData,
            id,
            this,
            TournamentEditor
        );

        selectLayerNum = TournamentPainter.ChangeLayer(individualTournamentLayerObjects, 2, 1);
    }

    public void CloseMenu()
    {
        int dummy = TournamentPainter.ChangeLayer(individualTournamentLayerObjects, 2, 2);

        selectLayerNum = 1;
    }

    void SetTournamentInformation(tournamentCounter tournamentCounter)
    {
        if (tournamentCounter == null) return;

        // UniversalFunction.SetText(sumGameText, individualTournamentCounter.sumGame.ToString());
        UniversalFunction.SetText(finishGameText, individualTournamentCounter.finishGame.ToString());
        UniversalFunction.SetText(remainingGameText, individualTournamentCounter.remainingGame.ToString());
    }

    public void ImportFilePath()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("Some File", "txt", "csv"),
            new ExtensionFilter("Player List", "txt"),
            new ExtensionFilter("Tournament Data", "csv")
        };

        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensionList, true, (string[] paths) =>
        {
            if (paths[0].EndsWith("txt"))
            {
                loadFilePath = paths[0];
                loadFileType = 1;

                UniversalFunction.SetInteractableInputField(sumPlayerInput, false);
                UniversalFunction.SetInteractableInputField(numGroupInput, true);

                UniversalFunction.SetText(dataStatusText, "選択内容：リスト書");
            }
            else if (paths[0].EndsWith("csv"))
            {
                loadFilePath = paths[0];
                loadFileType = 2;

                UniversalFunction.SetInteractableInputField(sumPlayerInput, false);
                UniversalFunction.SetInteractableInputField(numGroupInput, false);

                UniversalFunction.SetText(dataStatusText, "選択内容：データ表");
            }
            else
            {
                RejectFilePath();
            }
        });
    }

    public void RejectFilePath()
    {
        loadFilePath = "";
        loadFileType = 0;

        UniversalFunction.SetInteractableInputField(sumPlayerInput, true);
        UniversalFunction.SetInteractableInputField(numGroupInput, true);

        UniversalFunction.SetText(dataStatusText, "選択内容：初期状態");
    }

    public void ImportDataPath()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("Tournament Data", "csv")
        };

        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensionList, true, (string[] paths) =>
        {
            loadFilePath = paths[0];
            loadFileType = 2;

            UniversalFunction.SetText(dataStatusText, "選択内容：データ表");

            individualTournamentData = TournamentReader.ImportData(loadFilePath, TournamentMaker);

            individualTournamentData = TournamentPainter.GenerareInitialTournament
            (
                individualTournamentContainerObjects,
                individualTournamentOriginalObjects,
                individualTournamentData,
                false,
                screenSize,
                this
            );

            selectLayerNum = TournamentPainter.ChangeLayer(individualTournamentLayerObjects, 1, -1);
        });
    }

    public void SaveDataPath()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("Tournament Data", "csv")
        };

        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "Data", extensionList, (string path) =>
        {
            TournamentWriter.SaveData(individualTournamentData, path);
        });
    }

    public void ReadyResetAllResults()
    {
        Button[] go = NotificationController.SetWarningNotification("全ての勝敗を初期化しますか？");

        // Debug.Log("Do you want to initialize all results?");

        go[0].onClick.AddListener(() => { ResetAllResults(); });
    }

    public void ResetAllResults()
    {
        TournamentEditor.ResetAllResults();
    }

    public void ReadyShufflePlayer()
    {
        Button[] go = NotificationController.SetWarningNotification("プレイヤーをシャッフルしますか？");

        // Debug.Log("Do you want to shuffle all players?");

        go[0].onClick.AddListener(() => { ShufflePlayer(); });
    }

    public void ShufflePlayer()
    {
        TournamentEditor.ShufflePlayer();
    }

    public void ReadyExitGame()
    {
        Button[] go = NotificationController.SetWarningNotification("試合を終了しますか？");

        // Debug.Log("Do you want to exit tournament?");

        go[0].onClick.AddListener(() => { ExitGame(); });
    }

    public void ExitGame()
    {
        RejectFilePath();

        selectLayerNum = TournamentPainter.ChangeLayer(individualTournamentLayerObjects, 0, -1);

        TournamentPainter.ClearContainer(individualTournamentContainerObjects);

        individualTournamentData = null;
    }

    // Specific Function

    tournamentContainerObject SetTournamentContainerObjects
    (
        GameObject canvasObject,
        GameObject baseLineContainer,
        GameObject resultLineContainer,
        GameObject playerContainer
    )
    {
        tournamentContainerObject tournamentContainerObjects = new tournamentContainerObject();

        tournamentContainerObjects.canvasObject = canvasObject;
        tournamentContainerObjects.baseLineContainer = baseLineContainer;
        tournamentContainerObjects.resultLineContainer = resultLineContainer;
        tournamentContainerObjects.playerContainer = playerContainer;

        return tournamentContainerObjects;
    }

    tournamentOriginalObject SetTournamentOriginalObjects
    (
        GameObject baseLineObject,
        GameObject resultLineObject,
        GameObject playerObject
    )
    {
        tournamentOriginalObject tournamentOriginalObjects = new tournamentOriginalObject();

        tournamentOriginalObjects.baseLineObject = baseLineObject;
        tournamentOriginalObjects.resultLineObject = resultLineObject;
        tournamentOriginalObjects.playerObject = playerObject;

        return tournamentOriginalObjects;
    }

    tournamentMenuObject SetTournamentMenuObjects
    (
        Image playerImage,
        Button closeMenuButton,
        GameObject playerNameInput,
        GameObject playerColorInput,
        GameObject playerNumberText,
        Button changePlayerNameButton,
        Button changePlayerColorButton,
        Button winActionButton,
        Button cancelActionButton
    )
    {
        tournamentMenuObject tournamentMenuObjects = new tournamentMenuObject();

        tournamentMenuObjects.playerImage = playerImage;
        tournamentMenuObjects.closeMenuButton = closeMenuButton;
        tournamentMenuObjects.playerNameInput = playerNameInput;
        tournamentMenuObjects.playerColorInput = playerColorInput;
        tournamentMenuObjects.playerNumberText = playerNumberText;
        tournamentMenuObjects.changePlayerNameButton = changePlayerNameButton;
        tournamentMenuObjects.changePlayerColorButton = changePlayerColorButton;
        tournamentMenuObjects.winActionButton = winActionButton;
        tournamentMenuObjects.cancelActionButton = cancelActionButton;

        return tournamentMenuObjects;
    }

    tournamentCounter GetTournamentCounter(tournamentData tournamentData)
    {
        if (tournamentData == null) return null;

        stageCollider[,] stageColliders = tournamentData.stageColliders;
        stageProperty[] stageProperties = tournamentData.stageProperties;

        int stageHeight = stageColliders.GetLength(0);

        tournamentCounter tournamentCounter = new tournamentCounter();

        tournamentCounter.nextGamePos[0] = -1;
        tournamentCounter.nextGamePos[1] = -1;

        for (int y = 0; y < stageHeight; y++)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                if (stageColliders[y, x].type != 1)
                {
                    tournamentCounter.sumGame++;

                    if (stageColliders[y, x].winner != -1)
                    {
                        tournamentCounter.finishGame++;
                    }
                    else
                    {
                        tournamentCounter.remainingGame++;

                        int[] nextGamePos = tournamentCounter.nextGamePos;

                        if (nextGamePos[0] == -1 && nextGamePos[1] == -1)
                        {
                            tournamentCounter.nextGamePos[0] = y;
                            tournamentCounter.nextGamePos[1] = x;
                        }
                    }
                }
            }
        }

        return tournamentCounter;
    }

    Vector2 FocusTournamentGame(tournamentData tournamentData, tournamentCounter tournamentCounter, Vector2 limitDisplaySize)
    {
        if (tournamentData == null || tournamentCounter == null) return Vector2.zero;

        int posY = tournamentCounter.nextGamePos[0];
        int posX = tournamentCounter.nextGamePos[1];

        Vector2 buf = tournamentData.stageColliders[posY, posX].startDrawRatioBranchPos;

        Vector2 res = new Vector2
        (
            limitDisplaySize.x * (buf.x - 0.5f) * 2f,
            limitDisplaySize.y * (buf.y - 0.5f) * 2f
        );

        return res;
    }
}
