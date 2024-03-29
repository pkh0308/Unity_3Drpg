﻿using UnityEngine;

public class Npc : MonoBehaviour
{
    [SerializeField] int npcId;
    public int NpcId { get { return npcId; } }
    [SerializeField] string npcName;
    public string NpcName { get { return npcName; } }
    [SerializeField] bool hasShop;
    public bool HasShop { get { return hasShop; } }

    public void Turn(Vector3 pos)
    {
        transform.LookAt(pos);
    }
}