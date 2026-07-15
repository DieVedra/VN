using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MyProject;
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
    private MyProject.EnumPopupDrawer _enumPopupDrawer;

    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private LocalizationString _localizationStringText;
    private LocalizationString _localizationStringOverridedName;
    private MethodInfo _privateMethod;
    private CharacterNode _characterNode;
    private PopupDrawer _popupDrawer;
    private LineDrawer _lineDrawer;
    private StringBuilder _stringBuilder;
    private Character _character;
    private string[] _namesCharactersToPopupArray;
    private string[] _namesLookCharactersToPopup;
    private string[] _namesEmotionsCharactersToPopup;
    private string _name;
    private int _numberCoincidences = 0;
    private bool _initialized = false;

    public override void OnBodyGUI()
    {
        if (!_initialized)
        {
            _characterNode = target as CharacterNode;
            if (_characterNode == null) return;
            
            _lineDrawer = new LineDrawer();
            _popupDrawer = new PopupDrawer();
            _enumPopupDrawer = new EnumPopupDrawer();
            _stringBuilder = new StringBuilder();
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
            
            if (_localizationTextCharacterProperty != null)
                _localizationStringText = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_localizationTextCharacterProperty);
            if (_overridedNameProperty != null)
                _localizationStringOverridedName = _localizationStringTextDrawer.GetLocalizationStringFromProperty(_overridedNameProperty);
            
            _initialized = true;
            return;
        }
        
        if (_characterNode == null || _inputPortProperty == null || _outputPortProperty == null)
        {
            _initialized = false;
            return;
        }
        
        serializedObject.Update();
        
        try
        {
            EditorGUILayout.BeginVertical();
            
            NodeEditorGUILayout.PropertyField(_inputPortProperty);
            NodeEditorGUILayout.PropertyField(_outputPortProperty);
            
            EditorGUI.BeginChangeCheck();
            
            _localizationStringTextDrawer.DrawTextField(_localizationStringText, _currentTextLabel, validateText: false);
            _lineDrawer.DrawHorizontalLine(Color.green);
            
            DrawPopupCharacters();
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                SetInfoToView();
            }
            
            EditorGUILayout.EndVertical();
        }
        catch (ArgumentException e)
        {
            Debug.LogWarning($"[CharacterNodeDrawer] GUI error: {e.Message}");
            
            try { EditorGUILayout.EndVertical(); } catch { }
        }
    }

    private void DrawPopupCharacters()
    {
        if (_characterNode.Characters == null || _characterNode.Characters.Count == 0)
            return;

        SetCharactersNames();
        
        EditorGUILayout.LabelField(_currentCharacterLabel);
        _popupDrawer.DrawPopup(_namesCharactersToPopupArray, _indexCharacterProperty);
        
        if (_previousIndexCharacterProperty != null && _indexCharacterProperty != null)
        {
            if (_previousIndexCharacterProperty.intValue != _indexCharacterProperty.intValue)
            {
                if (_indexEmotionProperty != null) _indexEmotionProperty.intValue = 0;
                if (_indexLookProperty != null) _indexLookProperty.intValue = 0;
                _previousIndexCharacterProperty.intValue = _indexCharacterProperty.intValue;
            }
        }

        DrawToggle(_overrideNameProperty, "Override NameText ");
        
        if (_overrideNameProperty != null && _overrideNameProperty.boolValue)
        {
            _localizationStringTextDrawer.DrawTextField(_localizationStringOverridedName, _overrideNameLabel, false);
        }
        
        if (_foldoutIsOpenProperty != null)
        {
            _foldoutIsOpenProperty.boolValue = EditorGUILayout.Foldout(
                _foldoutIsOpenProperty.boolValue, _fouldoutLabel, true);
            
            if (_foldoutIsOpenProperty.boolValue)
            {
                DrawFoldoutContent();
            }
        }
    }

    private void DrawFoldoutContent()
    {
        if (_directionCharacterProperty != null)
            _enumPopupDrawer.DrawEnumPopup<DirectionType>(_directionCharacterProperty, _currentDirectionLabel);

        if (_indexCharacterProperty == null || _characterNode.Characters == null)
            return;
            
        if (_indexCharacterProperty.intValue >= _characterNode.Characters.Count)
            return;

        var character = _characterNode.Characters[_indexCharacterProperty.intValue];
        
        if (character is CustomizableCharacter customizableCharacter)
        {
            var emotions = customizableCharacter.GetCurrentEmotionsDataByBodyIndex()?.GetMySprites;
            if (emotions != null)
            {
                TryInitEmotionsNames(ref _namesEmotionsCharactersToPopup, emotions);
                _popupDrawer.DrawSpritePopup(_namesEmotionsCharactersToPopup,
                    _currentEmotionLabel, emotions, _indexEmotionProperty);
            }
                
            DrawToggle(_toggleIsSwimsuitProperty, "Is Swimsuit Look");
        }
        else if (character is SimpleCharacter simpleCharacter)
        {
            if (simpleCharacter.Looks != null)
            {
                TryInitNames(ref _namesLookCharactersToPopup, simpleCharacter.Looks);
                _popupDrawer.DrawSpritePopup(_namesLookCharactersToPopup, _currentLookLabel,
                    simpleCharacter.Looks, _indexLookProperty);
            }

            if (simpleCharacter.Emotions != null)
            {
                TryInitEmotionsNames(ref _namesEmotionsCharactersToPopup, simpleCharacter.Emotions);
                _popupDrawer.DrawSpritePopup(_namesEmotionsCharactersToPopup, _currentEmotionLabel,
                    simpleCharacter.Emotions, _indexEmotionProperty);
            }
        }
        
        DrawToggle(_toggleShowPanelProperty, "Show text panel");
    }

    private void TryInitNames(ref string[] names, IReadOnlyList<MySprite> spriteData)
    {
        if (spriteData == null)
        {
            names = Array.Empty<string>();
            return;
        }
        
        var count = spriteData.Count;
        List<string> names1 = new List<string>(count);
        for (int i = 0; i < count; i++)
        {
            if (spriteData[i] != null)
            {
                names1.Add(spriteData[i].Name);
            }
        }
        names = names1.ToArray();
    }
    
    private void TryInitEmotionsNames(ref string[] names, IReadOnlyList<MySprite> spriteData)
    {
        TryInitNames(ref names, spriteData);
        string[] names2 = {"Стандарт"};
        string[] newNames = names2.Concat(names).ToArray();
        names = newNames;
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
        _privateMethod?.Invoke(_characterNode, null);
    }

    private void SetCharactersNames()
    {
        if (_previousCharactersCountProperty == null || _characterNode?.Characters == null)
            return;
            
        if (_previousCharactersCountProperty.intValue != _characterNode.Characters.Count)
        {
            InitCharactersNames();
            _previousCharactersCountProperty.intValue = _characterNode.Characters.Count;
        }
        else if (_namesCharactersToPopupArray == null)
        {
            InitCharactersNames();
        }
    }

    private void InitCharactersNames()
    {
        if (_characterNode?.Characters == null)
            return;
            
        int count = _characterNode.Characters.Count;
        
        if (_namesCharactersToPopupArray == null || _namesCharactersToPopupArray.Length != count)
        {
            _namesCharactersToPopupArray = new string[count];
        }
        else
        {
            for (int i = 0; i < _namesCharactersToPopupArray.Length; i++)
            {
                _namesCharactersToPopupArray[i] = String.Empty;
            }
        }

        for (int i = 0; i < count; i++)
        {
            _character = _characterNode.Characters[i];
            _numberCoincidences = 0;
            _name = _characterNode.Characters[i].MyNameText;
            _stringBuilder.Clear();
            
            if (_character != null)
            {
                for (int j = 0; j < _namesCharactersToPopupArray.Length; j++)
                {
                    if (_namesCharactersToPopupArray[j] != null && _name == _namesCharactersToPopupArray[j])
                    {
                        _stringBuilder.Append(_name);
                        _stringBuilder.Append(' ');
                        _stringBuilder.Append(++_numberCoincidences);
                        _name = _stringBuilder.ToString();
                    }
                }
                _namesCharactersToPopupArray[i] = _name;
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
        if (serializedProperty == null) return;
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);
        serializedProperty.boolValue = EditorGUILayout.Toggle(serializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
    }
}