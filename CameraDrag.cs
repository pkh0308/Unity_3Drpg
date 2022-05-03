using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    Vector3 lastPos;
    Vector3 curPos;
    public CameraMove camMove;
    public float rotationSpeed;

    public CursorManager cursorManager;

    bool lDown;
    bool rDown;
    bool isDraging;

    void Update()
    {
        InputCheck();
        EndDrag();
        BeginDrag();
        OnDrag();
    }

    void InputCheck()
    {
        lDown = Input.GetKey(KeyCode.Mouse0);
        rDown = Input.GetKey(KeyCode.Mouse1);
    }

    void BeginDrag()
    {
        if (!rDown) return;
        if (isDraging) return;

        lastPos = Input.mousePosition;
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.ROTATE);
        isDraging = true;
    }

    void OnDrag()
    {
        if (lDown) return;
        if (!isDraging) return;

        curPos = Input.mousePosition;
        float xOffset = (curPos.x - lastPos.x) * rotationSpeed;
        float yOffset = (curPos.y - lastPos.y) * rotationSpeed;

        camMove.Turn(xOffset, yOffset);
        lastPos = curPos;
    }

    void EndDrag()
    {
        if (rDown) return;

        isDraging = false;
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.DEFAULT);
    }
}
