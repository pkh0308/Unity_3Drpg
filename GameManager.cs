using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] UiManager uiManager;
    [SerializeField] CursorManager cursorManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] PlayerQuest playerQuest;

    bool pause;
    public bool Pause { get { return pause; } }
    bool isDraging;
    public bool IsDraging { get { return isDraging; } }
    int tempQuestId;

    Dictionary<int, string[]> convDic;
    Dictionary<int, int> npcConvMatchDic;
    Dictionary<int, string[]> npcDataDic;

    Dictionary<int, int> invenDic;
    int[] invenArr;
    [SerializeField] ItemSlot[] invenSlots;

    public static Action<int, int> exchangeSlots;
    public static Action<bool> setBoolDrag;
    public static Action<int, int, int> startQuestConv;

    void Awake()
    {
        convDic = new Dictionary<int, string[]>();
        npcConvMatchDic = new Dictionary<int, int>();
        npcDataDic = new Dictionary<int, string[]>();
        invenDic = new Dictionary<int, int>();
        invenArr = new int[invenSlots.Length];

        exchangeSlots = (a, b) => { ExchangeSlots(a, b); };
        setBoolDrag = (a) => { SetBoolDrag(a); };
        startQuestConv = (a, b, c) => { Conv_StartQuest(a, b, c); };

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

        // npc 데이터 세팅
        TextAsset npcData = Resources.Load("npcData") as TextAsset;
        StringReader npcDataReader = new StringReader(npcData.text);

        while (npcDataReader != null)
        {
            string line = npcDataReader.ReadLine();
            if (line == null) break;

            line = npcDataReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                // 0 : id, 1 : name, 2 : hasShop(상점 보유 여부)
                npcDataDic.Add(int.Parse(datas[0]), new string[] { datas[1], datas[2] });

                line = npcDataReader.ReadLine();
                if (line == null) break;
            }
        }
        npcDataReader.Close();
    }

    public void SetPause(bool act)
    {
        pause = act;
    }

    public void Conv_Start(string npcName, int npcId, bool hasShop)
    {
        int key = npcConvMatchDic[npcId];
        uiManager.Conv_Set(npcName, convDic[key], hasShop);
        uiManager.Conv_SetActive(true);
        uiManager.UpdateQuestPanels(npcId);
        if (hasShop) shopManager.SetShopSlots(npcId);
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
        pause = true;
    }

    public void Conv_StartQuest(int convId, int questId, int questStatus)
    {
        uiManager.Conv_QuestSet(convDic[convId], questStatus);
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
        QuestManager.Instance.AddPlayerQuest(tempQuestId);
        QuestManager.Instance.SetQuestStatus(tempQuestId, (int)QuestData.QuestStatusType.OnGoing);
        playerQuest.QuestAccept(tempQuestId);
    }

    public void Conv_QuestClearBtn()
    {
        Conv_ExitBtn();
        QuestManager.Instance.RemovePlayerQuest(tempQuestId);
        playerQuest.QuestClear(tempQuestId);
        uiManager.SetQuestDesc(null);
        QuestManager.Instance.SetQuestStatus(tempQuestId, (int)QuestData.QuestStatusType.Cleared);
    }

    public void ProgressStart(string name, float time)
    {
        uiManager.ControllProgressBarSet(name, time);
    }

    public void GetItem(int id, int count)
    {
        //골드일 경우
        if(id > 90000)
        {
            if(GoodsManager.Instance.GetGold(count) == false) Debug.Log("GetGold Failed...");
            uiManager.UpdateGold();
            return;
        }

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
        
        QuestManager.Instance.UpdateCollectQuest(id, count);
        uiManager.UpdateQuestDescCount();
        //인벤토리에 없던 아이템 습득 시
        if (invenDic[id] == 0)
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

    public void SpendItem(int id, int count)
    {
        if (invenSlots[Array.IndexOf(invenArr, id)].SpendItem(count) == false)
        {
            Debug.Log("SpendItem Failed...");
            return;
        }
        invenDic[id] -= count;
        if (invenDic[id] == 0)
            invenArr[GetItemSlotIdx(id)] = 0;
    }

    public int GetItemCount(int id)
    {
        if (invenDic.ContainsKey(id) == false)
            return 0;
        else
            return invenDic[id];
    }

    public int GetItemSlotIdx(int id)
    {
        for (int idx = 0; idx < invenArr.Length; idx++)
        {
            if(invenArr[idx] == id)
                return idx;
        }
        return -1;
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

    //[BurstCompile(CompileSynchronously = true)]
}
