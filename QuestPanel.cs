using UnityEngine;
using TMPro;

public class QuestPanel : MonoBehaviour
{
    QuestData data;
    public int QuestId { get { return data.QuestId; } }
    public int QuestNpc { get { return data.QuestNpc; } }

    [SerializeField] TextMeshProUGUI questNameText;
    
    public void SetQuestData(QuestData data)
    {
        this.data = data;
        questNameText.text = data.QuestName;
    }
}
