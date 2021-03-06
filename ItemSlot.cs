using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] Image itemImg;
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] Image dragImg;
    [SerializeField] DragItem dragItem;

    int itemCount;
    public int ItemCount { get { return itemCount; } }
    int itemId;
    public int ItemId { get { return itemId; } }
    int slotIdx;
    public int SlotIdx { get { return slotIdx; } }

    public void SetIdx(int idx)
    {
        slotIdx = idx;
    }

    public void SetData(int id, int cnt)
    {
        itemId = id;
        itemCount = cnt;
        if (id == 0 && cnt == 0)
        {
            itemCountText.gameObject.SetActive(false);
            itemImg.gameObject.SetActive(false);
            return;
        }

        itemImg.sprite = ImgContainer.getItemImg(id);
        itemImg.gameObject.SetActive(true);
        
        if (itemCount > 1)
        {
            itemCountText.text = string.Format("{0:n0}", itemCount);
            itemCountText.gameObject.SetActive(true);
        }
    }

    public bool AddItem(int cnt)
    {
        if (itemCount + cnt > int.MaxValue) return false;

        itemCount += cnt;
        if(itemCount == 1)
        {
            itemCountText.gameObject.SetActive(false);
        }
        else if(itemCount == 0)
        {
            itemCountText.gameObject.SetActive(false);
            itemImg.gameObject.SetActive(false);
        }
        else
        {
            itemCountText.text = string.Format("{0:n0}", itemCount);
            itemCountText.gameObject.SetActive(true);
        }
        return true;
    }

    public bool SpendItem(int cnt)
    {
        if (itemCount - cnt < 0) return false;

        itemCount -= cnt;
        if (itemCount == 1)
        {
            itemCountText.gameObject.SetActive(false);
        }
        else if (itemCount == 0)
        {
            itemId = 0;
            itemCountText.gameObject.SetActive(false);
            itemImg.gameObject.SetActive(false);
        }
        else
        {
            itemCountText.text = string.Format("{0:n0}", itemCount);
            itemCountText.gameObject.SetActive(true);
        }
        return true;
    }
    
    // 아이템 드래그 로직
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!itemImg.gameObject.activeSelf) return;

        GameManager.setBoolDrag(true);
        UiManager.itemDescOff();

        dragItem.SetData(itemId, itemCount);
        dragItem.SetFrom(slotIdx);
        dragImg.sprite = itemImg.sprite;
        dragImg.transform.position = eventData.position;
        dragImg.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragImg.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.setBoolDrag(false);
        SetData(dragItem.ItemId, dragItem.ItemCount);
        dragImg.gameObject.SetActive(false);
        GameManager.exchangeSlots(dragItem.From, dragItem.To);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (dragItem.ItemId == itemId) return;

        int tempId = itemId, tempCount = itemCount;
        SetData(dragItem.ItemId, dragItem.ItemCount);
        dragItem.SetData(tempId, tempCount);
        dragItem.SetTo(slotIdx);
    }
}
