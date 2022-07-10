using UnityEditor;
using UnityEngine;

public class EditorUtils
{
    public const float FIELD_SIZE_SMALL = 150f;
    public const float FIELD_SIZE_MEDIUM = 200f;
    public const float FIELD_SIZE_LARGE = 250f;
    public const float FIELD_SIZE_XLARGE = 300f;

    public const float SPACE_SIZE_SMALL = 10f;
    public const float SPACE_SIZE_MEDIUM = 20f;
    public const float SPACE_SIZE_LARGE = 30f;
    public const float SPACE_SIZE_XLARGE = 40f;

    private static Rect _horizontalLineRect = new Rect();
    public static void DrawHorizontalLine(Color color, float width = 0f)
    {
        _horizontalLineRect = EditorGUILayout.GetControlRect(false, 1f);

        if (width != 0f)
        {
            _horizontalLineRect.width = width;
        }

        EditorGUI.DrawRect(_horizontalLineRect, color);
    }
}
