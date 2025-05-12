using UnityEngine;

public class CursorUI : MonoBehaviour
{
    public RectTransform crosshairUI;

    void Start()
    {
        Cursor.visible = false; // 시스템 커서 숨기기
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        crosshairUI.position = mousePos;
    }
}
