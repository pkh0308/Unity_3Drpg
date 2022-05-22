﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

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

    [SerializeField] TextMeshProUGUI conv_NpcName;
    [SerializeField] TextMeshProUGUI conv_ConversationText;
    [SerializeField] GameObject conv_ExitBtn;
    [SerializeField] GameObject conv_QuestBtn;
    [SerializeField] GameObject conv_AccpetBtn;
    [SerializeField] GameObject conv_ClearBtn;
    [SerializeField] GameObject conv_NextBtn;
    [SerializeField] float conv_speed;
    bool isTexting;
    int questStatus;
    Coroutine co_Texting;

    int conv_Idx;
    string[] currentConversation;

    [SerializeField] GameObject noQuestText;
    [SerializeField] QuestPanel[] questPanels;
    [SerializeField] GameObject[] questOnGoingPanels;
    [SerializeField] GameObject[] questCompletePanels;

    [SerializeField] GameObject noQuestTextForPlayer;
    [SerializeField] QuestPanelForPlayer[] questPanelsForPlayer;
    [SerializeField] GameObject questInfoSet;
    [SerializeField] TextMeshProUGUI questInfoNameText;
    [SerializeField] TextMeshProUGUI questInfoDescripionText;
    [SerializeField] TextMeshProUGUI questCountText;
    QuestData curQuestData;

    Dictionary<int, ItemData> itemDic;
    public static Action itemDescOff;
    public static Action<QuestData> questDescSet;

    void Awake()
    {
        itemDescOff = () => { ItemDescOff(); };
        questDescSet = (a) => { SetQuestDesc(a); };

        itemDic = new Dictionary<int, ItemData>();
        UpdateGold();
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

        // 퀘스트 판넬 업데이트
        SetQuestPanels();
    }

    //대화 관련
    public void Conv_SetActive(bool act)
    {
        conversationSet.SetActive(act);

        if(act)
        {
            convQuestSet.SetActive(false);
            conv_AccpetBtn.SetActive(false);
            conv_ClearBtn.SetActive(false);
        }
    }

    public void Conv_Set(string npcName, string[] texts)
    {
        conv_Idx = 0;
        questStatus = -1; //Not Quest
        conv_NpcName.text = npcName;
        currentConversation = texts;

        conv_ExitBtn.SetActive(texts.Length == 1);
        conv_QuestBtn.SetActive(texts.Length == 1);
        co_Texting = StartCoroutine(Conv_Texting());
    }

    public void Conv_QuestSet(string[] texts, int questStatus)
    {
        conv_Idx = 0;
        this.questStatus = questStatus;
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

        SetConvBtns();
    }

    public void Conv_CencleTexting()
    {
        if (isTexting == false) return;

        StopCoroutine(co_Texting);
        isTexting = false;
        conv_ConversationText.text = currentConversation[conv_Idx];

        SetConvBtns();
    }

    void SetConvBtns()
    {
        switch (questStatus)
        {
            case -1:// Not Quest
                if (conv_Idx == currentConversation.Length - 1)
                {
                    conv_ExitBtn.SetActive(true);
                    conv_QuestBtn.SetActive(true);
                }
                else
                    conv_NextBtn.SetActive(true);
                break;
            case (int)QuestData.QuestStatusType.NotBegin:
                if (conv_Idx == currentConversation.Length - 1)
                {
                    conv_ExitBtn.SetActive(true);
                    conv_AccpetBtn.SetActive(true);
                }
                else
                    conv_NextBtn.SetActive(true);
                break;
            case (int)QuestData.QuestStatusType.FullFill:
                if (conv_Idx == currentConversation.Length - 1)
                    conv_ClearBtn.SetActive(true);
                else
                    conv_NextBtn.SetActive(true);
                break;
        }
    }

    public void Conv_QuestOpenBtn()
    {
        if(convQuestSet.activeSelf) convQuestSet.SetActive(false);
        else convQuestSet.SetActive(true);
    }

    // UI 관련
    public void ControlInventorySet()
    {
        inventorySet.SetActive(inventorySet.activeSelf == false);
    }

    public void ControlQuestSet()
    {
        questSet.SetActive(questSet.activeSelf == false);
        questInfoSet.SetActive(false);
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

    // 아이템 관련
    public void ItemDescOn(int itemId)
    {
        if (itemId == 0) { ItemDescOff(); return; }
        if (descriptionOn) return;

        itemNameText.text = itemDic[itemId].itemName;
        itemDescText.text = itemDic[itemId].itemDescription;
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

    public string GetItemName(int id)
    {
        return itemDic[id].itemName;
    }

    public void UpdateGold()
    {
        goldText.text = string.Format("{0:n0}", GoodsManager.Instance.Gold);
    }

    // 퀘스트 관련
    // for npc conversation quest panel
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
            switch (questPanels[idx].QuestStatus)
            {
                case (int)QuestData.QuestStatusType.NotBegin:
                    questOnGoingPanels[idx].SetActive(false);
                    questCompletePanels[idx].SetActive(false);
                    break;
                case (int)QuestData.QuestStatusType.OnGoing:
                    questOnGoingPanels[idx].SetActive(true);
                    questCompletePanels[idx].SetActive(false);
                    break;
                case (int)QuestData.QuestStatusType.FullFill:
                    questOnGoingPanels[idx].SetActive(false);
                    questCompletePanels[idx].SetActive(false);
                    break;
                case (int)QuestData.QuestStatusType.Cleared:
                    questOnGoingPanels[idx].SetActive(false);
                    questCompletePanels[idx].SetActive(true);
                    break;
                default:
                    Debug.Log(questPanels[idx].QuestStatus);
                    break;
            }
            idx++;
        }
        while (idx < questPanels.Length)
        {
            questPanels[idx].gameObject.SetActive(false);
            idx++;
        }
    }

    // for player quest panel 
    public void SetQuestPanels()
    {
        Dictionary<int, QuestData> datas = QuestManager.Instance.playerQuestDic;
        List<int> list = datas.Keys.ToList();

        int idx = 0;
        if (datas.Count == 0)
        {
            foreach (var panel in questPanelsForPlayer)
                panel.gameObject.SetActive(false);

            noQuestTextForPlayer.SetActive(true);
            return;
        }

        noQuestTextForPlayer.SetActive(false);
        while (idx < datas.Count)
        {
            questPanelsForPlayer[idx].SetQuestData(datas[list[idx]]);
            idx++;
        }
        while (idx < questPanelsForPlayer.Length)
        {
            questPanelsForPlayer[idx].gameObject.SetActive(false);
            idx++;
        }
    }

    public void SetQuestDesc(QuestData data)
    {
        if (data == null)
        {
            questInfoSet.SetActive(false);
            return;
        }

        if (data != curQuestData) curQuestData = data;

        if (questInfoSet.activeSelf)
        {
            questInfoSet.SetActive(false);
        }
        else
        {
            questInfoNameText.text = data.QuestName;
            questInfoDescripionText.text = " " + data.QuestDescription;
            questCountText.text = itemDic[data.TargetId].itemName + " " + data.CurCount + "/" + data.QuestCount;
            questInfoSet.SetActive(true);
        }
    }

    public void UpdateQuestDescCount()
    {
        if (questInfoSet.activeSelf == false) return;

        questCountText.text = itemDic[curQuestData.TargetId].itemName + " " + curQuestData.CurCount + "/" + curQuestData.QuestCount;
    }
}
