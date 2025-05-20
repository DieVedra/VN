
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.PackageManager;
using UnityEditor.U2D;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;
using MyNamespace;

public class BackgroundDataCreatorWindow : EditorWindow
{
    private const string _pathPart1 = "Assets/Серия ";
    private const string _pathPart2 = "/InstantiatedBackgroundContent";
    private const string _compressed = "Compressed";
    private SerializedProperty _backgroundDataValuesListSerializedProperty;
    private SerializedProperty _backgroundDataSpriteAtlasSerializedProperty;
    private SerializedObject _backgroundDataSerializedObject;
    private ReorderableList _reorderableList;
    private SpriteAtlasCreator _spriteAtlasCreator;
    private FolderCreator _folderCreator;
    private LineDrawer _lineDrawer;
    private SerializedObject _thisSerializedObject;
    private SerializedProperty _spritesSerializedProperty;
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private List<BackgroundContentValues> _backgroundContentValues;
    private Vector2 position;
    private string[] _dataTypesNames = new []{"Locations", "AdditionalImages", "Arts"};
    private string[] _paddingValuesNames = new []{"2" , "4", "8"};
    private string[] _addressablesGroupsNames;
    private int[] _paddingValues = new []{2 , 4, 8};
    private string _pathFolderBackgroundDataAsset;
    private int _seriaNumber = 1;
    private int _dataTypesNamesIndex = 0;
    private int _paddingIndex = 0;
    private int _addressablesGroupIndex = 0;
    private SpriteAtlasPackingSettings _spriteAtlasPackingSettings;
    private bool _keyOverridePath = false;
    private bool _keyAtlasSettings = false;
    private bool _keyCrunchedCompression = false;
    private bool _keyAvailableAddresables;
    private bool _keyMakeAddresables = true;
    private bool _keyShowAddresablesSettings = true;
    private bool _keyShowBackgroundDataValues = false;
    private bool _keyShowReorderableList = false;
    private void OnEnable()
    {
        if (_spriteAtlasCreator == null)
        {
            _spriteAtlasCreator = new SpriteAtlasCreator();
            _lineDrawer = new LineDrawer();
        }

        if (_folderCreator == null)
        {
            _folderCreator = new FolderCreator();
        }

        if (_thisSerializedObject == null)
        {
            _thisSerializedObject = new SerializedObject(this);
        }

        if (_spritesSerializedProperty == null)
        {
            _spritesSerializedProperty = _thisSerializedObject.FindProperty("_sprites");
        }
        TryInitReordableList();
        CreatePath();
        if (_backgroundDataValuesListSerializedProperty == null)
        {
            _backgroundDataValuesListSerializedProperty = _thisSerializedObject.FindProperty("_backgroundContentValues");
        }

        CheckAvailableAddresables();
    }

