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

    //전달받은 id와 갯수로 해당 슬롯의 이미지 및 카운트 갱신
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

    //아이템 갯수가 1일 경우 카운트 비활성화, 0일 경우 이미지 및 카운트 비활성화
    //2개 이상일 경우 카운트 및 이미지 모두 활성화
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
    //드래그 시작 시 DragItem 클래스에 현재 슬롯의 아이템 정보 저장
    //빈 슬롯 또는 다른 아이템이 있는 슬롯에서 드래그 종료 시, DragItem 을 통해 아이템 정보 교환
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
