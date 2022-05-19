using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] UiManager uiManager;
    [SerializeField] CursorManager cursorManager;
    [SerializeField] PlayerQuest playerQuest;

    bool pause;
    public bool Pause { get { return pause; } }
    bool isDraging;
    public bool IsDraging { get { return isDraging; } }
    int tempQuestId;

    Dictionary<int, string[]> convDic;
    Dictionary<int, int> npcConvMatchDic;

    Dictionary<int, int> invenDic;
    int[] invenArr;
    [SerializeField] ItemSlot[] invenSlots;

    public static Action<int, int> exchangeSlots;
    public static Action<bool> setBoolDrag;
    public static Action<int, int> startQuestConv;

    void Awake()
    {
        convDic = new Dictionary<int, string[]>();
        npcConvMatchDic = new Dictionary<int, int>();
        invenDic = new Dictionary<int, int>();
        invenArr = new int[invenSlots.Length];

        exchangeSlots = (a, b) => { ExchangeSlots(a, b); };
        setBoolDrag = (a) => { SetBoolDrag(a); };
        startQuestConv = (a, b) => { Conv_StartQuest(a, b); };

        for (int i = 0; i < invenSlots.Length; i++)
            invenSlots[i].SetIdx(i);

        Initilaize();
    }

    void Initilaize()
    {
        // 대화 내용 저장
        TextAsset convText = Resources.Load("conversationData") as TextAsset;
        StringReader convReader = new StringReader(convText.text);

        while (convReader != null)
        {
            string line = convReader.ReadLine();
            if (line == null) break;

            line = convReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split('@');
                string[] conversations = new string[datas.Length - 1];
                Array.Copy(datas, 1, conversations, 0, datas.Length - 1);
                convDic.Add(int.Parse(datas[0]), conversations);

                line = convReader.ReadLine();
                if (line == null) break;
            }
        }
        convReader.Close();

        // npc별 대화 번호 저장
        TextAsset npcConvMatch = Resources.Load("npcConversation") as TextAsset;
        StringReader npcConvReader = new StringReader(npcConvMatch.text);

        while (npcConvReader != null)
        {
            string line = npcConvReader.ReadLine();
            if (line == null) break;

            line = npcConvReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                npcConvMatchDic.Add(int.Parse(datas[0]), int.Parse(datas[1]));

                line = npcConvReader.ReadLine();
                if (line == null) break;
            }
        }
        npcConvReader.Close();
    }

    public void SetPause(bool act)
    {
        pause = act;
    }

    public void Conv_Start(string npcName, int npcId)
    {
        int key = npcConvMatchDic[npcId];
        uiManager.Conv_Set(npcName, convDic[key]);

        uiManager.Conv_SetActive(true);
        uiManager.SetQuestPanels(npcId);
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
        pause = true;
    }

    public void Conv_StartQuest(int convId, int questId)
    {
        uiManager.Conv_QuestSet(convDic[convId]);
        uiManager.Conv_SetActive(true);
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
        tempQuestId = questId;
        pause = true;
    }

    public void Conv_ExitBtn()
    {
        uiManager.Conv_SetActive(false);
        pause = false;
    }

    public void Conv_QuestAcceptBtn()
    {
        Conv_ExitBtn();
        playerQuest.QuestAccept(tempQuestId);
        QuestManager.Instance.SetQuestStatus(tempQuestId, (int)QuestData.QuestStatusType.OnGoing);
    }

    public void ProgressStart(string name, float time)
    {
        uiManager.ControllProgressBarSet(name, time);
    }

    public void GetItem(int id, int count)
    {
        //최초 획득 시
        if(invenDic.ContainsKey(id) == false)
        {
            if (invenArr.Min() > 0)
            {
                //인벤토리가 꽉 찼을 때 습득 시 처리
            }
            else
                invenDic.Add(id, 0);
        }

        //인벤토리에 없던 아이템 습득 시
        if(invenDic[id] == 0)
        {
            if(invenArr.Min() > 0)
            {
                //인벤토리가 꽉 찼을 때 습득 시 처리
            }
            else
            {
                for(int i = 0; i < invenArr.Length; i++)
                {
                    if (invenArr[i] > 0) continue;

                    invenArr[i] = id;
                    break;
                }
                invenDic[id] += count;
                int idx = Array.IndexOf(invenArr, id);
                invenSlots[idx].SetData(id, count);
            }
            return;
        }

        //인벤토리에 있는 아이템 습득 시
        invenSlots[Array.IndexOf(invenArr, id)].AddItem(count);
        invenDic[id] += count;
    }

    public void ExchangeSlots(int idx_1, int idx_2)
    {
        int temp = invenArr[idx_1];
        invenArr[idx_1] = invenArr[idx_2];
        invenArr[idx_2] = temp;
    }

    public void SetBoolDrag(bool act)
    {
        isDraging = act;
    }

    public void GameSave()
    {
        GoodsManager.Instance.Save();
    }
}
