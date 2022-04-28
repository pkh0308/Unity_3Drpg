using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D[] cursorTextures;
    public enum CursorIndexes { DEFAULT = 0, ROTATE = 1 }

    public void CursorChange(int idx)
    {
        Cursor.SetCursor(cursorTextures[idx], Vector2.zero, CursorMode.Auto);
    }
}
