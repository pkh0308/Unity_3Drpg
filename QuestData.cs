using System.Collections.Generic;
using UnityEngine;

public class QuestData
{
    int questId;
    int questStatus;
    int questCount;
    int questNpc;
    int targetId;
    string questName;
    string questDescription;
    public int QuestId { get { return questId; } }
    public int QuestStatus { get { return questStatus; } }
    public int QuestCount { get { return questCount; } }
    public int QuestNpc { get { return questNpc; } }
    public int TargetId { get { return targetId; } }
    public string QuestName { get { return questName; } }
    public string QuestDescription { get { return questDescription; } }

    public enum QuestStatusType { NotBegin = 0, OnGoing = 1, FullFill = 2, Cleared = 3 }
    public enum QuestType { Collect, Kill }
    public QuestType type;

    int curCount;
    public int CurCount { get { return curCount; } }
    int convIdx;
    public readonly List<int> convList;
    public readonly List<int[]> rewardList;

    public QuestData()
    {
        convList = new List<int>();
        rewardList = new List<int[]>();
        convIdx = 0;
    }
    
    public void SetQuestData(int id, int npc, int count, int target, string type, string name, string description)
    {
        questId = id;
        questCount = count;
        questNpc = npc;
        questStatus = (int)QuestStatusType.NotBegin;
        targetId = target;
        this.type = type == QuestType.Collect.ToString() ? QuestType.Collect : QuestType.Kill;
        questName = name;
        questDescription = description;

        curCount = 0;
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
        curCount = Mathf.Clamp(curCount + cnt, 0, questCount);
        if (curCount == questCount) 
            questStatus = (int)QuestStatusType.FullFill;
    }

    public void SetStatus(int status)
    {
        questStatus = status;
    }
}
