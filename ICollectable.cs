using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable
{
    int ItemId { get; }
    int ItemCount { get; }
    int SpendTime { get; }

    void CompleteCollect();
}
