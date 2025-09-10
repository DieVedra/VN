using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(CharacterNode))]
public class CharacterNodeDrawer : NodeEditor
{
    private const string _currentCharacterLabel = "Current character: ";
    private const string _currentDirectionLabel = "Current direction: ";
    private const string _currentEmotionLabel = "Current emotion: ";
    private const string _currentLookLabel = "Current look: ";
    private const string _currentTextLabel = "Character Text: ";
    private const string _fouldoutLabel = "Settings character: ";
    
    private const string _indexCharacterNameProperty = "_indexCharacter";
    private const string _indexEmotionNameProperty = "_indexEmotion";
    private const string _indexLookNameProperty = "_indexLook";
    private const string _directionTypeNameProperty = "_directionType";
    private const string _textProperty = "_localizationText";
    private const string _foldoutIsOpenNameProperty = "_foldoutIsOpen";
    
    private const string _previousIndexCharacterNameProperty = "_previousIndexCharacter";
    private const string _previousCharactersCountNameProperty = "_previousCharactersCount";
    private const string _nameMethod = "SetInfoToView";
    private const string _overrideNameNameProperty = "_overrideName";
    private const string _overridedNameNameProperty = "_overridedNameLocalization";
    private const string _toggleShowPanelNameProperty = "_toggleShowPanel";
    private const string _toggleIsSwimsuitNameProperty = "_toggleIsSwimsuit";
    private const string _inputNameProperty = "Input";
    private const string _outputNameProperty = "Output";
    
    private const string _overrideNameLabel = "New name: ";
    private const int _symbolMaxCount = 159;
    private string _validText;

    private SerializedProperty _indexLookProperty;
    private SerializedProperty _indexEmotionProperty;
    private SerializedProperty _indexCharacterProperty;
    private SerializedProperty _directionCharacterProperty;
    private SerializedProperty _localizationTextCharacterProperty;
    private SerializedProperty _toggleShowPanelProperty;
    
