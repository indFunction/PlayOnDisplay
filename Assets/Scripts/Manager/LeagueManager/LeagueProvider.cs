using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using SFB;

public class LeagueProvider : MonoBehaviour
{
    public UserController UserController;
    public NotificationController NotificationController;
    public LeagueMaker LeagueMaker;
    public LeaguePainter LeaguePainter;
    public LeagueEditor LeagueEditor;
    public LeagueReader LeagueReader;
    public LeagueWriter LeagueWriter;
    public LeagueExporter LeagueExporter;

    [Header("Layer Object")]
    [SerializeField] private GameObject setupLayer;
    [SerializeField] private GameObject mainLayer;
    [SerializeField] private GameObject playerMenuLayer;
    [SerializeField] private GameObject scoreMenuLayer;
    [SerializeField] private GameObject configMenuLayer;
    [SerializeField] private GameObject[] interfaceObjects;

    [HideInInspector] private UniversalFunction.interfaceConfig[] individualInterfaceConfigs;

    [Header("Container Object")]
    [SerializeField] private GameObject RootObject;
    [SerializeField] private GameObject canvasObject;
    [SerializeField] private GameObject baseLineContainer;
    [SerializeField] private GameObject edgeLineContainer;
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private GameObject scoreContainer;
    [SerializeField] private GameObject textContainer;

    [Header("Original Object")]
    [SerializeField] private GameObject baseLineObject;
    [SerializeField] private GameObject edgeLineObject;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject scoreObject;
    [SerializeField] private GameObject textObject;

    [Header("Setup Object")]
    [SerializeField] private Button createButton;
    [SerializeField] private GameObject sumPlayerInput;
    [SerializeField] private GameObject changeGameModeButton;
    [SerializeField] private GameObject dataStatusText;
    [SerializeField] private Button importDataButton;
    [SerializeField] private Button rejectDataButton;

    [Header("Main Object")]
    [SerializeField] private GameObject gameTitleInput;
    // [SerializeField] private GameObject sumGameText;
    [SerializeField] private GameObject finishGameText;
    [SerializeField] private GameObject remainingGameText;

    [Header("Player Menu Object")]
    [SerializeField] private Button closePlayerMenuButton;
    [SerializeField] private Image playerImage;
    [SerializeField] private GameObject playerNameInput;
    [SerializeField] private GameObject playerColorInput;
    [SerializeField] private GameObject playerNumberText;
    [SerializeField] private GameObject playerScoreText;
    [SerializeField] private GameObject playerRankingText;
    [SerializeField] private Button changePlayerNameButton;
    [SerializeField] private Button changePlayerColorButton;

    [Header("Score Menu Object")]
    [SerializeField] private Button closeScoreMenuButton;
    [SerializeField] private Image playerImageA;
    [SerializeField] private Image playerImageB;
    [SerializeField] private GameObject playerNameTextA;
    [SerializeField] private GameObject playerNameTextB;
    [SerializeField] private GameObject playerNumberTextA;
    [SerializeField] private GameObject playerNumberTextB;
    [SerializeField] private GameObject numberModeLayer;
    [SerializeField] private GameObject booleanModeLayer;
    [SerializeField] private GameObject scoreInputA;
    [SerializeField] private GameObject scoreInputB;
    [SerializeField] private GameObject winnerButtonA;
    [SerializeField] private GameObject winnerButtonB;
    [SerializeField] private Button changeScoreButton;
    [SerializeField] private Button resetScoreButton;

    [Header("Config Menu Object")]
    [SerializeField] private Button closeConfigMenuButton;
    [SerializeField] private GameObject winnerPointInput;
    [SerializeField] private GameObject loserPointInput;
    [SerializeField] private GameObject winPointWeightInput;
    [SerializeField] private GameObject losePointWeightInput;
    [SerializeField] private Button changeGamePointButton;
    [SerializeField] private Button changeGamePointWeightButton;

    [Header("League Config")]
    [SerializeField] private int maxSumPlayer = 25;

