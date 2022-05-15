using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class NpcQuest : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Npc npc;
    List<QuestData> myQuests;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (myQuests == null) return;

        UiManager.setQuestPanel(myQuests);
    }

    void Awake()
    {
        myQuests = QuestManager.Instance.QuestSearch(npc.NpcId);
    }

    
}