    private SerializedProperty _previousIndexCharacterProperty;
    private SerializedProperty _previousCharactersCountProperty;
    private SerializedProperty _foldoutIsOpenProperty;
    private SerializedProperty _overrideNameProperty;
    private SerializedProperty _overridedNameProperty;
    private SerializedProperty _toggleIsSwimsuitProperty;
    private SerializedProperty _inputPortProperty;
    private SerializedProperty _outputPortProperty;

    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private LocalizationString _localizationStringText;
    private LocalizationString _localizationStringOverridedName;
    private MethodInfo _privateMethod;
    private CharacterNode _characterNode;
    private PopupDrawer _popupDrawer;
    private LineDrawer _lineDrawer;
    private List<string> _namesCharactersToPopup;
    private string[] _namesLookCharactersToPopup;
    private string[] _namesEmotionsCharactersToPopup;
    public override void OnBodyGUI()
    {
        if (_characterNode == null)
        {
            _characterNode = target as CharacterNode;
        }

        if (_indexLookProperty == null)
        {
            _lineDrawer = new LineDrawer();
            _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(_symbolMaxCount));
            serializedObject.Update();
            TryInitProperty(ref _indexLookProperty, _indexLookNameProperty);
            TryInitProperty(ref _indexEmotionProperty, _indexEmotionNameProperty);
            TryInitProperty(ref _overrideNameProperty, _overrideNameNameProperty);
            TryInitProperty(ref _overridedNameProperty, _overridedNameNameProperty);
            TryInitProperty(ref _toggleShowPanelProperty, _toggleShowPanelNameProperty);
            TryInitProperty(ref _indexCharacterProperty, _indexCharacterNameProperty);
            TryInitProperty(ref _previousIndexCharacterProperty, _previousIndexCharacterNameProperty);
            TryInitProperty(ref _previousCharactersCountProperty, _previousCharactersCountNameProperty);
            TryInitProperty(ref _directionCharacterProperty, _directionTypeNameProperty);
            TryInitProperty(ref _localizationTextCharacterProperty, _textProperty);
            TryInitProperty(ref _foldoutIsOpenProperty, _foldoutIsOpenNameProperty);
            TryInitProperty(ref _toggleIsSwimsuitProperty, _toggleIsSwimsuitNameProperty);
            TryInitProperty(ref _inputPortProperty, _inputNameProperty);
            TryInitProperty(ref _outputPortProperty, _outputNameProperty);
            _localizationStringText = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_localizationTextCharacterProperty);
            _localizationStringOverridedName = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_overridedNameProperty);
        }

        serializedObject.Update();
        NodeEditorGUILayout.PropertyField(_inputPortProperty);
        NodeEditorGUILayout.PropertyField(_outputPortProperty);
        EditorGUI.BeginChangeCheck();
        _localizationStringTextDrawer.DrawTextField(_localizationStringText, _currentTextLabel);
        _lineDrawer.DrawHorizontalLine(Color.green);
        DrawPopupCharacters();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            SetInfoToView();
        }
    }

    private void DrawPopupCharacters()
    {
        if (_popupDrawer == null)
        {
            _popupDrawer = new PopupDrawer();
        }
        if (_characterNode.Characters != null && _characterNode.Characters.Count > 0)
        {
            SetCharactersNames();
            EditorGUILayout.LabelField(_currentCharacterLabel);
            _popupDrawer.DrawPopup(_namesCharactersToPopup.ToArray(), _indexCharacterProperty);
            
            if (_previousIndexCharacterProperty.intValue != _indexCharacterProperty.intValue)
            {
                _indexEmotionProperty.intValue = 0;
                _indexLookProperty.intValue = 0;
                _previousIndexCharacterProperty.intValue = _indexCharacterProperty.intValue;
            }

            DrawToggle(_overrideNameProperty, "Override NameText ");
            if (_overrideNameProperty.boolValue == true)
            {
                _localizationStringTextDrawer.DrawTextField(_localizationStringOverridedName, _overrideNameLabel, false);
            }
            
            _foldoutIsOpenProperty.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutIsOpenProperty.boolValue, _fouldoutLabel);
            if (_foldoutIsOpenProperty.boolValue)
            {
                DrawEnumPopup();


                if (_characterNode.Characters[_indexCharacterProperty.intValue] is CustomizableCharacter customizableCharacter)
                {
                    TryInitEmotionsNames(ref _namesEmotionsCharactersToPopup,
                        customizableCharacter.GetCurrentEmotionsDataByBodyIndex());
                        
                    _popupDrawer.DrawSpritePopup(_namesEmotionsCharactersToPopup,
                        _currentEmotionLabel,
                        customizableCharacter.GetCurrentEmotionsDataByBodyIndex(), _indexEmotionProperty);
                        
                    DrawToggle(_toggleIsSwimsuitProperty, "Is Swimsuit Look");
                }
                else if(_characterNode.Characters[_indexCharacterProperty.intValue] is SimpleCharacter simpleCharacter)
                {
                    TryInitNames(ref _namesLookCharactersToPopup, simpleCharacter.LooksData);
                    _popupDrawer.DrawSpritePopup(_namesLookCharactersToPopup, _currentLookLabel,
                        simpleCharacter.LooksData, _indexLookProperty);

                    if (simpleCharacter.EmotionsData != null)
                    {
                        TryInitEmotionsNames(ref _namesEmotionsCharactersToPopup, simpleCharacter.EmotionsData);
                        _popupDrawer.DrawSpritePopup(_namesEmotionsCharactersToPopup, _currentEmotionLabel,
                            simpleCharacter.EmotionsData, _indexEmotionProperty);
                    }
                }
                DrawToggle(_toggleShowPanelProperty, "Show text panel");
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    private void TryInitNames(ref string[] names, SpriteData spriteData)
    {
        names = spriteData.GetNames();
    }
    private void TryInitEmotionsNames(ref string[] names, SpriteData spriteData)
    {
        TryInitNames(ref names, spriteData);
        string[] names2 = {"Стандарт"};
        string[] newNames = names2.Concat(names).ToArray();
        names = newNames;
    }
    private void DrawEnumPopup()
    {
        DirectionType directionType = (DirectionType)_directionCharacterProperty.enumValueIndex;
        EditorGUILayout.LabelField(_currentDirectionLabel);
        directionType = (DirectionType)EditorGUILayout.EnumPopup(directionType);
        _directionCharacterProperty.enumValueIndex = (int) directionType;
    }

    private SerializedProperty GetProperty(string propertyName)
    {
        return serializedObject.FindProperty(propertyName);
    }

    private void SetInfoToView()
    {
        if (_privateMethod == null)
        {
            _privateMethod = _characterNode.GetType().GetMethod(_nameMethod, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        _privateMethod.Invoke(_characterNode, null);
    }

    private void SetCharactersNames()
    {
        if (_previousCharactersCountProperty.intValue != _characterNode.Characters.Count)
        {
            InitCharactersNames();
            _previousCharactersCountProperty.intValue = _characterNode.Characters.Count;
        }
        else if(_namesCharactersToPopup == null)
        {
            InitCharactersNames();
        }
    }

    private void InitCharactersNames()
    {
        _namesCharactersToPopup = new List<string>();
        for (int i = 0; i < _characterNode.Characters.Count; i++)
        {
            if (_characterNode.Characters[i] != null)
            {
                _namesCharactersToPopup.Add(_characterNode.Characters[i].MyNameText);
            }
        }
    }

    private void TryInitProperty(ref SerializedProperty property, string name)
    {
        if (property == null)
        {
            property = GetProperty(name);
        }
    }

    private void DrawToggle(SerializedProperty serializedProperty, string label)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);
        serializedProperty.boolValue = EditorGUILayout.Toggle(serializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
    }
}