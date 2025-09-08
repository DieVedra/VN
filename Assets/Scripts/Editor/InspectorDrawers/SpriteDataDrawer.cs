using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteData))]
public class SpriteDataDrawer : Editor
{
    private SpriteData _spriteData;
    private SerializedProperty _mySpritesSerializedProperty;
    private string[] _namesToPopup;
    private int _index;
    private int _previosIndex;
    private int _previousSpriteCount;
    private float _sliderValue = 1f;
    private bool _drawPreviewAndPopup;
    private float _offsetXValue = 0.5f;
    private float _offsetYValue = 0.5f;
    private float _scaleValue = 0.5f;
    private int _price;
    private int _priceAdditional;
    private bool _spriteToWardrobeKey;
    public void OnEnable()
    {
        TryInitTarget();
        _mySpritesSerializedProperty = serializedObject.FindProperty("_mySprites");
        TryInitNames();
    }
    public override void OnInspectorGUI()
    {
        if (_spriteData != null)
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            if (_drawPreviewAndPopup == true)
            {
                DrawPopup();
                DrawPreview();

                if (_mySpritesSerializedProperty.arraySize > 0)
                {
                    DrawMySpritesSettings();
                }
            }
            DrawDefaultInspector();

            if (EditorGUI.EndChangeCheck())
            {
                _namesToPopup = _spriteData.GetNames();
                if (_namesToPopup.Length != 0 && _index > _namesToPopup.Length - 1)
                {
                    _index = _namesToPopup.Length - 1;
                }
                SetValuesToProperty();
            }
            if (_spriteData.Sprites.Count > 0)
            {
                if (_previousSpriteCount != _spriteData.Sprites.Count)
                {
                    _namesToPopup = _spriteData.GetNames();
                    _spriteData.InitMySprites();
                    GetValuesToProperty();
                }
                _previousSpriteCount = _spriteData.MySprites.Count;
            }

            if (_spriteData.Sprites.Count == 0)
            {
                _drawPreviewAndPopup = false;
            }
            else
            {
                _drawPreviewAndPopup = true;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawPopup()
    {
        GUIContent arrayLabel = new GUIContent("Settings Preview: ");
        EditorGUI.BeginChangeCheck();

        _index = EditorGUILayout.Popup(arrayLabel, _index,  _namesToPopup);

        if (EditorGUI.EndChangeCheck())
        {
            GetValuesToProperty();
        }
    }
    private void DrawPreview()
    {
        if (_spriteData.Sprites != null && _spriteData.Sprites.Count > 0)
        {
            Texture2D texture = GetTexture();
            if (texture != null)
            {
                texture.filterMode = FilterMode.Bilinear;
                GUILayout.Label("", GUILayout.Height(texture.height * _sliderValue), GUILayout.Width(texture.width * _sliderValue));
                GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
                _sliderValue = GUILayout.HorizontalSlider(_sliderValue, 1f, 1.5f, GUILayout.Width(200f));
                EditorGUILayout.Space(20f);
            }
        }
    }

    private Texture2D GetTexture()
    {
        Texture2D texture = null;
        if (_spriteData.Sprites != null && _spriteData.Sprites.Count > 0)
        {
            if (_spriteData.Sprites[_index] != null)
            {
                texture = AssetPreview.GetAssetPreview(_spriteData.Sprites[_index].texture);
            }
        }
        return texture;
    }

    private void DrawMySpritesSettings()
    {
        DrawProperty(ref _offsetXValue, "OffsetXValue");
        DrawProperty(ref _offsetYValue, "OffsetYValue");
        DrawProperty(ref _scaleValue, "ScaleValue");
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Price: ");
        _price = EditorGUILayout.IntField(_price);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Price Additional: ");
        _priceAdditional = EditorGUILayout.IntField(_priceAdditional);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("SpriteToWardrobe: ");
        _spriteToWardrobeKey = EditorGUILayout.Toggle(_spriteToWardrobeKey);
        EditorGUILayout.EndHorizontal();

        // if (GUILayout.Button("SetValues"))
        // {
        //     SetValuesToProperty();
        // }
    }

    private void DrawProperty(ref float sliderValue, string nameField)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(nameField);
        sliderValue = EditorGUILayout.Slider(sliderValue, 0f, 1f);
        EditorGUILayout.EndHorizontal();
    }
    private void TryInitNames()
    {
        if (_namesToPopup == null)
        {
            _namesToPopup = _spriteData.GetNames();
        }
    }
    private void TryInitTarget()
    {
        if (_spriteData == null)
        {
            _spriteData = target as SpriteData;
        }
    }

    private void SetValuesToProperty()
    {
        SerializedProperty serializedProperty = GetArrayElement();
        serializedProperty.FindPropertyRelative("_offsetXValue").floatValue = _offsetXValue;
        serializedProperty.FindPropertyRelative("_offsetYValue").floatValue = _offsetYValue;
        serializedProperty.FindPropertyRelative("_scaleValue").floatValue = _scaleValue;
        serializedProperty.FindPropertyRelative("_price").intValue = _price;
        serializedProperty.FindPropertyRelative("_priceAdditional").intValue = _priceAdditional;
        serializedProperty.FindPropertyRelative("_spriteToWardrobeKey").boolValue = _spriteToWardrobeKey;
    }
    private void GetValuesToProperty()
    {
        SerializedProperty serializedProperty = GetArrayElement();
        if (serializedProperty != null)
        {
            _offsetXValue = serializedProperty.FindPropertyRelative("_offsetXValue").floatValue;
            _offsetYValue = serializedProperty.FindPropertyRelative("_offsetYValue").floatValue;
            _scaleValue = serializedProperty.FindPropertyRelative("_scaleValue").floatValue;
            _price = serializedProperty.FindPropertyRelative("_price").intValue;
            _priceAdditional = serializedProperty.FindPropertyRelative("_priceAdditional").intValue;
            _spriteToWardrobeKey = serializedProperty.FindPropertyRelative("_spriteToWardrobeKey").boolValue;
        }
    }

    private SerializedProperty GetArrayElement()
    {
        return _mySpritesSerializedProperty.GetArrayElementAtIndex(_index);
    }
}