using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeagueEditor : MonoBehaviour
{
    public NotificationController NotificationController;
    public LeagueProvider LeagueProvider;
    public LeaguePainter LeaguePainter;
    public SpreadConfetti SpreadConfetti;

    class scoreItem
    {
        public int id = -1;
        public int score = 0;
    }

    // Usable Function

    public void ChangeScore(int id)
    {
        SetScore(id, 0, true);
    }

    public void WinnerA(int id)
    {
        SetScore(id, 1, true);
    }

    public void WinnerB(int id)
    {
        SetScore(id, 2, true);
    }

    public void ResetScore(int id)
    {
        SetScore(id, -1, true);
    }

    public void SetScore(int id, int mode, bool runEffect)
    {
        LeagueProvider.leagueData newLeagueData = LeagueProvider.individualLeagueData;

        LeagueProvider.stageRoot[] stageRoots = newLeagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = newLeagueData.stageTables;

        int sumPeople = stageRoots.Length;

        int posX = id % sumPeople;
        int posY = id / sumPeople;

        LeagueProvider.leagueScoreMenuObject ScoreObjects = LeagueProvider.individualLeagueScoreMenuObjects;

        LeagueProvider.stageTable stageTableA = stageTables[posY, posX];
        LeagueProvider.stageTable stageTableB = stageTables[posX, posY];

        if (mode == 0)
        {
            string scoreStringBufferA = UniversalFunction.GetInputFieldText(ScoreObjects.scoreInputA);
            string scoreStringBufferB = UniversalFunction.GetInputFieldText(ScoreObjects.scoreInputB);

            int scoreIntBufferA = scoreStringBufferA != "" ? int.Parse(scoreStringBufferA) : 0;
            int scoreIntBufferB = scoreStringBufferB != "" ? int.Parse(scoreStringBufferB) : 0;

            if (scoreIntBufferA != UniversalFunction.FixValueRange(scoreIntBufferA, 1, 0, 0))
            {
                Button[] go = NotificationController.SetErrorNotification("得点を負の値にすることはできません！");

                // Debug.Log("The score cannot be negative!");

                return;
            }

            if (scoreIntBufferB != UniversalFunction.FixValueRange(scoreIntBufferB, 1, 0, 0))
            {
                Button[] go = NotificationController.SetErrorNotification("得点を負の値にすることはできません！");

                // Debug.Log("The score cannot be negative!");

                return;
            }

            int scoreA = scoreStringBufferA != "" ? scoreIntBufferA : -1;
            int scoreB = scoreStringBufferB != "" ? scoreIntBufferB : -1;

            if (scoreA < 0 && scoreB < 0)
            {
                stageTableA.winner = -1;
                stageTableB.winner = -1;
                stageTableA.scoreA = -1;
                stageTableB.scoreA = -1;
                stageTableA.scoreB = -1;
                stageTableB.scoreB = -1;
            }
            else if (scoreA >= 0 && scoreB >= 0)
            {
                stageTableA.winner = GetWinner(scoreA, scoreB);
                stageTableB.winner = GetWinner(scoreB, scoreA);
                stageTableA.scoreA = scoreA;
                stageTableB.scoreA = scoreB;
                stageTableA.scoreB = scoreB;
                stageTableB.scoreB = scoreA;

                if (runEffect) SpreadConfetti.StartSpread();
            }
            else
            {
                Button[] go = NotificationController.SetErrorNotification("片方の得点を空白にすることはできません！");

                // Debug.Log("One score cannot be blank!");

                return;
            }
        }
        else if (mode == -1)
        {
            stageTableA.winner = -1;
            stageTableB.winner = -1;
            stageTableA.scoreA = -1;
            stageTableB.scoreA = -1;
            stageTableA.scoreB = -1;
            stageTableB.scoreB = -1;
        }
        else
        {
            int winnerPoint = newLeagueData.winnerPoint;
            int loserPoint = newLeagueData.loserPoint;

            stageTableA.winner = mode == 1 ? 1 : 2;
            stageTableB.winner = mode == 2 ? 1 : 2;
            stageTableA.scoreA = mode == 1 ? winnerPoint : loserPoint;
            stageTableB.scoreA = mode == 2 ? winnerPoint : loserPoint;
            stageTableA.scoreB = mode == 2 ? winnerPoint : loserPoint;
            stageTableB.scoreB = mode == 1 ? winnerPoint : loserPoint;

            if (runEffect) SpreadConfetti.StartSpread();
        }

        newLeagueData.stageRoots = SetLeagueStatistic(newLeagueData, posY);
        newLeagueData.stageRoots = SetLeagueStatistic(newLeagueData, posX);

        LeaguePainter.UpdateScore(newLeagueData, stageTableA, stageTableA.scoreObject);
        LeaguePainter.UpdateScore(newLeagueData, stageTableB, stageTableB.scoreObject);

        LeagueProvider.stageRoot stageRootA = newLeagueData.stageRoots[posY];
        LeagueProvider.stageRoot stageRootB = newLeagueData.stageRoots[posX];

        LeaguePainter.UpdateStatistic(stageRootA, stageRootA.statisticObjects, 1);
        LeaguePainter.UpdateStatistic(stageRootB, stageRootB.statisticObjects, 1);

        newLeagueData.stageRoots = SetLeagueRanking(newLeagueData.stageRoots);

        for (int i = 0; i < sumPeople; i++)
        {
            LeagueProvider.stageRoot stageRoot = newLeagueData.stageRoots[i];

            LeaguePainter.UpdateStatistic(stageRoot, stageRoot.statisticObjects, 2);
        }

        LeagueProvider.individualLeagueData = newLeagueData;

        LeagueProvider.individualLeagueData.isUpdate = 3;
    }

    public void ChangePlayerName(int id)
    {
        LeagueProvider.leagueData newLeagueData = LeagueProvider.individualLeagueData;

        LeagueProvider.stageRoot[] stageRoots = newLeagueData.stageRoots;

        LeagueProvider.leaguePlayerMenuObject playerObjects = LeagueProvider.individualLeaguePlayerMenuObjects;

        string playerName = UniversalFunction.GetInputFieldText(playerObjects.playerNameInput);

        stageRoots[id].playerName = playerName;

        for (int i = 0; i < 2; i++) UniversalFunction.SetButtonText(stageRoots[id].playerObjects[i], playerName);

        LeagueProvider.individualLeagueData = newLeagueData;
    }

    public void ChangePlayerColor(int id)
    {
        LeagueProvider.leagueData newLeagueData = LeagueProvider.individualLeagueData;

        LeagueProvider.stageRoot[] stageRoots = newLeagueData.stageRoots;

        LeagueProvider.leaguePlayerMenuObject playerObjects = LeagueProvider.individualLeaguePlayerMenuObjects;

        string inputPlayerColor = UniversalFunction.GetInputFieldText(playerObjects.playerColorInput);

        if (inputPlayerColor.Length % 3 != 0) return;

        Color playerColorBuffer;

        if (ColorUtility.TryParseHtmlString("#" + inputPlayerColor, out playerColorBuffer))
        {
            Color32 playerColor = playerColorBuffer;

            stageRoots[id].playerColor = playerColor;

            for (int i = 0; i < 2; i++)
            {
                stageRoots[id].playerObjects[i].GetComponent<Image>().color = playerColor;
                UniversalFunction.SetButtonText(stageRoots[id].playerObjects[i], 2, "", playerColor);
            }

            playerObjects.playerImage.color = playerColor;
            UniversalFunction.SetText(playerObjects.playerNumberText, 2, "", playerColor);

            LeagueProvider.individualLeagueData = newLeagueData;
        }
    }

    public void ChangeGamePoint()
    {
        LeagueProvider.leagueData newLeagueData = LeagueProvider.individualLeagueData;

        LeagueProvider.leagueConfigMenuObject configObjects = LeagueProvider.individualLeagueConfigMenuObjects;

        string winnerPointStringBuffer = UniversalFunction.GetInputFieldText(configObjects.winnerPointInput);
        string loserPointStringBuffer = UniversalFunction.GetInputFieldText(configObjects.loserPointInput);

        int winnerPoint = winnerPointStringBuffer != "" ? int.Parse(winnerPointStringBuffer) : 0;
        int loserPoint = loserPointStringBuffer != "" ? int.Parse(loserPointStringBuffer) : 0;

        if (winnerPointStringBuffer == "" || loserPointStringBuffer == "")
        {
            Button[] go = NotificationController.SetErrorNotification("得点を空白にすることはできません！");

            // Debug.Log("The point cannot be blank!");

            return;
        }

        newLeagueData.winnerPoint = winnerPoint;
        newLeagueData.loserPoint = loserPoint;

        FixStageTable();

        newLeagueData.stageRoots = SetAllStatistics(newLeagueData);

        LeagueProvider.individualLeagueData.isUpdate = 3;
    }

    public void ChangeGamePointWeight()
    {
        LeagueProvider.leagueData newLeagueData = LeagueProvider.individualLeagueData;

        LeagueProvider.leagueConfigMenuObject configObjects = LeagueProvider.individualLeagueConfigMenuObjects;

        string winPointWeightStringBuffer = UniversalFunction.GetInputFieldText(configObjects.winPointWeightInput);
        string losePointWeightStringBuffer = UniversalFunction.GetInputFieldText(configObjects.losePointWeightInput);

        int winPointWeight = winPointWeightStringBuffer != "" ? int.Parse(winPointWeightStringBuffer) : 0;
        int losePointWeight = losePointWeightStringBuffer != "" ? int.Parse(losePointWeightStringBuffer) : 0;

        if (winPointWeightStringBuffer == "" || losePointWeightStringBuffer == "")
        {
            Button[] go = NotificationController.SetErrorNotification("比重を空白にすることはできません！");

            // Debug.Log("The point weight cannot be blank!");

            return;
        }

        newLeagueData.winPointWeight = winPointWeight;
        newLeagueData.losePointWeight = losePointWeight;
        newLeagueData.stageRoots = SetAllStatistics(newLeagueData);

        LeagueProvider.individualLeagueData.isUpdate = 3;
    }

    LeagueProvider.stageRoot[] SetAllStatistics(LeagueProvider.leagueData leagueData)
    {
        LeagueProvider.stageRoot[] res = leagueData.stageRoots;

        int sumPeople = res.Length;

        res = SetLeagueStatistic(leagueData, -1);
        res = SetLeagueRanking(leagueData.stageRoots);

        for (int i = 0; i < sumPeople; i++)
        {
            LeagueProvider.stageRoot stageRoot = res[i];

            LeaguePainter.UpdateStatistic(stageRoot, stageRoot.statisticObjects, 0);
        }

        return res;
    }

    public void ResetAllResults()
    {
        LeagueProvider.leagueData leagueData = LeagueProvider.individualLeagueData;

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;

        int sumPeople = stageRoots.Length;

        for (int i = 0; i < sumPeople * sumPeople; i++)
        {
            int posX = i % sumPeople;
            int posY = i / sumPeople;

            if (posX != posY) SetScore(i, -1, false);
        }

        LeagueProvider.individualLeagueData.isUpdate = 3;
    }

    public void FixStageTable()
    {
        LeagueProvider.leagueData leagueData = LeagueProvider.individualLeagueData;

        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        int sumPeople = stageRoots.Length;

        for (int i = 0; i < sumPeople * sumPeople; i++)
        {
            int posX = i % sumPeople;
            int posY = i / sumPeople;

            if (posX == posY) continue;

            if (leagueData.isBooleanMode)
            {
                LeagueProvider.stageTable stageTable = stageTables[posY, posX];

                if (stageTable.scoreA > stageTable.scoreB)
                {
                    SetScore(i, 1, false);
                }
                else if (stageTable.scoreA < stageTable.scoreB)
                {
                    SetScore(i, 2, false);
                }
                else
                {
                    SetScore(i, -1, false);
                }
            }
            else
            {
                LeagueProvider.stageTable stageTable = stageTables[posY, posX];

                LeaguePainter.UpdateScore(leagueData, stageTable, stageTable.scoreObject);
            }
        }
    }

    public LeagueProvider.stageRoot[] SetLeagueStatistic(LeagueProvider.leagueData leagueData, int id)
    {
        LeagueProvider.stageRoot[] stageRoots = leagueData.stageRoots;
        LeagueProvider.stageTable[,] stageTables = leagueData.stageTables;

        int sumPeople = stageRoots.Length;

        LeagueProvider.stageRoot[] sub = leagueData.stageRoots;

        int sY = id < 0 ? 0 : id;
        int eY = id < 0 ? sumPeople : id + 1;

        for (int y = sY; y < eY; y++)
        {
            LeagueProvider.stageRoot stageRoot = sub[y];

            stageRoot.winPoint = 0;
            stageRoot.losePoint = 0;
            stageRoot.resultPoint = 0;

            for (int x = 0; x < sumPeople; x++)
            {
                if (x == y) continue;

                LeagueProvider.stageTable stageTable = stageTables[y, x];

                int winPoint = UniversalFunction.FixValueRange(stageTable.scoreA, 1, 0, 0);
                int losePoint = UniversalFunction.FixValueRange(stageTable.scoreB, 1, 0, 0);

                stageRoot.winPoint += winPoint;
                stageRoot.losePoint += losePoint;
                stageRoot.resultPoint += winPoint * leagueData.winPointWeight - losePoint * leagueData.losePointWeight;
            }

            sub[y] = stageRoot;
        }

        return sub;
    }

    public LeagueProvider.stageRoot[] SetLeagueRanking(LeagueProvider.stageRoot[] stageRoots)
    {
        LeagueProvider.stageRoot[] sub = stageRoots;

        int sumPeople = sub.Length;

        List<scoreItem> scoreItemsBuffer = new List<scoreItem>();
        List<scoreItem> scoreItems = new List<scoreItem>();

        for (int i = 0; i < sumPeople; i++)
        {
            scoreItem scoreItemBuffer = new scoreItem();
            scoreItemBuffer.id = i;
            scoreItemBuffer.score = sub[i].resultPoint;
            scoreItemsBuffer.Add(scoreItemBuffer);
        }

        for (int y = 0; y < sumPeople; y++)
        {
            int arrayLength = scoreItemsBuffer.Count;
            int idBuffer = scoreItemsBuffer[0].id;
            int scoreBuffer = scoreItemsBuffer[0].score;
            int indexBuffer = 0;

            for (int x = 1; x < arrayLength; x++)
            {
                if (scoreItemsBuffer[x].score > scoreBuffer)
                {
                    idBuffer = scoreItemsBuffer[x].id;
                    scoreBuffer = scoreItemsBuffer[x].score;
                    indexBuffer = x;
                }
            }

            scoreItem scoreItem = new scoreItem();
            scoreItem.id = idBuffer;
            scoreItem.score = scoreBuffer;
            scoreItems.Add(scoreItem);
            scoreItemsBuffer.RemoveAt(indexBuffer);
        }

        int rank = 1;
        int rankBuffer = 1;

        for (int i = 0; i < sumPeople; i++)
        {
            sub[scoreItems[i].id].ranking = rank;

            if (i != sumPeople - 1 && scoreItems[i].score != scoreItems[i + 1].score)
            {
                rank += rankBuffer;
                rankBuffer = 1;
            }
            else
            {
                rankBuffer++;
            }
        }

        return sub;
    }

    // Specific Function

    int GetWinner(int scoreA, int scoreB)
    {
        if (scoreA > scoreB)
        {
            return 1;
        }
        else if (scoreA < scoreB)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }
}
