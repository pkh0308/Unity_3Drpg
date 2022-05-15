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

    Dictionary<int, QuestData> questDic;

    public QuestManager()
    {
        questDic = new Dictionary<int, QuestData>();

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
                qd.SetQuestData(int.Parse(datas[0]), int.Parse(datas[1]), int.Parse(datas[2]), int.Parse(datas[3]), datas[4], datas[5]);
                questDic.Add(qd.QuestId, qd);

                for (int i = 6; i < datas.Length; i++)
                    qd.AddToConvList(int.Parse(datas[i]));

                line = questReader.ReadLine();
                if (line == null) break;
            }
        }
        questReader.Close();
    }

    public List<QuestData> QuestSearch(int id)
    {
        List<QuestData> qList = new List<QuestData>();

        foreach(KeyValuePair<int, QuestData> pair in questDic)
        {
            if (pair.Value.QuestNpc == id)
                qList.Add(questDic[pair.Key]);
        }
        return qList.Count > 0 ? qList : null;
    }
}
