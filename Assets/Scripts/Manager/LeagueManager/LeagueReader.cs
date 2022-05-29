using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LeagueReader : MonoBehaviour
{
    public NotificationController NotificationController;

    // Usable Function

    public LeagueProvider.leagueData ImportList(string filePath, int maxSumPlayer, bool isBooleanMode, LeagueMaker LeagueMaker)
    {
        LeagueProvider.leagueData leagueData = new LeagueProvider.leagueData();

        string[] playerList = new string[maxSumPlayer];

        int counter = 0;

        using (var sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            for (int i = 0; sr.Peek() != -1; i++)
            {
                counter++;

                if (i < maxSumPlayer)
                {
                    playerList[i] = sr.ReadLine();
                }
                else
                {
                    break;
                }
            }
        }

        Array.Resize(ref playerList, counter < maxSumPlayer ? counter : maxSumPlayer);

        if (counter < 2)
        {
            Button[] dummy = NotificationController.SetErrorNotification("プレイヤー数が足りません！");

            // Debug.Log("Not enough players!");

            return null;
        }

        if (counter > maxSumPlayer)
        {
            Button[] dummy = NotificationController.SetErrorNotification("プレイヤーが最大数に達したため、一部のプレイヤーが除外されました！");

            // Debug.Log("Some player(s) have been excluded because the number of players has reached the maximum!");
        }

        leagueData = LeagueMaker.SetInitialLeagueData(playerList, isBooleanMode);

        return leagueData;
    }

    public LeagueProvider.leagueData ImportData(string filePath, LeagueMaker LeagueMaker)
    {
        List<string[]> csv = new List<string[]>();

        using (var sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            for (int i = 0; sr.Peek() != -1; i++)
            {
                string line = sr.ReadLine();
                csv.Add(line.Split(','));
            }
        }

        string hash = "";
        string title = "";
        int sumPlayer = 0;
        bool isBooleanMode = false;
        int winnerPoint = 0;
        int loserPoint = 0;
        int winPointWeight = 0;
        int losePointWeight = 0;

        for (int i = 0; i < csv.Count; i++)
        {
            GetStageInformationProperty(csv[i], "Hash", ref hash);
            GetStageInformationProperty(csv[i], "Title", ref title);
            GetStageInformationProperty(csv[i], "People", ref sumPlayer);
            GetStageInformationProperty(csv[i], "Boolean Mode", ref isBooleanMode);
            GetStageInformationProperty(csv[i], "Winner Point", ref winnerPoint);
            GetStageInformationProperty(csv[i], "Loser Point", ref loserPoint);
            GetStageInformationProperty(csv[i], "Win Point Weight", ref winPointWeight);
            GetStageInformationProperty(csv[i], "Lose Point Weight", ref losePointWeight);
        }

        if (hash == "" || sumPlayer == 0)
        {
            CallError();

            return null;
        }

        LeagueProvider.leagueData leagueData = LeagueMaker.SetInitialLeagueData(sumPlayer, isBooleanMode);

        leagueData.hash = hash;
        leagueData.title = title;
        leagueData.isBooleanMode = isBooleanMode;
        leagueData.winnerPoint = winnerPoint;
        leagueData.loserPoint = loserPoint;
        leagueData.winPointWeight = winPointWeight;
        leagueData.losePointWeight = losePointWeight;

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        bool haveError = false;

        for (int i = 0; i < csv.Count; i++)
        {
            haveError = GetStageRootProperty(csv[i], sumPlayer, ref stageRoots) ? haveError : true;
            haveError = GetStageTableProperty(csv[i], sumPlayer, ref stageTables) ? haveError : true;
        }

        if (haveError)
        {
            CallError();

            return null;
        }

        return leagueData;
    }

    void CallError()
    {
        Button[] dummy = NotificationController.SetErrorNotification("ファイルが正常に読み込めません！");

        // Debug.Log("The file could not be read normally!");
    }

    // Specific Function

    void GetStageInformationProperty(string[] csv, string find, ref string val)
    {
        if (csv[0] == find) val = csv[1];
    }

    void GetStageInformationProperty(string[] csv, string find, ref int val)
    {
        if (csv[0] == find) val = int.Parse(csv[1]);
    }

    void GetStageInformationProperty(string[] csv, string find, ref bool val)
    {
        if (csv[0] == find) val = csv[1] == "True" || csv[1] == "true";
    }

    bool GetStageRootProperty(string[] csv, int sumPlayer, ref LeagueProvider.stageRoot[] stageRoots)
    {
        if (stageRoots == null) return false;

        if (csv[0] == "Player")
        {
            int id = int.Parse(csv[1]);

            if (id >= sumPlayer) return false;

            stageRoots[id].playerName = csv[2];

            Color playerColorBuffer;

            if (ColorUtility.TryParseHtmlString("#" + csv[3], out playerColorBuffer))
            {
                Color32 playerColor = playerColorBuffer;

                stageRoots[id].playerColor = playerColor;
            }
            else
            {
                stageRoots[id].playerColor = new Color32((byte)255f, (byte)255f, (byte)255f, (byte)255f);
            }
        }

        return true;
    }

    bool GetStageTableProperty(string[] csv, int sumPlayer, ref LeagueProvider.stageTable[,] stageTables)
    {
        if (stageTables == null) return false;

        if (csv[0] == "Data")
        {
            int y = int.Parse(csv[1]);
            int x = int.Parse(csv[2]);
            int winner = int.Parse(csv[3]);
            int scoreA = int.Parse(csv[4]);
            int scoreB = int.Parse(csv[5]);

            if (y >= sumPlayer || x >= sumPlayer || winner < -1) return false;

            stageTables[y, x].winner = winner;
            stageTables[y, x].scoreA = scoreA;
            stageTables[y, x].scoreB = scoreB;
        }

        return true;
    }
}
