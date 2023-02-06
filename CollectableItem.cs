using System.Collections;
using UnityEngine;
using Photon.Pun;

public class CollectableItem : MonoBehaviour, ICollectable
{
    [SerializeField] int itemId;
    [SerializeField] int itemCount;
    [SerializeField] float spendTime;
    [SerializeField] int spCount;
    public int ItemId { get { return itemId; } }
    public int ItemCount { get { return itemCount; } }
    public float SpendTime { get { return spendTime; } }
    public int SpCount { get { return spCount; } }

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

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
        PV.RPC(nameof(SetActiveFalse), RpcTarget.All);
    }

    [PunRPC]
    void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
