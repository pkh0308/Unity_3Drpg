using UnityEngine;
using System.Collections.Generic;

public class PlayerQuest : MonoBehaviour
{
    [SerializeField] QuestPanel[] questPanels;

    [SerializeField] GameManager gameManager;
    [SerializeField] UiManager uiManager;

    //퀘스트 수락 시 QuestManager에서 해당 id의 퀘스트 데이터를 불러옴
    //채집형 퀘스트의 경우 현재 인벤토리에 해당 아이템의 보유 여부를 체크해 카운팅
    public void QuestAccept(int questId)
    {
        QuestData data = QuestManager.Instance.GetDataById(questId);
        data.ConvIdxUp();

        if (data.type == QuestData.QuestType.Collect)
            data.QuestCountUp(gameManager.GetItemCount(data.targetId));
        
        uiManager.UpdateQuestPanels();
    }

    //퀘스트 클리어 시 QuestManager에서 해당 id의 퀘스트 데이터를 불러옴
    //채집형 퀘스트의 경우 현재 인벤토리에 해당 아이템을 퀘스트 요구 개수만큼 차감
    public void QuestClear(int questId)
    {
        QuestData data = QuestManager.Instance.GetDataById(questId);
        List<int[]> rewards = data.rewardList;

        if (data.type == QuestData.QuestType.Collect)
            gameManager.SpendItem(data.targetId, data.maxCount);

        for (int i = 0; i < rewards.Count; i++)
            gameManager.GetItem(rewards[i][0], rewards[i][1]);

        uiManager.UpdateQuestPanels();
    }
}
