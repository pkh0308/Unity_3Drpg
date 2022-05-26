using UnityEngine;

public class ShopItem : MonoBehaviour
{
    ItemData data;
    public ItemData Data { get { return data; } }

    public void SetItemData(ItemData data)
    {
        this.data = data;
    }
}
