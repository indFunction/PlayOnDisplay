using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TournamentReader : MonoBehaviour
{
    public NotificationController NotificationController;

    // Usable Function

    public TournamentProvider.tournamentData ImportList(string filePath, int numGroup, int maxSumPlayer, TournamentMaker TournamentMaker)
    {
        TournamentProvider.tournamentData tournamentData = new TournamentProvider.tournamentData();

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

        tournamentData = TournamentMaker.SetInitialTournamentData(playerList, numGroup);

        return tournamentData;
    }

    public TournamentProvider.tournamentData ImportData(string filePath, TournamentMaker TournamentMaker)
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

        string checkHash = "";
        string title = "";
        bool allowWalkover = false;
        int sumPlayer = 0;
        int numGroup = 0;

        for (int i = 0; i < csv.Count; i++)
        {
            GetStageInformationProperty(csv[i], "Export Hash", ref checkHash);
            GetStageInformationProperty(csv[i], "Title", ref title);
            GetStageInformationProperty(csv[i], "Allow Walkover", ref allowWalkover);
            GetStageInformationProperty(csv[i], "People", ref sumPlayer);
            GetStageInformationProperty(csv[i], "Group", ref numGroup);
        }

        if (checkHash == "" || sumPlayer == 0 || numGroup == 0)
        {
            CallError();

            return null;
        }

        TournamentProvider.tournamentData tournamentData = TournamentMaker.SetInitialTournamentData(sumPlayer, numGroup);

        tournamentData.title = title;
        tournamentData.allowWalkover = allowWalkover;

        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        bool haveError = false;

        for (int i = 0; i < csv.Count; i++)
        {
            haveError = GetStageRootProperty(csv[i], sumPlayer, ref stageRoots) ? haveError : true;
            haveError = GetStageColliderProperty(csv[i], sumPlayer, stageProperties, ref stageColliders) ? haveError : true;
        }

        if (haveError)
        {
            CallError();

            return null;
        }

        return tournamentData;
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
        if (csv[0] == find) val = (csv[1] == "True" || csv[1] == "true") ? true : false;
    }

    bool GetStageRootProperty(string[] csv, int sumPlayer, ref TournamentProvider.stageRoot[] stageRoots)
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

    bool GetStageColliderProperty(string[] csv, int sumPlayer, TournamentProvider.stageProperty[] stageProperties, ref TournamentProvider.stageCollider[,] stageColliders)
    {
        if (stageProperties == null || stageColliders == null) return false;

        if (csv[0] == "Branch")
        {
            int stageHeight = stageColliders.GetLength(0);
            int stageWidth = stageColliders.GetLength(1);

            int y = int.Parse(csv[1]);
            int x = int.Parse(csv[2]);
            int winner = int.Parse(csv[3]);

            if
            (
                y >= stageHeight ||
                x >= stageWidth ||
                !(
                    winner >= -1 &&
                    winner <= (y == 0 ? sumPlayer : stageProperties[y - 1].sumBranch)
                )
            ) return false;

            stageColliders[y, x].winner = winner;
        }

        return true;
    }
}
