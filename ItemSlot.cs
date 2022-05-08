using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image itemImg;
    [SerializeField] TextMeshProUGUI itemCount;
    int count;

    public void SetImg(int id, int cnt)
    {
        itemImg.sprite = Resources.Load<Sprite>("item" + id);
        itemImg.gameObject.SetActive(true);

        count = cnt;
        if (count > 1)
        {
            itemCount.text = string.Format("{0:n0}", count);
            itemCount.gameObject.SetActive(true);
        }
    }

    public bool ItemCount(int cnt)
    {
        if (count + cnt < 0) return false;

        count += cnt;
        if(count > 1)
        {
            itemCount.text = string.Format("{0:n0}", count);
            itemCount.gameObject.SetActive(true);
        }
        else if(count == 0)
        {
            itemCount.gameObject.SetActive(false);
            itemImg.gameObject.SetActive(false);
        }
        else
            itemCount.gameObject.SetActive(false);

        return true;
    }
}
