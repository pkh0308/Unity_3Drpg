using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class UiManager : MonoBehaviour
{
    [SerializeField] ShopManager shopManager;

    [SerializeField] GameObject conversationSet;
    [SerializeField] GameObject inventorySet;
    [SerializeField] GameObject convQuestSet;
    [SerializeField] GameObject questSet;
    [SerializeField] GameObject convShopSet;
    [SerializeField] TextMeshProUGUI goldText;

    //스테이터스 바
    [SerializeField] GameObject menuSet;
    [SerializeField] Image hpBar;
    [SerializeField] Text hpCount;
    [SerializeField] Image spBar;
    [SerializeField] Text spCount;
    [SerializeField] GameObject exitSet;

    [SerializeField] GameObject itemDescription;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescText;
    RectTransform descRect;
    Vector2 descSize;
    bool descOn;
    bool shopDescOn;

    public GameObject progressBarSet;
    public TextMeshProUGUI progressText;
    public Image progressBar;

    //대화 관련
    [SerializeField] TextMeshProUGUI conv_NpcName;
    [SerializeField] TextMeshProUGUI conv_ConversationText;
    [SerializeField] GameObject conv_ExitBtn;
    [SerializeField] GameObject conv_QuestBtn;
    [SerializeField] GameObject conv_AccpetBtn;
    [SerializeField] GameObject conv_ClearBtn;
    [SerializeField] GameObject conv_NextBtn;
    [SerializeField] GameObject conv_ShopBtn;
    [SerializeField] float conv_speed;
    bool isTexting;
    bool hasShop;
    int questStatus;
    Coroutine co_Texting;

    int conv_Idx;
    string[] currentConversation;

    //퀘스트 관련
    [SerializeField] GameObject noQuestText;
    [SerializeField] QuestPanel[] questPanels;
    [SerializeField] GameObject[] questOnGoingPanels;
    [SerializeField] GameObject[] questCompletePanels;
    [SerializeField] GameObject[] questFullFillChecks;

    [SerializeField] GameObject noQuestTextForPlayer;
    [SerializeField] QuestPanelForPlayer[] questPanelsForPlayer;
    [SerializeField] GameObject[] questFullFillChecksForPlayer;
    [SerializeField] GameObject questInfoSet;
    [SerializeField] TextMeshProUGUI questInfoNameText;
    [SerializeField] TextMeshProUGUI questInfoDescripionText;
    [SerializeField] TextMeshProUGUI questCountText;
    QuestData curQuestData;

    //알림 텍스트 관련
    [SerializeField] TextMeshProUGUI midNoticeText;
    [SerializeField] float noticeSeconds;
    WaitForSeconds noticeSecs;
    Coroutine noticeCor;
    public enum NoticeType { NotEnoughSp = 0 }

    //전투 관련
    [SerializeField] GameObject deadSet;

    Dictionary<int, ItemData> itemDic;
    public static Action itemDescOff;
    public static Action<QuestData> questDescSet;
    public static Action updateQuestPanel;
    public static Action updateGold;

    Dictionary<int, EnemyData> enemyDic;

    void Awake()
    {
        descRect = itemDescription.GetComponent<RectTransform>();
        descSize = descRect.sizeDelta * 0.8f;
        noticeSecs = new WaitForSeconds(noticeSeconds);

        itemDescOff = () => { ItemDescOff(); };
        questDescSet = (a) => { SetQuestDesc(a); };
        updateQuestPanel = () => { UpdateQuestPanels(); };
        updateGold = () => { UpdateGold(); };

        itemDic = new Dictionary<int, ItemData>();
        enemyDic = new Dictionary<int, EnemyData>();
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
                //0 : id, 1 : name, 2 : description, 3 : price for purchase, 4 : price for sell, 5 : spendable
                ItemData item = new ItemData(id, datas[1], datas[2], int.Parse(datas[3]), int.Parse(datas[4]), bool.Parse(datas[5]));
                itemDic.Add(id, item);

                line = itemReader.ReadLine();
                if (line == null) break;
            }
        }
        itemReader.Close();

        //몬스터 정보 저장
        TextAsset enemyList = Resources.Load("enemyDictionary") as TextAsset;
        StringReader enemyReader = new StringReader(enemyList.text);

        while (enemyReader != null)
        {
            string line = enemyReader.ReadLine();
            if (line == null) break;

            line = enemyReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split('@');
                int id = int.Parse(datas[0]);
                //0 : id, 1 : name, 2 : maxHp, 3 : description
                EnemyData enemy = new EnemyData(id, datas[1], int.Parse(datas[2]), datas[3]);
                enemyDic.Add(id, enemy);

                line = enemyReader.ReadLine();
                if (line == null) break;
            }
        }
        enemyReader.Close();

        // 퀘스트 판넬 업데이트
        UpdateQuestPanels();
    }

    //대화 관련
    public void Conv_SetActive(bool act)
    {
        conversationSet.SetActive(act);

        if(act)
        {
            convQuestSet.SetActive(false);
            convShopSet.SetActive(false);
            conv_AccpetBtn.SetActive(false);
            conv_ClearBtn.SetActive(false);
            conv_ShopBtn.SetActive(false);
        }
    }

    public void Conv_Set(string npcName, string[] texts, bool hasShop)
    {
        conv_Idx = 0;
        questStatus = -1; //Not Quest
        this.hasShop = hasShop;
        conv_NpcName.text = npcName;
        currentConversation = texts;

        conv_ExitBtn.SetActive(texts.Length == 1);
        conv_QuestBtn.SetActive(texts.Length == 1);
        conv_ShopBtn.SetActive(hasShop && texts.Length == 1);
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

    public void Conv_ShopBtn()
    {
        convQuestSet.SetActive(false);
        convShopSet.SetActive(convShopSet.activeSelf == false);
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
                    conv_ShopBtn.SetActive(hasShop);
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
        convShopSet.SetActive(false);
        convQuestSet.SetActive(convQuestSet.activeSelf == false);
    }

    //기타 UI 관련
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
            case Tags.Collectable:
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

    public void StsBar_MenuBtn()
    {
        menuSet.SetActive(menuSet.activeSelf == false);
    }

    public void StsBar_OptionBtn()
    {
        //옵션창 구현 후 작성
    }

    public void StsBar_SaveBtn()
    {
        //저장 관련 구현 후 작성
    }

    public void StsBar_ExitBtn()
    {
        exitSet.SetActive(exitSet.activeSelf == false);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void StsBar_HpUpdate(int curHp, int maxHp)
    {
        hpCount.text = string.Format("{0:n0}", curHp + " / " + maxHp);
        hpBar.rectTransform.localScale = new Vector3((float)curHp / maxHp, 1, 1);
    }

    public void StsBar_SpUpdate(int curSp, int maxSp)
    {
        spCount.text = string.Format("{0:n0}", curSp + " / " + maxSp);
        spBar.rectTransform.localScale = new Vector3((float)curSp / maxSp, 1, 1);
    }

    public void MidNotice(NoticeType type)
    {
        if (noticeCor != null)
        {
            StopCoroutine(noticeCor);
            midNoticeText.gameObject.SetActive(false);
        }

        string s = "";
        switch(type)
        {
            case NoticeType.NotEnoughSp:
                s = "스태미나가 부족합니다.";
                break;
        }
        noticeCor = StartCoroutine(Notice(s));
    }

    IEnumerator Notice(string s)
    {
        midNoticeText.text = s;
        midNoticeText.gameObject.SetActive(true);

        yield return noticeSecs;
        midNoticeText.gameObject.SetActive(false);
    }

    //Esc 입력 시 모든 UI창 비활성화
    //유저 입력으로 열리는 UI창 추가 시 갱신 필요
    public void CloseWindows()
    {
        inventorySet.SetActive(false);
        questSet.SetActive(false);
    }

    // 아이템 관련
    public void ItemDescOn(int itemId)
    {
        if (itemId == 0) { ItemDescOff(); return; }
        if (descOn) return;

        itemNameText.text = itemDic[itemId].itemName;
        itemDescText.text = itemDic[itemId].itemDescription;
        descOn = true;

        descRect.position = Input.mousePosition;
        if (Input.mousePosition.x + descSize.x > Screen.width)
            descRect.position -= new Vector3(descSize.x, 0, 0);
        if (Input.mousePosition.y < descSize.y)
            descRect.position += new Vector3(0, descSize.y * 0.5f, 0);
        itemDescription.SetActive(true);
    }

    public void ItemDescOff()
    {
        if (!descOn) return;

        descOn = false;
        itemDescription.SetActive(false);
    }

    public void ShopDescOn(int itemId)
    {
        if (itemId == 0) { ShopDescOff(); return; }
        if (shopDescOn) return;

        shopManager.ShopItemDescOn(itemDic[itemId].itemName, itemDic[itemId].itemDescription, itemDic[itemId].priceForPurchase);
        shopDescOn = true;
    }

    public void ShopDescOff()
    {
        if (!shopDescOn) return;

        shopDescOn = false;
        shopManager.ShopItemDescOff();
    }

    public string GetItemName(int itemId)
    {
        if (itemDic.TryGetValue(itemId, out ItemData data) == false)
        {
            Debug.Log("ItemId not in Dictionary...");
            return null;
        }
        return data.itemName;
    }

    public ItemData GetItemData(int itemId)
    {
        if (itemDic.TryGetValue(itemId, out ItemData data) == false)
        {
            Debug.Log("ItemId not in Dictionary...");
            return null;
        }
        return data;
    }

    public void UpdateGold()
    {
        goldText.text = string.Format("{0:n0}", GoodsManager.Instance.Gold);
    }

    // 퀘스트 관련
    // for npc conversation quest panel
    public void UpdateQuestPanels(int npcId)
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
            questPanels[idx].gameObject.SetActive(true);
            questFullFillChecks[idx].SetActive(questPanels[idx].QuestStatus == (int)QuestData.QuestStatusType.FullFill);
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
    public void UpdateQuestPanels()
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
            questFullFillChecksForPlayer[idx].SetActive(questPanelsForPlayer[idx].QuestStatus == (int)QuestData.QuestStatusType.FullFill);
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
            questInfoNameText.text = data.questName;
            questInfoDescripionText.text = " " + data.questDescription;
            switch (data.type)
            {
                case QuestData.QuestType.Collect:
                    questCountText.text = itemDic[data.targetId].itemName + " " + data.CurCount + "/" + data.maxCount;
                    break;
                case QuestData.QuestType.Kill:
                    questCountText.text = enemyDic[data.targetId].enemyName + " " + data.CurCount + "/" + data.maxCount;
                    break;
            }
            questInfoSet.SetActive(true);
        }
    }

    public void UpdateQuestDescCount()
    {
        if (questInfoSet.activeSelf == false) return;

        questCountText.text = itemDic[curQuestData.targetId].itemName + " " + curQuestData.CurCount + "/" + curQuestData.maxCount;
    }

    //사망 시 호출
    public void SetDeadScreen()
    {
        deadSet.SetActive(deadSet.activeSelf == false);
    }
}
