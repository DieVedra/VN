using System.Collections.Generic;
using UnityEditor;

public class PopupDrawer
{
    public void DrawPopup(string[] names, SerializedProperty serializedProperty)
    {
        if (names != null)
        {
            serializedProperty.intValue = EditorGUILayout.Popup(serializedProperty.intValue,  names);
        }
    }
    public void DrawSpritePopup(string[] names, string label, IReadOnlyList<MySprite> spriteData, SerializedProperty indexSpriteProperty)
    {
        if (spriteData != null && spriteData.Count > 0)
        {
            EditorGUILayout.LabelField(label);
            DrawPopup(names, indexSpriteProperty);
        }
    }
}