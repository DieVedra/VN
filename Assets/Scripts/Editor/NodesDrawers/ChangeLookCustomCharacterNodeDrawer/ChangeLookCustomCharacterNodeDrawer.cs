using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(ChangeLookCustomCharacterNode))]
public class ChangeLookCustomCharacterNodeDrawer : NodeEditor
{
    private ChangeLookCustomCharacterNode _changeLookCustomCharacterNode;
    private SerializedProperty _skipToWardrobeVariantProperty;
    private SerializedProperty _customizationCharacterIndexProperty;
    private SerializedProperty _hairstyleIndexProperty;
    private SerializedProperty _clothesIndexProperty;
    private SerializedProperty _swimsuitIndexProperty;
    
    private SerializedProperty _putHairstyleProperty;
    private SerializedProperty _putClothesProperty;
    private SerializedProperty _putSwimsuitProperty;
    
    private SerializedProperty _inputProperty;
    private SerializedProperty _outputProperty;
    private string[] _namesCharactersToPopup;
    private string[] _namesClothesToPopup;
    private string[] _namesHairstyleToPopup;
    private string[] _namesSwimsuitsToPopup;
    private int _clothesNameIndex =0;
    private int _hairstyleNameIndex=0;
    private int _swimsuitNameIndex=0;
    private CustomizableCharacterNonWardrobeIndexes _currentCustomizableCharacterNonWardrobeIndexes;
    private string GetNameKey => _changeLookCustomCharacterNode
        .CustomizableCharacters[_customizationCharacterIndexProperty.intValue].Name.Key;

    private PopupDrawer _popupDrawer;

    private Dictionary<string, CustomizableCharacterNonWardrobeIndexes> _customizableCharacterNonWardrobeIndexesDictionary;

