using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TournamentExporter : MonoBehaviour
{
    public UserController UserController;

    // Custom Function

    public void ExportGameRanking(TournamentProvider.tournamentData tournamentData)
    {
        GameObject nextDisplay = UserController.GetDisplayObject(1);

        if (nextDisplay.GetComponentInChildren<MemoManager>() != null)
        {
            MemoManager MemoManager = nextDisplay.GetComponentInChildren<MemoManager>();

            string memo = MemoManager.GetMemoInputField();
            string hash = tournamentData.hash;

            if (memo.LastIndexOf("Hash: " + hash) != -1)
            {
                MemoManager.SetMemoInputField(GetGameRanking(tournamentData));

                UserController.MoveDisplayTargetNum(1);

                return;
            }
        }

        GameObject memoDisplay = UserController.AddDisplay(3);

        memoDisplay.GetComponentInChildren<MemoManager>().SetMemoInputField(GetGameRanking(tournamentData));
    }

    // Specific Function

    string GetGameRanking(TournamentProvider.tournamentData tournamentData)
    {
        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        string res = (tournamentData.title != "" ? tournamentData.title : "試合") + "の勝敗順位\n\n";

        int sumPeople = stageRoots.Length;

        int stageHeight = stageColliders.GetLength(0);

        int rank = 1;
        int rankBuffer = 0;

        int head = 0;
        int[] rankTape = Enumerable.Repeat<int>(-1, sumPeople).ToArray();
        int[] playerTape = Enumerable.Repeat<int>(-1, sumPeople).ToArray();

        for (int y = stageHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                if (stageColliders[y, x].winner == -1) continue;

                int playerId = SearchPlayer
                (
                    stageColliders, stageProperties, y, x, Array.IndexOf(stageColliders[y, x].connectBranch, stageColliders[y, x].winner)
                );

                if (!playerTape.Contains(playerId))
                {
                    rankTape[head] = rank;
                    playerTape[head] = playerId;

                    head++;
                    rankBuffer++;
                }
            }

            rank += rankBuffer;
            rankBuffer = 0;

            for (int x = 0; x < stageProperties[y].sumBranch; x++)
            {
                for (int i = 0; i < stageColliders[y, x].type; i++)
                {
                    int playerId = SearchPlayer(stageColliders, stageProperties, y, x, i);

                    if (!playerTape.Contains(playerId))
                    {
                        rankTape[head] = rank;
                        playerTape[head] = playerId;

                        head++;
                        rankBuffer++;
                    }
                }
            }

            rank += rankBuffer;
            rankBuffer = 0;
        }

        for (int i = 0; i < sumPeople; i++)
        {
            res += "第" + UniversalFunction.AlignDigitsToMaxNum(rankTape[i], sumPeople) + "位：" + stageRoots[playerTape[i]].playerName + "\n";
        }

        DateTime dt = DateTime.Now;

        res += "\n";
        res += "Export Time: " + UniversalFunction.GenerateDateTimeString(dt) + "\n";
        res += "Hash: " + tournamentData.hash;

        return res;
    }

    int SearchPlayer
    (
        TournamentProvider.stageCollider[,] stageColliders,
        TournamentProvider.stageProperty[] stageProperties,
        int targetY,
        int targetX,
        int targetConnectBranch
    )
    {
        if (targetY == 0) return stageColliders[targetY, targetX].connectBranch[targetConnectBranch];

        int connecterId = stageColliders[targetY, targetX].connectBranch[targetConnectBranch];

        for (int y = targetY - 1; y >= 0; y--)
        {
            if (connecterId == -1) break;

            connecterId = stageColliders[y, connecterId].winner;
        }

        return connecterId;
    }
}