    private void OnGUI()
    {
        _thisSerializedObject.Update();
        GUILayout.Space(20f);
        GUILayout.Label($"Path: {_pathFolderBackgroundDataAsset}");
        _keyOverridePath = GUILayout.Toggle(_keyOverridePath, "Override path");
        if (_keyOverridePath)
        {
            _pathFolderBackgroundDataAsset = GUILayout.TextField(_pathFolderBackgroundDataAsset, GUILayout.Width(500f));
        }
        GUILayout.Space(20f);
        GUILayout.Label($"Type Asset:");
        _dataTypesNamesIndex = EditorGUILayout.Popup(_dataTypesNamesIndex, _dataTypesNames);
        EditorGUI.BeginChangeCheck();
        _seriaNumber = EditorGUILayout.IntField("Seria Number: ", _seriaNumber);

        _keyShowReorderableList = EditorGUILayout.Foldout(_keyShowReorderableList, "Show Sprite List");
        if (_keyShowReorderableList)
        {
            _reorderableList.DoLayoutList();
        }


        GUILayout.Label($"Select objects in project and press button");
        if (GUILayout.Button("Add Selected Sprites") && Selection.objects.Length > 0)
        {
            AddSelectedSprites();
            _thisSerializedObject.ApplyModifiedProperties();
        }

        if (EditorGUI.EndChangeCheck())
        {
            CreatePath();
            ChangeBackgroundContentValuesList();
            _thisSerializedObject.Update();

            _thisSerializedObject.ApplyModifiedProperties();
        }

        DrawAtlasSettings();
        DrawAddresableSettings();
        TryDrawBackgroundContentValuesList();

        if (GUILayout.Button("CreateAsset"))
        {
            if (_seriaNumber > 0)
            {
                _folderCreator.TryCreateFolder(_pathFolderBackgroundDataAsset);
                string nameData = GetBackgroundDataName();
                BackgroundData backgroundData = new ScriptableObjectCreator().CreateScriptableObjectAsset<BackgroundData>(_pathFolderBackgroundDataAsset, nameData);
                _backgroundDataSerializedObject = new SerializedObject(backgroundData);
                _backgroundDataSpriteAtlasSerializedProperty = _backgroundDataSerializedObject.FindProperty(BackgroundData.SpriteAtlasPropertyName);
                _backgroundDataValuesListSerializedProperty = _backgroundDataSerializedObject.FindProperty(BackgroundData.ContentValuesPropertyName);
                
                
                _spriteAtlasPackingSettings.padding = _paddingValues[_paddingIndex];
                _spriteAtlasPackingSettings.enableRotation = false;
                SpriteAtlas spriteAtlas =
                    _spriteAtlasCreator.CreateAtlas(_sprites, _spriteAtlasPackingSettings, GetAtlasBackgroundDataName(), _keyCrunchedCompression);
                _backgroundDataSpriteAtlasSerializedProperty.objectReferenceValue = spriteAtlas;
                _backgroundDataValuesListSerializedProperty.ClearArray();
                TransferListToList();
                _backgroundDataSerializedObject.ApplyModifiedProperties();
                TryMakeAssetAddresables(backgroundData, nameData);
                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] {spriteAtlas}, EditorUserBuildSettings.activeBuildTarget);
            }
        }

        if (GUILayout.Button("Clear"))
        {
            _sprites = new List<Sprite>();
            _backgroundContentValues = new List<BackgroundContentValues>();
            _backgroundDataValuesListSerializedProperty = null;
            _backgroundDataValuesListSerializedProperty = _thisSerializedObject.FindProperty("_backgroundContentValues");
            _spritesSerializedProperty = null;
            _spritesSerializedProperty = _thisSerializedObject.FindProperty("_sprites");
            _seriaNumber = 1;
            _thisSerializedObject.Update();
            _thisSerializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawAddresableSettings()
    {
        if (_keyAvailableAddresables)
        {
            _keyShowAddresablesSettings = EditorGUILayout.Foldout(_keyShowAddresablesSettings, "ShowAddresablesSettings");
            if (_keyShowAddresablesSettings)
            {
                _lineDrawer.DrawHorizontalLine(Color.green, 1);
                _keyMakeAddresables = EditorGUILayout.Toggle("MakeAddresables: ", _keyMakeAddresables);
                GUILayout.Space(5f);
                GUILayout.Label("AddressablesGroups: ");
                _addressablesGroupIndex = EditorGUILayout.Popup( _addressablesGroupIndex , _addressablesGroupsNames);
            }
        }
        else
        {
            GUILayout.Label("Addresable not available");
        }
    }

    private void DrawAtlasSettings()
    {
        _keyAtlasSettings = EditorGUILayout.Foldout(_keyAtlasSettings, "Show Atlas Settings");
        if (_keyAtlasSettings == true)
        {
            _lineDrawer.DrawHorizontalLine(Color.green, 2);
            _paddingIndex = EditorGUILayout.Popup(_paddingIndex, _paddingValuesNames);
            _keyCrunchedCompression = EditorGUILayout.Toggle("CrunchedCompression: ", _keyCrunchedCompression);
        }
    }

    [MenuItem("Window/MyBackgroundDataCreator")]
    public static void ShowWindow()
    {
        GetWindow<BackgroundDataCreatorWindow>("BackgroundDataCreatorWindow");
    }

    private void CreatePath()
    {
        _pathFolderBackgroundDataAsset = $"{_pathPart1}{_seriaNumber}{_pathPart2}";
    }
    private void TryInitReordableList()
    {
        if (_reorderableList != null)
        {
            return;
        }
        _sprites = new List<Sprite>();
        
        _reorderableList = new ReorderableList(_thisSerializedObject, _spritesSerializedProperty,  true, true,true,true);
        _reorderableList.drawElementCallback = (Rect rect, int index, bool active, bool focused) =>
        {
            var element = _spritesSerializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
            _thisSerializedObject.ApplyModifiedProperties();
        };

        _reorderableList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Sprites Add To Atlas: ");
        };
        _reorderableList.onChangedCallback = list =>
        {
            EditorUtility.SetDirty(this);
        };
    }

    private void TryDrawBackgroundContentValuesList()
    {
        _keyShowBackgroundDataValues = EditorGUILayout.Foldout(_keyShowBackgroundDataValues, "ShowBackgroundDataValuesSettings");
        if (_keyShowBackgroundDataValues)
        {
            if (_backgroundContentValues != null && _backgroundContentValues.Count > 0)
            {
                SerializedProperty serializedPropertyBackgroundContentValues;
                SerializedProperty serializedPropertyColor;
                SerializedProperty serializedPropertyColorLighting;
                SerializedProperty serializedPropertyMovementValue;
                SerializedProperty serializedPropertyScale;
                SerializedProperty serializedPropertyNameBackground;
                
                position = EditorGUILayout.BeginScrollView( position);

                for (int i = 0; i < _backgroundDataValuesListSerializedProperty.arraySize; ++i)
                {
                    if (i < _sprites.Count)
                    {
                        serializedPropertyBackgroundContentValues = _backgroundDataValuesListSerializedProperty.GetArrayElementAtIndex(i);
                        if (serializedPropertyBackgroundContentValues != null)
                        {
                            serializedPropertyColor = serializedPropertyBackgroundContentValues.FindPropertyRelative(BackgroundContentValues.ColorFieldName);
                                serializedPropertyColorLighting = serializedPropertyBackgroundContentValues.FindPropertyRelative(BackgroundContentValues.ColorLightingFieldName);
                                serializedPropertyMovementValue = serializedPropertyBackgroundContentValues.FindPropertyRelative(BackgroundContentValues.MovementDuringDialogueValueFieldName);
                                serializedPropertyScale = serializedPropertyBackgroundContentValues.FindPropertyRelative(BackgroundContentValues.ScaleFieldName);
                                serializedPropertyNameBackground = serializedPropertyBackgroundContentValues.FindPropertyRelative(BackgroundContentValues.NameBackgroundFieldName);
                                GUILayout.Space(10f);
                                _lineDrawer.DrawHorizontalLine(Color.red, 2);
                                GUILayout.Label($"Name: {_backgroundContentValues[i].NameSprite}");
                                serializedPropertyNameBackground.stringValue = EditorGUILayout.TextField("Name Background:",
                                    serializedPropertyNameBackground.stringValue);
                                GUILayout.BeginHorizontal();
                                serializedPropertyScale.vector2Value = EditorGUILayout.Vector2Field("Scale:",
                                    serializedPropertyScale.vector2Value);
                                serializedPropertyMovementValue.floatValue = 
                                    EditorGUILayout.FloatField("MovementValue:",serializedPropertyMovementValue.floatValue);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                GUILayout.BeginVertical();
                                GUILayout.Label($"Color Lighting: ");
                                serializedPropertyColorLighting.colorValue = EditorGUILayout.ColorField(serializedPropertyColorLighting.colorValue);
                                GUILayout.EndVertical();
                                GUILayout.BeginVertical();
                                GUILayout.Label($"Color: ");
                                serializedPropertyColor.colorValue = EditorGUILayout.ColorField(serializedPropertyColor.colorValue);
                                GUILayout.EndVertical();
                                GUILayout.EndHorizontal();
                                _thisSerializedObject.ApplyModifiedProperties();
                        }
                    }
                }
                EditorGUILayout.EndScrollView();

            }
        }
    }
    private void ChangeBackgroundContentValuesList()
    {
        if (_sprites.Count > 0)
        {
            List<BackgroundContentValues> newBackgroundContentValues = new List<BackgroundContentValues>(_sprites.Count);
            for (int i = 0; i < _sprites.Count; i++)
            {
                if (_sprites[i] != null && TryRewritingElement(newBackgroundContentValues, _sprites[i].name) == false)
                {
                    newBackgroundContentValues.Add(new BackgroundContentValues(_sprites[i].name, _sprites[i].name, Vector2.one, Color.white, Color.white));
                }
            }
            _backgroundContentValues = newBackgroundContentValues;
        }
    }

    private bool TryRewritingElement(List<BackgroundContentValues> newBackgroundContentValues, string nameSprite)
    {
        bool result = false;
        if (_backgroundContentValues.Count > 0 )
        {
            for (int j = 0; j < _backgroundContentValues.Count; ++j)
            {
                if (_backgroundContentValues[j].NameSprite == nameSprite)
                {
                    newBackgroundContentValues.Add(
                        new BackgroundContentValues(
                            _backgroundContentValues[j].NameSprite,
                            _backgroundContentValues[j].NameBackground,
                            _backgroundContentValues[j].Scale,
                            _backgroundContentValues[j].ColorLighting,
                            _backgroundContentValues[j].Color,
                            _backgroundContentValues[j].MovementDuringDialogueValue));
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

    private void TransferListToList()
    {
        SerializedProperty serializedProperty;
        SerializedProperty serializedPropertyName;
        SerializedProperty serializedPropertyColor;
        SerializedProperty serializedPropertyColorLighting;
        SerializedProperty serializedPropertyNameBackground;
        SerializedProperty serializedPropertyScale;

        SerializedProperty serializedPropertyFloat;
        for (int i = 0; i < _backgroundContentValues.Count; i++)
        {
            _backgroundDataValuesListSerializedProperty.InsertArrayElementAtIndex(i);
            serializedProperty = _backgroundDataValuesListSerializedProperty.GetArrayElementAtIndex(i);
            serializedPropertyName = serializedProperty.FindPropertyRelative(BackgroundContentValues.NameSpriteFieldName);
            serializedPropertyColor = serializedProperty.FindPropertyRelative(BackgroundContentValues.ColorFieldName);
            serializedPropertyColorLighting = serializedProperty.FindPropertyRelative(BackgroundContentValues.ColorLightingFieldName);
            serializedPropertyNameBackground = serializedProperty.FindPropertyRelative(BackgroundContentValues.NameBackgroundFieldName);
            serializedPropertyScale = serializedProperty.FindPropertyRelative(BackgroundContentValues.ScaleFieldName);

            serializedPropertyFloat = serializedProperty.FindPropertyRelative(BackgroundContentValues.MovementDuringDialogueValueFieldName);
                    
            serializedPropertyName.stringValue = _backgroundContentValues[i].NameSprite;
            serializedPropertyColor.colorValue = _backgroundContentValues[i].ColorLighting;
            serializedPropertyColorLighting.colorValue = _backgroundContentValues[i].Color;
            serializedPropertyNameBackground.stringValue = _backgroundContentValues[i].NameBackground;
            serializedPropertyFloat.floatValue = _backgroundContentValues[i].MovementDuringDialogueValue;
            serializedPropertyScale.vector2Value = _backgroundContentValues[i].Scale;
        }
    }
    private string GetBackgroundDataName()
    {
        if (_keyCrunchedCompression)
        {
            return $"{_dataTypesNames[_dataTypesNamesIndex]}{_compressed}{BackgroundData.AssetName}{_seriaNumber}{BackgroundData.Format}";
        }
        else
        {
            return $"{_dataTypesNames[_dataTypesNamesIndex]}{BackgroundData.AssetName}{_seriaNumber}{BackgroundData.Format}";
        }
    }

    private string GetAtlasBackgroundDataName()
    {
        if (_keyCrunchedCompression)
        {
            return $"{_pathFolderBackgroundDataAsset}{FolderCreator.Separator}{_dataTypesNames[_dataTypesNamesIndex]}{_compressed}{SpriteAtlasCreator.Name}{BackgroundData.AssetName}{_seriaNumber}{SpriteAtlasCreator.Format}";

        }
        else
        {
            return $"{_pathFolderBackgroundDataAsset}{FolderCreator.Separator}{_dataTypesNames[_dataTypesNamesIndex]}{SpriteAtlasCreator.Name}{BackgroundData.AssetName}{_seriaNumber}{SpriteAtlasCreator.Format}";
        }
    }

    private void AddSelectedSprites()
    {
        SerializedProperty serializedPropertyElement;
        foreach (var VARIABLE in Selection.objects)
        {
            int index = _spritesSerializedProperty.arraySize;
            _spritesSerializedProperty.InsertArrayElementAtIndex(index);
            serializedPropertyElement = _spritesSerializedProperty.GetArrayElementAtIndex(index);
            if (VARIABLE is Texture2D texture2D)
            {
                string path = AssetDatabase.GetAssetPath(texture2D);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer != null && importer.spriteImportMode == SpriteImportMode.Single && importer.textureType == TextureImporterType.Sprite)
                {
                    serializedPropertyElement.objectReferenceValue = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
            }
            else if (VARIABLE is Sprite sprite)
            {
                serializedPropertyElement.objectReferenceValue = sprite;
            }
        }
    }

    private void TryMakeAssetAddresables(BackgroundData backgroundData, string name)
    {
        if (_keyMakeAddresables == false)
        {
            return;
        }
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
        {
            AddressableAssetGroup assetGroup = settings.groups[0];
            foreach (var group in settings.groups)
            {
                if (_addressablesGroupsNames[_addressablesGroupIndex] == group.name)
                {
                    assetGroup = group;
                }
            }

            AddressableAssetEntry assetEntry = settings.CreateOrMoveEntry(
            AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(backgroundData)), assetGroup);
            assetEntry.address = name;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }

    private void InitAddressablesGroups()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings.groups.Count > 0)
        {
            List<string> groups = new List<string>();
            foreach (var group in settings.groups)
            {
                groups.Add(group.name);
            }
            _addressablesGroupsNames = groups.ToArray();
        }
    }
    private void CheckAvailableAddresables()
    {
        _keyAvailableAddresables = false;
        var request = Client.List();
        while (!request.IsCompleted) { }
        if (request.Status == StatusCode.Success)
        {
            foreach (var package in request.Result)
            {
                if (package.name == "com.unity.addressables")
                {
                    _keyAvailableAddresables = true;
                    InitAddressablesGroups();
                    break;
                }
            }
        }
    }
}