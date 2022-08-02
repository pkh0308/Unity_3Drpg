public class ItemData
{
    public readonly int itemId;
    public readonly string itemName;
    public readonly string itemDescription;
    public readonly int priceForPurchase;
    public readonly int priceForSell;
    public readonly bool spendable;

    //아이템 id, 아이템 이름, 아이템 설명, 상점 구매 가격, 상점 판매 가격, 사용 가능 여부
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
