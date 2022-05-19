using UnityEngine;
using System.Collections.Generic;

public class PlayerQuest : MonoBehaviour
{
    List<QuestData> questList;
    [SerializeField] QuestPanel[] questPanels;

    [SerializeField] UiManager uiManager;

    void Awake()
    {
        questList = new List<QuestData>();
    }

    public void QuestAccept(int questId)
    {
        questList.Add(QuestManager.Instance.GetDataById(questId));
        uiManager.SetQuestPanels(questList);
    }
}
