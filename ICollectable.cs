using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable
{
    int ItemId { get; }
    int ItemCount { get; }
    float SpendTime { get; }

    void StartCollect();
    IEnumerator OnCollect();
    void CompleteCollect();
}
