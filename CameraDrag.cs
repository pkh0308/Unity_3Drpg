using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Vector3 lastPos;
    Vector3 curPos;
    public CameraMove camMove;
    public float rotationSpeed;

    public CursorManager cursorManager;

    bool lDown;
    bool rDown;

    void Update()
    {
        InputCheck();
    }

    void InputCheck()
    {
        lDown = Input.GetKey(KeyCode.Mouse0);
        rDown = Input.GetKey(KeyCode.Mouse1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!rDown) return;
        
        lastPos = eventData.position;
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.ROTATE);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (lDown) return;

        curPos = eventData.position;
        float xOffset = (curPos.x - lastPos.x) * rotationSpeed;
        float yOffset = (curPos.y - lastPos.y) * rotationSpeed;

        camMove.Turn(xOffset, yOffset);
        lastPos = curPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
    }
}