    [Header("Graphic Config")]
    [SerializeField] private Vector2 screenSize = new Vector2(1920f - 120f, 1080f - 120f);

    [HideInInspector] private string loadFilePath = "";
    [HideInInspector] private int loadFileType = 0;
    [HideInInspector] private int sumPlayer = 0;
    [HideInInspector] private bool isBooleanMode = false;
    [HideInInspector] private int selectLayerNum = -1;
    [HideInInspector] private int selectPlayerId = -1;
    [HideInInspector] private int selectScoreId = -1;

    [HideInInspector] public leagueData individualLeagueData;

    public class leagueData
    {
        public int isUpdate = 0;
        public string hash = "";
        public string title = "";
        public bool isBooleanMode = false;
        public int winnerPoint = 0;
        public int loserPoint = 0;
        public int winPointWeight = 0;
        public int losePointWeight = 0;
        public Color32 normalScoreColor = UniversalFunction.GetColor(255f, 255f, 255f, 0f);
        public Color32 winnerScoreColor = UniversalFunction.GetColor(0f, 255f, 0f, 128f);
        public Color32 loserScoreColor = UniversalFunction.GetColor(255f, 0f, 0f, 128f);
        public Color32 drawScoreColor = UniversalFunction.GetColor(255f, 255f, 0f, 128f);
        public stageRoot[] stageRoots;
        public stageTable[,] stageTables;
        public gridPos[,] stageGridPoses;
        public gridPos[,] statisticGridPoses;
    }

    public class stageRoot
    {
        public GameObject[] playerObjects;
        public GameObject[] statisticObjects;
        public string playerName;
        public Color32 playerColor;
        public float tablePos = 0f;
        public int winPoint = 0;
        public int losePoint = 0;
        public int resultPoint = 0;
        public int ranking = 0;
    }

    public class stageTable
    {
        public GameObject scoreObject;
        public int winner = -1;
        public int scoreA = -1;
        public int scoreB = -1;
    }

    public class gridPos
    {
        public Vector2 startPos = Vector2.zero;
        public Vector2 centerPos = Vector2.zero;
        public Vector2 endPos = Vector2.zero;
    }

    [HideInInspector] private leagueCounter individualLeagueCounter;

    public class leagueCounter
    {
        public int sumGame = 0;
        public int finishGame = 0;
        public int remainingGame = 0;
        public int[] nextGamePos = new int[2];
    }

    [HideInInspector] private GameObject[] individualLeagueLayerObjects;

    [HideInInspector] private leagueContainerObject individualLeagueContainerObjects;

    public class leagueContainerObject
    {
        public GameObject canvasObject;
        public GameObject baseLineContainer;
        public GameObject edgeLineContainer;
        public GameObject playerContainer;
        public GameObject scoreContainer;
        public GameObject textContainer;
    }

    [HideInInspector] private leagueOriginalObject individualLeagueOriginalObjects;

    public class leagueOriginalObject
    {
        public GameObject baseLineObject;
        public GameObject edgeLineObject;
        public GameObject playerObject;
        public GameObject scoreObject;
        public GameObject textObject;
    }

    [HideInInspector] public leaguePlayerMenuObject individualLeaguePlayerMenuObjects;

    public class leaguePlayerMenuObject
    {
        public Button closePlayerMenuButton;
        public Image playerImage;
        public GameObject playerNameInput;
        public GameObject playerColorInput;
        public GameObject playerNumberText;
        public GameObject playerScoreText;
        public GameObject playerRankingText;
        public Button changePlayerNameButton;
        public Button changePlayerColorButton;
    }

    [HideInInspector] public leagueScoreMenuObject individualLeagueScoreMenuObjects;

