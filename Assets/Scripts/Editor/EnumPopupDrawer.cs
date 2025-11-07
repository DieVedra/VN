using System;
using UnityEditor;

public class EnumPopupDrawer
{
    public void DrawEnumPopup<T>(SerializedProperty property, string label) where T : Enum
    {
        if (Enum.IsDefined(typeof(T), property.enumValueIndex))
        {
            T directionType = (T)(object)property.enumValueIndex;
            // EditorGUILayout.LabelField(label);
            directionType = (T)EditorGUILayout.EnumPopup(label,directionType);
            property.enumValueIndex = Convert.ToInt32(directionType);
        }
    }
}