using System.Collections.Generic;

public class QuestData
{
    int questId;
    int questStatus;
    int questCount;
    int questNpc;
    int targetId;
    string questName;
    public int QuestId { get { return questId; } }
    public int QuestStatus { get { return questStatus; } }
    public int QuestCount { get { return questCount; } }
    public int QuestNpc { get { return questNpc; } }
    public int TargetId { get { return targetId; } }
    public string QuestName { get { return questName; } }

    public enum QuestStatusType { NotBegin = -1, OnGoing = 0, Cleared = 1 }
    public enum QuestType { Collect, Kill }
    public QuestType type;

    int curCount;
    List<int> convList;

    public QuestData()
    {
        convList = new List<int>();
    }
    
    public void SetQuestData(int id, int npc, int count, int target, string type, string name)
    {
        questId = id;
        questCount = count;
        questNpc = npc;
        questStatus = (int)QuestStatusType.NotBegin;
        targetId = target;
        this.type = type == QuestType.Collect.ToString() ? QuestType.Collect : QuestType.Kill;
        questName = name;
    }

    public void AddToConvList(int convId)
    {
        convList.Add(convId);
    }

    public int GetConvId()
    {
        return convList[questStatus];
    }

    public void QuestCountUp(int cnt)
    {
        curCount += cnt;
        if (curCount >= questCount) questStatus = 1;
    }
}