    public class leagueScoreMenuObject
    {
        public Button closeScoreMenuButton;
        public Image playerImageA;
        public Image playerImageB;
        public GameObject playerNameTextA;
        public GameObject playerNameTextB;
        public GameObject playerNumberTextA;
        public GameObject playerNumberTextB;
        public GameObject numberModeLayer;
        public GameObject booleanModeLayer;
        public GameObject scoreInputA;
        public GameObject scoreInputB;
        public GameObject winnerButtonA;
        public GameObject winnerButtonB;
        public Button changeScoreButton;
        public Button resetScoreButton;
    }

    [HideInInspector] public leagueConfigMenuObject individualLeagueConfigMenuObjects;

    public class leagueConfigMenuObject
    {
        public Button closeConfigMenuButton;
        public GameObject winnerPointInput;
        public GameObject loserPointInput;
        public GameObject winPointWeightInput;
        public GameObject losePointWeightInput;
        public Button changeGamePointButton;
        public Button changeGamePointWeightButton;
    }

    // Unity

    void Awake()
    {
        individualInterfaceConfigs = UniversalFunction.CreateInterfaceConfigs(interfaceObjects);

        individualLeagueLayerObjects = new GameObject[]
        {
            setupLayer,
            mainLayer,
            playerMenuLayer,
            scoreMenuLayer,
            configMenuLayer
        };

        individualLeagueContainerObjects = SetLeagueContainerObjects
        (
            canvasObject,
            baseLineContainer,
            edgeLineContainer,
            playerContainer,
            scoreContainer,
            textContainer
        );

        individualLeagueOriginalObjects = SetLeagueOriginalObjects
        (
            baseLineObject,
            edgeLineObject,
            playerObject,
            scoreObject,
            textObject
        );

        individualLeaguePlayerMenuObjects = SetLeaguePlayerMenuObjects
        (
            closePlayerMenuButton,
            playerImage,
            playerNameInput,
            playerColorInput,
            playerNumberText,
            playerScoreText,
            playerRankingText,
            changePlayerNameButton,
            changePlayerColorButton
        );

        individualLeagueScoreMenuObjects = SetLeagueScoreMenuObjects
        (
            closeScoreMenuButton,
            playerImageA,
            playerImageB,
            playerNameTextA,
            playerNameTextB,
            playerNumberTextA,
            playerNumberTextB,
            numberModeLayer,
            booleanModeLayer,
            scoreInputA,
            scoreInputB,
            winnerButtonA,
            winnerButtonB,
            changeScoreButton,
            resetScoreButton
        );

        individualLeagueConfigMenuObjects = SetLeagueConfigMenuObjects
        (
            closeConfigMenuButton,
            winnerPointInput,
            loserPointInput,
            winPointWeightInput,
            losePointWeightInput,
            changeGamePointButton,
            changeGamePointWeightButton
        );

        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 0, -1);
    }

    void Update()
    {
        if (selectLayerNum > 0) UpdateMainScreen();
    }

    // Custom Function

    void UpdateMainScreen()
    {
        if (individualLeagueData.isUpdate != 0)
        {
            switch (individualLeagueData.isUpdate)
            {
                case 1:
                    LeaguePainter.UpdateLeague
                    (
                        individualLeagueContainerObjects,
                        individualLeagueOriginalObjects,
                        individualLeagueData,
                        false,
                        screenSize,
                        this
                    );
                    break;
                case 2:
                    individualLeagueData = LeaguePainter.GenerateInitialLeague
                    (
                        individualLeagueContainerObjects,
                        individualLeagueOriginalObjects,
                        individualLeagueData,
                        gameTitleInput,
                        false,
                        screenSize,
                        this
                    );
                    break;
            }

            individualLeagueData.isUpdate = 0;

            individualLeagueCounter = GetLeagueCounter(individualLeagueData);

            SetLeagueInformation(individualLeagueCounter);

            // Debug.Log(LeaguePainter.DebugLeagueData(individualLeagueData));

            // Debug.Log(LeaguePainter.DebugLeagueCounter(individualLeagueCounter));
        }

        bool isInvalidInput = UniversalFunction.CheckSelectInputField() || NotificationController.isActive != 0 || RootObject != UserController.GetCurrentDisplay();

        // if (!isInvalidInput && selectLayerNum == 1 && Keyboard.current.fKey.wasPressedThisFrame) UserController.targetTranslation = FocusLeagueGame(individualLeagueData, individualLeagueCounter, UserController.limitDisplaySizeB);

        if (UserController.updateDisplayTargetNum || (!isInvalidInput && Keyboard.current.escapeKey.wasPressedThisFrame))
        {
            CloseScoreMenu();
            ClosePlayerMenu();
            CloseConfigMenu();
        }

        if (UserController.updateHideInterface) individualInterfaceConfigs = UniversalFunction.SetInterfaceConfigs(individualInterfaceConfigs, UserController.hideInterface);
    }

    public void CreateLeague()
    {
        string sumPlayerStringBuffer = UniversalFunction.GetInputFieldText(sumPlayerInput);

        int sumPlayerIntBuffer = sumPlayerStringBuffer != "" ? int.Parse(sumPlayerStringBuffer) : 10;

        if (sumPlayerIntBuffer != UniversalFunction.FixValueRange(sumPlayerIntBuffer, 0, 2, maxSumPlayer))
        {
            Button[] go = NotificationController.SetErrorNotification("参加人数を1人以下、もしくは" + (maxSumPlayer + 1).ToString() + "人以上にすることはできません！");

            // Debug.Log("The number of players cannot be less than 1 or more than" + (maxSumPlayer + 1).ToString() + "!");

            return;
        }

        sumPlayer = sumPlayerIntBuffer;

        leagueData leagueData = null;

        switch (loadFileType)
        {
            case 0:
                leagueData = LeagueMaker.SetInitialLeagueData(sumPlayer, isBooleanMode);
                break;
            case 1:
                leagueData = LeagueReader.ImportList(loadFilePath, maxSumPlayer, isBooleanMode, LeagueMaker);
                break;
            case 2:
                leagueData = LeagueReader.ImportData(loadFilePath, LeagueMaker);
                break;
        }

        if (loadFileType == 0 || leagueData != null)
        {
            individualLeagueData = leagueData;

            individualLeagueData.stageRoots = LeagueEditor.SetLeagueStatistic(individualLeagueData, -1);
            individualLeagueData.stageRoots = LeagueEditor.SetLeagueRanking(individualLeagueData.stageRoots);
        }
        else
        {
            return;
        }

        individualLeagueData = LeaguePainter.GenerateInitialLeague
        (
            individualLeagueContainerObjects,
            individualLeagueOriginalObjects,
            individualLeagueData,
            gameTitleInput,
            loadFileType == 2 ? false : true,
            screenSize,
            this
        );

        // Debug.Log(LeaguePainter.DebugLeagueData(individualLeagueData));

        // Debug.Log(LeaguePainter.DebugLeagueCounter(individualLeagueCounter));

        individualLeagueData.isUpdate = 3;

        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 1, -1);
    }

    public void OpenPlayerMenu(int id)
    {
        selectPlayerId = LeaguePainter.SetPlayerMenu
        (
            individualLeaguePlayerMenuObjects,
            individualLeagueData,
            id,
            this,
            LeagueEditor
        );

        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 1, -1);
        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 2, 1);
    }

    public void ClosePlayerMenu()
    {
        int dummy = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 2, 2);

        selectLayerNum = 1;
    }

    public void OpenScoreMenu(int id)
    {
        selectScoreId = LeaguePainter.SetScoreMenu
        (
            individualLeagueScoreMenuObjects,
            individualLeagueData,
            id,
            this,
            LeagueEditor
        );

        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 1, -1);
        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 3, 1);
    }

    public void CloseScoreMenu()
    {
        int dummy = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 3, 2);

        selectLayerNum = 1;
    }

    public void OpenConfigMenu()
    {
        LeaguePainter.SetConfigMenu(individualLeagueConfigMenuObjects, individualLeagueData);

        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 1, -1);
        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 4, 1);
    }

    public void CloseConfigMenu()
    {
        int dummy = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 4, 2);

        selectLayerNum = 1;
    }

    void SetLeagueInformation(leagueCounter leagueCounter)
    {
        if (leagueCounter == null) return;

        // UniversalFunction.SetText(sumGameText, individualLeagueCounter.sumGame.ToString());
        UniversalFunction.SetText(finishGameText, individualLeagueCounter.finishGame.ToString());
        UniversalFunction.SetText(remainingGameText, individualLeagueCounter.remainingGame.ToString());
    }

    public void ImportFilePath()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("Some File", "txt", "csv"),
            new ExtensionFilter("Player List", "txt"),
            new ExtensionFilter("League Data", "csv")
        };

        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensionList, true, (string[] paths) =>
        {
            if (paths[0].EndsWith("txt"))
            {
                loadFilePath = paths[0];
                loadFileType = 1;

                UniversalFunction.SetInteractableInputField(sumPlayerInput, false);

                UniversalFunction.SetText(dataStatusText, "選択内容：リスト書");
            }
            else if (paths[0].EndsWith("csv"))
            {
                loadFilePath = paths[0];
                loadFileType = 2;

                UniversalFunction.SetInteractableInputField(sumPlayerInput, false);

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

        UniversalFunction.SetText(dataStatusText, "選択内容：初期状態");
    }

    public void ImportDataPath()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("League Data", "csv")
        };

        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensionList, true, (string[] paths) =>
        {
            loadFilePath = paths[0];
            loadFileType = 2;

            UniversalFunction.SetText(dataStatusText, "選択内容：データ表");

            individualLeagueData = LeagueReader.ImportData(loadFilePath, LeagueMaker);

            individualLeagueData = LeaguePainter.GenerateInitialLeague
            (
                individualLeagueContainerObjects,
                individualLeagueOriginalObjects,
                individualLeagueData,
                gameTitleInput,
                false,
                screenSize,
                this
            );

            selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 1, -1);
        });
    }

    public void SaveDataPath()
    {
        var extensionList = new[]
        {
            new ExtensionFilter("League Data", "csv")
        };

        StandaloneFileBrowser.SaveFilePanelAsync("Save File", "", "Data", extensionList, (string path) =>
        {
            individualLeagueData.title = UniversalFunction.GetInputFieldText(gameTitleInput);

            LeagueWriter.SaveData(individualLeagueData, path);
        });
    }

    public void ChangeGameMode()
    {
        isBooleanMode = !isBooleanMode;

        UniversalFunction.SetButtonText(changeGameModeButton, isBooleanMode ? "勝敗計算" : "得点計算");
    }

    public void ChangeGamePoint()
    {
        LeagueEditor.ChangeGamePoint();
    }

    public void ChangeGamePointWeight()
    {
        LeagueEditor.ChangeGamePointWeight();
    }

    public void ReadyResetAllResults()
    {
        CloseScoreMenu();
        ClosePlayerMenu();
        CloseConfigMenu();

        Button[] go = NotificationController.SetWarningNotification("全ての勝敗を初期化しますか？");

        // Debug.Log("Do you want to initialize all results?");

        go[0].onClick.AddListener(ResetAllResults);
    }

    public void ResetAllResults()
    {
        LeagueEditor.ResetAllResults();
    }

    public void ReadyApplicationAndChangeGameMode()
    {
        CloseScoreMenu();
        ClosePlayerMenu();
        CloseConfigMenu();

        Button[] go = NotificationController.SetWarningNotification("得点方式を変更しますか？");

        // Debug.Log("Do you want to change the game mode?");

        go[0].onClick.AddListener(ApplicationAndChangeGameMode);
    }

    public void ApplicationAndChangeGameMode()
    {
        individualLeagueData.isBooleanMode = !individualLeagueData.isBooleanMode;

        LeagueEditor.FixStageTable();
    }

    public void ExportGameOrder()
    {
        individualLeagueData.title = UniversalFunction.GetInputFieldText(gameTitleInput);

        LeagueExporter.ExportGameOrder(individualLeagueData);
    }

    public void ReadyExitGame()
    {
        CloseScoreMenu();
        ClosePlayerMenu();
        CloseConfigMenu();

        Button[] go = NotificationController.SetWarningNotification("試合を終了しますか？");

        // Debug.Log("Do you want to exit league?");

        go[0].onClick.AddListener(ExitGame);
    }

    public void ExitGame()
    {
        RejectFilePath();

        selectLayerNum = LeaguePainter.ChangeLayer(individualLeagueLayerObjects, 0, -1);

        LeaguePainter.ClearContainer(individualLeagueContainerObjects);

        individualLeagueData = null;
    }

    // Specific Function

    leagueContainerObject SetLeagueContainerObjects
    (
        GameObject canvasObject,
        GameObject baseLineContainer,
        GameObject edgeLineContainer,
        GameObject playerContainer,
        GameObject scoreContainer,
        GameObject textContainer
    )
    {
        leagueContainerObject leagueContainerObjects = new leagueContainerObject();

        leagueContainerObjects.canvasObject = canvasObject;
        leagueContainerObjects.baseLineContainer = baseLineContainer;
        leagueContainerObjects.edgeLineContainer = edgeLineContainer;
        leagueContainerObjects.playerContainer = playerContainer;
        leagueContainerObjects.scoreContainer = scoreContainer;
        leagueContainerObjects.textContainer = textContainer;

        return leagueContainerObjects;
    }

    leagueOriginalObject SetLeagueOriginalObjects
    (
        GameObject baseLineObject,
        GameObject edgeLineObject,
        GameObject playerObject,
        GameObject scoreObject,
        GameObject textObject
    )
    {
        leagueOriginalObject leagueOriginalObjects = new leagueOriginalObject();

        leagueOriginalObjects.baseLineObject = baseLineObject;
        leagueOriginalObjects.edgeLineObject = edgeLineObject;
        leagueOriginalObjects.playerObject = playerObject;
        leagueOriginalObjects.scoreObject = scoreObject;
        leagueOriginalObjects.textObject = textObject;

        return leagueOriginalObjects;
    }

    leaguePlayerMenuObject SetLeaguePlayerMenuObjects
    (
        Button closePlayerMenuButton,
        Image playerImage,
        GameObject playerNameInput,
        GameObject playerColorInput,
        GameObject playerNumberText,
        GameObject playerScoreText,
        GameObject playerRankingText,
        Button changePlayerNameButton,
        Button changePlayerColorButton
    )
    {
        leaguePlayerMenuObject leaguePlayerMenuObjects = new leaguePlayerMenuObject();

        leaguePlayerMenuObjects.closePlayerMenuButton = closePlayerMenuButton;
        leaguePlayerMenuObjects.playerImage = playerImage;
        leaguePlayerMenuObjects.playerNameInput = playerNameInput;
        leaguePlayerMenuObjects.playerColorInput = playerColorInput;
        leaguePlayerMenuObjects.playerNumberText = playerNumberText;
        leaguePlayerMenuObjects.playerScoreText = playerScoreText;
        leaguePlayerMenuObjects.playerRankingText = playerRankingText;
        leaguePlayerMenuObjects.changePlayerNameButton = changePlayerNameButton;
        leaguePlayerMenuObjects.changePlayerColorButton = changePlayerColorButton;

        return leaguePlayerMenuObjects;
    }

    leagueScoreMenuObject SetLeagueScoreMenuObjects
    (
        Button closeScoreMenuButton,
        Image playerImageA,
        Image playerImageB,
        GameObject playerNameTextA,
        GameObject playerNameTextB,
        GameObject playerNumberTextA,
        GameObject playerNumberTextB,
        GameObject numberModeLayer,
        GameObject booleanModeLayer,
        GameObject scoreInputA,
        GameObject scoreInputB,
        GameObject winnerButtonA,
        GameObject winnerButtonB,
        Button changeScoreButton,
        Button resetScoreButton
    )
    {
        leagueScoreMenuObject leagueScoreMenuObjects = new leagueScoreMenuObject();

        leagueScoreMenuObjects.closeScoreMenuButton = closeScoreMenuButton;
        leagueScoreMenuObjects.playerImageA = playerImageA;
        leagueScoreMenuObjects.playerImageB = playerImageB;
        leagueScoreMenuObjects.playerNameTextA = playerNameTextA;
        leagueScoreMenuObjects.playerNameTextB = playerNameTextB;
        leagueScoreMenuObjects.playerNumberTextA = playerNumberTextA;
        leagueScoreMenuObjects.playerNumberTextB = playerNumberTextB;
        leagueScoreMenuObjects.numberModeLayer = numberModeLayer;
        leagueScoreMenuObjects.booleanModeLayer = booleanModeLayer;
        leagueScoreMenuObjects.scoreInputA = scoreInputA;
        leagueScoreMenuObjects.scoreInputB = scoreInputB;
        leagueScoreMenuObjects.winnerButtonA = winnerButtonA;
        leagueScoreMenuObjects.winnerButtonB = winnerButtonB;
        leagueScoreMenuObjects.changeScoreButton = changeScoreButton;
        leagueScoreMenuObjects.resetScoreButton = resetScoreButton;

        return leagueScoreMenuObjects;
    }

    leagueConfigMenuObject SetLeagueConfigMenuObjects
    (
        Button closeConfigMenuButton,
        GameObject winnerPointInput,
        GameObject loserPointInput,
        GameObject winPointWeightInput,
        GameObject losePointWeightInput,
        Button changeGamePointButton,
        Button changeGamePointWeightButton
    )
    {
        leagueConfigMenuObject leagueConfigMenuObjects = new leagueConfigMenuObject();

        leagueConfigMenuObjects.closeConfigMenuButton = closeConfigMenuButton;
        leagueConfigMenuObjects.winnerPointInput = winnerPointInput;
        leagueConfigMenuObjects.loserPointInput = loserPointInput;
        leagueConfigMenuObjects.winPointWeightInput = winPointWeightInput;
        leagueConfigMenuObjects.losePointWeightInput = losePointWeightInput;
        leagueConfigMenuObjects.changeGamePointButton = changeGamePointButton;
        leagueConfigMenuObjects.changeGamePointWeightButton = changeGamePointWeightButton;

        return leagueConfigMenuObjects;
    }

    leagueCounter GetLeagueCounter(leagueData leagueData)
    {
        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        int sumPeople = stageRoots.Length;

        leagueCounter sub = new leagueCounter();

        sub.nextGamePos[0] = -1;
        sub.nextGamePos[1] = -1;

        for (int y = 0; y < sumPeople; y++)
        {
            for (int x = y + 1; x < sumPeople; x++)
            {
                sub.sumGame++;

                if (stageTables[y, x].winner != -1)
                {
                    sub.finishGame++;
                }
                else
                {
                    sub.remainingGame++;

                    if (sub.nextGamePos[0] == -1 && sub.nextGamePos[1] == -1)
                    {
                        sub.nextGamePos[0] = y;
                        sub.nextGamePos[1] = x;
                    }
                }
            }
        }

        return sub;
    }

    Vector2 FocusLeagueGame(leagueData leagueData, leagueCounter leagueCounter, Vector2 limitDisplaySize)
    {
        if (leagueData == null || leagueCounter == null) return Vector2.zero;

        int posY = leagueCounter.nextGamePos[0] + 1;
        int posX = leagueCounter.nextGamePos[1] + 1;

        Vector2 buf = leagueData.stageGridPoses[posY, posX].centerPos;

        Vector2 res = new Vector2
        (
            limitDisplaySize.x * (buf.x - 0.5f) * 2f,
            limitDisplaySize.y * (buf.y - 0.5f) * 2f
        );

        return res;
    }
}
