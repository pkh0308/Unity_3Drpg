using UnityEngine;

public class DragItem : MonoBehaviour
{
    int itemId;
    public int ItemId { get { return itemId; } }
    int itemCount;
    public int ItemCount { get { return itemCount; } }

    int from;
    public int From { get { return from; } }
    int to;
    public int To { get { return to; } }

    public void SetData(int id, int count)
    {
        itemId = id;
        itemCount = count;
    }

    public void SetFrom(int from)
    {
        this.from = from;
    }

    public void SetTo(int to)
    {
        this.to = to;
    }
}
