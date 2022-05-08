using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public UiManager uiManager;
    public CursorManager cursorManager;

    bool pause;
    public bool Pause { get { return pause; } }
    bool invenOpen;
    public bool InvenOpen { get { return invenOpen; } }

    Dictionary<int, string[]> convDic;
    Dictionary<int, int> npcConvMatchDic;

    Dictionary<int, ItemData> itemDic;
    Dictionary<int, int> invenDic;
    List<int> invenList;
    [SerializeField] ItemSlot[] invenSlots;

    void Awake()
    {
        convDic = new Dictionary<int, string[]>();
        npcConvMatchDic = new Dictionary<int, int>();
        itemDic = new Dictionary<int, ItemData>();
        invenDic = new Dictionary<int, int>();
        invenList = new List<int>();

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
                System.Array.Copy(datas, 1, conversations, 0, datas.Length - 1);
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

        // 아이템 정보 저장
        TextAsset itemList = Resources.Load("itemDictionary") as TextAsset;
        StringReader itemReader = new StringReader(itemList.text);

        while (itemReader != null)
        {
            string line = itemReader.ReadLine();
            if (line == null) break;

            line = itemReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split('@');
                int id = int.Parse(datas[0]);
                ItemData item = new ItemData(datas[1], datas[2]);
                itemDic.Add(id, item);
                invenDic.Add(id, 0);

                line = itemReader.ReadLine();
                if (line == null) break;
            }
        }
        itemReader.Close();
    }

    public void SetPause(bool act)
    {
        pause = act;
    }

    public void StartConversation(string npcName, int npcId)
    {
        int key = npcConvMatchDic[npcId];
        uiManager.SetConversation(npcName, convDic[key]);

        uiManager.ControllConversationSet(true);
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
        pause = true;
    }

    public void ExitConversation()
    {
        uiManager.ControllConversationSet(false);
        pause = false;
    }

    public void InventoryControll()
    {
        invenOpen = uiManager.ControllInventorySet();
    }

    public void ProgressStart(string name, float time)
    {
        uiManager.ControllProgressBarSet(name, time);
    }

    public void GetItem(int id, int count)
    {
        if(invenDic[id] == 0)
        {
            if(invenList.Count == invenSlots.Length)
            {
                //인벤토리가 꽉 찼을 때 습득 시 처리
            }
            else
            {
                invenList.Add(id);
                invenDic[id] += count;
                int idx = invenList.IndexOf(id);
                invenSlots[idx].SetImg(id, count);
            }
            return;
        }

        invenSlots[invenList.IndexOf(id)].ItemCount(count);
        invenDic[id] += count;
    }
}
