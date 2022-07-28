using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D[] cursorTextures;
    public enum CursorIndexes { DEFAULT = 0, ROTATE = 1, CONV = 2, COLLECT = 3, ENTRANCE = 4, Enemy = 5, ROTATEEND = 99 }

    bool isRotating;
    int currentIdx;

    void Awake()
    {
        Cursor.SetCursor(cursorTextures[0], Vector2.zero, CursorMode.Auto);
        currentIdx = 0;
    }

    //idx에 따라 커서 스프라이트 변경
    //회전중인 경우, ROTATEEND 외 값 무시
    public void CursorChange(int idx)
    {
        if (currentIdx == idx) return;
        if (isRotating && idx != 99) return;

        if (idx == 99) idx = 0;
        Cursor.SetCursor(cursorTextures[idx], Vector2.zero, CursorMode.Auto);
        currentIdx = idx;

        if (idx == 1) isRotating = true;
        else isRotating = false;
    }
}
