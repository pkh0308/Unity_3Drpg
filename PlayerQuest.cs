using UnityEngine;
using System.Collections.Generic;

public class PlayerQuest : MonoBehaviour
{
    [SerializeField] QuestPanel[] questPanels;

    [SerializeField] GameManager gameManager;
    [SerializeField] UiManager uiManager;

    public void QuestAccept(int questId)
    {
        QuestData data = QuestManager.Instance.GetDataById(questId);
        data.ConvIdxUp();

        if (data.type == QuestData.QuestType.Collect)
            data.QuestCountUp(gameManager.GetItemCount(data.targetId));
        
        uiManager.UpdateQuestPanels();
    }

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
