
using UnityEditor;
using UnityEngine;

public static class LineDrawer
{
    public static void DrawHorizontalLine(Color color, int height = 20, int minWidth = 10, int maxWidth = 1000)
    {
        EditorGUILayout.Space(5f);
        Rect layoutRectangle = GUILayoutUtility.GetRect(minWidth,maxWidth,20,height);
        GUI.BeginClip(layoutRectangle);
        Handles.color = color;
        Handles.DrawAAPolyLine(
            Texture2D.whiteTexture,
            2,
            Vector3.zero,
            new Vector3(120, 0, 0),
            new Vector3(550, 0, 0));
        GUI.EndClip();
    }
}