using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragBar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject targetObject;
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
