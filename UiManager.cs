using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject conversationSet;
    public GameObject inventorySet;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

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
}
