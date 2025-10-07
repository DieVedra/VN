using System.Collections.Generic;
using System.Linq;
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
    private SerializedProperty _blockScreenNotificationKeySerializedProperty;
    private SerializedProperty _startScreenCharacterIndexSerializedProperty;
    private SerializedProperty _onlineContactsSerializedProperty;
    private SerializedProperty _notificationContactsSerializedProperty;
    private PhoneNodeDrawer _phoneNodeDrawer;
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
                _blockScreenNotificationKeySerializedProperty = serializedObject.FindProperty("_blockScreenNotificationKey");
                _startScreenCharacterIndexSerializedProperty = serializedObject.FindProperty("_startScreenCharacterIndex");
                _onlineContactsSerializedProperty = serializedObject.FindProperty("_onlineContacts");
                _notificationContactsSerializedProperty = serializedObject.FindProperty("_notificationContacts");
            }
            else
            {
                serializedObject.Update();
                NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
                NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Current phone:");
                InitNamesPhones();
                _phoneIndexSerializedProperty.intValue = EditorGUILayout.Popup(_phoneIndexSerializedProperty.intValue, _phonesNames);
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

                DrawStartScreenEnumPopup();

                if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.BlockScreen)
                {
                    _posScrollView1 = EditorGUILayout.BeginScrollView(_posScrollView1, GUILayout.Height(100f));
                    DrawVariants(_notificationContactsSerializedProperty, $" notification ");
                    EditorGUILayout.EndScrollView();
                }
                else if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.ContactsScreen)
                {
                    
                }
                else if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.DialogScreen)
                {
                    if (_characterNames.Length > 0)
                    {
                        
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Dialog with character: ");
                        _startScreenCharacterIndexSerializedProperty.intValue = EditorGUILayout.Popup(_startScreenCharacterIndexSerializedProperty.intValue, _characterNames);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                // _posScrollView2 = EditorGUILayout.BeginScrollView(_posScrollView2, GUILayout.Height(100f));
                DrawVariants(_onlineContactsSerializedProperty, $" online status: ");
                // EditorGUILayout.EndScrollView();

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
            if (_phoneNode.PhoneContactDatasLocalizable != null && _phoneNode.PhoneContactDatasLocalizable.Count > 0)
            { 
                _characterNames = new string[_phoneNode.PhoneContactDatasLocalizable.Count];
                for (int i = 0; i < _phoneNode.PhoneContactDatasLocalizable.Count; i++)
                {
                    _characterNames[i] = _phoneNode.PhoneContactDatasLocalizable[i].NameContactLocalizationString;
                }
            }
        }
    }

    // private bool CreateContactsToOnlineAndNotifications(IReadOnlyList<PhoneContactDataLocalizable> contacts)
    // {
    //     bool result = false;
    //     if (contacts != null)
    //     {
    //         Dictionary<string, ContactInfoToGame> contactsDictionary = new Dictionary<string, ContactInfoToGame>();
    //         for (int i = 0; i < contacts.Count; i++)
    //         {
    //             TryAdd(contacts[i]);
    //         }
    //
    //         int count;
    //         PhoneDataLocalizable dataLocalizable;
    //         for (int i = 0; i < _phoneNode.Phones.Count; i++)
    //         {
    //             count = _phoneNode.Phones[i].PhoneDataLocalizable.PhoneContactDatasLocalizable.Count;
    //             dataLocalizable = _phoneNode.Phones[i].PhoneDataLocalizable;
    //             for (int j = 0; j < count; j++)
    //             {
    //                 TryAdd(dataLocalizable.PhoneContactDatasLocalizable[j]);
    //             }
    //         }
    //
    //         _allContactsToSetOnline = contactsDictionary.Select(x=>x.Value).ToArray();
    //         _notificationContacts = contactsDictionary.Select(x=>x.Value).ToArray();
    //         if (_allContactsToSetOnline.Length > 0 && _notificationContacts.Length > 0 )
    //         {
    //             result = true;
    //         }
    //         void TryAdd(PhoneContactDataLocalizable phoneContactDataLocalizable, bool statusKey = false)
    //         {
    //             if (contactsDictionary.ContainsKey(phoneContactDataLocalizable.NameContactLocalizationString.Key) == false)
    //             {
    //                 contactsDictionary.Add(
    //                     phoneContactDataLocalizable.NameContactLocalizationString.Key,
    //                     new ContactInfoToGame()
    //                     {
    //                         KeyName = phoneContactDataLocalizable.NameContactLocalizationString.Key,
    //                         Name = phoneContactDataLocalizable.NameContactLocalizationString.DefaultText,
    //                         Key = statusKey
    //                     });
    //             }
    //         }
    //     }
    //
    //     return result;
    // }
    
    private void DrawVariants(SerializedProperty array, string label)
    {
        EditorGUILayout.Space();
        _lineDrawer.DrawHorizontalLine(Color.red);
        SerializedProperty sp;
        for (int i = 0; i < array.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            sp = array.FindPropertyRelative("_key");
            EditorGUILayout.LabelField($"{array.FindPropertyRelative("_name").stringValue}{label}");
            sp.boolValue = EditorGUILayout.Toggle(sp.boolValue);
            EditorGUILayout.EndHorizontal();
        }
    }

    private void TrySetKey(SerializedProperty array, ContactInfoToGame contact)
    {
        SerializedProperty sp;
        if (contact.Key == true)
        {
            int insertIndex = array.arraySize;
            array.InsertArrayElementAtIndex(insertIndex);
            sp = array.GetArrayElementAtIndex(insertIndex);
            sp.stringValue = contact.KeyName;
        }
        else
        {
            for (int j = array.arraySize - 1; j >= 0; j--)
            {
                sp = array.GetArrayElementAtIndex(j);
                if (contact.KeyName == sp.stringValue)
                {
                    array.DeleteArrayElementAtIndex(j);
                    break;
                }
            }
        }
    }
    // private void DrawVariants()
    // {
    //     EditorGUILayout.Space();
    //     _lineDrawer.DrawHorizontalLine(Color.red);
    //     _pos = EditorGUILayout.BeginScrollView(_pos, GUILayout.Height(400f));
    //
    //     for (int i = 0; i < _onlineContactsSerializedProperty.arraySize; i++)
    //     {
    //         DrawVariant(_onlineContactsSerializedProperty.GetArrayElementAtIndex(i));
    //     }
    //     EditorGUILayout.EndScrollView();
    // }
    // private void DrawVariant(SerializedProperty contact)
    // {
    //     SerializedProperty onlineKeySp = contact.FindPropertyRelative("_onlineKey");
    //     EditorGUILayout.BeginHorizontal();
    //     string part = contact.FindPropertyRelative("_name").stringValue;
    //     string label = $"{part} online status: ";
    //     EditorGUILayout.LabelField(label);
    //     onlineKeySp.boolValue = EditorGUILayout.Toggle(onlineKeySp.boolValue);
    //     EditorGUILayout.EndHorizontal();
    // }
    // private void DrawVariant(SerializedProperty contact)
    // {
    //     SerializedProperty onlineKeySp = contact.FindPropertyRelative("_onlineKey");
    //     EditorGUILayout.BeginHorizontal();
    //     string part = contact.FindPropertyRelative("_name").stringValue;
    //     string label = $"{part} online status: ";
    //     EditorGUILayout.LabelField(label);
    //     onlineKeySp.boolValue = EditorGUILayout.Toggle(onlineKeySp.boolValue);
    //     EditorGUILayout.EndHorizontal();
    // }
}