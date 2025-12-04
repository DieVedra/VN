using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditorAttribute(typeof(PhoneNode))]
public class PhoneNodeDrawer : NodeEditor
{
    private const int _maxCountSymbol = 20;
    private const string _deleteNotification = "Delete Notification";
    private const string _deleteOnline = "Delete Online";
    private PhoneNode _phoneNode;
    private LineDrawer _lineDrawer;
    private LocalizationStringTextDrawer _localizationStringTextDrawer;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;
    private SerializedProperty _phoneIndexSerializedProperty;
    private SerializedProperty _startHourSerializedProperty;
    private SerializedProperty _startMinuteSerializedProperty;
    private SerializedProperty _dateSerializedProperty;
    private SerializedProperty _butteryPercentSerializedProperty;
    private SerializedProperty _phoneNodeCasesSerializedProperty;
    private SerializedProperty _notificationsInBlockScreenSerializedProperty;
    private SerializedProperty _onlineContactsSerializedProperty;
    private SerializedProperty _buferSP1;
    private SerializedProperty _buferSP2;
    private MethodInfo _removeAllCases;
    private MethodInfo _addCase;
    private MethodInfo _removeCase;
    private PhoneContact _phoneContact;
    private int _indexContact;
    private int _phonePreviousIndex;
    private bool _showStartInfo = false;
    private bool _showContactsKey = false;
    private string[] _phonesNames;

    public override void OnBodyGUI()
    {
        if (_phoneNode == null)
        {
            _phoneNode = target as PhoneNode;
            _lineDrawer = new LineDrawer();
            InitNamesPhones();
            _localizationStringTextDrawer = new LocalizationStringTextDrawer(new SimpleTextValidator(_maxCountSymbol));
            _phoneIndexSerializedProperty = serializedObject.FindProperty("_phoneIndex");
            _butteryPercentSerializedProperty = serializedObject.FindProperty("_butteryPercent");
            _startHourSerializedProperty = serializedObject.FindProperty("_startHour");
            _startMinuteSerializedProperty = serializedObject.FindProperty("_startMinute");
            _dateSerializedProperty = serializedObject.FindProperty("_date");
            _inputSerializedProperty = serializedObject.FindProperty("Input");
            _outputSerializedProperty = serializedObject.FindProperty("Output");
            _phoneNodeCasesSerializedProperty = serializedObject.FindProperty("_phoneNodeCases");
            _notificationsInBlockScreenSerializedProperty = serializedObject.FindProperty("_notificationsInBlockScreen");
            _onlineContactsSerializedProperty = serializedObject.FindProperty("_onlineContacts");
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
                EditorGUILayout.EndFoldoutHeaderGroup();
                
                _showContactsKey = EditorGUILayout.BeginFoldoutHeaderGroup(_showContactsKey, "Show contacts:");
                if (_showContactsKey)
                {
                    EditorGUILayout.LabelField($"DynamicOutputs: {_phoneNode.DynamicOutputs.Count()}");
                    _lineDrawer.DrawHorizontalLine(Color.magenta);
                    DrawContactsVariants();
                    _lineDrawer.DrawHorizontalLine(Color.blue);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();


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
            
                DrawContactsInfo(_notificationsInBlockScreenSerializedProperty, _deleteNotification);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Index Add: ", GUILayout.Width(65f));
                _indexContact = EditorGUILayout.IntField(_indexContact, GUILayout.Width(20f));
                if (GUILayout.Button("Add To Online",GUILayout.Width(100f)))
                {
                    AddOnlineContact();
                }
                EditorGUILayout.EndHorizontal();
                
                DrawContactsInfo(_onlineContactsSerializedProperty, _deleteOnline);
                
                
                serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawContactsInfo(SerializedProperty targetArray, string delete)
    {
        for (int i = 0; i < targetArray.arraySize; i++)
        {
            _buferSP1 = targetArray.GetArrayElementAtIndex(i);
            _buferSP2 = _buferSP1.FindPropertyRelative("Name");
            EditorGUILayout.LabelField($"Name contact: {_buferSP2.stringValue}");
            _buferSP2 = _buferSP1.FindPropertyRelative("Key");
            EditorGUILayout.LabelField($"Key contact: {_buferSP2.stringValue}");


            if (GUILayout.Button(delete, GUILayout.Width(120f)))
            {
                targetArray.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
            }

            _lineDrawer.DrawHorizontalLine(Color.gray);
        }
        _lineDrawer.DrawHorizontalLine(Color.blue);
    }


    private void AddContactInfo(SerializedProperty targetArray)
    {
        _phoneContact = _phoneNode.AllContacts[_indexContact];
        for (int i = 0; i < targetArray.arraySize; i++)
        {
            _buferSP1 = targetArray.GetArrayElementAtIndex(i);
            _buferSP1 = _buferSP1.FindPropertyRelative("Key");
            if (_phoneContact.NameLocalizationString.Key == _buferSP1.stringValue)
            {
                return;
            }
        }
        int index = targetArray.arraySize;
        targetArray.InsertArrayElementAtIndex(index);
        _buferSP1 = targetArray.GetArrayElementAtIndex(index);
        _buferSP2 = _buferSP1.FindPropertyRelative("Key");
        _buferSP2.stringValue = _phoneContact.NameLocalizationString.Key;
        _buferSP2 = _buferSP1.FindPropertyRelative("Name");
        _buferSP2.stringValue = _phoneContact.NameLocalizationString.DefaultText;
        serializedObject.ApplyModifiedProperties();
    }

    private void AddOnlineContact()
    {
        AddContactInfo(_onlineContactsSerializedProperty);
    }
    private void AddNotification()
    {
        AddContactInfo(_notificationsInBlockScreenSerializedProperty);
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
            _removeCase.Invoke(_phoneNode, new object[] {_buferSP1.FindPropertyRelative("Key").stringValue});
        }
        _lineDrawer.DrawHorizontalLine(Color.black);
    }
}