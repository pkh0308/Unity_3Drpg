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

    public void ControllConversationSet(bool act)
    {
        conversationSet.SetActive(act);
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
                progressText.text = "Collecting...";
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
