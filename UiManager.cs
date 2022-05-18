using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class UiManager : MonoBehaviour
{
    public GameObject conversationSet;
    public GameObject inventorySet;
    public GameObject convQuestSet;
    public GameObject questSet;
    public TextMeshProUGUI goldText;

    public GameObject itemDescription;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescText;
    bool descriptionOn;

    public GameObject progressBarSet;
    public TextMeshProUGUI progressText;
    public Image progressBar;

    public TextMeshProUGUI conv_NpcName;
    public TextMeshProUGUI conv_ConversationText;
    public GameObject conv_ExitBtn;
    public GameObject conv_QuestBtn;
    public GameObject conv_AccpetBtn;
    public GameObject conv_NextBtn;
    [SerializeField] float conv_speed;
    bool isTexting;
    bool isQuest;
    Coroutine co_Texting;

    int conv_Idx;
    string[] currentConversation;

    [SerializeField] GameObject noQuestText;
    [SerializeField] QuestPanel[] questPanels;
    Dictionary<int, ItemData> itemDic;

    public static Action itemDescOff;
    //public static Action<List<QuestData>> setQuestPanel;

    void Awake()
    {
        itemDescOff = () => { ItemDescOff(); };
        //setQuestPanel = (a) => { SetQuestPanels(a); };

        itemDic = new Dictionary<int, ItemData>();
        GoldUpdate();
        Initialize();
    }

    void Initialize()
    {
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

                line = itemReader.ReadLine();
                if (line == null) break;
            }
        }
        itemReader.Close();
    }

    public void Conv_SetActive(bool act)
    {
        conversationSet.SetActive(act);

        if(act)
        {
            convQuestSet.SetActive(false);
            conv_AccpetBtn.SetActive(false);
        }
    }

    public void Conv_Set(string npcName, string[] texts)
    {
        conv_Idx = 0;
        isQuest = false;
        conv_NpcName.text = npcName;
        currentConversation = texts;

        conv_ExitBtn.SetActive(texts.Length == 1);
        conv_QuestBtn.SetActive(texts.Length == 1);
        co_Texting = StartCoroutine(Conv_Texting());
    }

    public void Conv_QuestSet(string[] texts)
    {
        conv_Idx = 0;
        isQuest = true;
        currentConversation = texts;
        conv_QuestBtn.SetActive(false);

        conv_ExitBtn.SetActive(texts.Length == 1);
        conv_AccpetBtn.SetActive(texts.Length == 1);
        co_Texting = StartCoroutine(Conv_Texting());
    }

    public void Conv_NextBtn()
    {
        conv_Idx++;
        conv_ExitBtn.SetActive(false);
        conv_QuestBtn.SetActive(false);
        conv_NextBtn.SetActive(false);
        co_Texting = StartCoroutine(Conv_Texting());
    }

    IEnumerator Conv_Texting()
    {
        isTexting = true;
        string line = currentConversation[conv_Idx];
        string temp = "";
        WaitForSeconds seconds = new WaitForSeconds(conv_speed);

        for(int i = 0; i < line.Length; i++)
        {
            temp += line[i];
            conv_ConversationText.text = temp;
            yield return seconds;
        }
        isTexting = false;

        if(isQuest)
        {
            if (conv_Idx == currentConversation.Length - 1)
            {
                conv_ExitBtn.SetActive(true);
                conv_AccpetBtn.SetActive(true);
            }
            else
                conv_NextBtn.SetActive(true);
        }
        else
        {
            if (conv_Idx == currentConversation.Length - 1)
            {
                conv_ExitBtn.SetActive(true);
                conv_QuestBtn.SetActive(true);
            }
            else
                conv_NextBtn.SetActive(true);
        }
    }

    public void Conv_CencleTexting()
    {
        if (isTexting == false) return;

        StopCoroutine(co_Texting);
        isTexting = false;
        conv_ConversationText.text = currentConversation[conv_Idx];

        if (isQuest)
        {
            if (conv_Idx == currentConversation.Length - 1)
            {
                conv_ExitBtn.SetActive(true);
                conv_AccpetBtn.SetActive(true);
            }
            else
                conv_NextBtn.SetActive(true);

            return;
        }

        if (conv_Idx == currentConversation.Length - 1)
        {
            conv_ExitBtn.SetActive(true);
            conv_QuestBtn.SetActive(true);
        }
        else
            conv_NextBtn.SetActive(true);
    }

    public void Conv_QuestOpenBtn()
    {
        if(convQuestSet.activeSelf) convQuestSet.SetActive(false);
        else convQuestSet.SetActive(true);
    }

    public void ControlInventorySet()
    {
        if (!inventorySet.activeSelf)
            inventorySet.SetActive(true);
        else
            inventorySet.SetActive(false);
    }

    public void ControlQuestSet()
    {
        if (!questSet.activeSelf)
            questSet.SetActive(true);
        else
            questSet.SetActive(false);
    }

    public void ItemDescOn(int id)
    {
        if (id == 0) { ItemDescOff(); return; }
        if (descriptionOn) return;

        itemNameText.text = itemDic[id].itemName;
        itemDescText.text = itemDic[id].itemDescription;
        descriptionOn = true;
        itemDescription.transform.position = Input.mousePosition;
        itemDescription.SetActive(true);
    }

    public void ItemDescOff()
    {
        if (!descriptionOn) return;

        descriptionOn = false;
        itemDescription.SetActive(false);
    }

    public void ControllProgressBarSet(string name, float time)
    {
        switch(name)
        {
            case "Collectable":
                progressText.text = "채집중...";
                break;
        }
        StartCoroutine(Progress(time));
    }

    IEnumerator Progress(float time)
    {
        progressBarSet.SetActive(true);
        Vector3 barScale = Vector3.up + Vector3.forward;
        Vector3 offset = new Vector3(1 / time, 0, 0);

        while(barScale.x < 1)
        {
            barScale += offset * Time.deltaTime;
            progressBar.rectTransform.localScale = barScale;
            yield return null;
        }
        progressBarSet.SetActive(false);
    }

    public void GoldUpdate()
    {
        goldText.text = string.Format("{0:n0}", GoodsManager.Instance.Gold);
    }

    public void SetQuestPanels(int npcId)
    {
        int idx = 0;
        List<QuestData> datas = QuestManager.Instance.GetQuestDatas(npcId);
        if(datas == null)
        {
            foreach (var panel in questPanels)
                panel.gameObject.SetActive(false);

            noQuestText.SetActive(true);
            return;
        }

        noQuestText.SetActive(false);
        while (idx < datas.Count)
        {
            questPanels[idx].SetQuestData(datas[idx]);
            idx++;
        }
        while (idx < questPanels.Length)
        {
            questPanels[idx].gameObject.SetActive(false);
            idx++;
        }
    }
}
