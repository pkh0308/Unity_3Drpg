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
    RectTransform infoSetRect;
    Vector2 infoSetSize;

    [SerializeField] GameObject purchaseSet;
    [SerializeField] GameObject purchaseFailSet;
    [SerializeField] Image purchaseItemImg;
    [SerializeField] TextMeshProUGUI purchaseCountText;
    [SerializeField] TextMeshProUGUI purchasePriceText;
    int curCount;

    [SerializeField] GameManager gameManager;
    [SerializeField] UiManager uiManager;
    ItemData curItemData;
    Dictionary<int, List<int>> npcShopDic;

    void Awake()
    {
        infoSetRect = infoSet.GetComponent<RectTransform>();
        infoSetSize = infoSetRect.sizeDelta * 0.8f;
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

    //npc id를 넘겨받아 해당 npc의 상품 목록으로 샵 갱신
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

        infoSetRect.position = Input.mousePosition;
        if (Input.mousePosition.x + infoSetSize.x > Screen.width)
            infoSetRect.position -= new Vector3(infoSetSize.x, 0, 0);
        if (Input.mousePosition.y < infoSetSize.y)
            infoSetRect.position += new Vector3(0, infoSetSize.y * 0.5f, 0);
        infoSet.SetActive(true);
        infoSet.SetActive(true);
    }

    public void ShopItemDescOff()
    {
        infoSet.SetActive(false);
    }

    //해당 상품의 ShopItem 데이터로 가격 및 수량 갱신
    public void PurchaseBtn(ShopItem item)
    {
        if (item.Data == null) return;

        curCount = 1;
        purchaseCountText.text = string.Format("{0:n0}", curCount);
        purchasePriceText.text = string.Format("{0:n0}", item.Data.priceForPurchase);
        purchaseItemImg.sprite = ImgContainer.getItemImg(item.Data.itemId);
        purchaseSet.SetActive(true);
        curItemData = item.Data;
    }

    public void Purchase_ExitBtn()
    {
        purchaseSet.SetActive(false);
    }

    public void Purchase_CountPlusBtn()
    {
        curCount++;
        purchaseCountText.text = string.Format("{0:n0}", curCount);
        purchasePriceText.text = string.Format("{0:n0}", curItemData.priceForPurchase * curCount);
    }

    public void Purchase_CountMinusBtn()
    {
        if (curCount <= 1) return;
            
        curCount--;
        purchaseCountText.text = string.Format("{0:n0}", curCount);
        purchasePriceText.text = string.Format("{0:n0}", curItemData.priceForPurchase * curCount);
    }

    public void Purchase_OkBtn()
    {
        if(GoodsManager.Instance.SpendGold(curItemData.priceForPurchase * curCount) == false)
        {
            purchaseFailSet.SetActive(true);
            purchaseSet.SetActive(false);
            return;
        }

        gameManager.GetItem(curItemData.itemId, curCount);
        purchaseSet.SetActive(false);
    }

    public void Purchase_FailBtn()
    {
        purchaseFailSet.SetActive(false);
    }
}
