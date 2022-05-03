using System.Collections;
using UnityEngine;

public class Mushroom : MonoBehaviour, ICollectable
{
    [SerializeField] int itemId;
    [SerializeField] int itemCount;
    [SerializeField] float spendTime;
    public int ItemId { get { return itemId; } }
    public int ItemCount { get { return itemCount; } }
    public float SpendTime { get { return spendTime; } }

    public void StartCollect()
    {
        StartCoroutine(OnCollect());
    }

    public IEnumerator OnCollect()
    {
        yield return new WaitForSeconds(spendTime);
        CompleteCollect();
    }

    public void CompleteCollect()
    {
        gameObject.SetActive(false);
    }
}
