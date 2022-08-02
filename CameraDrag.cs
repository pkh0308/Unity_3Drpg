using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour
{
    Vector3 lastPos;
    Vector3 curPos;
    public CameraMove camMove;
    public float rotationSpeed;

    public CursorManager cursorManager;
    Player player;

    bool lDown;
    bool rDown;
    bool isDraging;

    void Awake()
    {
        player = Player.getPlayer();
    }

    void Update()
    {
        if (player.OnUi) return;
        InputCheck();
        BeginDrag();
    }

    void InputCheck()
    {
        lDown = Input.GetKey(KeyCode.Mouse0);
        rDown = Input.GetKey(KeyCode.Mouse1);
    }

    //마우스 오른쪽 드래그 시 커서 변경 및 코루틴 실행
    void BeginDrag()
    {
        if (!rDown) return;
        if (isDraging) return;

        lastPos = Input.mousePosition;
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.ROTATE);
        isDraging = true;
        StartCoroutine(Drag());
    }

    //마우스의 x축, y축 이동 거리를 측정하여 CameraMove의 Turn 함수 호출
    IEnumerator Drag()
    {
        while(rDown && !lDown)
        {
            curPos = Input.mousePosition;
            float xOffset = (curPos.x - lastPos.x) * rotationSpeed;
            float yOffset = (curPos.y - lastPos.y) * rotationSpeed;

            camMove.Turn(xOffset, yOffset);
            lastPos = curPos;
            yield return null;
        }
        isDraging = false;
        cursorManager.CursorChange((int)CursorManager.CursorIndexes.ROTATEEND);
    }
}
