
using System.Collections.Generic;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(ShowArtNode))]
public class ShowArtNodeDrawer : NodeEditor
{
    private ShowArtNode _showArtNode;
    private SerializedProperty _serializedPropertyIndexArt;
    private SerializedProperty _serializedPropertyInputPort;
    private SerializedProperty _serializedPropertyOutputPort;
    private SerializedProperty _modeSerializedProperty;
    private string[] _namesArts;
    public override void OnBodyGUI()
    {
        if (_showArtNode == null)
        {
            _showArtNode = target as ShowArtNode;
            _serializedPropertyIndexArt = serializedObject.FindProperty("_spriteIndex");
            _serializedPropertyInputPort = serializedObject.FindProperty("Input");
            _serializedPropertyOutputPort = serializedObject.FindProperty("Output");
            _modeSerializedProperty = serializedObject.FindProperty("_artMode");
            InitPopup();
        }
        else
        {
            NodeEditorGUILayout.PropertyField(_serializedPropertyInputPort);
            NodeEditorGUILayout.PropertyField(_serializedPropertyOutputPort);
            DrawEnumPopup();
            EditorGUILayout.LabelField("Arts:");
            _serializedPropertyIndexArt.intValue = EditorGUILayout.Popup(_serializedPropertyIndexArt.intValue, _namesArts);
        }
    }
    
    private void InitPopup()
    {
        List<string> namesArtsToPopup = new List<string>();
        if (_showArtNode.Arts != null)
        {
            for (int i = 0; i < _showArtNode.Arts.Count; ++i)
            {
                namesArtsToPopup.Add(_showArtNode.Arts[i].name);
            }
        }
        _namesArts = namesArtsToPopup.ToArray();
    }
    private void DrawEnumPopup()
    {
        ShowArtMode directionType = (ShowArtMode)_modeSerializedProperty.enumValueIndex;
        EditorGUILayout.LabelField("Mode: ");
        directionType = (ShowArtMode)EditorGUILayout.EnumPopup(directionType);
        _modeSerializedProperty.enumValueIndex = (int) directionType;
    }
}