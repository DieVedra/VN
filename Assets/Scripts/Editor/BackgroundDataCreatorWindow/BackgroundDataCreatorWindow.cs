
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class BackgroundDataCreatorWindow : EditorWindow
{
    private string _pathPart1 = "Assets/Серия ";
    private string _pathPart2 = "/BackgroundContent";
    // private string _format = ".asset";
    // private string _pathPluginsFolder = "Assets/Plugins";
    // private string _pathBackgroundDataCreatorWindow = "Assets/Plugins/BackgroundDataCreatorWindow";
    // private char _separator = '/';
    // private string _pathBackgroundDataCreatorWindowListAsset = "Assets/Plugins/BackgroundDataCreatorWindow";
    
    // private BackgroundData _backgroundData;
    private SpriteAtlasCreator _spriteAtlasCreator;
    // private SerializedProperty _spriteAtlasSerializedProperty;
    // private SerializedProperty _spritesListSerializedProperty;
    // private SerializedProperty _nameAtlasSerializedProperty;
    private SerializedProperty _backgroundDataCreatorWindowListSerializedProperty;

    private SerializedObject _backgroundDataCreatorWindowListAsset;
    // private SerializedProperty _serializedProperty;

    // private SerializedProperty _serializedProperty;
    private ReorderableList _reorderableList;
    private FolderCreator _folderCreator;
    private string _pathFolderBackgroundDataAsset;
    // private string _pathBackgroundDataAtlasAsset;
    // private string _path;
    private int _seriaNumber = 1;
    private void OnEnable()
    {
        if (_spriteAtlasCreator == null)
        {
            _spriteAtlasCreator = new SpriteAtlasCreator();
        }

        if (_folderCreator == null)
        {
            _folderCreator = new FolderCreator();
        }

        TryCreateBackgroundDataCreatorWindowListAsset();
        
        CreatePath();
    }

    private void OnDestroy()
    {
        DeleteBackgroundDataCreatorWindowList();
    }

    private void OnGUI()
    {
        // GUILayout.BeginHorizontal();
        // if (GUILayout.Button("CreateAtlas"))
        // {
        //     
        //     // if (_reorderableList.count > 0)
        //     // {
        //     //     _spriteAtlasSerializedProperty.objectReferenceValue = 
        //     //         _spriteAtlasCreator.CreateAtlas(_spritesListSerializedProperty,
        //     //             _nameAtlasSerializedProperty.stringValue ,1);
        //     // }
        // }
        //
        // if (GUILayout.Button("DeleteAtlas"))
        // {
        //     _spriteAtlasCreator.DeleteAtlas();
        // }
        // GUILayout.EndHorizontal();
        
        GUILayout.Space(20f);
        
        GUILayout.Label($"Path: {_pathFolderBackgroundDataAsset}");
        
        _pathFolderBackgroundDataAsset = GUILayout.TextField(_pathFolderBackgroundDataAsset, GUILayout.Width(500f));
        
        GUILayout.Space(20f);
        
        EditorGUI.BeginChangeCheck();
        _seriaNumber = EditorGUILayout.IntField("Seria Number: ", _seriaNumber);
        _reorderableList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            CreatePath();
            _backgroundDataCreatorWindowListAsset.ApplyModifiedProperties();
            Debug.Log(3333);
        }

        if (GUILayout.Button("CreateAsset"))
        {
            if (_seriaNumber > 0)
            {
                BackgroundData backgroundData = CreateScriptableObjectAsset<BackgroundData>(_pathFolderBackgroundDataAsset, $"{BackgroundData.AssetName}{_seriaNumber}{BackgroundData.Format}");
                SerializedObject serializedObject = new SerializedObject(backgroundData);
                // ComplectionBackgroundData(backgroundData);
                // CreateNamesSprites()


                // BackgroundData backgroundData = ScriptableObject.CreateInstance<BackgroundData>();
                // SerializedObject serializedObject = new SerializedObject(backgroundData);
                // SerializedProperty spriteAtlasSerializedProperty = serializedObject.FindProperty("_spriteAtlas");


                // AssetDatabase.CreateAsset(backgroundData, _pathFolderBackgroundDataAsset);
                // _spriteAtlasCreator.CreateAtlas()
            }
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

    private void TryCreateBackgroundDataCreatorWindowList()
    {
        
    }

    private T CreateScriptableObjectAsset<T>(string path, string name)  where T : ScriptableObject
    {
        string newPath = path;
        if (name[0] != FolderCreator.Separator)
        {
            newPath = $"{path}{FolderCreator.Separator}{name}";
        }
        else
        {
            newPath = $"{path}{name}";
        }
        T scriptableObject = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(scriptableObject, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return scriptableObject;
    }

    private void TryCreateBackgroundDataCreatorWindowListAsset()
    {
        if (_folderCreator == null)
        {
            _folderCreator = new FolderCreator();
        }
        if (_backgroundDataCreatorWindowListSerializedProperty == null)
        {
            _folderCreator.CreateFolder(BackgroundDataCreatorWindowList.MyPathFolder);
            BackgroundDataCreatorWindowList backgroundDataCreatorWindowList = CreateScriptableObjectAsset<BackgroundDataCreatorWindowList>(BackgroundDataCreatorWindowList.MyPathFolder, BackgroundDataCreatorWindowList.MyFileName);

            _backgroundDataCreatorWindowListAsset = new SerializedObject(backgroundDataCreatorWindowList);

            _backgroundDataCreatorWindowListSerializedProperty = _backgroundDataCreatorWindowListAsset.FindProperty($"{BackgroundDataCreatorWindowList.MySpritePropertyName}");
            InitReordableList(backgroundDataCreatorWindowList);
        }
    }

    private void DeleteBackgroundDataCreatorWindowList()
    {
        string path = $"{BackgroundDataCreatorWindowList.MyPathFolder}{FolderCreator.Separator}{BackgroundDataCreatorWindowList.MyFileName}";
        if (AssetDatabase.LoadAssetAtPath<BackgroundDataCreatorWindowList>(path) == true)
        {
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.Refresh();
            _backgroundDataCreatorWindowListSerializedProperty = null;
        }
    }
    private void InitReordableList(BackgroundDataCreatorWindowList backgroundDataCreatorWindowList)
    {
        _reorderableList = new ReorderableList(_backgroundDataCreatorWindowListAsset, _backgroundDataCreatorWindowListSerializedProperty,  true, true,true,true);
        _reorderableList.drawElementCallback = (Rect rect, int index, bool active, bool focused) =>
        {
            var element = _backgroundDataCreatorWindowListSerializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
            // Debug.Log("drawElementCallback");

        };

        _reorderableList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Sprites Add To Atlas: ");
            // Debug.Log("drawHeaderCallback");

        };
        _reorderableList.onChangedCallback = list =>
        {
            EditorUtility.SetDirty(backgroundDataCreatorWindowList);
            // Debug.Log("onChangedCallback");
        };
        // _reorderableList.onAddCallback = list =>
        // {
        //     Debug.Log("onAddCallback");
        //
        // };
        // _reorderableList.onRemoveCallback = list =>
        // {
        //     Debug.Log("onRemoveCallback");
        //
        // };
        // _reorderableList.onAddDropdownCallback = (rect, list) =>
        // {
        //     Debug.Log("onAddDropdownCallback");
        //
        // };
        // _reorderableList.onRemoveCallback = list =>
        // {
        //     Debug.Log("onRemoveCallback");
        //
        // };
        // _reorderableList.onSelectCallback = list =>
        // {
        //     Debug.Log("onSelectCallback");
        //
        // };
    }

    // private List<string> ComplectionBackgroundData(BackgroundData backgroundData)
    // {
    //     // List<string> names = new List<string>(serializedPropertyBackgroundDataList.arraySize);
    //     SerializedProperty serializedProperty;
    //     string name;
    //     Sprite sprite;
    //     SerializedObject serializedObject = new SerializedObject(backgroundData);
    //     for (int i = 0; i < _backgroundDataCreatorWindowListSerializedProperty.arraySize; ++i)
    //     {
    //         sprite = _backgroundDataCreatorWindowListSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
    //         name = sprite.name;
    //         
    //         serializedProperty = serializedObject.FindProperty(BackgroundData.)
    //         serializedPropertyBackgroundDataList.InsertArrayElementAtIndex(serializedPropertyBackgroundDataList.arraySize);
    //         
    //         serializedProperty = serializedPropertyBackgroundDataList.GetArrayElementAtIndex()
    //         
    //         // names.Add(spriteInWindowList.name);
    //     }
    //     return names;
    // }

    private void ComplectionContentValues()
    {
        
    }

}