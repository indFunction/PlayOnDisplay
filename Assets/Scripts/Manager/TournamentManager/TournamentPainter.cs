using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentPainter : MonoBehaviour
{
    public class stageRootDrawRatioPos
    {
        public Vector2 startBranchPos = Vector2.zero;
        public Vector2 endBranchPos = Vector2.zero;
    }

    public class stageColliderDrawRatioPos
    {
        public Vector2 startBranchPos = Vector2.zero;
        public Vector2 endBranchPos = Vector2.zero;
        public Vector2 startJointPos = Vector2.zero;
        public Vector2 endJointPos = Vector2.zero;
    }

    public class colliderCache
    {
        public int id = -1;
        public Vector2 pos = Vector2.zero;
    }

    // Usable Function

    public TournamentProvider.tournamentData GenerareInitialTournament
    (
        TournamentProvider.tournamentContainerObject tournamentContainerObjects,
        TournamentProvider.tournamentOriginalObject tournamentOriginalObjects,
        TournamentProvider.tournamentData tournamentData,
        GameObject gameTitleText,
        bool setColor,
        Vector2 screenSize,
        TournamentProvider TournamentProvider
    )
    {
        GameObject canvasObject = tournamentContainerObjects.canvasObject;
        GameObject baseLineContainer = tournamentContainerObjects.baseLineContainer;
        GameObject resultLineContainer = tournamentContainerObjects.resultLineContainer;
        GameObject playerContainer = tournamentContainerObjects.playerContainer;
        GameObject baseLineObject = tournamentOriginalObjects.baseLineObject;
        GameObject resultLineObject = tournamentOriginalObjects.resultLineObject;
        GameObject playerObject = tournamentOriginalObjects.playerObject;

        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;

        ClearContainer(tournamentContainerObjects);

        int sumPeople = stageRoots.Length;

        if (sumPeople <= 1) return null;

        UniversalFunction.SetInputFieldText(gameTitleText, tournamentData.title);

        TournamentProvider.tournamentData sub = tournamentData;

        sub = SetEveryBaseLineRenderer(canvasObject, baseLineContainer, baseLineObject, tournamentData, screenSize);
        sub = SetEveryResultLineRenderer(canvasObject, resultLineContainer, resultLineObject, tournamentData, screenSize);
        sub = SetEveryPlayerObject(canvasObject, playerContainer, playerObject, tournamentData, setColor ? 2 : 0, screenSize, TournamentProvider);

        UpdateTournament(sub, 0, screenSize);

        return sub;
    }

    public void UpdateTournament
    (
        TournamentProvider.tournamentData tournamentData,
        int mode,
        Vector2 screenSize
    )
    {
        if (mode < 0 || mode > 2) mode = 0;

        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        int sumPeople = stageRoots.Length;

        int stageHeight = stageColliders.GetLength(0);

        if (sumPeople <= 1) return;

        for (int y = 0; y < stageHeight; y++)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                Vector2 startBranchPos = OutputLinePos(stageRoots, stageColliders, 1, y, x);
                Vector2 endBranchPos = OutputLinePos(stageRoots, stageColliders, 2, y, x);
                Vector2 startJointPos = OutputLinePos(stageRoots, stageColliders, 3, y, x);
                Vector2 endJointPos = OutputLinePos(stageRoots, stageColliders, 4, y, x);

                if (mode == 0 || mode == 1)
                {
                    MoveLineRenderer(stageColliders[y, x].resultBranchObject.GetComponent<LineRenderer>(), startBranchPos, endBranchPos, screenSize);
                    MoveLineRenderer(stageColliders[y, x].resultJointObject.GetComponent<LineRenderer>(), startJointPos, endJointPos, screenSize);
                }

                if (mode == 0 || mode == 2)
                {
                    if (startBranchPos == endBranchPos && stageColliders[y, x].resultBranchObject.activeSelf) stageColliders[y, x].resultBranchObject.SetActive(false);
                    if (startJointPos == endJointPos && stageColliders[y, x].resultJointObject.activeSelf) stageColliders[y, x].resultJointObject.SetActive(false);
                }
            }
        }

        if (mode == 0 || mode == 1)
        {
            for (int x = 0; x < sumPeople; x++)
            {
                stageRoots[x].currentPos = GetPlayerCurrentPos(tournamentData, x);

                MovePlayerObject
                (
                    stageRoots[x].playerObject,
                    stageRoots[x].currentPos,
                    screenSize
                );
            }
        }
    }

    public void SmoothUpdateTournament
    (
        TournamentProvider.tournamentData tournamentData,
        float smoothTimes,
        Vector2 screenSize
    )
    {
        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        int sumPeople = stageRoots.Length;

        int stageHeight = stageColliders.GetLength(0);

        if (sumPeople <= 1) return;

        for (int y = 0; y < stageHeight; y++)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                Vector2 startBranchCurrentPos = UniversalFunction.GetLineRendererPos(stageColliders[y, x].resultBranchObject.GetComponent<LineRenderer>(), 0, screenSize);
                Vector2 endBranchCurrentPos = UniversalFunction.GetLineRendererPos(stageColliders[y, x].resultBranchObject.GetComponent<LineRenderer>(), 1, screenSize);
                Vector2 startJointCurrentPos = UniversalFunction.GetLineRendererPos(stageColliders[y, x].resultJointObject.GetComponent<LineRenderer>(), 0, screenSize);
                Vector2 endJointCurrentPos = UniversalFunction.GetLineRendererPos(stageColliders[y, x].resultJointObject.GetComponent<LineRenderer>(), 1, screenSize);

                Vector2 startBranchResultPos = OutputLinePos(stageRoots, stageColliders, 1, y, x);
                Vector2 endBranchResultPos = OutputLinePos(stageRoots, stageColliders, 2, y, x);
                Vector2 startJointResultPos = OutputLinePos(stageRoots, stageColliders, 3, y, x);
                Vector2 endJointResultPos = OutputLinePos(stageRoots, stageColliders, 4, y, x);

                Vector2 startBranchBufferPos = UniversalFunction.MathfLerp
                (
                    startBranchCurrentPos,
                    startBranchResultPos,
                    Time.deltaTime * smoothTimes
                );
                Vector2 endBranchBufferPos = UniversalFunction.MathfLerp
                (
                    endBranchCurrentPos,
                    endBranchResultPos,
                    Time.deltaTime * smoothTimes
                );
                Vector2 startJointBufferPos = UniversalFunction.MathfLerp
                (
                    startJointCurrentPos,
                    startJointResultPos,
                    Time.deltaTime * smoothTimes
                );
                Vector2 endJointBufferPos = UniversalFunction.MathfLerp
                (
                    endJointCurrentPos,
                    endJointResultPos,
                    Time.deltaTime * smoothTimes
                );

                Vector2 startBranchPos = startBranchBufferPos;
                Vector2 endBranchPos = endBranchBufferPos;
                Vector2 startJointPos = (endBranchCurrentPos - endBranchResultPos).magnitude >= 0.001f ? endBranchCurrentPos : startJointBufferPos;
                Vector2 endJointPos = (endBranchCurrentPos - endBranchResultPos).magnitude >= 0.001f ? endBranchCurrentPos : endJointBufferPos;

                if (startBranchCurrentPos != startBranchResultPos && stageColliders[y, x].winner != -1)
                {
                    stageColliders[y, x].resultBranchObject.SetActive(true);
                    stageColliders[y, x].resultJointObject.SetActive(true);

                    MoveLineRenderer(stageColliders[y, x].resultBranchObject.GetComponent<LineRenderer>(), startBranchResultPos, startBranchResultPos, screenSize);
                    MoveLineRenderer(stageColliders[y, x].resultJointObject.GetComponent<LineRenderer>(), endBranchResultPos, endBranchResultPos, screenSize);
                }
                else
                {
                    MoveLineRenderer(stageColliders[y, x].resultBranchObject.GetComponent<LineRenderer>(), startBranchPos, endBranchPos, screenSize);
                    MoveLineRenderer(stageColliders[y, x].resultJointObject.GetComponent<LineRenderer>(), startJointPos, endJointPos, screenSize);
                }
            }
        }

        for (int x = 0; x < sumPeople; x++)
        {
            Vector2 playerObjectCurrentPos = GetMovePlayerObjectPos(stageRoots[x].playerObject, screenSize);

            MovePlayerObject
            (
                stageRoots[x].playerObject,
                UniversalFunction.MathfLerp(playerObjectCurrentPos, stageRoots[x].currentPos, Time.deltaTime * smoothTimes),
                screenSize
            );
        }
    }

    public string DebugTournamentData(TournamentProvider.tournamentData tournamentData)
    {
        if (tournamentData == null) return "Tournament Data does not exist!";

        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;

        string log = "";

        int sumPeople = stageRoots.Length;

        int stageHeight = stageColliders.GetLength(0);

        for (int x = 0; x < sumPeople; x++)
        {
            if (x != 0 && x < sumPeople) log += ", \n";

            log +=
            (
                "{ ID: " +
                x.ToString() + ", " +
                "PlayerName: " +
                stageRoots[x].playerName + ", " +
                "Branch: { PosX: " +
                (stageRoots[x].startDrawRatioBranchPos.x * 100f).ToString("F2") + "%, " +
                "LengthY: " +
                (stageRoots[x].startDrawRatioBranchPos.y * 100f).ToString("F2") + "% ~ " +
                (stageRoots[x].endDrawRatioBranchPos.y * 100f).ToString("F2") + "% } }"
            );
        }

        log += "\n\n";

        for (int y = 0; y < stageHeight; y++)
        {
            log +=
            (
                "Game: " +
                stageProperties[y].sumGame + ", " +
                "Branch: " +
                stageProperties[y].sumBranch + ", " +
                "isReverseOrder: " +
                (stageProperties[y].reverseOrder ? "Yes" : "No") + ", " +
                "incompleteGroupBranch: " +
                stageProperties[y].incompleteGroup + ", " +
                "Collider: \n[ "
            );

            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                if (x != 0 && x < stageProperties[y].sumBranch) log += ",\n";

                string connectBranches = "";

                for (int i = 0; i < stageColliders[y, x].type; i++)
                {
                    connectBranches +=
                    (
                        i != 0 ?
                        " vs " + stageColliders[y, x].connectBranch[i] :
                        "" + stageColliders[y, x].connectBranch[i]
                    );
                }

                log +=
                (
                    "{ ID: " +
                    x + ", " +
                    "Type: " +
                    stageColliders[y, x].type + ", " +
                    "(" +
                    connectBranches +
                    "), Winner: " +
                    stageColliders[y, x].winner + ", " +
                    "Branch: { PosX: " +
                    (stageColliders[y, x].startDrawRatioBranchPos.x * 100f).ToString("F2") + "%, " +
                    "LengthY: " +
                    (stageColliders[y, x].startDrawRatioBranchPos.y * 100f).ToString("F2") + "% ~ " +
                    (stageColliders[y, x].endDrawRatioBranchPos.y * 100f).ToString("F2") + "% }, " +
                    "Joint: { LengthX: " +
                    (stageColliders[y, x].startDrawRatioJointPos.x * 100f).ToString("F2") + "% ~ " +
                    (stageColliders[y, x].endDrawRatioJointPos.x * 100f).ToString("F2") + "%, " +
                    "PosY: " +
                    (stageColliders[y, x].startDrawRatioJointPos.y * 100f).ToString("F2") + "% } }"
                );
            }

            log += " ]\n\n";
        }

        return log;
    }

    public string DebugTournamentCounter(TournamentProvider.tournamentCounter tournamentCounter)
    {
        if (tournamentCounter == null) return "Tournament Counter does not exist!";

        string log = "";

        log +=
        (
            "sumGame: " + tournamentCounter.sumGame + ",\n" +
            "finishGame: " + tournamentCounter.finishGame + ",\n" +
            "remainingGame: " + tournamentCounter.remainingGame + ",\n" +
            "nextGamePos: { " + tournamentCounter.nextGamePos[0] + ", " + tournamentCounter.nextGamePos[1] + " }"
        );

        return log;
    }

    public int ChangeLayer(GameObject[] tournamentLayerObjects, int num, int showLayer)
    {
        if (showLayer < -1 || showLayer > 2) showLayer = 0;

        if (showLayer == -1)
        {
            for (int i = 0; i < tournamentLayerObjects.Length; i++)
            {
                tournamentLayerObjects[i].SetActive(i == num ? true : false);
            }
        }
        else
        {
            tournamentLayerObjects[num].SetActive
            (
                showLayer == 0 ? !tournamentLayerObjects[num].activeSelf : showLayer == 1 ? true : false
            );
        }

        return num;
    }

    public void ClearContainer(TournamentProvider.tournamentContainerObject tournamentContainerObjects)
    {
        GameObject baseLineContainer = tournamentContainerObjects.baseLineContainer;
        GameObject resultLineContainer = tournamentContainerObjects.resultLineContainer;
        GameObject playerContainer = tournamentContainerObjects.playerContainer;

        UniversalFunction.DestroyChildObject(baseLineContainer);
        UniversalFunction.DestroyChildObject(resultLineContainer);
        UniversalFunction.DestroyChildObject(playerContainer);
    }

    public int SetMenu
    (
        TournamentProvider.tournamentMenuObject tournamentMenuObjects,
        TournamentProvider.tournamentData tournamentData,
        int id,
        TournamentProvider TournamentProvider,
        TournamentEditor TournamentEditor
    )
    {
        Image playerImage = tournamentMenuObjects.playerImage;
        GameObject playerNameInput = tournamentMenuObjects.playerNameInput;
        GameObject playerColorInput = tournamentMenuObjects.playerColorInput;
        GameObject playerNumberText = tournamentMenuObjects.playerNumberText;
        Button changePlayerNameButton = tournamentMenuObjects.changePlayerNameButton;
        Button changePlayerColorButton = tournamentMenuObjects.changePlayerColorButton;
        Button winActionButton = tournamentMenuObjects.winActionButton;
        Button cancelActionButton = tournamentMenuObjects.cancelActionButton;

        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;

        int sumPeople = stageRoots.Length;

        string optionZeroFill = "D" + UniversalFunction.CalcValeToClosePow((float)sumPeople, 10f);

        Color32 playerColor = stageRoots[id].playerColor;

        playerImage.color = playerColor;
        UniversalFunction.SetText(playerNumberText, 0, id.ToString(optionZeroFill), playerColor);

        UniversalFunction.SetInputFieldText(playerNameInput, stageRoots[id].playerName);
        UniversalFunction.SetInputFieldText(playerColorInput, ColorUtility.ToHtmlStringRGB(playerColor));

        if (changePlayerNameButton != null)
        {
            changePlayerNameButton.onClick.RemoveAllListeners();
            changePlayerNameButton.onClick.AddListener(() => { TournamentEditor.ChangePlayerName(id); });
        }

        if (changePlayerColorButton != null)
        {
            changePlayerColorButton.onClick.RemoveAllListeners();
            changePlayerColorButton.onClick.AddListener(() => { TournamentEditor.ChangePlayerColor(id); });
        }

        if (winActionButton != null)
        {
            winActionButton.onClick.RemoveAllListeners();
            winActionButton.onClick.AddListener(() => { TournamentEditor.WinAction(id); });
            winActionButton.onClick.AddListener(TournamentProvider.CloseMenu);
        }

        if (cancelActionButton != null)
        {
            cancelActionButton.onClick.RemoveAllListeners();
            cancelActionButton.onClick.AddListener(() => { TournamentEditor.CancelAction(id); });
            cancelActionButton.onClick.AddListener(TournamentProvider.CloseMenu);
        }

        return id;
    }

    // Specific Function

    TournamentProvider.tournamentData SetEveryBaseLineRenderer
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject lineObject,
        TournamentProvider.tournamentData tournamentData,
        Vector2 screenSize
    )
    {
        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        int sumPeople = stageRoots.Length;

        int stageHeight = stageColliders.GetLength(0);

        for (int x = 0; x < sumPeople; x++)
        {
            stageRoots[x].branchObject = SetLineRenderer
            (
                canvasObject,
                containerObject,
                lineObject,
                stageRoots[x].startDrawRatioBranchPos,
                stageRoots[x].endDrawRatioBranchPos,
                screenSize
            );
        }

        for (int y = 0; y < stageHeight; y++)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                stageColliders[y, x].baseBranchObject = SetLineRenderer
                (
                    canvasObject,
                    containerObject,
                    lineObject,
                    stageColliders[y, x].startDrawRatioBranchPos,
                    stageColliders[y, x].endDrawRatioBranchPos,
                    screenSize
                );

                stageColliders[y, x].baseJointObject = SetLineRenderer
                (
                    canvasObject,
                    containerObject,
                    lineObject,
                    stageColliders[y, x].startDrawRatioJointPos,
                    stageColliders[y, x].endDrawRatioJointPos,
                    screenSize
                );
            }
        }

        TournamentProvider.tournamentData sub = tournamentData;

        sub.stageRoots = stageRoots;
        sub.stageColliders = stageColliders;

        return sub;
    }

    TournamentProvider.tournamentData SetEveryResultLineRenderer
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject lineObject,
        TournamentProvider.tournamentData tournamentData,
        Vector2 screenSize
    )
    {
        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        int sumPeople = stageRoots.Length;

        int stageHeight = stageColliders.GetLength(0);

        for (int y = 0; y < stageHeight; y++)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                Vector2 startBranchPos = OutputLinePos(stageRoots, stageColliders, 1, y, x);
                Vector2 endBranchPos = OutputLinePos(stageRoots, stageColliders, 2, y, x);
                Vector2 startJointPos = OutputLinePos(stageRoots, stageColliders, 3, y, x);
                Vector2 endJointPos = OutputLinePos(stageRoots, stageColliders, 4, y, x);

                stageColliders[y, x].resultBranchObject = SetLineRenderer
                (
                    canvasObject,
                    containerObject,
                    lineObject,
                    startBranchPos,
                    endBranchPos,
                    screenSize
                );

                stageColliders[y, x].resultJointObject = SetLineRenderer
                (
                    canvasObject,
                    containerObject,
                    lineObject,
                    startJointPos,
                    endJointPos,
                    screenSize
                );

                if (stageColliders[y, x].winner == -1)
                {
                    stageColliders[y, x].resultBranchObject.SetActive(false);
                    stageColliders[y, x].resultJointObject.SetActive(false);
                }
            }
        }

        TournamentProvider.tournamentData sub = tournamentData;

        sub.stageColliders = stageColliders;

        return sub;
    }

    Vector2 OutputLinePos
    (
        TournamentProvider.stageRoot[] stageRoots,
        TournamentProvider.stageCollider[,] stageColliders,
        int mode,
        int y,
        int x
    )
    {
        Vector2 result = Vector2.zero;

        switch (mode)
        {
            case 1:
                result = stageColliders[y, x].winner != -1 ?
                (
                    y == 0 ?
                    (
                        stageRoots
                        [
                            stageColliders[y, x].winner
                        ].startDrawRatioBranchPos
                    )
                    :
                    (
                        stageColliders
                        [
                            y - 1,
                            stageColliders[y, x].winner
                        ].startDrawRatioBranchPos
                    )
                )
                :
                (
                    stageColliders[y, x].startDrawRatioBranchPos
                );
                break;
            case 2:
                result = stageColliders[y, x].winner != -1 ?
                (
                    y == 0 ?
                    (
                        stageRoots
                        [
                            stageColliders[y, x].winner
                        ].endDrawRatioBranchPos
                    )
                    :
                    (
                        stageColliders
                        [
                            y - 1,
                            stageColliders[y, x].winner
                        ].endDrawRatioBranchPos
                    )
                )
                :
                (
                    stageColliders[y, x].startDrawRatioBranchPos
                );
                break;
            case 3:
                result = stageColliders[y, x].winner != -1 ?
                (
                    y == 0 ?
                    (
                        stageRoots
                        [
                            stageColliders[y, x].winner
                        ].endDrawRatioBranchPos
                    )
                    :
                    (
                        stageColliders
                        [
                            y - 1,
                            stageColliders[y, x].winner
                        ].endDrawRatioBranchPos
                    )
                )
                :
                (
                    stageColliders[y, x].startDrawRatioBranchPos
                );
                break;
            case 4:
                result = stageColliders[y, x].startDrawRatioBranchPos;
                break;
        }

        return result;
    }

    TournamentProvider.tournamentData SetEveryPlayerObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject playerObject,
        TournamentProvider.tournamentData tournamentData,
        int setColor,
        Vector2 screenSize,
        TournamentProvider TournamentProvider
    )
    {
        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;

        int sumPeople = stageRoots.Length;

        for (int x = 0; x < sumPeople; x++)
        {
            switch (setColor)
            {
                case 1:
                    stageRoots[x].playerColor = UniversalFunction.GetColor(128f, 255f, -1f, 255f);
                    break;
                case 2:
                    stageRoots[x].playerColor = UniversalFunction.GetColor((float)x / (float)sumPeople, 0.5f, 1f, -1f);
                    break;
            }

            stageRoots[x].playerObject = SetPlayerObject
            (
                canvasObject,
                containerObject,
                playerObject,
                x,
                stageRoots[x].currentPos,
                stageRoots[x].playerName,
                stageRoots[x].playerColor,
                screenSize,
                TournamentProvider
            );
        }

        TournamentProvider.tournamentData sub = tournamentData;

        sub.stageRoots = stageRoots;

        return sub;
    }

    Vector2 GetPlayerCurrentPos(TournamentProvider.tournamentData tournamentData, int id)
    {
        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        int stageHeight = stageColliders.GetLength(0);

        colliderCache connecter = new colliderCache();

        connecter.id = id;
        connecter.pos = stageRoots[id].currentPos;

        for (int y = 0; y < stageHeight; y++)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                if (Array.IndexOf(stageColliders[y, x].connectBranch, connecter.id) != -1)
                {
                    if (y != stageHeight && stageColliders[y, x].winner == connecter.id)
                    {
                        connecter.id = x;
                        connecter.pos = stageColliders[y, x].startDrawRatioBranchPos;

                        break;
                    }
                    else if (stageColliders[y, x].winner != connecter.id)
                    {
                        return connecter.pos;
                    }
                }
            }
        }

        return connecter.pos;
    }

    GameObject SetLineRenderer
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

        MoveLineRenderer(cloneLineRenderer, startPos, endPos, screenSize);

        return cloneLineObject;
    }

    void MoveLineRenderer(LineRenderer lineRenderer, Vector2 startPos, Vector2 endPos, Vector2 screenSize)
    {
        Vector3[] pos = new Vector3[2]
        {
            new Vector3
            (
                UniversalFunction.ConvScreenCentralReference(startPos.x, screenSize.x, false),
                UniversalFunction.ConvScreenCentralReference(startPos.y, screenSize.y, false),
                0f
            ),
            new Vector3
            (
                UniversalFunction.ConvScreenCentralReference(endPos.x, screenSize.x, false),
                UniversalFunction.ConvScreenCentralReference(endPos.y, screenSize.y, false),
                0f
            )
        };

        lineRenderer.SetPosition(0, pos[0]);
        lineRenderer.SetPosition(1, pos[1]);
    }

    GameObject SetPlayerObject
    (
        GameObject canvasObject,
        GameObject containerObject,
        GameObject playerObject,
        int id,
        Vector2 pos,
        string name,
        Color32 playerColor,
        Vector2 screenSize,
        TournamentProvider TournamentProvider
    )
    {
        GameObject clonePlayerObject = UniversalFunction.SetCloneObject(playerObject, containerObject);

        UniversalFunction.ConnectRectParent(playerObject, clonePlayerObject);

        MovePlayerObject(clonePlayerObject, pos, screenSize);

        Button clonePlayerButton = clonePlayerObject.GetComponent<Button>();
        clonePlayerButton.onClick.AddListener(() => { TournamentProvider.OpenMenu(id); });

        Image clonePlayerImage = clonePlayerObject.GetComponent<Image>();
        clonePlayerImage.color = playerColor;

        UniversalFunction.SetButtonText(clonePlayerObject, 0, name.ToString(), playerColor);

        return clonePlayerObject;
    }

    void MovePlayerObject(GameObject playerObject, Vector2 pos, Vector2 screenSize)
    {
        playerObject.GetComponent<RectTransform>().anchoredPosition = new Vector3
        (
            UniversalFunction.ConvScreenCentralReference(pos.x, screenSize.x, false),
            UniversalFunction.ConvScreenCentralReference(pos.y, screenSize.y, false),
            0f
        );
    }

    Vector2 GetMovePlayerObjectPos(GameObject playerObject, Vector2 screenSize)
    {
        Vector3 playerObjectPos = playerObject.GetComponent<RectTransform>().anchoredPosition;

        Vector2 pos = new Vector2
        (
            UniversalFunction.ConvScreenCentralReference(playerObjectPos.x, screenSize.x, true),
            UniversalFunction.ConvScreenCentralReference(playerObjectPos.y, screenSize.y, true)
        );

        return pos;
    }
}
