using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LeagueWriter : MonoBehaviour
{
    // Unity

    void Start()
    {
        UniversalFunction.SetInitState("");
    }

    // Usable Function

    public void SaveData(LeagueProvider.leagueData leagueData, string filePath)
    {
        if (leagueData == null)
        {
            Debug.LogError("League Data does not exist!");

            return;
        }

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        StreamWriter sw = new StreamWriter(@filePath, false, System.Text.Encoding.GetEncoding("UTF-8"));

        DateTime dt = DateTime.Now;

        int sumPeople = stageRoots.Length;

        int sector = 0;

        while (true)
        {
            if (sector == 0)
            {
                sw.WriteLine(GetDataSector(0) + ",Value");

                sw.WriteLine("");

                sw.WriteLine("Export Time," + UniversalFunction.GenerateDateTimeString(dt));
                sw.WriteLine("Hash," + leagueData.hash);
                sw.WriteLine("Title," + leagueData.title);
                sw.WriteLine("People," + sumPeople.ToString());
                sw.WriteLine("Boolean Mode," + (leagueData.isBooleanMode ? "True" : "False"));
                sw.WriteLine("Winner Point," + leagueData.winnerPoint.ToString());
                sw.WriteLine("Loser Point," + leagueData.loserPoint.ToString());
                sw.WriteLine("Win Point Weight," + leagueData.winPointWeight.ToString());
                sw.WriteLine("Lose Point Weight," + leagueData.losePointWeight.ToString());

                sw.WriteLine("");

                sector++;
            }

            if (sector == 1)
            {
                sw.WriteLine(GetDataSector(1) + ",ID,Name,Color");

                sw.WriteLine("");

                for (int i = 0; i < sumPeople; i++)
                {
                    sw.WriteLine("Player," + i + "," + stageRoots[i].playerName + "," + ColorUtility.ToHtmlStringRGB(stageRoots[i].playerColor));
                }

                sw.WriteLine("");

                sector++;
            }

            if (sector == 2)
            {
                sw.WriteLine(GetDataSector(2) + ",Table Y,Table X,Winner,Score A,Score B");

                sw.WriteLine("");

                for (int y = 0; y < sumPeople; y++)
                {
                    for (int x = 0; x < sumPeople; x++)
                    {
                        if (stageTables[y, x].winner != -1)
                        {
                            sw.WriteLine
                            (
                                "Data," +
                                y + "," +
                                x + "," +
                                stageTables[y, x].winner + "," +
                                stageTables[y, x].scoreA + "," +
                                stageTables[y, x].scoreB
                            );
                        }
                    }
                }

                break;
            }
        }

        sw.Flush();
        sw.Close();
    }

    // Specific Function

    string GetDataSector(int num)
    {
        string sub = "";

        switch (num)
        {
            case 0:
                sub = "@ Stage Information";
                break;
            case 1:
                sub = "@ Stage Root";
                break;
            case 2:
                sub = "@ Stage Table";
                break;
        }

        return sub;
    }
}
