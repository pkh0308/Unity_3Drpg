using System.Collections.Generic;
using System.IO;
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

    public QuestManager()
    {
        questIdDic = new Dictionary<int, QuestData>();
        npcIds = new List<int>();
        npcIdDic = new Dictionary<int, List<QuestData>>();

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
                QuestData qd = new QuestData();
                int npcId = int.Parse(datas[1]);
                //0 : QuesId, 1 : NpcId, 2 : QuestCount, 3 : TargetId, 4 : QuestType, 5 : QuestName
                qd.SetQuestData(int.Parse(datas[0]), npcId, int.Parse(datas[2]), int.Parse(datas[3]), datas[4], datas[5]);
                questIdDic.Add(qd.QuestId, qd);
                if (!npcIds.Contains(npcId)) npcIds.Add(npcId);

                for (int i = 6; i < datas.Length; i++)
                    qd.AddToConvList(int.Parse(datas[i]));

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
                if (pair.Value.QuestNpc == npcId)
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

    public QuestData GetDataById(int questId)
    {
        return questIdDic[questId];
    }

    public void SetQuestStatus(int id, int status)
    {
        questIdDic[id].SetStatus(status);
    }
}
