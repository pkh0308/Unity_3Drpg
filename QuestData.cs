using System.Collections.Generic;
using UnityEngine;

public class QuestData
{
    public readonly int questId;
    public readonly int maxCount;
    public readonly int questNpc;
    public readonly int targetId;
    public readonly string questName;
    public readonly string questDescription;

    int questStatus;
    public int QuestStatus { get { return questStatus; } }
    int curCount;
    public int CurCount { get { return curCount; } }

    public enum QuestStatusType { NotBegin = 0, OnGoing = 1, FullFill = 2, Cleared = 3 }
    public enum QuestType { Collect, Kill }
    public QuestType type; 

    int convIdx;
    public readonly List<int> convList;
    public readonly List<int[]> rewardList;

    public QuestData(int id, int npc, int count, int target, string type, string name, string description)
    {
        convList = new List<int>();
        rewardList = new List<int[]>();
        convIdx = 0;
        curCount = 0;

        questId = id;
        maxCount = count;
        questNpc = npc;
        questStatus = (int)QuestStatusType.NotBegin;
        targetId = target;
        this.type = type == QuestType.Collect.ToString() ? QuestType.Collect : QuestType.Kill;
        questName = name;
        questDescription = description;
    }

    public void AddToConvList(int convId)
    {
        convList.Add(convId);
    }

    public void AddToRewardList(int[] reward)
    {
        rewardList.Add(reward);
    }

    public int GetConvId()
    {
        return convList[convIdx];
    }

    public void ConvIdxUp()
    {
        convIdx++;
    }

    public void QuestCountUp(int cnt)
    {
        curCount = Mathf.Clamp(curCount + cnt, 0, maxCount);
        if (curCount == maxCount) 
            questStatus = (int)QuestStatusType.FullFill;
    }

    public void SetStatus(int status)
    {
        questStatus = status;
    }
}
