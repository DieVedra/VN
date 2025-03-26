using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(CustomizationNode))]
public class CustomizationNodeDrawer : NodeEditor
{
    private CustomizationNode _customizationNode;
    private PopupDrawer _popupDrawer;
    private SerializedProperty _foldoutHairstyleIsOpenProperty;
    private SerializedProperty _foldoutClothesIsOpenProperty;
    private SerializedProperty _foldoutBodiesIsOpenProperty;
    private SerializedProperty _foldoutSwimsuitsIsOpenProperty;

    private SerializedProperty _listSettingsBodyProperty;
    private SerializedProperty _listSettingsHairstylesProperty;
    private SerializedProperty _listSettingsClothesProperty;
    private SerializedProperty _listSettingsSwimsuitsProperty;
    private Vector2 pos;
    public override void OnBodyGUI()
    {
        if (_customizationNode == null)
        {
            _customizationNode = target as CustomizationNode;
        }
        serializedObject.Update();
        
        
        
        if (_popupDrawer == null)
        {
            _popupDrawer = new PopupDrawer();
        }

        if (_foldoutHairstyleIsOpenProperty == null)
        {
            _foldoutHairstyleIsOpenProperty = serializedObject.FindProperty("_showFoldoutSettingsHairstyles");
            _foldoutClothesIsOpenProperty = serializedObject.FindProperty("_showFoldoutSettingsClothes");
            _foldoutBodiesIsOpenProperty = serializedObject.FindProperty("_showFoldoutSettingsBodies");
            _foldoutSwimsuitsIsOpenProperty = serializedObject.FindProperty("_showFoldoutSettingsSwimsuits");
            
            _listSettingsHairstylesProperty = serializedObject.FindProperty("_settingsHairstyles");
            _listSettingsClothesProperty = serializedObject.FindProperty("_settingsClothes");
            _listSettingsBodyProperty = serializedObject.FindProperty("_settingsBodies");
            _listSettingsSwimsuitsProperty = serializedObject.FindProperty("_settingsSwimsuits");
        }

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));

        if (_listSettingsBodyProperty != null)
        {
            DrawCustomizationFields(_listSettingsBodyProperty, _foldoutBodiesIsOpenProperty,
                "Choice Bodies","ResetBodiesCustomizationSettings", "ReinitBodiesCustomizationSettings");
        }

        if (_listSettingsHairstylesProperty != null)
        {
            DrawCustomizationFields(_listSettingsHairstylesProperty, _foldoutHairstyleIsOpenProperty,
                "Choice Hairstyles","ResetHairstylesCustomizationSettings", "ReinitHairstylesCustomizationSettings");
        }

        if (_listSettingsClothesProperty != null)
        {
            DrawCustomizationFields(_listSettingsClothesProperty, _foldoutClothesIsOpenProperty,
                "Choice Clothes","ResetClothesCustomizationSettings", "ReinitClothesCustomizationSettings");
        }

        if (_listSettingsSwimsuitsProperty != null)
        {
            DrawCustomizationFields(_listSettingsSwimsuitsProperty, _foldoutSwimsuitsIsOpenProperty,
                "Choice Swimsuits","ResetSwimsuitsCustomizationSettings", "ReinitSwimsuitsCustomizationSettings");
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCustomizationFields(SerializedProperty listSerializedProperty, SerializedProperty foldoutSerializedProperty, string label,string nameResetMethod, string nameReinitMethod)
    {
        DrawCustomizationSettingsFields(listSerializedProperty, foldoutSerializedProperty, label);
        EditorGUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset", GUILayout.Width(50f), GUILayout.Height(20f)))
        {
            InvokeMethod(nameResetMethod);
        }
        EditorGUILayout.EndHorizontal();
        LineDrawer.DrawHorizontalLine(Color.black);
    }
    private void DrawCustomizationSettingsFields(SerializedProperty listSerializedProperty, SerializedProperty foldoutSerializedProperty, string label)
    {
        foldoutSerializedProperty.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutSerializedProperty.boolValue,  label);
        if (foldoutSerializedProperty.boolValue)
        {
            pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Height(400f));
            SerializedProperty customizationSettingsSerializedProperty;
            SerializedProperty priceSerializedProperty;
            SerializedProperty keyAddSerializedProperty;
            SerializedProperty showParamsKeySerializedProperty;
            SerializedProperty showStatKeySerializedProperty;
            string nameLabel;
            for (int i = 0; i < listSerializedProperty.arraySize; i++)
            {
                customizationSettingsSerializedProperty = listSerializedProperty.GetArrayElementAtIndex(i);
                nameLabel = customizationSettingsSerializedProperty.FindPropertyRelative("_name").stringValue;
                keyAddSerializedProperty = customizationSettingsSerializedProperty.FindPropertyRelative("_keyAdd");
                showParamsKeySerializedProperty = customizationSettingsSerializedProperty.FindPropertyRelative("_keyShowParams");
                showStatKeySerializedProperty = customizationSettingsSerializedProperty.FindPropertyRelative("_keyShowStats");
                LineDrawer.DrawHorizontalLine(Color.green);
                EditorGUILayout.LabelField(nameLabel, GUILayout.Width(300f));
                EditorGUILayout.Space(5f);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Add: ", GUILayout.Width(30f));
                keyAddSerializedProperty.boolValue = EditorGUILayout.Toggle(keyAddSerializedProperty.boolValue, GUILayout.Width(30f));

                if (keyAddSerializedProperty.boolValue)
                {
                    EditorGUILayout.LabelField("Show Params: ", GUILayout.Width(90f));
                    showParamsKeySerializedProperty.boolValue = EditorGUILayout.Toggle(showParamsKeySerializedProperty.boolValue, GUILayout.Width(30f));
                    if (showParamsKeySerializedProperty.boolValue)
                    {
                        priceSerializedProperty = customizationSettingsSerializedProperty.FindPropertyRelative("_price");
                        EditorGUILayout.LabelField("Price: ", GUILayout.Width(40f));
                        priceSerializedProperty.intValue = EditorGUILayout.IntField(priceSerializedProperty.intValue, GUILayout.Width(30f));
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                
                if (showParamsKeySerializedProperty.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Show stat in game: ", GUILayout.Width(120f));

                    showStatKeySerializedProperty.boolValue = EditorGUILayout.Toggle(showStatKeySerializedProperty.boolValue, GUILayout.Width(50f));
                    EditorGUILayout.EndHorizontal();

                    DrawStats(customizationSettingsSerializedProperty.FindPropertyRelative("_gameStats"));
                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawStats(SerializedProperty gameStatsFormsSerializedProperty)
    {
        SerializedProperty statFormSerializedProperty;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10f);
        for (int i = 0; i < gameStatsFormsSerializedProperty.arraySize; i++)
        {
            statFormSerializedProperty = gameStatsFormsSerializedProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();

            DrawIntField(statFormSerializedProperty.FindPropertyRelative("_value"), statFormSerializedProperty.FindPropertyRelative("_name").stringValue);
            DrawBoolField(statFormSerializedProperty.FindPropertyRelative("_showKey"), "ShowKey: ");
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();
    }
    private void DrawIntField(SerializedProperty serializedProperty, string nameField)
    {
        EditorGUILayout.LabelField(nameField, GUILayout.Width(150f));
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue, GUILayout.Width(30f));
    }

    private void DrawBoolField(SerializedProperty serializedProperty, string nameField)
    {
        EditorGUILayout.LabelField(nameField, GUILayout.Width(60f));
        serializedProperty.boolValue = EditorGUILayout.Toggle(serializedProperty.boolValue, GUILayout.Width(30f));

    }
    private void InvokeMethod(string name)
    {
        MethodInfo methodInfo = _customizationNode.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(_customizationNode, null);
    }
}