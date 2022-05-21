using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TournamentMaker : MonoBehaviour
{
    [HideInInspector] private string useRandomString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // Usable Function

    public TournamentProvider.tournamentData SetInitialTournamentData(int sumPeople, int numGroup)
    {
        string[] playerName = new string[sumPeople];

        for (int i = 0; i < sumPeople; i++) playerName[i] = "John Doe " + UniversalFunction.AlignDigitsToMaxNum(i, sumPeople);

        TournamentProvider.tournamentData tournamentData = SetInitialStage(sumPeople, playerName, numGroup);

        return tournamentData;
    }

    public TournamentProvider.tournamentData SetInitialTournamentData(string[] playerList, int numGroup)
    {
        int sumPeople = playerList.Length;

        TournamentProvider.tournamentData tournamentData = SetInitialStage(sumPeople, playerList, numGroup);

        return tournamentData;
    }

    public TournamentProvider.tournamentData SetInitialStage(int sumPeople, string[] playerName, int numGroup)
    {
        if (sumPeople <= 1) return null;

        int stageHeight = sumPeople > numGroup ? UniversalFunction.CalcValeToClosePow((float)sumPeople, (float)numGroup) : 1;
        int stageWidth = UniversalFunction.CeilDivideIntVale(sumPeople, numGroup);

        TournamentProvider.tournamentData tournamentData = new TournamentProvider.tournamentData();

        TournamentProvider.stageRoot[] stageRoots = new TournamentProvider.stageRoot[sumPeople];
        TournamentProvider.stageCollider[,] stageColliders = new TournamentProvider.stageCollider[stageHeight, stageWidth];
        TournamentProvider.stageProperty[] stageProperties = new TournamentProvider.stageProperty[stageHeight];

        for (int x = 0; x < sumPeople; x++)
        {
            stageRoots[x] = SetInitialStageRoot(stageHeight, sumPeople, playerName[x], x);
        }

        for (int y = 0; y < stageHeight; y++)
        {
            stageProperties[y] = SetInitialStageProperty(stageProperties, sumPeople, numGroup, y);

            for (int x = 0; x < stageWidth; x++)
            {
                stageColliders[y, x] = SetInitialStageCollider(stageRoots, stageColliders, stageProperties, sumPeople, y, x);
            }
        }

        stageRoots = FixInitialStageRoot(stageRoots, stageColliders, stageProperties);

        tournamentData.stageRoots = stageRoots;
        tournamentData.stageColliders = stageColliders;
        tournamentData.stageProperties = stageProperties;

        UniversalFunction.SetInitState
        (
            UniversalFunction.GenerateRandomString(useRandomString, 32)
        );

        tournamentData.hash = UniversalFunction.GenerateRandomString(useRandomString, 32);
        tournamentData.defaultNumGroup = numGroup;

        return tournamentData;
    }

    // Specific Function

    TournamentProvider.stageRoot SetInitialStageRoot(int stageHeight, int sumPeople, string playerName, int x)
    {
        TournamentProvider.stageRoot sub = new TournamentProvider.stageRoot();

        float drawRatioBranchPosX = (float)x / (float)(sumPeople - 1);

        sub.playerName = playerName;
        sub.startDrawRatioBranchPos.x = drawRatioBranchPosX;
        sub.endDrawRatioBranchPos.x = drawRatioBranchPosX;
        sub.startDrawRatioBranchPos.y = 0f;
        sub.endDrawRatioBranchPos.y = 1f / (float)(stageHeight + 1);

        sub.currentPos = sub.startDrawRatioBranchPos;

        return sub;
    }

    TournamentProvider.stageRoot[] FixInitialStageRoot
    (
        TournamentProvider.stageRoot[] stageRoots,
        TournamentProvider.stageCollider[,] stageColliders,
        TournamentProvider.stageProperty[] stageProperties
    )
    {
        TournamentProvider.stageRoot[] sub = stageRoots;

        for (int x = 0; x < stageProperties[0].sumBranch; x++)
        {
            if (stageColliders[0, x].type == 1) stageRoots[stageColliders[0, x].connectBranch[0]].currentPos.y = stageColliders[0, x].startDrawRatioBranchPos.y;
        }

        return sub;
    }

    TournamentProvider.stageProperty SetInitialStageProperty
    (
        TournamentProvider.stageProperty[] stageProperties,
        int sumPeople,
        int numGroup,
        int y
    )
    {
        TournamentProvider.stageProperty sub = new TournamentProvider.stageProperty();

        if (y == 0)
        {
            sub.reverseOrder = false;
            sub.incompleteGroup = sumPeople % numGroup;
            sub.sumGame = sumPeople / numGroup;
            sub.sumBranch = UniversalFunction.CeilDivideIntVale(sumPeople, numGroup);
        }
        else
        {
            TournamentProvider.stageProperty upperStageProperties = stageProperties[y - 1];
            int upperBranch = upperStageProperties.sumGame + (upperStageProperties.incompleteGroup != 0 ? 1 : 0);

            sub.reverseOrder = upperStageProperties.incompleteGroup != 0 ? !upperStageProperties.reverseOrder : upperStageProperties.reverseOrder;
            sub.incompleteGroup = upperBranch % numGroup;
            sub.sumGame = upperBranch / numGroup;
            sub.sumBranch = UniversalFunction.CeilDivideIntVale(upperBranch, numGroup);
        }

        sub.numGroup = numGroup;

        return sub;
    }

    TournamentProvider.stageCollider SetInitialStageCollider
    (
        TournamentProvider.stageRoot[] stageRoots,
        TournamentProvider.stageCollider[,] stageColliders,
        TournamentProvider.stageProperty[] stageProperties,
        int sumPeople,
        int y,
        int x
    )
    {
        int stageHeight = stageColliders.GetLength(0);
        int stageWidth = stageColliders.GetLength(1);

        TournamentProvider.stageCollider sub = new TournamentProvider.stageCollider();

        sub.connectBranch = new int[stageProperties[y].numGroup];

        float drawRatioJointPosY = (float)(y + 1) / (float)(stageHeight + 1);

        sub.startDrawRatioBranchPos.y = drawRatioJointPosY;
        sub.endDrawRatioBranchPos.y = (float)(y + 2) / (float)(stageHeight + 1);
        sub.startDrawRatioJointPos.y = drawRatioJointPosY;
        sub.endDrawRatioJointPos.y = drawRatioJointPosY;

        if (y == 0)
        {
            sub.type = (sumPeople % stageProperties[y].numGroup != 0 && x == stageWidth - 1) ? stageProperties[y].incompleteGroup : stageProperties[y].numGroup;

            float[] drawRatioWidthArrayX = new float[sub.type];

            for (int i = 0; i < sub.type; i++)
            {
                sub.connectBranch[i] = x * stageProperties[y].numGroup + i;
                drawRatioWidthArrayX[i] = stageRoots[sub.connectBranch[i]].startDrawRatioBranchPos.x;
            }

            if (sub.type == 1) sub.winner = sub.connectBranch[0];

            float drawRatioBranchPosX = drawRatioWidthArrayX.Average();

            sub.startDrawRatioBranchPos.x = drawRatioBranchPosX;
            sub.endDrawRatioBranchPos.x = drawRatioBranchPosX;
            sub.startDrawRatioJointPos.x = drawRatioWidthArrayX[0];
            sub.endDrawRatioJointPos.x = drawRatioWidthArrayX[sub.type - 1];
        }
        else if (x < stageProperties[y].sumBranch)
        {
            int u = stageProperties[y].reverseOrder ? stageProperties[y].sumBranch - x - 1 : x;

            sub.type = (stageProperties[y].incompleteGroup != 0 && u == stageProperties[y].sumBranch - 1) ? stageProperties[y].incompleteGroup : stageProperties[y].numGroup;

            float[] drawRatioWidthArrayX = new float[sub.type];

            int shift =
            (
                stageProperties[y].incompleteGroup != 0 && sub.type == stageProperties[y].numGroup && stageProperties[y].reverseOrder ?
                (stageProperties[y].numGroup - stageProperties[y].incompleteGroup) * -1 :
                0
            );

            for (int i = 0; i < sub.type; i++)
            {
                sub.connectBranch[i] = x * stageProperties[y].numGroup + shift + i;
                drawRatioWidthArrayX[i] = stageColliders[y - 1, sub.connectBranch[i]].startDrawRatioBranchPos.x;
            }

            if (sub.type == 1) sub.winner = sub.connectBranch[0];

            float drawRatioBranchPosX = y != stageHeight - 1 ? drawRatioWidthArrayX.Average() : 0.5f;

            sub.startDrawRatioBranchPos.x = drawRatioBranchPosX;
            sub.endDrawRatioBranchPos.x = drawRatioBranchPosX;
            sub.startDrawRatioJointPos.x = drawRatioWidthArrayX[0];
            sub.endDrawRatioJointPos.x = drawRatioWidthArrayX[sub.type - 1];
        }

        return sub;
    }
}
