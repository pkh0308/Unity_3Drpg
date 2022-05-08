using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UiManager : MonoBehaviour
{
    public GameObject conversationSet;
    public GameObject inventorySet;

    public GameObject progressBarSet;
    public TextMeshProUGUI progressText;
    public Image progressBar;

    public TextMeshProUGUI conv_NpcName;
    public TextMeshProUGUI conv_ConversationText;
    public GameObject conv_ExitBtn;
    int conv_Idx;

    string[] currentConversation;

    public void ControllConversationSet(bool act)
    {
        conversationSet.SetActive(act);
    }

    public void SetConversation(string npcName, string[] texts)
    {
        conv_Idx = 0;
        conv_NpcName.text = npcName;
        currentConversation = texts;
        conv_ExitBtn.SetActive(texts.Length == 1 ? true : false);
        conv_ConversationText.text = currentConversation[conv_Idx];
    }

    public void NextConversation()
    {
        if (conv_Idx >= currentConversation.Length - 1) return;

        conv_Idx++;
        if (conv_Idx == currentConversation.Length - 1) conv_ExitBtn.SetActive(true);
        conv_ConversationText.text = currentConversation[conv_Idx];
    }

    public bool ControllInventorySet()
    {
        if (!inventorySet.activeSelf)
        {
            inventorySet.SetActive(true);
            return true;
        }
        else
        {
            inventorySet.SetActive(false);
            return false;
        }
    }

    public void ControllProgressBarSet(string name, float time)
    {
        switch(name)
        {
            case "Collectable":
                progressText.text = "채집중...";
                break;
        }
        StartCoroutine(Progress(time));
    }

    IEnumerator Progress(float time)
    {
        progressBarSet.SetActive(true);
        Vector3 barScale = Vector3.up + Vector3.forward;
        Vector3 offset = new Vector3(1 / time, 0, 0);

        while(barScale.x < 1)
        {
            barScale += offset * Time.deltaTime;
            progressBar.rectTransform.localScale = barScale;
            yield return null;
        }
        progressBarSet.SetActive(false);
    }
}
