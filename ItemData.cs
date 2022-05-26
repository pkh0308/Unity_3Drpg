public class ItemData
{
    public readonly int itemId;
    public readonly string itemName;
    public readonly string itemDescription;
    public readonly int priceForPurchase;
    public readonly int priceForSell;

    public ItemData(int id, string name, string des, int pfp, int pfs)
    {
        itemId = id;
        itemName = name;
        itemDescription = des;
        priceForPurchase = pfp;
        priceForSell = pfs;
    }
}
