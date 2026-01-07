using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(CustomizationNode))]
public class CustomizationNodeDrawer : NodeEditor
{
    private CustomizationNode _customizationNode;
    private PopupDrawer _popupDrawer;
    private LineDrawer _lineDrawer;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private SerializedProperty _customizationCharacterIndexProperty;

    private SerializedProperty _listSettingsBodyProperty;
    private SerializedProperty _listSettingsHairstylesProperty;
    private SerializedProperty _listSettingsClothesProperty;
    private SerializedProperty _listSettingsSwimsuitsProperty;
    
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    
    private SerializedProperty _addNotificationKeySerializedProperty;
    private SerializedProperty _notificationTextSerializedProperty;
    
    SerializedProperty _customizationSettingsSerializedProperty;
    SerializedProperty _priceSerializedProperty;
    SerializedProperty _keyAddSerializedProperty;
    SerializedProperty _showParamsKeySerializedProperty;
    SerializedProperty _showStatKeySerializedProperty;
    SerializedProperty _nameSprite;
    string _nameToGame;

    private Vector2 pos;
    private string[] _namesCharactersToPopup;
    
    private bool _foldoutHairstyle;
    private bool _foldoutClothes;
    private bool _foldoutBodies;
    private bool _foldoutSwimsuits;

    public override void OnBodyGUI()
    {
        if (_customizationNode == null)
        {
            _customizationNode = target as CustomizationNode;
            _popupDrawer = new PopupDrawer();
            _customizationCharacterIndexProperty = serializedObject.FindProperty("_customizationCharacterIndex");
            
            _listSettingsHairstylesProperty = serializedObject.FindProperty("_settingsHairstyles");
            _listSettingsClothesProperty = serializedObject.FindProperty("_settingsClothes");
            _listSettingsBodyProperty = serializedObject.FindProperty("_settingsBodies");
            _listSettingsSwimsuitsProperty = serializedObject.FindProperty("_settingsSwimsuits");
            
            _inputSerializedProperty = serializedObject.FindProperty("Input");
            _outputSerializedProperty = serializedObject.FindProperty("Output");
            _addNotificationKeySerializedProperty = serializedObject.FindProperty("_addNotificationKey");
            _notificationTextSerializedProperty = serializedObject.FindProperty("_notificationText");
            _localizationStringTextDrawer = new LocalizationStringTextDrawer();
            _lineDrawer = new LineDrawer();
            InitCharactersNames();
        }
        else
        {
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
            NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Notification: ", GUILayout.Width(120f));

            _addNotificationKeySerializedProperty.boolValue = EditorGUILayout.Toggle(_addNotificationKeySerializedProperty.boolValue, GUILayout.Width(50f));
            EditorGUILayout.EndHorizontal();
            if (_addNotificationKeySerializedProperty.boolValue)
            {
                _localizationStringTextDrawer.DrawTextField(
                    _localizationStringTextDrawer.GetLocalizationStringFromProperty(_notificationTextSerializedProperty),
                    "Notification: ", false, false);
                
            }
            _popupDrawer.DrawPopup(_namesCharactersToPopup, _customizationCharacterIndexProperty);
            if (_listSettingsBodyProperty != null)
            {
                DrawCustomizationFields(ref _foldoutBodies, _customizationNode.SettingsBodies,_listSettingsBodyProperty,
                    "Choice Bodies","ResetBodiesCustomizationSettings", "ReinitBodiesCustomizationSettings");
            }

            if (_listSettingsHairstylesProperty != null)
            {
                DrawCustomizationFields(ref _foldoutHairstyle, _customizationNode.SettingsHairstyles, _listSettingsHairstylesProperty,
                    "Choice Hairstyles","ResetHairstylesCustomizationSettings", "ReinitHairstylesCustomizationSettings");
            }

            if (_listSettingsClothesProperty != null)
            {
                DrawCustomizationFields(ref _foldoutClothes, _customizationNode.SettingsClothes, _listSettingsClothesProperty,
                    "Choice Clothes","ResetClothesCustomizationSettings", "ReinitClothesCustomizationSettings");
            }

            if (_listSettingsSwimsuitsProperty != null)
            {
                DrawCustomizationFields(ref _foldoutSwimsuits, _customizationNode.SettingsSwimsuits, _listSettingsSwimsuitsProperty,
                    "Choice Swimsuits","ResetSwimsuitsCustomizationSettings", "ReinitSwimsuitsCustomizationSettings");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawCustomizationFields(ref bool foldOutKey, IReadOnlyList<ICustomizationSettings> settings, SerializedProperty listSerializedProperty, string label,string nameResetMethod, string nameReinitMethod)
    {
        DrawCustomizationSettingsFields(ref foldOutKey, settings, listSerializedProperty, label);
        EditorGUILayout.Space(5f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset", GUILayout.Width(50f), GUILayout.Height(20f)))
        {
            InvokeMethod(nameResetMethod);
        }
        EditorGUILayout.EndHorizontal();
        _lineDrawer.DrawHorizontalLine(Color.black);
    }
    private void DrawCustomizationSettingsFields(ref bool foldOutKey, IReadOnlyList<ICustomizationSettings> settings,
        SerializedProperty listSerializedProperty, string label)
    {
        foldOutKey = EditorGUILayout.BeginFoldoutHeaderGroup(foldOutKey,  label);
        if (foldOutKey)
        {
            pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Height(400f));
            for (int i = 0; i < listSerializedProperty.arraySize; i++)
            {
                _customizationSettingsSerializedProperty = listSerializedProperty.GetArrayElementAtIndex(i);
                
                _nameToGame = settings[i].LocalizationNameToGame;
                
                _keyAddSerializedProperty = _customizationSettingsSerializedProperty.FindPropertyRelative("_keyAdd");
                _showParamsKeySerializedProperty = _customizationSettingsSerializedProperty.FindPropertyRelative("_keyShowParams");
                _showStatKeySerializedProperty = _customizationSettingsSerializedProperty.FindPropertyRelative("_keyShowStats");
                _nameSprite = _customizationSettingsSerializedProperty.FindPropertyRelative("_spriteName");
                _lineDrawer.DrawHorizontalLine(Color.green);
                EditorGUILayout.LabelField(_nameSprite.stringValue, GUILayout.Width(300f));
                
                EditorGUILayout.Space(5f);
                EditorGUILayout.LabelField("Add: ", GUILayout.Width(30f));
                _keyAddSerializedProperty.boolValue = EditorGUILayout.Toggle(_keyAddSerializedProperty.boolValue, GUILayout.Width(30f));

                if (_keyAddSerializedProperty.boolValue)
                {
                    EditorGUILayout.LabelField("Name to game: ", GUILayout.Width(100f));
                    EditorGUI.BeginChangeCheck();
                    _nameToGame = EditorGUILayout.TextField(_nameToGame, GUILayout.Width(450f));
                    if (EditorGUI.EndChangeCheck())
                    {
                        settings[i].LocalizationNameToGame.SetText(_nameToGame);
                        settings[i].LocalizationNameToGame.TryRegenerateKey();
                    }
                    EditorGUILayout.LabelField("Show Params: ", GUILayout.Width(90f));
                    _showParamsKeySerializedProperty.boolValue = EditorGUILayout.Toggle(_showParamsKeySerializedProperty.boolValue, GUILayout.Width(30f));
                    if (_showParamsKeySerializedProperty.boolValue)
                    {
                        _priceSerializedProperty = _customizationSettingsSerializedProperty.FindPropertyRelative("_price");
                        EditorGUILayout.LabelField($"Price: {_priceSerializedProperty.intValue}", GUILayout.Width(80f));
                    }
                }
                
                
                if (_showParamsKeySerializedProperty.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Show stat in game: ", GUILayout.Width(120f));

                    _showStatKeySerializedProperty.boolValue = EditorGUILayout.Toggle(_showStatKeySerializedProperty.boolValue, GUILayout.Width(50f));
                    EditorGUILayout.EndHorizontal();

                    DrawStats(settings[i].GameStatsLocalizationStrings, _customizationSettingsSerializedProperty.FindPropertyRelative("_gameStats"));
                }
            }
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawStats(IReadOnlyList<ILocalizationString> gameStatsLocalizationStrings, SerializedProperty gameStatsFormsSerializedProperty)
    {
        SerializedProperty statFormSerializedProperty;
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space(10f);
        for (int i = 0; i < gameStatsFormsSerializedProperty.arraySize; i++)
        {
            statFormSerializedProperty = gameStatsFormsSerializedProperty.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();

            DrawIntField(statFormSerializedProperty.FindPropertyRelative("_value"), gameStatsLocalizationStrings[i].LocalizationNameToGame.DefaultText);
            DrawBoolField(statFormSerializedProperty.FindPropertyRelative("_showKey"), "ShowKey: ");
            DrawBoolField(statFormSerializedProperty.FindPropertyRelative("_notificationKey"), "ShowNotification: ");

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
    private void InitCharactersNames()
    {
        if (_customizationNode.CustomizableCharacters != null)
        {
            var namesCharactersToPopup = new List<string>();
            for (int i = 0; i < _customizationNode.CustomizableCharacters.Count; i++)
            {
                if (_customizationNode.CustomizableCharacters[i] != null)
                {
                    namesCharactersToPopup.Add(_customizationNode.CustomizableCharacters[i].MyNameText);
                }
            }

            _namesCharactersToPopup = namesCharactersToPopup.ToArray();
        }
    }
}