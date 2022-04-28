using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour, ICollectable
{
    public int ItemId { get; }
    public int ItemCount { get; }
    public int SpendTime { get; }

    public void CompleteCollect()
    {
        gameObject.SetActive(false);
    }
}
