using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(CharacterNode))]
public class CharacterNodeDrawer : NodeEditor
{
    private readonly string _currentCharacterLabel = "Current character: ";
    private readonly string _currentDirectionLabel = "Current direction: ";
    private readonly string _currentEmotionLabel = "Current emotion: ";
    private readonly string _currentLookLabel = "Current look: ";
    private readonly string _currentTextLabel = "Character Text: ";
    private readonly string _fouldoutLabel = "Settings character: ";
    
    private readonly string _indexCharacterNameProperty = "_indexCharacter";
    private readonly string _indexEmotionNameProperty = "_indexEmotion";
    private readonly string _indexLookNameProperty = "_indexLook";
    private readonly string _directionTypeNameProperty = "_directionType";
    private readonly string _textNameProperty = "_text";
    private readonly string _foldoutIsOpenNameProperty = "_foldoutIsOpen";
    
    private readonly string _previousIndexCharacterNameProperty = "_previousIndexCharacter";
    private readonly string _previousCharactersCountNameProperty = "_previousCharactersCount";
    private readonly string _nameMethod = "SetInfoToView";
    
    private readonly int _symbolMaxCount = 159;
    private string _validText;

    private SerializedProperty _indexLookProperty;
    private SerializedProperty _indexEmotionProperty;
    private SerializedProperty _indexCharacterProperty;
    private SerializedProperty _directionCharacterProperty;
    private SerializedProperty _textCharacterProperty;
    private SerializedProperty _toggleShowPanelProperty;
    
    private SerializedProperty _previousIndexCharacterProperty;
    private SerializedProperty _previousCharactersCountProperty;
    private SerializedProperty _foldoutIsOpenProperty;
    private SerializedProperty _overrideNameProperty;
    private SerializedProperty _overridedNameProperty;
    
    
    private MethodInfo _privateMethod;
    private CharacterNode _characterNode;
    private SimpleTextValidator _simpleTextValidator;
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
            _lineDrawer = new LineDrawer();
            serializedObject.Update();
            TryInitProperty(ref _indexLookProperty, _indexLookNameProperty);
            TryInitProperty(ref _indexEmotionProperty, _indexEmotionNameProperty);
            TryInitProperty(ref _overrideNameProperty, "_overrideName");
            TryInitProperty(ref _overridedNameProperty, "_overridedName");
            TryInitProperty(ref _toggleShowPanelProperty, "_toggleShowPanel");

            
            TryInitProperty(ref _indexCharacterProperty, _indexCharacterNameProperty);
            TryInitProperty(ref _previousIndexCharacterProperty, _previousIndexCharacterNameProperty);
            TryInitProperty(ref _previousCharactersCountProperty, _previousCharactersCountNameProperty);
            TryInitProperty(ref _directionCharacterProperty, _directionTypeNameProperty);
            TryInitProperty(ref _textCharacterProperty, _textNameProperty);
            TryInitProperty(ref _foldoutIsOpenProperty, _foldoutIsOpenNameProperty);
        }

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
        EditorGUI.BeginChangeCheck();
        DrawTextField();
        _lineDrawer.DrawHorizontalLine(Color.green);
        DrawPopupCharacters();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            SetInfoToView();
        }
    }

    private void DrawTextField()
    {
        if (_simpleTextValidator == null)
        {
            _simpleTextValidator = new SimpleTextValidator();
            _validText = _textCharacterProperty.stringValue;
        }
        EditorGUILayout.LabelField(_currentTextLabel);
        _validText = EditorGUILayout.TextArea(_validText, GUILayout.Height(50f), GUILayout.Width(150f));

        if (_simpleTextValidator.TryValidate(ref _validText, _symbolMaxCount))
        {
            _textCharacterProperty.stringValue = _validText;
        }
        else
        {
            EditorGUILayout.HelpBox("Размер строки превышен " , MessageType.Error);
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
            // _overrideNameProperty = serializedObject.FindProperty("_overrideName");

            DrawToggle(_overrideNameProperty, "Override Name ");
            if (_overrideNameProperty.boolValue == true)
            {
                _overridedNameProperty.stringValue =
                    EditorGUILayout.TextField("New name: ", _overridedNameProperty.stringValue);
            }
            
            _foldoutIsOpenProperty.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutIsOpenProperty.boolValue, _fouldoutLabel);
            if (_foldoutIsOpenProperty.boolValue)
            {
                DrawEnumPopup();
                if (CheckCustomizableCharacter() == false)
                {
                    TryInitNames(ref _namesLookCharactersToPopup, _characterNode.Characters[_indexCharacterProperty.intValue].LooksData);
                    _popupDrawer.DrawSpritePopup(_namesLookCharactersToPopup, _currentLookLabel,
                        _characterNode.Characters[_indexCharacterProperty.intValue].LooksData, _indexLookProperty);

                    if (_characterNode.Characters[_indexCharacterProperty.intValue].EmotionsData != null)
                    {
                        TryInitEmotionsNames(ref _namesEmotionsCharactersToPopup, _characterNode.Characters[_indexCharacterProperty.intValue].EmotionsData);
                        _popupDrawer.DrawSpritePopup(_namesEmotionsCharactersToPopup, _currentEmotionLabel,
                            _characterNode.Characters[_indexCharacterProperty.intValue].EmotionsData, _indexEmotionProperty);
                    }
                }
                else
                {
                    CustomizableCharacter customizableCharacter = _characterNode.Characters[_indexCharacterProperty.intValue] as CustomizableCharacter;
                    if (customizableCharacter != null)
                    {
                        TryInitEmotionsNames(ref _namesEmotionsCharactersToPopup, _characterNode.Characters[_indexCharacterProperty.intValue].EmotionsData);
                        _popupDrawer.DrawSpritePopup(_namesEmotionsCharactersToPopup,
                            _currentEmotionLabel,
                            customizableCharacter.GetCurrentEmotionsData(), _indexEmotionProperty);
                        
                        DrawToggle(serializedObject.FindProperty("_toggleIsSwimsuit"), "Is Swimsuit Look");
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
        string[] newNames = names.Concat(names2).ToArray();
        names = newNames;
    }
    private bool CheckCustomizableCharacter()
    {
        if (_characterNode.Characters[_indexCharacterProperty.intValue] is CustomizableCharacter)
        {
            return true;
        }
        else
        {
            return false;
        }
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
                _namesCharactersToPopup.Add(_characterNode.Characters[i].name);
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