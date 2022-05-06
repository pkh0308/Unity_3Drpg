using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public UiManager uiManager;

    bool pause;
    public bool Pause { get { return pause; } }
    bool invenOpen;
    public bool InvenOpen { get { return invenOpen; } }

    Dictionary<int, string[]> convDic;
    Dictionary<int, int> npcConvMatchDic;

    void Awake()
    {
        convDic = new Dictionary<int, string[]>();
        npcConvMatchDic = new Dictionary<int, int>();
        Initilaize();
    }

    void Initilaize()
    {
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

    public void StartConversation(string npcName, int npcId)
    {
        int key = npcConvMatchDic[npcId];
        uiManager.SetConversation(npcName, convDic[key]);

        uiManager.ControllConversationSet(true);
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
}
