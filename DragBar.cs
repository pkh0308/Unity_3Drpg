using UnityEngine;
using UnityEngine.EventSystems;


//마우스 드래그로 이동시킬 UI에 사용(인벤토리, 퀘스트 창 등)
//드래그 가능한 영역의 UI에 할당, 이동시킬 UI 전체를 targetObject로 지정하여 사용
public class DragBar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] GameObject targetObject;
    Vector3 beforePos;
    Vector3 dif;
    [SerializeField] float offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        beforePos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dif = (Vector3)eventData.position - beforePos;
        targetObject.transform.position += dif * offset;
        beforePos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dif = Vector3.zero;
    }
}
