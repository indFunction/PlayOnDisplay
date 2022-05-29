using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeagueExporter : MonoBehaviour
{
    public UserController UserController;

    // Custom Function

    public void ExportGameOrder(LeagueProvider.leagueData leagueData)
    {
        GameObject nextDisplay = UserController.GetDisplayObject(1);

        if (nextDisplay.GetComponentInChildren<MemoManager>() != null)
        {
            MemoManager MemoManager = nextDisplay.GetComponentInChildren<MemoManager>();

            string memo = MemoManager.GetMemoInputField();
            string hash = leagueData.hash;

            if (memo.LastIndexOf("Hash: " + hash) != -1)
            {
                MemoManager.SetMemoInputField(GetGameOrder(leagueData));

                UserController.MoveDisplayTargetNum(1);

                return;
            }
        }

        GameObject memoDisplay = UserController.AddDisplay(3);

        memoDisplay.GetComponentInChildren<MemoManager>().SetMemoInputField(GetGameOrder(leagueData));
    }

    // Specific Function

    string GetGameOrder(LeagueProvider.leagueData leagueData)
    {
        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;

        string res = (leagueData.title != "" ? leagueData.title : "試合") + "の対戦順序\n\n";

        int sumPeople = stageRoots.Length;
        int sumGame = sumPeople * (sumPeople - 1) / 2;

        int[] order = new int[sumPeople];
        int count = 1;

        for (int i = 0; i < sumPeople; i++) order[i] = i;

        res += SetGameOrderPartText
        (
            leagueData,
            sumGame,
            order,
            ref count
        );

        for (int i = 0; i < sumPeople - 2; i++)
        {
            int orderBuffer = order[sumPeople - 1];

            if (sumPeople % 2 == 0)
            {
                for (int j = sumPeople - 1; j > 1; j--) order[j] = order[j - 1];

                order[1] = orderBuffer;
            }
            else
            {
                for (int j = sumPeople - 1; j > 0; j--) order[j] = order[j - 1];

                order[0] = orderBuffer;
            }

            res += SetGameOrderPartText
            (
                leagueData,
                sumGame,
                order,
                ref count
            );
        }

        DateTime dt = DateTime.Now;

        res += "\n";
        res += "Export Time: " + UniversalFunction.GenerateDateTimeString(dt) + "\n";
        res += "Hash: " + leagueData.hash;

        return res;
    }

    string SetGameOrderPartText(LeagueProvider.leagueData leagueData, int sumGame, int[] order, ref int count)
    {
        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        string res = "";

        int sumPeople = order.Length;
        int sumPart = sumPeople / 2;

        for (int i = 0; i < sumPart; i++)
        {
            int posY = order[i];
            int posX = order[sumPeople - i - 1];

            if (stageTables[posY, posX].winner != -1) continue;

            res +=
            (
                "第" + UniversalFunction.AlignDigitsToMaxNum(count, sumGame) + "試合：" +
                "[" + UniversalFunction.AlignDigitsToMaxNum(posY, sumPeople) + "] " +
                stageRoots[posY].playerName + " vs " +
                "[" + UniversalFunction.AlignDigitsToMaxNum(posX, sumPeople) + "] " +
                stageRoots[posX].playerName + "\n"
            );

            count++;
        }

        return res;
    }
}