    public override void OnBodyGUI()
    {
        if (_changeLookCustomCharacterNode == null)
        {
            _changeLookCustomCharacterNode = target as ChangeLookCustomCharacterNode;
        }
        else
        {
            if (_changeLookCustomCharacterNode.CustomizableCharacters != null)
            {
                serializedObject.Update();
                if (_skipToWardrobeVariantProperty == null)
                {
                    _skipToWardrobeVariantProperty = serializedObject.FindProperty("_skipToWardrobeVariant");
                    _customizationCharacterIndexProperty = serializedObject.FindProperty("_customizationCharacterIndex");
                    _hairstyleIndexProperty = serializedObject.FindProperty("_hairstyleIndex");
                    _clothesIndexProperty = serializedObject.FindProperty("_clothesIndex");
                    _swimsuitIndexProperty = serializedObject.FindProperty("_swimsuitIndex");
                    
                    _putHairstyleProperty = serializedObject.FindProperty("_putHairstyle");
                    _putClothesProperty = serializedObject.FindProperty("_putClothes");
                    _putSwimsuitProperty = serializedObject.FindProperty("_putSwimsuit");
                    
                    _inputProperty = serializedObject.FindProperty("Input");
                    _outputProperty = serializedObject.FindProperty("Output");
                    _popupDrawer = new PopupDrawer();
                }
                NodeEditorGUILayout.PropertyField(_inputProperty);
                if (_customizableCharacterNonWardrobeIndexesDictionary != null)
                {
                    EditorGUI.BeginChangeCheck();
                    _popupDrawer.DrawPopup(_namesCharactersToPopup, _customizationCharacterIndexProperty);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (_customizableCharacterNonWardrobeIndexesDictionary.TryGetValue(GetNameKey, out _currentCustomizableCharacterNonWardrobeIndexes))
                        {
                            InitCharacterContentsNames(GetNameKey);
                        }
                    }
                    if (_skipToWardrobeVariantProperty.boolValue == false && _currentCustomizableCharacterNonWardrobeIndexes != null)
                    {
                        if (_namesClothesToPopup != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Put Clothes:", GUILayout.Width(200f));
                            _putClothesProperty.boolValue = EditorGUILayout.Toggle(_putClothesProperty.boolValue, GUILayout.Width(30f));
                            EditorGUILayout.EndHorizontal();
                            if (_putClothesProperty.boolValue == true)
                            {
                                _clothesNameIndex = EditorGUILayout.Popup(_clothesNameIndex, _namesClothesToPopup);
                                _clothesIndexProperty.intValue = _currentCustomizableCharacterNonWardrobeIndexes.ClothesIndexes[_clothesNameIndex].Item2;
                            }
                        }


                        if (_namesHairstyleToPopup != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Put Hairstyle:", GUILayout.Width(200f));
                            _putHairstyleProperty.boolValue = EditorGUILayout.Toggle(_putHairstyleProperty.boolValue, GUILayout.Width(30f));
                            EditorGUILayout.EndHorizontal();
                            if (_putHairstyleProperty.boolValue == true)
                            {
                                _hairstyleNameIndex = EditorGUILayout.Popup(_hairstyleNameIndex, _namesHairstyleToPopup);
                                _hairstyleIndexProperty.intValue = _currentCustomizableCharacterNonWardrobeIndexes.HairstyleIndexes[_clothesNameIndex].Item2;
                            }
                        }

                        if (_namesSwimsuitsToPopup != null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Put Swimsuit:", GUILayout.Width(200f));
                            _putSwimsuitProperty.boolValue = EditorGUILayout.Toggle(_putSwimsuitProperty.boolValue, GUILayout.Width(30f));
                            EditorGUILayout.EndHorizontal();
                            if (_putSwimsuitProperty.boolValue == true)
                            {
                                _swimsuitNameIndex = EditorGUILayout.Popup(_swimsuitNameIndex, _namesSwimsuitsToPopup);
                                _swimsuitIndexProperty.intValue = _currentCustomizableCharacterNonWardrobeIndexes.SwimsuitsIndexes[_swimsuitNameIndex].Item2;
                            }
                        }
                    }

                    if ((_putHairstyleProperty.boolValue == false && _putClothesProperty.boolValue == false && _putSwimsuitProperty.boolValue == false))
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("SkipToWardrobeVariant: ", GUILayout.Width(200f));
                        _skipToWardrobeVariantProperty.boolValue = EditorGUILayout.Toggle(_skipToWardrobeVariantProperty.boolValue, GUILayout.Width(30f));
                        EditorGUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Reinit"))
                    {
                        TryInit();
                        InitCharactersNames();
                        InitCharacterContentsNames(GetNameKey);
                    }
                }
                else if(_changeLookCustomCharacterNode.CustomizableCharacters != null)
                {
                    TryInit();
                    InitCharactersNames();
                    InitCharacterContentsNames(GetNameKey);
                    if (_customizableCharacterNonWardrobeIndexesDictionary.TryGetValue(GetNameKey, out _currentCustomizableCharacterNonWardrobeIndexes))
                    {
                        InitCharacterContentsNames(GetNameKey);
                        SetNameIndexOnInit(ref _clothesNameIndex, _namesClothesToPopup,
                            _currentCustomizableCharacterNonWardrobeIndexes.ClothesIndexes, _clothesIndexProperty);
                        
                        SetNameIndexOnInit(ref _hairstyleNameIndex, _namesHairstyleToPopup,
                            _currentCustomizableCharacterNonWardrobeIndexes.HairstyleIndexes, _hairstyleIndexProperty);
                        
                        SetNameIndexOnInit(ref _swimsuitNameIndex, _namesSwimsuitsToPopup,
                            _currentCustomizableCharacterNonWardrobeIndexes.SwimsuitsIndexes, _swimsuitIndexProperty);
                    }
                    
                }

                NodeEditorGUILayout.PropertyField(_outputProperty);
            }
        }
    }
    private void InitCharactersNames()
    {
        if (_changeLookCustomCharacterNode.CustomizableCharacters != null)
        {
            var namesCharactersToPopup = new List<string>();
            for (int i = 0; i < _changeLookCustomCharacterNode.CustomizableCharacters.Count; i++)
            {
                if (_changeLookCustomCharacterNode.CustomizableCharacters[i] != null)
                {
                    namesCharactersToPopup.Add(_changeLookCustomCharacterNode.CustomizableCharacters[i].MyNameText);
                }
            }
            _namesCharactersToPopup = namesCharactersToPopup.ToArray();
        }
    }

    private void TryInit()
    {
        _customizableCharacterNonWardrobeIndexesDictionary = new Dictionary<string, CustomizableCharacterNonWardrobeIndexes>();
        var customizableCharacters = _changeLookCustomCharacterNode.CustomizableCharacters;
        for (int i = 0; i < customizableCharacters.Count; i++)
        {
            var indexes = GetChangeLookCustomCharacterNodeInfo(customizableCharacters[i]);
            _customizableCharacterNonWardrobeIndexesDictionary.Add(customizableCharacters[i].Name.Key, indexes);
        }
    }

    private void InitCharacterContentsNames(string nameKey)
    {
        if (_customizableCharacterNonWardrobeIndexesDictionary.TryGetValue(nameKey, out CustomizableCharacterNonWardrobeIndexes customizableCharacterNonWardrobeIndexes))
        {
            InitNames(ref _namesClothesToPopup, customizableCharacterNonWardrobeIndexes.ClothesIndexes);
            InitNames(ref _namesHairstyleToPopup, customizableCharacterNonWardrobeIndexes.HairstyleIndexes);
            InitNames(ref _namesSwimsuitsToPopup, customizableCharacterNonWardrobeIndexes.SwimsuitsIndexes);
        }
    }

    private void InitNames(ref string[] namesToPopup, (string, int)[] indexess)
    {
        if (indexess == null)
        {
            namesToPopup = null;
            return;
        }
        List<string> newNamesToPopup = new List<string>();
        for (int j = 0; j < indexess.Length; j++)
        {
            newNamesToPopup.Add(indexess[j].Item1);
        }
        namesToPopup = newNamesToPopup.ToArray();
    }
    private CustomizableCharacterNonWardrobeIndexes GetChangeLookCustomCharacterNodeInfo(CustomizableCharacter customizableCharacter)
    {
        return new CustomizableCharacterNonWardrobeIndexes(
            GetNonWardrobeIndexes(customizableCharacter.ClothesData),
            GetNonWardrobeIndexes(customizableCharacter.HairstylesData),
            GetNonWardrobeIndexes(customizableCharacter.SwimsuitsData));
    }

    private (string, int)[] GetNonWardrobeIndexes(IReadOnlyList<MySprite> data)
    {
        List<(string, int)> indexes = new List<(string, int)>();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].SpriteToWardrobeKey == false)
            {
                indexes.Add((data[i].Name, i));
            }
        }
        if (indexes.Count > 0)
        {
            return indexes.ToArray();
        }
        else
        {
            return null;
        } 
    }

    private void SetNameIndexOnInit(ref int nameIndex, string[] names, (string, int)[] indexes, SerializedProperty property)
    {
        if (names != null)
        {
            bool res = false;
            for (int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i].Item2 == property.intValue)
                {
                    res = true;
                    for (int j = 0; j < names.Length; j++)
                    {
                        if (names[j] == indexes[i].Item1)
                        {
                            nameIndex = j;
                            break;
                        }
                    }
                    break;
                }
            }

            if (res == false)
            {
                property.intValue = indexes[0].Item2;
            }
        }
    }
}