
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BackgroundData))]
public class BackgroundDataDrawer : Editor
{
    private BackgroundData _backgroundData;
    private SpriteAtlasCreator _spriteAtlasCreator;
    private SerializedProperty _spriteAtlasSerializedProperty;
    private SerializedProperty _spritesListSerializedProperty;
    private SerializedProperty _nameAtlasSerializedProperty;
    // private SerializedProperty _serializedProperty;
    // private SerializedProperty _serializedProperty;
    private ReorderableList _reorderableList;
    
    
    // [SerializeField] private int _seriaNumber;
// [SerializeField] private string _spriteAtlasName;

    // private List<Sprite> _sprites;

    private void OnEnable()
    {
        _spriteAtlasCreator = new SpriteAtlasCreator();
        _backgroundData = target as BackgroundData;
        // if (_sprites == null)
        // {
        //     _sprites = new List<Sprite>();
        // }
        _spriteAtlasSerializedProperty = serializedObject.FindProperty("_spriteAtlas");
        _spritesListSerializedProperty = serializedObject.FindProperty("_sprites");
        _nameAtlasSerializedProperty = serializedObject.FindProperty("_spriteAtlasName");
        InitReordableList();
        // _spriteAtlasSerializedProperty.objectReferenceValue
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // _sprites = EditorGUILayout.PropertyField(_sprites, new GUIContent("Sprites: "), true);
        // _sprites = EditorGUILayout.li  (_sprites, new GUIContent("Sprites: "), true);
        // _reorderableList = new ReorderableList(_sprites, typeof(Sprite), true, true,true,true);
        // serializedObject.Update();
        // _reorderableList.DoLayoutList();
        // if (GUILayout.Button("CreateAtlas"))
        // {
            
            
            
            // if (_reorderableList.count > 0)
            // {
            //     _spriteAtlasSerializedProperty.objectReferenceValue = 
            //         _spriteAtlasCreator.CreateAtlas(_spritesListSerializedProperty,
            //             _nameAtlasSerializedProperty.stringValue ,1);
            // }
        // }

        // if (GUILayout.Button("DeleteAtlas"))
        // {
            // _spriteAtlasCreator.DeleteAtlas();
        // }
        // serializedObject.ApplyModifiedProperties();
    }

    private void InitReordableList()
    {
        _reorderableList = new ReorderableList(serializedObject, _spritesListSerializedProperty,  true, true,true,true);

        _reorderableList.drawElementCallback = (Rect rect, int index, bool active, bool focused) =>
        {
            var element = _spritesListSerializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };

        _reorderableList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Sprite List: ");
        };
        _reorderableList.onChangedCallback = (ReorderableList list) =>
        {
            EditorUtility.SetDirty(target);
        };
    }
}