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

    //npc의 QuestPanel 클릭 시 호출 함수
    //해당 퀘스트의 상태값이 NotBegin 또는 FullFill 상태일 경우 해당 상태값에 맞는 대화 시작
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
