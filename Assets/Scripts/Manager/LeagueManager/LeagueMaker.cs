using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeagueMaker : MonoBehaviour
{
    [HideInInspector] private string useRandomString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // Usable Function

    public LeagueProvider.leagueData SetInitialLeagueData(int sumPeople, bool isBooleanMode)
    {
        string[] playerName = new string[sumPeople];

        for (int i = 0; i < sumPeople; i++) playerName[i] = "John Doe " + UniversalFunction.AlignDigitsToMaxNum(i, sumPeople);

        LeagueProvider.leagueData leagueData = SetInitialStage(sumPeople, playerName, isBooleanMode);

        return leagueData;
    }

    public LeagueProvider.leagueData SetInitialLeagueData(string[] playerList, bool isBooleanMode)
    {
        int sumPeople = playerList.Length;

        LeagueProvider.leagueData leagueData = SetInitialStage(sumPeople, playerList, isBooleanMode);

        return leagueData;
    }

    public LeagueProvider.leagueData SetInitialStage(int sumPeople, string[] playerName, bool isBooleanMode)
    {
        if (sumPeople <= 1) return null;

        LeagueProvider.leagueData leagueData = new LeagueProvider.leagueData();

        LeagueProvider.stageRoot[] stageRoots = new LeagueProvider.stageRoot[sumPeople];
        LeagueProvider.stageTable[,] stageTables = new LeagueProvider.stageTable[sumPeople, sumPeople];

        for (int x = 0; x < sumPeople; x++)
        {
            stageRoots[x] = SetInitialStageRoot(sumPeople, playerName[x], x);
        }

        for (int y = 0; y < sumPeople; y++)
        {
            for (int x = 0; x < sumPeople; x++)
            {
                stageTables[y, x] = SetInitialStageTable();
            }
        }

        leagueData.stageRoots = stageRoots;
        leagueData.stageTables = stageTables;

        UniversalFunction.SetInitState
        (
            UniversalFunction.GenerateRandomString(useRandomString, 32)
        );

        leagueData.hash = UniversalFunction.GenerateRandomString(useRandomString, 32);
        leagueData.isBooleanMode = isBooleanMode;
        leagueData.winnerPoint = 1;
        leagueData.loserPoint = 0;
        leagueData.winPointWeight = 2;
        leagueData.losePointWeight = 1;

        return leagueData;
    }

    LeagueProvider.stageRoot SetInitialStageRoot(int sumPeople, string playerName, int x)
    {
        LeagueProvider.stageRoot sub = new LeagueProvider.stageRoot();

        sub.playerName = playerName;
        sub.tablePos = (float)x / (float)(sumPeople - 1);

        return sub;
    }

    LeagueProvider.stageTable SetInitialStageTable()
    {
        LeagueProvider.stageTable sub = new LeagueProvider.stageTable();

        return sub;
    }
}
