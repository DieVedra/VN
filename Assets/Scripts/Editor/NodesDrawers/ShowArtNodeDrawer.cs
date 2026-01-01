using System.Collections.Generic;
using System.Reflection;
using MyProject;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(ShowArtNode))]
public class ShowArtNodeDrawer : NodeEditor
{
    private ShowArtNode _showArtNode;
    private SerializedProperty _serializedPropertyInputPort;
    private SerializedProperty _serializedPropertyOutputPort;
    private SerializedProperty _modeSerializedProperty;
    private SerializedProperty _spriteKeySerializedProperty;
    private MyProject.EnumPopupDrawer _enumPopupDrawer;
    private int _currentIndex;
    private int _index;
    private List<string> _namesArtsToPopup;
    private MethodInfo _showMethod;

    public override void OnBodyGUI()
    {
        if (_showArtNode == null)
        {
            _showArtNode = target as ShowArtNode;
            _serializedPropertyInputPort = serializedObject.FindProperty("Input");
            _serializedPropertyOutputPort = serializedObject.FindProperty("Output");
            _modeSerializedProperty = serializedObject.FindProperty("_artMode");
            _spriteKeySerializedProperty = serializedObject.FindProperty("_spriteKey");
            _showMethod = _showArtNode.GetType().GetMethod("SetInfoToView", BindingFlags.NonPublic | BindingFlags.Instance);
            _enumPopupDrawer = new EnumPopupDrawer();
            _namesArtsToPopup = new List<string>();
            _currentIndex = 0;
        }
        else
        {
            serializedObject.Update();
            NodeEditorGUILayout.PropertyField(_serializedPropertyInputPort);
            NodeEditorGUILayout.PropertyField(_serializedPropertyOutputPort);
            _enumPopupDrawer.DrawEnumPopup<ShowArtMode>(_modeSerializedProperty, "Mode: ");
            EditorGUILayout.LabelField("Arts:");
            UpdatePopup();
            EditorGUI.BeginChangeCheck();
            _currentIndex = EditorGUILayout.Popup(_currentIndex, _namesArtsToPopup.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                _spriteKeySerializedProperty.stringValue = _namesArtsToPopup[_currentIndex];
                serializedObject.ApplyModifiedProperties();
                _showMethod?.Invoke(_showArtNode, null);
            }
        }
    }
    
    private void UpdatePopup()
    {
        if (_showArtNode.GetArtsSpritesDictionary != null)
        {
            _index = 0;
            _namesArtsToPopup.Clear();
            foreach (var pair in _showArtNode.GetArtsSpritesDictionary)
            {
                if (pair.Key == _spriteKeySerializedProperty.stringValue)
                {
                    _currentIndex = _index;
                }
                _namesArtsToPopup.Add(pair.Key);
                _index++;
            }
        }
    }
}