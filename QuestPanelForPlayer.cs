using UnityEngine;
using TMPro;

public class QuestPanelForPlayer : MonoBehaviour
{
    QuestData data;
    public int QuestType { get { return data.QuestStatus; } }

    [SerializeField] TextMeshProUGUI questNameText;
    [SerializeField] GameObject questInfoSet;
    [SerializeField] TextMeshProUGUI questInfoNameText;
    [SerializeField] TextMeshProUGUI questInfoDescripionText;

    public void SetQuestData(QuestData data)
    {
        this.data = data;
        questNameText.text = questInfoNameText.text = data.QuestName;
        questInfoDescripionText.text = data.QuestName;
    }

    public void OnClick_Player()
    {
        questInfoSet.SetActive(questInfoSet.activeSelf == false);
    }
}