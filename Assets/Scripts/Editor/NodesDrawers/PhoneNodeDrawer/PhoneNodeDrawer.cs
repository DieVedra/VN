using System;
using System.Linq;
using System.Reflection;
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
    private EnumPopupDrawer _enumPopupDrawer;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    private SerializedProperty _phoneIndexSerializedProperty;
    private SerializedProperty _phoneStartScreenSerializedProperty;
    private SerializedProperty _startHourSerializedProperty;
    private SerializedProperty _startMinuteSerializedProperty;
    private SerializedProperty _dateSerializedProperty;
    private SerializedProperty _butteryPercentSerializedProperty;
    // private SerializedProperty _startScreenCharacterIndexSerializedProperty;
    private SerializedProperty _contactsInfoToGameSerializedProperty;
    private SerializedProperty _phoneNodeCasesSerializedProperty;
    private SerializedProperty _notificationsInBlockScreenSerializedProperty;
    private SerializedProperty _buferSP1;
    private SerializedProperty _buferSP2;
    private MethodInfo _removeAllCases;
    private MethodInfo _addCase;
    private MethodInfo _removeCase;
    private PhoneContact _phoneContact;
    private Vector2 _posScrollView1;
    private Vector2 _posScrollView2;
    private int _indexContact;
    private int _phonePreviousIndex;
    private bool _showStartInfo = false;
    private string[] _phonesNames;
    private string[] _characterNames;

    public override void OnBodyGUI()
    {
        if (_phoneNode == null)
        {
            _phoneNode = target as PhoneNode;
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
            // _startScreenCharacterIndexSerializedProperty = serializedObject.FindProperty("_startScreenCharacterIndex");
            _contactsInfoToGameSerializedProperty = serializedObject.FindProperty("_contactsInfoToGame");
            _phoneNodeCasesSerializedProperty = serializedObject.FindProperty("_phoneNodeCases");
            _notificationsInBlockScreenSerializedProperty = serializedObject.FindProperty("_notificationsInBlockScreen");
            Type type = _phoneNode.GetType();
            _removeAllCases = type.GetMethod("RemoveAllCases", BindingFlags.NonPublic | BindingFlags.Instance);
            _addCase = type.GetMethod("AddCase", BindingFlags.NonPublic | BindingFlags.Instance);
            _removeCase = type.GetMethod("RemoveCase", BindingFlags.NonPublic | BindingFlags.Instance);
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
                    EditorGUI.BeginChangeCheck();
                    _phonePreviousIndex = _phoneIndexSerializedProperty.intValue;
                    _phoneIndexSerializedProperty.intValue = EditorGUILayout.Popup(_phoneIndexSerializedProperty.intValue, _phonesNames);
                    if (EditorGUI.EndChangeCheck() && _phonePreviousIndex != _phoneIndexSerializedProperty.intValue)
                    {
                        serializedObject.ApplyModifiedProperties();
                        _removeAllCases.Invoke(_phoneNode, null);
                        serializedObject.Update();
                    }
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

                
                
                // _enumPopupDrawer.DrawEnumPopup<PhoneBackgroundScreen>(_phoneStartScreenSerializedProperty, "Current screen: ");
                
                // _posScrollView1 = EditorGUILayout.BeginScrollView(_posScrollView1, GUILayout.Height(100f));
                // if (expr)
                // {
                //     
                // }
                
                
                EditorGUILayout.LabelField($"DynamicOutputs: {_phoneNode.DynamicOutputs.Count()}");
                _lineDrawer.DrawHorizontalLine(Color.magenta);
                DrawContactsVariants();

                _lineDrawer.DrawHorizontalLine(Color.blue);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index Add: ", GUILayout.Width(65f));

                _indexContact = EditorGUILayout.IntField(_indexContact, GUILayout.Width(20f));

                if (GUILayout.Button("Add Case",GUILayout.Width(80f)))
                {
                    _addCase.Invoke(_phoneNode, new object[] {_indexContact});
                }

                EditorGUILayout.EndHorizontal();
                DrawCases();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Index Add: ", GUILayout.Width(65f));
                _indexContact = EditorGUILayout.IntField(_indexContact, GUILayout.Width(20f));
                if (GUILayout.Button("Add Notification",GUILayout.Width(100f)))
                {
                    AddNotification();
                }
                EditorGUILayout.EndHorizontal();
                
                DrawNotifications();

                // _notificationsInBlockScreenSerializedProperty
                // EditorGUILayout.EndScrollView();


                // if ((PhoneBackgroundScreen)_phoneStartScreenSerializedProperty.enumValueIndex == PhoneBackgroundScreen.BlockScreen)
                // {
                //     _posScrollView1 = EditorGUILayout.BeginScrollView(_posScrollView1, GUILayout.Height(100f));
                //     DrawVariants(_contactsInfoToGameSerializedProperty, $" notification ", $"_keyNotification");
                //     EditorGUILayout.EndScrollView();
                // }


                // _posScrollView2 = EditorGUILayout.BeginScrollView(_posScrollView2, GUILayout.Height(100f));
                // DrawVariants(_contactsInfoToGameSerializedProperty, $" online status: ", $"_keyOnline");
                // EditorGUILayout.EndScrollView();


                // _lineDrawer.DrawHorizontalLine(Color.blue);

                serializedObject.ApplyModifiedProperties();
            
        }
    }

    private void DrawNotifications()
    {
        for (int i = 0; i < _notificationsInBlockScreenSerializedProperty.arraySize; i++)
        {
            _buferSP1 = _notificationsInBlockScreenSerializedProperty.GetArrayElementAtIndex(i);
            _buferSP2 = _buferSP1.FindPropertyRelative("_contactName");
            EditorGUILayout.LabelField($"Name contact: {_buferSP2.stringValue}");
            _buferSP2 = _buferSP1.FindPropertyRelative("_contactKey");
            EditorGUILayout.LabelField($"Key contact: {_buferSP2.stringValue}");


            if (GUILayout.Button("Delete Notification", GUILayout.Width(120f)))
            {
                _notificationsInBlockScreenSerializedProperty.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
            }

            _lineDrawer.DrawHorizontalLine(Color.gray);
        }
        _lineDrawer.DrawHorizontalLine(Color.blue);
    }

    private void AddNotification()
    {
        _phoneContact = _phoneNode.AllContacts[_indexContact];
        
        for (int i = 0; i < _notificationsInBlockScreenSerializedProperty.arraySize; i++)
        {
            _buferSP1 = _notificationsInBlockScreenSerializedProperty.GetArrayElementAtIndex(i);
            _buferSP1 = _buferSP1.FindPropertyRelative("_contactKey");
            if (_phoneContact.NameLocalizationString.Key == _buferSP1.stringValue)
            {
                return;
            }
        }
        
        
        int index = _notificationsInBlockScreenSerializedProperty.arraySize;
        _notificationsInBlockScreenSerializedProperty.InsertArrayElementAtIndex(index);
        _buferSP1 = _notificationsInBlockScreenSerializedProperty.GetArrayElementAtIndex(index);

        _buferSP2 = _buferSP1.FindPropertyRelative("_contactKey");
        _buferSP2.stringValue = _phoneContact.NameLocalizationString.Key;
        _buferSP2 = _buferSP1.FindPropertyRelative("_contactName");
        _buferSP2.stringValue = _phoneContact.NameLocalizationString.DefaultText;
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawContactsVariants()
    {
        for (int i = 0; i < _phoneNode.AllContacts.Count; i++)
        {
            if (_phoneNode.AllContacts[i].ToPhoneKey == _phoneNode.CurrentPhone.NamePhone.Key)
            {
                DrawContactInfo(i);
                _lineDrawer.DrawHorizontalLine(Color.white);
                EditorGUILayout.Space();
            }
        }
    }

    private void DrawContactInfo(int i)
    {
        EditorGUILayout.LabelField($"Index: {i}");
        EditorGUILayout.LabelField($"{_phoneNode.AllContacts[i].NameLocalizationString.DefaultText}");
        EditorGUILayout.LabelField($"{_phoneNode.AllContacts[i].NameLocalizationString.Key}");
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
        // if (_phoneNode.Phones != null && _phoneNode.Phones.Count > 0)
        // {
        //     if (_phoneNode.PhoneContactDatasLocalizable != null && _phoneNode.PhoneContactDatasLocalizable.Count > 0)
        //     { 
        //         _characterNames = new string[_phoneNode.PhoneContactDatasLocalizable.Count];
        //         for (int i = 0; i < _phoneNode.PhoneContactDatasLocalizable.Count; i++)
        //         {
        //             _characterNames[i] = _phoneNode.PhoneContactDatasLocalizable[i].NameContact;
        //         }
        //     }
        // }
    }
    private void DrawVariants(SerializedProperty array, string label, string nameKey)
    {
        EditorGUILayout.Space();
        _lineDrawer.DrawHorizontalLine(Color.red);
        for (int i = 0; i < array.arraySize; i++)
        {
            _buferSP1 = array.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{_buferSP1.FindPropertyRelative("_name").stringValue}{label}");
            _buferSP1 = _buferSP1.FindPropertyRelative(nameKey);
            _buferSP1.boolValue = EditorGUILayout.Toggle(_buferSP1.boolValue);
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawCases()
    {
        for (int i = 0; i < _phoneNodeCasesSerializedProperty.arraySize; i++)
        {
            _buferSP1 = _phoneNodeCasesSerializedProperty.GetArrayElementAtIndex(i);
            DrawCase();
        }
        _lineDrawer.DrawHorizontalLine(Color.blue);
    }

    private void DrawCase()
    {
        string portName = _buferSP1.FindPropertyRelative("_portName").stringValue;
        int index = _buferSP1.FindPropertyRelative("_contactIndex").intValue;
        EditorGUILayout.LabelField($"Name contact: {_phoneNode.AllContacts[index].NameLocalizationString.DefaultText}");
        EditorGUILayout.LabelField($"Key contact: {_phoneNode.AllContacts[index].NameLocalizationString.Key}");
        
        var port = _phoneNode.GetOutputPort(portName);
        NodeEditorGUILayout.PortField(port);
        if (GUILayout.Button("Delete case",GUILayout.Width(90f)))
        {
            _removeCase.Invoke(_phoneNode, new object[] {_buferSP1.FindPropertyRelative("_contactKey").stringValue});
        }
        _lineDrawer.DrawHorizontalLine(Color.black);
    }
}