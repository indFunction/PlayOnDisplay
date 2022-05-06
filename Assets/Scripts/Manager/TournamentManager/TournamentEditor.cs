using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentEditor : MonoBehaviour
{
    public NotificationController NotificationController;
    public TournamentProvider TournamentProvider;
    public SpreadConfetti SpreadConfetti;

    public class colliderCache
    {
        public int id = -1;
        public int posX = -1;
        public int posY = -1;
    }

    // Usable Function

    public void WinAction(int id)
    {
        TournamentProvider.tournamentData newTournamentData = TournamentProvider.individualTournamentData;

        TournamentProvider.stageRoot[] stageRoots = newTournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = newTournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = newTournamentData.stageProperties;

        int stageHeight = stageColliders.GetLength(0);

        colliderCache connecter = new colliderCache();
        colliderCache replaceWinner = new colliderCache();

        bool isUpdate = false;
        bool isEnd = false;

        connecter.id = id;

        for (int y = 0; y < stageHeight; y++)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                if (connecter.posY != -1 && connecter.posX != -1)
                {
                    if
                    (
                        stageColliders[y, x].type == 1 &&
                        y == connecter.posY + 1 &&
                        stageColliders[y, x].connectBranch[0] == connecter.posX
                    )
                    {
                        stageRoots[id].currentPos = stageColliders[y, x].startDrawRatioBranchPos;

                        isEnd = true;
                    }
                }
                else
                {
                    if
                    (
                        isUpdate &&
                        Array.IndexOf(stageColliders[y, x].connectBranch, connecter.id) != -1 &&
                        stageColliders[y, x].winner != -1
                    )
                    {
                        connecter.id = x;

                        break;
                    }

                    if (!isUpdate && Array.IndexOf(stageColliders[y, x].connectBranch, connecter.id) != -1)
                    {
                        if (stageColliders[y, x].winner == -1 || stageColliders[y, x].winner != connecter.id)
                        {
                            if (stageColliders[y, x].winner != -1) isUpdate = true;

                            if (newTournamentData.allowWalkover || CheckOverOtherGames(stageColliders, y, x))
                            {
                                if (stageColliders[y, x].winner != -1)
                                {
                                    replaceWinner.id = SearchWinner(stageColliders, stageProperties, y, x);
                                    replaceWinner.posY = y;
                                    replaceWinner.posX = x;
                                }
                                else if (stageColliders[y, x].winner != connecter.id)
                                {
                                    stageRoots[id].currentPos = stageColliders[y, x].startDrawRatioBranchPos;
                                }

                                connecter.posY = y;
                                connecter.posX = x;

                                stageColliders[y, x].winner = connecter.id;

                                if (y == stageHeight - 1)
                                {
                                    // WIP

                                    // Debug.Log("The champion is decided!");
                                }

                                SpreadConfetti.StartSpread();
                            }
                            else
                            {
                                Button[] dummy = NotificationController.SetErrorNotification("他の試合が終わっていません！");

                                // Debug.Log("Other game(s) are not over!");
                            }

                            connecter.id = x;

                            break;
                        }
                        else if (stageColliders[y, x].winner == connecter.id)
                        {
                            connecter.id = x;

                            break;
                        }
                    }
                }

                if (isEnd) break;
            }

            if (isEnd) break;
        }

        if (replaceWinner.id != -1)
        {
            int x = replaceWinner.posX;
            int y = replaceWinner.posY;

            stageRoots[id].currentPos = stageRoots[replaceWinner.id].currentPos;

            if (y != 0)
            {
                for (int i = 0; i < stageColliders[y, x].type; i++)
                {
                    if
                    (
                        SearchWinner
                        (
                            stageColliders,
                            stageProperties,
                            y - 1,
                            stageColliders[y, x].connectBranch[i]
                        ) == replaceWinner.id
                    )
                    {
                        stageRoots[replaceWinner.id].currentPos = stageColliders
                        [
                            y - 1,
                            stageColliders[y, x].connectBranch[i]
                        ].startDrawRatioBranchPos;
                    }
                }
            }
            else
            {
                stageRoots[replaceWinner.id].currentPos = stageRoots[replaceWinner.id].startDrawRatioBranchPos;
            }
        }

        TournamentProvider.individualTournamentData = newTournamentData;

        TournamentProvider.individualTournamentData.isUpdate = 1;
    }

    public void CancelAction(int id)
    {
        TournamentProvider.tournamentData newTournamentData = TournamentProvider.individualTournamentData;

        TournamentProvider.stageRoot[] stageRoots = newTournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = newTournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = newTournamentData.stageProperties;

        int stageHeight = stageColliders.GetLength(0);

        bool isEnd = false;

        int connecterId = id;

        for (int y = 0; y < stageHeight + 1; y++)
        {
            for (int x = 0; y == stageHeight ? x < 1 : x < stageProperties[y].sumBranch; x++)
            {
                if (y != stageHeight && Array.IndexOf(stageColliders[y, x].connectBranch, connecterId) != -1)
                {
                    if (stageColliders[y, x].winner == connecterId)
                    {
                        connecterId = x;

                        break;
                    }
                    else if (y == 0 && stageColliders[y, x].winner != connecterId)
                    {
                        Button[] dummy = NotificationController.SetErrorNotification("勝敗を持たないプレイヤーを取り消すことはできません！");

                        // Debug.Log("You can't cancel the player who does not win or lose!");

                        isEnd = true;

                        break;
                    }
                }

                if
                (
                    y == stageHeight ||
                    (
                        y != 0 && Array.IndexOf(stageColliders[y, x].connectBranch, connecterId) != -1
                    )
                )
                {
                    if
                    (
                        y == stageHeight ||
                        (
                            (
                                stageColliders[y, x].winner == -1 ||
                                (
                                    newTournamentData.allowWalkover &&
                                    stageColliders[y, x].winner != connecterId
                                )
                            ) &&
                            (
                                y != 1 ||
                                stageColliders[y - 1, connecterId].type != 1
                            )
                        )
                    )
                    {
                        if (stageColliders[y - 1, connecterId].type == 1)
                        {
                            stageRoots[id].currentPos =
                            (
                                y == 2 ?
                                stageRoots[id].startDrawRatioBranchPos :
                                stageColliders
                                [
                                    y - 3,
                                    stageColliders
                                    [
                                        y - 2,
                                        stageColliders[y - 1, connecterId].winner
                                    ].winner
                                ].startDrawRatioBranchPos
                            );

                            stageColliders
                            [
                                y - 2,
                                stageColliders[y - 1, connecterId].winner
                            ].winner = -1;
                        }
                        else
                        {
                            stageRoots[id].currentPos =
                            (
                                y == 1 ?
                                stageRoots[id].startDrawRatioBranchPos :
                                stageColliders
                                [
                                    y - 2,
                                    stageColliders[y - 1, connecterId].winner
                                ].startDrawRatioBranchPos
                            );

                            stageColliders[y - 1, connecterId].winner = -1;
                        }

                        SpreadConfetti.StopSpread();
                    }
                    else
                    {
                        Button[] dummy = NotificationController.SetErrorNotification("上位の試合が既に決まっています！");

                        // Debug.Log("The game(s) of the upper level has already been decided!");
                    }

                    isEnd = true;

                    break;
                }

                if (isEnd) break;
            }

            if (isEnd) break;
        }

        TournamentProvider.individualTournamentData = newTournamentData;

        TournamentProvider.individualTournamentData.isUpdate = 1;
    }

    public void ChangePlayerName(int id)
    {
        TournamentProvider.tournamentData newTournamentData = TournamentProvider.individualTournamentData;

        TournamentProvider.stageRoot[] stageRoots = newTournamentData.stageRoots;

        string playerName = UniversalFunction.GetInputFieldText(TournamentProvider.individualTournamentMenuObjects.playerNameInput);

        stageRoots[id].playerName = playerName;

        UniversalFunction.SetButtonText
        (
            stageRoots[id].playerObject,
            1,
            playerName,
            new Color(0f, 0f, 0f, 0f)
        );

        TournamentProvider.individualTournamentData = newTournamentData;

        TournamentProvider.individualTournamentData.isUpdate = 1;
    }

    public void ChangePlayerColor(int id)
    {
        TournamentProvider.tournamentData newTournamentData = TournamentProvider.individualTournamentData;

        TournamentProvider.stageRoot[] stageRoots = newTournamentData.stageRoots;

        string inputPlayerColor = UniversalFunction.GetInputFieldText(TournamentProvider.individualTournamentMenuObjects.playerColorInput);

        if (inputPlayerColor.Length % 3 != 0) return;

        Color playerColorBuffer;

        if (ColorUtility.TryParseHtmlString("#" + inputPlayerColor, out playerColorBuffer))
        {
            Color32 playerColor = playerColorBuffer;

            stageRoots[id].playerColor = playerColor;

            stageRoots[id].playerObject.GetComponent<Image>().color = playerColor;
            UniversalFunction.SetButtonText(stageRoots[id].playerObject, 2, "", playerColor);

            TournamentProvider.individualTournamentMenuObjects.playerImage.color = playerColor;
            UniversalFunction.SetText(TournamentProvider.individualTournamentMenuObjects.playerNumberText, 2, "", playerColor);

            TournamentProvider.individualTournamentData = newTournamentData;

            TournamentProvider.individualTournamentData.isUpdate = 1;
        }
    }

    public void ShufflePlayer()
    {
        TournamentProvider.tournamentData newTournamentData = TournamentProvider.individualTournamentData;

        TournamentProvider.stageRoot[] stageRoots = newTournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = newTournamentData.stageColliders;

        int sumPeople = stageRoots.Length;

        int stageWidth = stageColliders.GetLength(1);

        for (int i = 0; i < stageWidth; i++)
        {
            if (stageColliders[0, i].winner != -1)
            {
                Button[] dummy = NotificationController.SetErrorNotification("1つ以上の試合の勝敗が確定しているため、プレイヤーをシャッフルすることができません！");

                // Debug.Log("Players cannot be shuffled because one or more matches have been decided!");

                return;
            }
        }

        List<TournamentProvider.stageRoot> stageRootList = new List<TournamentProvider.stageRoot>(stageRoots);

        TournamentProvider.stageRoot[] newStageRoots = new TournamentProvider.stageRoot[sumPeople];

        for (int i = 0; i < sumPeople; i++)
        {
            int random = UnityEngine.Random.Range(0, stageRootList.Count - 1);

            newStageRoots[i] = stageRoots[i].Clone();

            newStageRoots[i].playerName = stageRootList[random].playerName;
            newStageRoots[i].playerColor = stageRootList[random].playerColor;

            stageRootList[random] = stageRootList[stageRootList.Count - 1];
            stageRootList.RemoveAt(stageRootList.Count - 1);
        }

        newTournamentData.stageRoots = newStageRoots;

        TournamentProvider.individualTournamentData = newTournamentData;

        TournamentProvider.individualTournamentData.isUpdate = 2;
    }

    // Specific Function

    bool CheckOverOtherGames
    (
        TournamentProvider.stageCollider[,] stageColliders,
        int y,
        int x
    )
    {
        if (y == 0) return true;

        bool result = true;

        for (int i = 0; i < stageColliders[y, x].type; i++)
        {
            if
            (
                stageColliders
                [
                    y - 1,
                    stageColliders[y, x].connectBranch[i]
                ].winner == -1
            ) result = false;

            if
            (
                y >= 2 &&
                stageColliders
                [
                    y - 1,
                    stageColliders[y, x].connectBranch[i]
                ].type == 1 &&
                stageColliders
                [
                    y - 2,
                    stageColliders
                    [
                        y - 1,
                        stageColliders[y, x].connectBranch[i]
                    ].winner
                ].winner == -1
            ) result = false;
        }

        return result;
    }

    int SearchWinner
    (
        TournamentProvider.stageCollider[,] stageColliders,
        TournamentProvider.stageProperty[] stageProperties,
        int targetY,
        int targetX
    )
    {
        if (targetY == 0) return stageColliders[targetY, targetX].winner;

        int connecterId = stageColliders[targetY, targetX].winner;

        for (int y = targetY - 1; y >= 0; y--)
        {
            if (connecterId == -1) break;

            connecterId = stageColliders[y, connecterId].winner;
        }

        return connecterId;
    }
}
