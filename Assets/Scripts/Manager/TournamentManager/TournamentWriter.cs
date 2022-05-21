using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TournamentWriter : MonoBehaviour
{
    // Unity

    void Start()
    {
        UniversalFunction.SetInitState("");
    }

    // Usable Function

    public void SaveData(TournamentProvider.tournamentData tournamentData, string filePath)
    {
        if (tournamentData == null)
        {
            Debug.LogError("Tournament Data does not exist!");

            return;
        }

        TournamentProvider.stageRoot[] stageRoots = tournamentData.stageRoots;
        TournamentProvider.stageCollider[,] stageColliders = tournamentData.stageColliders;
        TournamentProvider.stageProperty[] stageProperties = tournamentData.stageProperties;

        StreamWriter sw = new StreamWriter(@filePath, false, System.Text.Encoding.GetEncoding("UTF-8"));

        DateTime dt = DateTime.Now;

        int sumPeople = stageRoots.Length;

        int stageHeight = stageColliders.GetLength(0);
        int stageWidth = stageColliders.GetLength(1);

        int sector = 0;

        while (true)
        {
            if (sector == 0)
            {
                sw.WriteLine(GetDataSector(0) + ",Value");

                sw.WriteLine("");

                sw.WriteLine("Export Time," + UniversalFunction.GenerateDateTimeString(dt));
                sw.WriteLine("Hash," + tournamentData.hash);
                sw.WriteLine("Title," + tournamentData.title);
                sw.WriteLine("Allow Walkover," + (tournamentData.allowWalkover ? "True" : "False"));
                sw.WriteLine("People," + sumPeople.ToString());
                sw.WriteLine("Group," + tournamentData.defaultNumGroup.ToString());

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
                sw.WriteLine(GetDataSector(2) + ",Stage Y,Stage X,Winner");

                sw.WriteLine("");

                for (int y = 0; y < stageHeight; y++)
                {
                    for (int x = 0; x < stageProperties[y].sumBranch; x++)
                    {
                        sw.WriteLine("Branch," + y + "," + x + "," + stageColliders[y, x].winner);
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
                sub = "@ Stage Collider";
                break;
        }

        return sub;
    }
}
