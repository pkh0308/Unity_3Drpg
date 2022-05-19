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
        questNameText.text = data.QuestName;
    }

    public void OnClick_Npc()
    {
        if (data.QuestStatus == (int)QuestData.QuestStatusType.NotBegin)
            GameManager.startQuestConv(data.convList[0], data.QuestId);
    }
}
