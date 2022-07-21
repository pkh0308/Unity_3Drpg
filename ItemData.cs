public class ItemData
{
    public readonly int itemId;
    public readonly string itemName;
    public readonly string itemDescription;
    public readonly int priceForPurchase;
    public readonly int priceForSell;
    public readonly bool spendable;

    public ItemData(int id, string name, string des, int pfp, int pfs, bool spendable)
    {
        itemId = id;
        itemName = name;
        itemDescription = des;
        priceForPurchase = pfp;
        priceForSell = pfs;
        this.spendable = spendable;
    }
}
