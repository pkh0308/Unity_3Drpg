using UnityEngine;
using TMPro;

public class QuestPanel : MonoBehaviour
{
    QuestData data;
    public int QuestStatus { get { return data.QuestStatus; } }
    [SerializeField] TextMeshProUGUI questNameText;

    public void SetQuestData(QuestData data)
    {
        this.data = data;
        questNameText.text = data.questName;
    }

    public void OnClick_Npc()
    {
        switch(data.QuestStatus)
        {
            case (int)QuestData.QuestStatusType.NotBegin:
                GameManager.startQuestConv(data.GetConvId(), data.questId, (int)QuestData.QuestStatusType.NotBegin);
                break;
            case (int)QuestData.QuestStatusType.FullFill:
                GameManager.startQuestConv(data.GetConvId(), data.questId, (int)QuestData.QuestStatusType.FullFill);
                break;
        }
    }
}
