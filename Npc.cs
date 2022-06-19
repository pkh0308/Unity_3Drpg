using UnityEngine;

public class Npc : MonoBehaviour
{
    [SerializeField] int npcId;
    public int NpcId { get { return npcId; } }
    string npcName;
    public string NpcName { get { return npcName; } }
    bool hasShop;
    public bool HasShop { get { return hasShop; } }

    public void SetData(string name, bool shop)
    {
        npcName = name;
        hasShop = shop;
    }

    public void Turn(Vector3 pos)
    {
        transform.LookAt(pos);
    }
}
