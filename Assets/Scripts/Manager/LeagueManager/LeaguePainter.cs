using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaguePainter : MonoBehaviour
{
    // Usable Function

    public LeagueProvider.leagueData GenerateInitialLeague
    (
        LeagueProvider.leagueContainerObject leagueContainerObjects,
        LeagueProvider.leagueOriginalObject leagueOriginalObjects,
        LeagueProvider.leagueData leagueData,
        GameObject gameTitleText,
        bool setColor,
        int criteriaScale,
        Vector2 screenSize,
        LeagueProvider LeagueProvider
    )
    {
        GameObject canvasObject = leagueContainerObjects.canvasObject;
        GameObject baseLineContainer = leagueContainerObjects.baseLineContainer;
        GameObject edgeLineContainer = leagueContainerObjects.edgeLineContainer;
        GameObject baseLineObject = leagueOriginalObjects.baseLineObject;
        GameObject edgeLineObject = leagueOriginalObjects.edgeLineObject;

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;

        ClearContainer(leagueContainerObjects);

        int sumPeople = stageRoots.Length;

        if (sumPeople <= 1) return null;

        UniversalFunction.SetInputFieldText(gameTitleText, leagueData.title);

        LeagueProvider.leagueData sub = leagueData;

        sub.stageGridPoses = SetTableGrid
        (
            canvasObject,
            baseLineContainer,
            baseLineObject,
            edgeLineContainer,
            edgeLineObject,
            sumPeople + 1,
            sumPeople + 1,
            new Vector2(0.5f, 0.5f),
            new Vector2(screenSize.y, screenSize.y),
            screenSize
        );

        SetLineRenderer
        (
            canvasObject,
            edgeLineContainer,
            edgeLineObject,
            sub.stageGridPoses[0, 0].startPos,
            sub.stageGridPoses[sumPeople, sumPeople].endPos,
            screenSize
        );

        sub.statisticGridPoses = SetTableGrid
        (
            canvasObject,
            baseLineContainer,
            baseLineObject,
            edgeLineContainer,
            edgeLineObject,
            4,
            sumPeople + 1,
            new Vector2(1f, 0.5f),
            new Vector2(420f, screenSize.y),
            screenSize
        );

        sub = UpdateLeague
        (
            leagueContainerObjects,
            leagueOriginalObjects,
            sub,
            setColor,
            criteriaScale,
            screenSize,
            LeagueProvider
        );

        return sub;
    }

    public LeagueProvider.leagueData UpdateLeague
    (
        LeagueProvider.leagueContainerObject leagueContainerObjects,
        LeagueProvider.leagueOriginalObject leagueOriginalObjects,
        LeagueProvider.leagueData leagueData,
        bool setColor,
        int criteriaScale,
        Vector2 screenSize,
        LeagueProvider LeagueProvider
    )
    {
        GameObject canvasObject = leagueContainerObjects.canvasObject;
        GameObject playerContainer = leagueContainerObjects.playerContainer;
        GameObject scoreContainer = leagueContainerObjects.scoreContainer;
        GameObject textContainer = leagueContainerObjects.scoreContainer;
        GameObject playerObject = leagueOriginalObjects.playerObject;
        GameObject scoreObject = leagueOriginalObjects.scoreObject;
        GameObject textObject = leagueOriginalObjects.textObject;

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;

        UniversalFunction.DestroyChildObject(playerContainer);
        UniversalFunction.DestroyChildObject(scoreContainer);
        UniversalFunction.DestroyChildObject(textContainer);

        int sumPeople = stageRoots.Length;

        if (sumPeople <= 1) return null;

        LeagueProvider.leagueData sub = leagueData;

        float objectScale = sumPeople < criteriaScale ? (float)(criteriaScale * 2f - sumPeople) / criteriaScale : 1f;

        sub = SetEveryPlayerObject(canvasObject, playerContainer, playerObject, leagueData, setColor ? 2 : 0, objectScale, screenSize, LeagueProvider);
        sub = SetEveryScoreObject(canvasObject, scoreContainer, scoreObject, leagueData, objectScale, screenSize, LeagueProvider);
        sub = SetEveryStatisticObject(canvasObject, textContainer, textObject, leagueData, screenSize);

        return sub;
    }

    public string DebugLeagueData(LeagueProvider.leagueData leagueData)
    {
        if (leagueData == null) return "League Data does not exist!";

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        string log = "";

        int sumPeople = stageRoots.Length;

        for (int x = 0; x < sumPeople; x++)
        {
            if (x != 0 && x < sumPeople) log += ", \n";

            log +=
            (
                "{ ID: " +
                x.ToString() + ", " +
                "PlayerName: " +
                stageRoots[x].playerName + ", " +
                "tablePos: " +
                stageRoots[x].tablePos.ToString("F2") + " }"
            );
        }

        log += "\n\nTable: \n[ ";

        for (int y = 0; y < sumPeople; y++)
        {
            for (int x = 0; x < sumPeople; x++)
            {
                if (y != x)
                {
                    int scoreA = stageTables[y, x].scoreA;
                    int scoreB = stageTables[y, x].scoreB;

                    if (scoreA >= 0 && scoreB >= 0)
                    {
                        log +=
                        (
                            "{ " +
                            scoreA.ToString() + " - " +
                            scoreB.ToString() + " }"
                        );
                    }
                    else
                    {
                        log += "{ N }";
                    }
                }
                else
                {
                    log += "{ * }";
                }

                log += ", ";
            }

            log +=
            (
                "{ WinPoint: " +
                stageRoots[y].winPoint + " }, { " +
                "LosePoint: " +
                stageRoots[y].losePoint + " }, { " +
                "ResultPoint: " +
                stageRoots[y].resultPoint + " } }"
            );

            log += y < sumPeople - 1 ? ",\n" : " ]\n";
        }

        return log;
    }

    public string DebugLeagueCounter(LeagueProvider.leagueCounter leagueCounter)
    {
        if (leagueCounter == null) return "League Counter does not exist!";

        string log = "";

        log +=
        (
            "sumGame: " + leagueCounter.sumGame + ",\n" +
            "finishGame: " + leagueCounter.finishGame + ",\n" +
            "remainingGame: " + leagueCounter.remainingGame + ",\n" +
            "nextGamePos: { " + leagueCounter.nextGamePos[0] + ", " + leagueCounter.nextGamePos[1] + " }"
        );

        return log;
    }

    public int ChangeLayer(GameObject[] leagueLayerObjects, int num, int showLayer)
    {
        if (showLayer < -1 || showLayer > 2) showLayer = 0;

        if (showLayer == -1)
        {
            for (int i = 0; i < leagueLayerObjects.Length; i++)
            {
                leagueLayerObjects[i].SetActive(i == num ? true : false);
            }
        }
        else
        {
            leagueLayerObjects[num].SetActive
            (
                showLayer == 0 ? !leagueLayerObjects[num].activeSelf : showLayer == 1 ? true : false
            );
        }

        return num;
    }

    public void ClearContainer(LeagueProvider.leagueContainerObject leagueContainerObjects)
    {
        GameObject baseLineContainer = leagueContainerObjects.baseLineContainer;
        GameObject edgeLineContainer = leagueContainerObjects.edgeLineContainer;
        GameObject playerContainer = leagueContainerObjects.playerContainer;
        GameObject scoreContainer = leagueContainerObjects.scoreContainer;
        GameObject textContainer = leagueContainerObjects.textContainer;

        UniversalFunction.DestroyChildObject(baseLineContainer);
        UniversalFunction.DestroyChildObject(edgeLineContainer);
        UniversalFunction.DestroyChildObject(playerContainer);
        UniversalFunction.DestroyChildObject(scoreContainer);
        UniversalFunction.DestroyChildObject(textContainer);
    }

    public int SetPlayerMenu
    (
        LeagueProvider.leaguePlayerMenuObject leaguePlayerMenuObjects,
        LeagueProvider.leagueData leagueData,
        int id,
        LeagueProvider LeagueProvider,
        LeagueEditor LeagueEditor
    )
    {
        Image playerImage = leaguePlayerMenuObjects.playerImage;
        GameObject playerNameInput = leaguePlayerMenuObjects.playerNameInput;
        GameObject playerColorInput = leaguePlayerMenuObjects.playerColorInput;
        GameObject playerNumberText = leaguePlayerMenuObjects.playerNumberText;
        GameObject playerScoreText = leaguePlayerMenuObjects.playerScoreText;
        GameObject playerRankingText = leaguePlayerMenuObjects.playerRankingText;
        Button changePlayerNameButton = leaguePlayerMenuObjects.changePlayerNameButton;
        Button changePlayerColorButton = leaguePlayerMenuObjects.changePlayerColorButton;

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;

        int sumPeople = stageRoots.Length;

        Color32 playerColor = stageRoots[id].playerColor;

        playerImage.color = playerColor;
        UniversalFunction.SetText(playerNumberText, 0, UniversalFunction.AlignDigitsToMaxNum(id, sumPeople), playerColor);
        UniversalFunction.SetText(playerScoreText, stageRoots[id].resultPoint + "点");
        UniversalFunction.SetText(playerRankingText, UniversalFunction.AlignDigitsToMaxNum(stageRoots[id].ranking, sumPeople) + "位");

        UniversalFunction.SetInputFieldText(playerNameInput, stageRoots[id].playerName);
        UniversalFunction.SetInputFieldText(playerColorInput, ColorUtility.ToHtmlStringRGB(playerColor));

        if (changePlayerNameButton != null)
        {
            changePlayerNameButton.onClick.RemoveAllListeners();
            changePlayerNameButton.onClick.AddListener(() => { LeagueEditor.ChangePlayerName(id); });
        }

        if (changePlayerColorButton != null)
        {
            changePlayerColorButton.onClick.RemoveAllListeners();
            changePlayerColorButton.onClick.AddListener(() => { LeagueEditor.ChangePlayerColor(id); });
        }

        return id;
    }

    public int SetScoreMenu
    (
        LeagueProvider.leagueScoreMenuObject leagueScoreMenuObjects,
        LeagueProvider.leagueData leagueData,
        int id,
        LeagueProvider LeagueProvider,
        LeagueEditor LeagueEditor
    )
    {
        Image playerImageA = leagueScoreMenuObjects.playerImageA;
        Image playerImageB = leagueScoreMenuObjects.playerImageB;
        GameObject playerNameTextA = leagueScoreMenuObjects.playerNameTextA;
        GameObject playerNameTextB = leagueScoreMenuObjects.playerNameTextB;
        GameObject playerNumberTextA = leagueScoreMenuObjects.playerNumberTextA;
        GameObject playerNumberTextB = leagueScoreMenuObjects.playerNumberTextB;
        GameObject numberModeLayer = leagueScoreMenuObjects.numberModeLayer;
        GameObject booleanModeLayer = leagueScoreMenuObjects.booleanModeLayer;
        GameObject scoreInputA = leagueScoreMenuObjects.scoreInputA;
        GameObject scoreInputB = leagueScoreMenuObjects.scoreInputB;
        GameObject winnerButtonA = leagueScoreMenuObjects.winnerButtonA;
        GameObject winnerButtonB = leagueScoreMenuObjects.winnerButtonB;
        Button changeScoreButton = leagueScoreMenuObjects.changeScoreButton;
        Button resetScoreButton = leagueScoreMenuObjects.resetScoreButton;

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        bool isBooleanMode = leagueData.isBooleanMode;

        numberModeLayer.SetActive(!isBooleanMode);
        booleanModeLayer.SetActive(isBooleanMode);

        int sumPeople = stageRoots.Length;

        int posY = id / sumPeople;
        int posX = id % sumPeople;

        Color32 playerColorA = stageRoots[posY].playerColor;
        Color32 playerColorB = stageRoots[posX].playerColor;

        UniversalFunction.SetText(playerNameTextA, 0, stageRoots[posY].playerName, playerColorA);
        playerImageA.color = playerColorA;
        UniversalFunction.SetText(playerNumberTextA, 0, UniversalFunction.AlignDigitsToMaxNum(posY, sumPeople), playerColorA);

        UniversalFunction.SetText(playerNameTextB, 0, stageRoots[posX].playerName, playerColorB);
        playerImageB.color = playerColorB;
        UniversalFunction.SetText(playerNumberTextB, 0, UniversalFunction.AlignDigitsToMaxNum(posX, sumPeople), playerColorB);

        int scoreA = stageTables[posY, posX].scoreA;
        int scoreB = stageTables[posY, posX].scoreB;

        if (isBooleanMode)
        {
            int winner = stageTables[posY, posX].winner;

            if (winnerButtonA != null)
            {
                if (winner != -1)
                {
                    UniversalFunction.SetButtonText(winnerButtonA, winner == 1 ? "勝者" : "敗者");
                }
                else
                {
                    UniversalFunction.SetButtonText(winnerButtonA, "前者の勝利");
                }

                Button button = winnerButtonA.GetComponent<Button>();
                if (button != null)
                {
                    UniversalFunction.SetInteractableButton(button, winner != 1);

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => { LeagueEditor.WinnerA(id); });
                    button.onClick.AddListener(LeagueProvider.CloseScoreMenu);
                }
            }

            if (winnerButtonB != null)
            {
                if (winner != -1)
                {
                    UniversalFunction.SetButtonText(winnerButtonB, winner == 2 ? "勝者" : "敗者");
                }
                else
                {
                    UniversalFunction.SetButtonText(winnerButtonB, "後者の勝利");
                }

                Button button = winnerButtonB.GetComponent<Button>();
                if (button != null)
                {
                    UniversalFunction.SetInteractableButton(button, winner != 2);

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => { LeagueEditor.WinnerB(id); });
                    button.onClick.AddListener(LeagueProvider.CloseScoreMenu);
                }
            }

            if (resetScoreButton != null)
            {
                resetScoreButton.onClick.RemoveAllListeners();
                resetScoreButton.onClick.AddListener(() => { LeagueEditor.ResetScore(id); });
                resetScoreButton.onClick.AddListener(LeagueProvider.CloseScoreMenu);
            }
        }
        else
        {
            UniversalFunction.SetInputFieldText(scoreInputA, scoreA < 0 ? "" : scoreA.ToString());
            UniversalFunction.SetInputFieldText(scoreInputB, scoreB < 0 ? "" : scoreB.ToString());

            if (changeScoreButton != null)
            {
                changeScoreButton.onClick.RemoveAllListeners();
                changeScoreButton.onClick.AddListener(() => { LeagueEditor.ChangeScore(id); });
                changeScoreButton.onClick.AddListener(LeagueProvider.CloseScoreMenu);
            }
        }

        return id;
    }

    public void SetConfigMenu
    (
        LeagueProvider.leagueConfigMenuObject leagueConfigMenuObjects,
        LeagueProvider.leagueData leagueData
    )
    {
        GameObject winnerPointInput = leagueConfigMenuObjects.winnerPointInput;
        GameObject loserPointInput = leagueConfigMenuObjects.loserPointInput;
        GameObject winPointWeightInput = leagueConfigMenuObjects.winPointWeightInput;
        GameObject losePointWeightInput = leagueConfigMenuObjects.losePointWeightInput;

        UniversalFunction.SetInputFieldText(winnerPointInput, leagueData.winnerPoint.ToString());
        UniversalFunction.SetInputFieldText(loserPointInput, leagueData.loserPoint.ToString());
        UniversalFunction.SetInputFieldText(winPointWeightInput, leagueData.winPointWeight.ToString());
        UniversalFunction.SetInputFieldText(losePointWeightInput, leagueData.losePointWeight.ToString());
    }

    public void UpdateScore
    (
        LeagueProvider.leagueData leagueData,
        LeagueProvider.stageTable stageTable,
        GameObject scoreObject
    )
    {
        UniversalFunction.SetButtonText
        (
            scoreObject,
            leagueData.isBooleanMode ?
            (
                stageTable.winner == 1 ?
                    "勝" : stageTable.winner == 2 ?
                        "負" : "未"
            )
            :
            (
                stageTable.winner != -1 ? stageTable.scoreA.ToString() + "-" + stageTable.scoreB.ToString() : "N"
            )
        );

        UniversalFunction.SetButtonColor
        (
            scoreObject.GetComponent<Button>(),
            0,
            stageTable.winner == 0 ?
                leagueData.drawScoreColor : stageTable.winner == 1 ?
                    leagueData.winnerScoreColor : stageTable.winner == 2 ?
                        leagueData.loserScoreColor : leagueData.normalScoreColor
        );
    }

    public void UpdateStatistic
    (
        LeagueProvider.stageRoot stageRoot,
        GameObject[] statisticObjects,
        int mode
    )
    {
        if (mode < 0 || mode > 3) mode = 3;
        if (mode == 3) return;

        if (mode == 0 || mode == 1)
        {
            UniversalFunction.SetText(statisticObjects[0], stageRoot.winPoint.ToString());
            UniversalFunction.SetText(statisticObjects[1], stageRoot.losePoint.ToString());
            UniversalFunction.SetText(statisticObjects[2], stageRoot.resultPoint.ToString());
        }

        if (mode == 0 || mode == 2)
        {
            UniversalFunction.SetText(statisticObjects[3], stageRoot.ranking.ToString());
        }
    }

    // Specific Function

    LeagueProvider.leagueData SetEveryPlayerObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject playerObject,
        LeagueProvider.leagueData leagueData,
        int setColor,
        float objectScale,
        Vector2 screenSize,
        LeagueProvider LeagueProvider
    )
    {
        LeagueProvider.leagueData sub = leagueData;

        LeagueProvider.stageRoot[] stageRoots = sub.stageRoots;
        LeagueProvider.gridPos[,] stageGridPoses = sub.stageGridPoses;

        int sumPeople = stageRoots.Length;

        for (int i = 0; i < sumPeople; i++)
        {
            switch (setColor)
            {
                case 1:
                    stageRoots[i].playerColor = UniversalFunction.GetColor(128f, 255f, -1f, 255f);
                    break;
                case 2:
                    stageRoots[i].playerColor = UniversalFunction.GetColor((float)i / (float)sumPeople, 0.5f, 1f, -1f);
                    break;
            }

            Vector2[] pos = { stageGridPoses[i + 1, 0].centerPos, stageGridPoses[0, i + 1].centerPos };

            stageRoots[i].playerObjects = SetPlayerObject
            (
                canvasObject,
                containerObject,
                playerObject,
                i,
                pos,
                stageRoots[i].playerName,
                stageRoots[i].playerColor,
                objectScale,
                screenSize,
                LeagueProvider
            );
        }

        sub.stageRoots = stageRoots;

        return sub;
    }

    LeagueProvider.leagueData SetEveryScoreObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject scoreObject,
        LeagueProvider.leagueData leagueData,
        float objectScale,
        Vector2 screenSize,
        LeagueProvider LeagueProvider
    )
    {
        LeagueProvider.leagueData sub = leagueData;

        LeagueProvider.stageRoot[] stageRoots = sub.stageRoots;
        LeagueProvider.stageTable[,] stageTables = sub.stageTables;
        LeagueProvider.gridPos[,] stageGridPoses = sub.stageGridPoses;

        int sumPeople = stageRoots.Length;

        for (int y = 0; y < sumPeople; y++)
        {
            for (int x = 0; x < sumPeople; x++)
            {
                if (x == y) continue;

                stageTables[y, x].scoreObject = SetScoreObject
                (
                    canvasObject,
                    containerObject,
                    scoreObject,
                    y * sumPeople + x,
                    leagueData,
                    stageTables[y, x],
                    stageGridPoses[y + 1, x + 1].centerPos,
                    objectScale,
                    screenSize,
                    LeagueProvider
                );
            }
        }

        sub.stageRoots = stageRoots;

        return sub;
    }

    LeagueProvider.leagueData SetEveryStatisticObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject statisticObject,
        LeagueProvider.leagueData leagueData,
        Vector2 screenSize
    )
    {
        LeagueProvider.leagueData sub = leagueData;

        LeagueProvider.stageRoot[] stageRoots = sub.stageRoots;
        LeagueProvider.gridPos[,] statisticGridPoses = sub.statisticGridPoses;

        GameObject[] dummy = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            string text = "";

            switch (i)
            {
                case 0:
                    text = "勝点";
                    break;
                case 1:
                    text = "敗点";
                    break;
                case 2:
                    text = "合計";
                    break;
                case 3:
                    text = "順位";
                    break;
            }

            dummy[i] = SetStatisticObject
            (
                canvasObject,
                containerObject,
                statisticObject,
                text,
                statisticGridPoses[0, i].centerPos,
                screenSize
            );
        }

        int sumPeople = stageRoots.Length;

        for (int i = 0; i < sumPeople; i++)
        {
            Vector2[] pos =
            {
                statisticGridPoses[i + 1, 0].centerPos,
                statisticGridPoses[i + 1, 1].centerPos,
                statisticGridPoses[i + 1, 2].centerPos,
                statisticGridPoses[i + 1, 3].centerPos
            };

            stageRoots[i].statisticObjects = SetStatisticObject
            (
                canvasObject,
                containerObject,
                statisticObject,
                stageRoots[i],
                pos,
                screenSize
            );
        }

        sub.stageRoots = stageRoots;

        return sub;
    }

    LeagueProvider.gridPos[,] SetTableGrid
    (
        GameObject canvasObject,
        GameObject baseContainerObject,
        GameObject baseLineObject,
        GameObject edgeContainerObject,
        GameObject edgeLineObject,
        int sizeX,
        int sizeY,
        Vector2 setPos,
        Vector2 setSize,
        Vector2 screenSize
    )
    {
        LeagueProvider.gridPos[,] res = new LeagueProvider.gridPos[sizeY, sizeX];

        Vector2 fixSetPos = new Vector2(setPos.x - 0.5f, setPos.y - 0.5f);
        Vector2 newSetPosA = (Vector2.one - setSize / screenSize) * fixSetPos;
        Vector2 newSetPosB = (screenSize / setSize - Vector2.one) * fixSetPos;

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                int reverseY = sizeY - 1 - y;

                res[reverseY, x] = new LeagueProvider.gridPos();

                Vector2 fixSize = (screenSize - setSize) / screenSize;

                Vector2 startPos = newSetPosA + fixSize / 2f + new Vector2((float)x / (float)sizeX, (float)y / (float)sizeY) * (Vector2.one - fixSize);
                Vector2 endPos = newSetPosA + fixSize / 2f + new Vector2((float)(x + 1) / (float)sizeX, (float)(y + 1) / (float)sizeY) * (Vector2.one - fixSize);

                res[reverseY, x].startPos = new Vector2(startPos.x, endPos.y);
                res[reverseY, x].endPos = new Vector2(endPos.x, startPos.y);
                res[reverseY, x].centerPos = startPos + (endPos - startPos) / 2f;
            }
        }

        for (int y = 0; y <= sizeY; y++)
        {
            float posY = (float)y / (float)sizeY;

            SetLineRenderer
            (
                canvasObject,
                baseContainerObject,
                baseLineObject,
                newSetPosB + new Vector2(0f, posY),
                newSetPosB + new Vector2(1f, posY),
                screenSize * (setSize / screenSize)
            );

            if (y == 0 || y == sizeY)
            {
                SetLineRenderer
                (
                    canvasObject,
                    edgeContainerObject,
                    edgeLineObject,
                    newSetPosB + new Vector2(0f, posY),
                    newSetPosB + new Vector2(1f, posY),
                    screenSize * (setSize / screenSize)
                );
            }
        }

        for (int x = 0; x <= sizeX; x++)
        {
            float posX = (float)x / (float)sizeX;

            SetLineRenderer
            (
                canvasObject,
                baseContainerObject,
                baseLineObject,
                newSetPosB + new Vector2(posX, 0f),
                newSetPosB + new Vector2(posX, 1f),
                screenSize * (setSize / screenSize)
            );

            if (x == 0 || x == sizeX)
            {
                SetLineRenderer
                (
                    canvasObject,
                    edgeContainerObject,
                    edgeLineObject,
                    newSetPosB + new Vector2(posX, 0f),
                    newSetPosB + new Vector2(posX, 1f),
                    screenSize * (setSize / screenSize)
                );
            }
        }

        return res;
    }

    void SetLineRenderer
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject lineObject,
        Vector2 startPos,
        Vector2 endPos,
        Vector2 screenSize
    )
    {
        GameObject cloneLineObject = UniversalFunction.SetCloneObject(lineObject, containerObject);

        UniversalFunction.ConnectRectParent(lineObject, cloneLineObject);

        LineRenderer cloneLineRenderer = cloneLineObject.GetComponent<LineRenderer>();

        UniversalFunction.MoveLineRenderer(cloneLineRenderer, startPos, endPos, screenSize);
    }

    GameObject[] SetPlayerObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject playerObject,
        int id,
        Vector2[] pos,
        string name,
        Color32 playerColor,
        float objectScale,
        Vector2 screenSize,
        LeagueProvider LeagueProvider
    )
    {
        GameObject[] res = new GameObject[2];

        for (int i = 0; i < 2; i++)
        {
            GameObject clonePlayerObject = UniversalFunction.SetCloneObject(playerObject, containerObject);

            UniversalFunction.ConnectRectParent(playerObject, clonePlayerObject);

            MoveTableObject(clonePlayerObject, pos[i], screenSize);

            clonePlayerObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

            Button clonePlayerButton = clonePlayerObject.GetComponent<Button>();
            clonePlayerButton.onClick.AddListener(() => { LeagueProvider.OpenPlayerMenu(id); });

            Image clonePlayerImage = clonePlayerObject.GetComponent<Image>();
            clonePlayerImage.color = playerColor;

            UniversalFunction.SetButtonText(clonePlayerObject, 0, name.ToString(), playerColor);

            res[i] = clonePlayerObject;
        }

        return res;
    }

    GameObject SetScoreObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject scoreObject,
        int id,
        LeagueProvider.leagueData leagueData,
        LeagueProvider.stageTable stageTable,
        Vector2 pos,
        float objectScale,
        Vector2 screenSize,
        LeagueProvider LeagueProvider
    )
    {
        GameObject cloneScoreObject = UniversalFunction.SetCloneObject(scoreObject, containerObject);

        UniversalFunction.ConnectRectParent(scoreObject, cloneScoreObject);

        MoveTableObject(cloneScoreObject, pos, screenSize);

        cloneScoreObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

        Button cloneScoreButton = cloneScoreObject.GetComponent<Button>();
        cloneScoreButton.onClick.AddListener(() => { LeagueProvider.OpenScoreMenu(id); });

        UpdateScore(leagueData, stageTable, cloneScoreObject);

        return cloneScoreObject;
    }

    GameObject SetStatisticObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject statisticObject,
        string text,
        Vector2 pos,
        Vector2 screenSize
    )
    {
        GameObject cloneStatisticObject = UniversalFunction.SetCloneObject(statisticObject, containerObject);

        UniversalFunction.ConnectRectParent(statisticObject, cloneStatisticObject);

        MoveTableObject(cloneStatisticObject, pos, screenSize);

        UniversalFunction.SetText(cloneStatisticObject, text);

        return cloneStatisticObject;
    }

    GameObject[] SetStatisticObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject statisticObject,
        LeagueProvider.stageRoot stageRoot,
        Vector2[] pos,
        Vector2 screenSize
    )
    {
        GameObject[] res = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject cloneStatisticObject = UniversalFunction.SetCloneObject(statisticObject, containerObject);

            UniversalFunction.ConnectRectParent(statisticObject, cloneStatisticObject);

            MoveTableObject(cloneStatisticObject, pos[i], screenSize);

            res[i] = cloneStatisticObject;
        }

        UpdateStatistic(stageRoot, res, 0);

        return res;
    }

    void MoveTableObject(GameObject playerObject, Vector2 pos, Vector2 screenSize)
    {
        playerObject.GetComponent<RectTransform>().anchoredPosition = new Vector3
        (
            UniversalFunction.ConvScreenCentralReference(pos.x, screenSize.x, false),
            UniversalFunction.ConvScreenCentralReference(pos.y, screenSize.y, false),
            0f
        );
    }
}
