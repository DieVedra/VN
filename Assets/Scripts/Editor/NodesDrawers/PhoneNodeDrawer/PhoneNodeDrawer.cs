using MyProject;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditorAttribute(typeof(PhoneNode))]
public class PhoneNodeDrawer : NodeEditor
{
    private const int _maxCountSymbol = 20;
    private PhoneNode _phoneNode;
    private LineDrawer _lineDrawer;
    private MyProject.EnumPopupDrawer _enumPopupDrawer;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    private SerializedProperty _phoneIndexSerializedProperty;
    private SerializedProperty _phoneStartScreenSerializedProperty;
    private SerializedProperty _startHourSerializedProperty;
    private SerializedProperty _startMinuteSerializedProperty;
    private SerializedProperty _dateSerializedProperty;
    private SerializedProperty _butteryPercentSerializedProperty;
    private SerializedProperty _startScreenCharacterIndexSerializedProperty;
    private SerializedProperty _contactsInfoToGameSerializedProperty;
    private SerializedProperty _sp;
    private Vector2 _posScrollView1;
    private Vector2 _posScrollView2;
    private bool _showStartInfo = false;
    private string[] _phonesNames;
    private string[] _characterNames;
    public override void OnBodyGUI()
    {
        if (_phoneNode == null)
        {
            _phoneNode = target as PhoneNode;
        }
        else
        {
            if (_phoneIndexSerializedProperty == null)
            {
                _lineDrawer = new LineDrawer();
                _enumPopupDrawer = new EnumPopupDrawer();
                InitNamesCharacters();
                InitNamesPhones();
                _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(_maxCountSymbol));
                _phoneIndexSerializedProperty = serializedObject.FindProperty("_phoneIndex");
                _phoneStartScreenSerializedProperty = serializedObject.FindProperty("_phoneStartScreen");
                _butteryPercentSerializedProperty = serializedObject.FindProperty("_butteryPercent");
                _startHourSerializedProperty = serializedObject.FindProperty("_startHour");
                _startMinuteSerializedProperty = serializedObject.FindProperty("_startMinute");
                _dateSerializedProperty = serializedObject.FindProperty("_date");
                _inputSerializedProperty = serializedObject.FindProperty("Input");
                _outputSerializedProperty = serializedObject.FindProperty("Output");
                _startScreenCharacterIndexSerializedProperty = serializedObject.FindProperty("_startScreenCharacterIndex");
                _contactsInfoToGameSerializedProperty = serializedObject.FindProperty("_contactsInfoToGame");
            }
            else
            {
                serializedObject.Update();
                NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
                NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Current phone:");
                InitNamesPhones();
                if (_phonesNames != null)
                {
                    _phoneIndexSerializedProperty.intValue = EditorGUILayout.Popup(_phoneIndexSerializedProperty.intValue, _phonesNames);
                }

                EditorGUILayout.EndHorizontal();
                _showStartInfo = EditorGUILayout.BeginFoldoutHeaderGroup(_showStartInfo, "Show start info:");
                if (_showStartInfo == true)
                {
                    DrawDateInfo(_startHourSerializedProperty, "Start hour: ");
                    DrawDateInfo(_startMinuteSerializedProperty, "Start Minute: ");
                    
                    EditorGUILayout.BeginHorizontal();
                    _localizationStringTextDrawer.DrawTextField(_localizationStringTextDrawer.GetLocalizationStringFromProperty(_dateSerializedProperty),
                        "Date: ", false);
                    EditorGUILayout.EndHorizontal();
                    
                    DrawDateInfo(_butteryPercentSerializedProperty, "Buttery percent: ");
                }

                _enumPopupDrawer.DrawEnumPopup<PhoneBackgroundScreen>(_phoneStartScreenSerializedProperty, "Current phone screen: ");
                if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.BlockScreen)
                {
                    _posScrollView1 = EditorGUILayout.BeginScrollView(_posScrollView1, GUILayout.Height(100f));
                    DrawVariants(_contactsInfoToGameSerializedProperty, $" notification ", $"_keyNotification");
                    EditorGUILayout.EndScrollView();
                }
                else if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.DialogScreen)
                {
                    if (_characterNames != null && _characterNames.Length > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Dialog with character: ");
                        _startScreenCharacterIndexSerializedProperty.intValue = EditorGUILayout.Popup(_startScreenCharacterIndexSerializedProperty.intValue, _characterNames);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                _posScrollView2 = EditorGUILayout.BeginScrollView(_posScrollView2, GUILayout.Height(100f));
                DrawVariants(_contactsInfoToGameSerializedProperty, $" online status: ", $"_keyOnline");
                EditorGUILayout.EndScrollView();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    private void DrawDateInfo(SerializedProperty serializedProperty, string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name);
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue);
        EditorGUILayout.EndHorizontal();

    }
    private void InitNamesPhones()
    {
        if (_phoneNode.Phones != null && _phoneNode.Phones.Count > 0)
        {
            _phonesNames = new string[_phoneNode.Phones.Count] ;
            for (int i = 0; i < _phoneNode.Phones.Count; i++)
            {
                _phonesNames[i] = _phoneNode.Phones[i].NamePhone;
            }
        }
    }
    private void InitNamesCharacters()
    {
        if (_phoneNode.Phones != null && _phoneNode.Phones.Count > 0)
        {
            // if (_phoneNode.PhoneContactDatasLocalizable != null && _phoneNode.PhoneContactDatasLocalizable.Count > 0)
            // { 
            //     _characterNames = new string[_phoneNode.PhoneContactDatasLocalizable.Count];
            //     for (int i = 0; i < _phoneNode.PhoneContactDatasLocalizable.Count; i++)
            //     {
            //         _characterNames[i] = _phoneNode.PhoneContactDatasLocalizable[i].NameContact;
            //     }
            // }
        }
    }
    private void DrawVariants(SerializedProperty array, string label, string nameKey)
    {
        EditorGUILayout.Space();
        _lineDrawer.DrawHorizontalLine(Color.red);
        for (int i = 0; i < array.arraySize; i++)
        {
            _sp = array.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{_sp.FindPropertyRelative("_name").stringValue}{label}");
            _sp = _sp.FindPropertyRelative(nameKey);
            _sp.boolValue = EditorGUILayout.Toggle(_sp.boolValue);
            EditorGUILayout.EndHorizontal();
        }
    }
}