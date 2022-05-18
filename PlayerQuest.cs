using UnityEngine;
using System.Collections.Generic;

public class PlayerQuest : MonoBehaviour
{
    List<QuestData> questList;

    void Awake()
    {
        questList = new List<QuestData>();
    }

    public void QuestAccept()
    {
        Debug.Log("QuestAccept");
    }
}
