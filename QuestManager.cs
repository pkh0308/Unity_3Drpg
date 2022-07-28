using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class QuestManager
{
    private static QuestManager instance;
    public static QuestManager Instance
    {
        get
        {
            if (instance == null) instance = new QuestManager();

            return instance;
        }
    }

    readonly Dictionary<int, QuestData> questIdDic;
    readonly List<int> npcIds;
    readonly Dictionary<int, List<QuestData>> npcIdDic;
    public Dictionary<int, QuestData> playerQuestDic;

    public QuestManager()
    {
        questIdDic = new Dictionary<int, QuestData>();
        npcIds = new List<int>();
        npcIdDic = new Dictionary<int, List<QuestData>>();
        playerQuestDic = new Dictionary<int, QuestData>();
        
        //퀘스트 데이터 읽어오기
        TextAsset questData = Resources.Load("questData") as TextAsset;
        StringReader questReader = new StringReader(questData.text);

        while (questReader != null)
        {
            string line = questReader.ReadLine();
            if (line == null) break;

            line = questReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                int npcId = int.Parse(datas[1]);
                //0 : QuesId, 1 : NpcId, 2 : QuestCount, 3 : TargetId, 4 : QuestType, 5 : QuestName, 6 : QuestDescription, 7 : Rewards, 8 : ConvIds
                QuestData qd = new QuestData(int.Parse(datas[0]), npcId, int.Parse(datas[2]), int.Parse(datas[3]), datas[4], datas[5], datas[6]);
                questIdDic.Add(qd.questId, qd);
                if (!npcIds.Contains(npcId)) npcIds.Add(npcId);

                string[] rewards = datas[7].Split('-');
                string[] convIds = datas[8].Split('-');

                for (int i = 0; i < rewards.Length; i += 2)
                    qd.AddToRewardList(new int[] { int.Parse(rewards[i]), int.Parse(rewards[i + 1]) });
                for (int i = 0; i < convIds.Length; i++)
                    qd.AddToConvList(int.Parse(convIds[i]));

                    line = questReader.ReadLine();
                if (line == null) break;
            }
        }
        questReader.Close();

        //npcId로 분류해서 저장
        for(int i = 0; i < npcIds.Count; i++)
        {
            List<QuestData> qList = new List<QuestData>();
            int npcId = npcIds[i];

            foreach (KeyValuePair<int, QuestData> pair in questIdDic)
            {
                if (pair.Value.questNpc == npcId)
                    qList.Add(questIdDic[pair.Key]);
            }
            npcIdDic.Add(npcId, qList);
        }
    }

    public List<QuestData> GetQuestDatas(int id)
    {
        if (npcIdDic.TryGetValue(id, out List<QuestData> list))
            return list;
        else
            return null;
    }

    public void AddPlayerQuest(int questId)
    {
        playerQuestDic.Add(questId, questIdDic[questId]);
    }

    public void RemovePlayerQuest(int questId)
    {
        playerQuestDic.Remove(questId);
    }

    public QuestData GetDataById(int questId)
    {
        return questIdDic[questId];
    }

    public void SetQuestStatus(int questId, int status)
    {
        questIdDic[questId].SetStatus(status);
    }

    public void UpdateCollectQuest(int itemId, int count)
    {
        bool needUpdate = false;
        List<int> list = playerQuestDic.Keys.ToList();
        for(int i = 0; i < list.Count; i++)
        {
            QuestData data = playerQuestDic[list[i]];
            if (data.type != QuestData.QuestType.Collect) continue;
            if (data.targetId != itemId) continue;

            data.QuestCountUp(count);
            if (data.QuestStatus == (int)QuestData.QuestStatusType.FullFill) needUpdate = true;
        }
        if (needUpdate) UiManager.updateQuestPanel();
    }

    public void UpdateKillQuest(int enemyId)
    {
        bool needUpdate = false;
        List<int> list = playerQuestDic.Keys.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            QuestData data = playerQuestDic[list[i]];
            if (data.type != QuestData.QuestType.Kill) continue;
            if (data.targetId != enemyId) continue;

            data.QuestCountUp(1);
            if (data.QuestStatus == (int)QuestData.QuestStatusType.FullFill) needUpdate = true;
        }
        if (needUpdate) UiManager.updateQuestPanel();
    }
}
