using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Image[] itemImgs;
    [SerializeField] ShopItem[] shopItems;
    [SerializeField] GameObject infoSet;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] TextMeshProUGUI itemDescText;
    [SerializeField] TextMeshProUGUI itemPriceText;

    [SerializeField] GameObject purchaseSet;
    [SerializeField] GameObject purchaseFailSet;

    [SerializeField] GameManager gameManager;
    [SerializeField] UiManager uiManager;
    ItemData curItemData;
    Dictionary<int, List<int>> npcShopDic;

    void Awake()
    {
        npcShopDic = new Dictionary<int, List<int>>();

        // npc별 상점 정보 저장
        TextAsset itemList = Resources.Load("npcShop") as TextAsset;
        StringReader itemReader = new StringReader(itemList.text);

        while (itemReader != null)
        {
            string line = itemReader.ReadLine();
            if (line == null) break;

            line = itemReader.ReadLine();
            while (line.Length > 1)
            {
                string[] datas = line.Split(',');
                int id = int.Parse(datas[0]);
                List<int> list = new List<int>();
                for(int i = 1; i < datas.Length; i++)
                {
                    list.Add(int.Parse(datas[i]));
                }
                npcShopDic.Add(id, list);

                line = itemReader.ReadLine();
                if (line == null) break;
            }
        }
        itemReader.Close();
    }

    public void SetShopSlots(int npcId)
    {
        List<int> items = npcShopDic[npcId];

        int idx = 0;
        while (idx < items.Count)
        {
            shopItems[idx].SetItemData(uiManager.GetItemData(items[idx]));
            itemImgs[idx].sprite = ImgContainer.getItemImg(items[idx]);
            itemImgs[idx].gameObject.SetActive(true);
            idx++;
        }
        while (idx < shopItems.Length)
        {
            shopItems[idx].SetItemData(null);
            itemImgs[idx].gameObject.SetActive(false);
            idx++;
        }
    }

    public void ShopItemDescOn(string name, string desc, int price)
    {
        itemNameText.text = name;
        itemDescText.text = desc;
        itemPriceText.text = string.Format("{0:n0}", price);

        infoSet.transform.position = Input.mousePosition;
        infoSet.SetActive(true);
    }

    public void ShopItemDescOff()
    {
        infoSet.SetActive(false);
    }

    public void PurchaseBtn(ShopItem item)
    {
        if (item.Data == null) return;

        purchaseSet.SetActive(true);
        curItemData = item.Data;
    }

    public void PurchaseExitBtn()
    {
        purchaseSet.SetActive(false);
    }

    public void PurchaseOkBtn()
    {
        if(GoodsManager.Instance.SpendGold(curItemData.priceForPurchase) == false)
        {
            purchaseFailSet.SetActive(true);
            purchaseSet.SetActive(false);
            return;
        }

        // 한개 구매로 테스트, 추후 다중구매 추가 구현
        gameManager.GetItem(curItemData.itemId, 1);
        purchaseSet.SetActive(false);
    }

    public void PurchaseFailBtn()
    {
        purchaseFailSet.SetActive(false);
    }
}
