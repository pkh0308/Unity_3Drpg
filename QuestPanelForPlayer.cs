using UnityEngine;
using TMPro;

public class QuestPanelForPlayer : MonoBehaviour
{
    QuestData data;
    public int QuestStatus { get { return data.QuestStatus; } }

    [SerializeField] TextMeshProUGUI questNameText;

    public void SetQuestData(QuestData data)
    {
        this.data = data;
        questNameText.text = data.questName;
        gameObject.SetActive(true);
    }

    public void OnClick_Player()
    {
        UiManager.questDescSet(data);
    }
}