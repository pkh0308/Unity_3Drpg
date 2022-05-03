using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UiManager uiManager;

    bool pause;
    public bool Pause { get { return pause; } }
    bool invenOpen;
    public bool InvenOpen { get { return invenOpen; } }

    public void SetPause(bool act)
    {
        pause = act;
    }

    public void StartConversation()
    {
        uiManager.ControllConversationSet(true);
        pause = true;
    }

    public void ExitConversation()
    {
        uiManager.ControllConversationSet(false);
        pause = false;
    }

    public void InventoryControll()
    {
        invenOpen = uiManager.ControllInventorySet();
    }

    public void ProgressStart(string name, float time)
    {
        uiManager.ControllProgressBarSet(name, time);
    }
}
