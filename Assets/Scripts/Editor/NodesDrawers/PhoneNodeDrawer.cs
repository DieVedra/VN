using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditorAttribute(typeof(PhoneNode))]
public class PhoneNodeDrawer : NodeEditor
{
    private const int _maxCountSymbol = 20;
    private PhoneNode _phoneNode;
    private LineDrawer _lineDrawer;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    private SerializedProperty _phoneIndexSerializedProperty;
    private SerializedProperty _phoneStartScreenSerializedProperty;
    private SerializedProperty _startHourSerializedProperty;
    private SerializedProperty _startMinuteSerializedProperty;
    private SerializedProperty _dateSerializedProperty;
    private SerializedProperty _butteryPercentSerializedProperty;
    private SerializedProperty _initStartInfoSerializedProperty;
    private SerializedProperty _blockScreenNotificationKeySerializedProperty;
    private SerializedProperty _startScreenCharacterIndexSerializedProperty;
    private SerializedProperty _onlineContactsSerializedProperty;

    private string[] _phonesNames;
    private string[] _characaterNames;
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
                _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(_maxCountSymbol));
                _phoneIndexSerializedProperty = serializedObject.FindProperty("_phoneIndex");
                _phoneStartScreenSerializedProperty = serializedObject.FindProperty("_phoneStartScreen");
                _butteryPercentSerializedProperty = serializedObject.FindProperty("_butteryPercent");
                _startHourSerializedProperty = serializedObject.FindProperty("_startHour");
                _startMinuteSerializedProperty = serializedObject.FindProperty("_startMinute");
                _dateSerializedProperty = serializedObject.FindProperty("_date");
                _inputSerializedProperty = serializedObject.FindProperty("Input");
                _outputSerializedProperty = serializedObject.FindProperty("Output");
                _initStartInfoSerializedProperty = serializedObject.FindProperty("_initStartInfo");
                _blockScreenNotificationKeySerializedProperty = serializedObject.FindProperty("_blockScreenNotificationKey");
                _startScreenCharacterIndexSerializedProperty = serializedObject.FindProperty("_startScreenCharacterIndex");
                _onlineContactsSerializedProperty = serializedObject.FindProperty("_onlineContacts");
            }
            else
            {
                serializedObject.Update();
                NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
                NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Current phone:");
                _phonesNames = InitNamesPhones();
                _phoneIndexSerializedProperty.intValue = EditorGUILayout.Popup(_phoneIndexSerializedProperty.intValue, _phonesNames);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Show start info:");
                _initStartInfoSerializedProperty.boolValue = EditorGUILayout.Toggle(_initStartInfoSerializedProperty.boolValue);
                EditorGUILayout.EndHorizontal();

                if (_initStartInfoSerializedProperty.boolValue == true)
                {
                    DrawDateInfo(_startHourSerializedProperty, "Start hour: ");
                    DrawDateInfo(_startMinuteSerializedProperty, "Start Minute: ");
                    
                    EditorGUILayout.BeginHorizontal();
                    _localizationStringTextDrawer.DrawTextField(_localizationStringTextDrawer.GetLocalizationStringFromProperty(_dateSerializedProperty),
                        "Date: ", false);
                    EditorGUILayout.EndHorizontal();
                    
                    DrawDateInfo(_butteryPercentSerializedProperty, "Buttery percent: ");
                }

                DrawStartScreenEnumPopup();

                if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.BlockScreen)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Add notification: ");
                    _blockScreenNotificationKeySerializedProperty.boolValue = EditorGUILayout.Toggle(_blockScreenNotificationKeySerializedProperty.boolValue);
                    EditorGUILayout.EndHorizontal();
                    if (_blockScreenNotificationKeySerializedProperty.boolValue == true)
                    {
                        if (InitNamesCharacters())
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Notification from character: ");
                            _startScreenCharacterIndexSerializedProperty.intValue = EditorGUILayout.Popup(_startScreenCharacterIndexSerializedProperty.intValue, _characaterNames);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    
                }
                else if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.ContactsScreen)
                {
                    
                }
                else if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.DialogScreen)
                {
                    if (InitNamesCharacters())
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Dialog with character: ");
                        _startScreenCharacterIndexSerializedProperty.intValue = EditorGUILayout.Popup(_startScreenCharacterIndexSerializedProperty.intValue, _characaterNames);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                DrawOnlineVariants();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
    private void DrawStartScreenEnumPopup()
    {
        PhoneBackgroundScreen phoneBackgroundScreen = (PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Current phone screen: ");
        phoneBackgroundScreen = (PhoneBackgroundScreen)EditorGUILayout.EnumPopup(phoneBackgroundScreen);
        EditorGUILayout.EndHorizontal();
        _phoneStartScreenSerializedProperty.enumValueIndex = (int) phoneBackgroundScreen;
    }

    private void DrawDateInfo(SerializedProperty serializedProperty, string name)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(name);
        serializedProperty.intValue = EditorGUILayout.IntField(serializedProperty.intValue);
        EditorGUILayout.EndHorizontal();

    }
    private string[] InitNamesPhones()
    {
        List<string> names = new List<string>();
        if (_phoneNode.Phones != null && _phoneNode.Phones.Count > 0)
        {
            for (int i = 0; i < _phoneNode.Phones.Count; i++)
            {
                names.Add(_phoneNode.Phones[i].NamePhone);
            }
        }

        return names.ToArray();
    }
    private bool InitNamesCharacters()
    {
        List<string> names = new List<string>();
        if (_phoneNode.Phones != null && _phoneNode.Phones.Count > 0)
        {
            if (_phoneNode.PhoneContactDatasLocalizable != null && _phoneNode.PhoneContactDatasLocalizable.Count > 0)
            {
                for (int i = 0; i < _phoneNode.PhoneContactDatasLocalizable.Count; i++)
                {
                    names.Add(_phoneNode.PhoneContactDatasLocalizable[i].NameContactLocalizationString);
                }
            }
        }

        if (names.Count > 0)
        {
            _characaterNames = names.ToArray();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DrawOnlineVariants()
    {
        EditorGUILayout.Space();
        _lineDrawer.DrawHorizontalLine(Color.red);
        for (int i = 0; i < _onlineContactsSerializedProperty.arraySize; i++)
        {
            DrawVariant(_onlineContactsSerializedProperty.GetArrayElementAtIndex(i));
        }
    }
    private void DrawVariant(SerializedProperty contact)
    {
        SerializedProperty onlineKeySp = contact.FindPropertyRelative("_onlineKey");
        EditorGUILayout.BeginHorizontal();
        string part = contact.FindPropertyRelative("_name").stringValue;
        string label = $"{part} online status: ";
        EditorGUILayout.LabelField(label);
        onlineKeySp.boolValue = EditorGUILayout.Toggle(onlineKeySp.boolValue);
        EditorGUILayout.EndHorizontal();
    }
}