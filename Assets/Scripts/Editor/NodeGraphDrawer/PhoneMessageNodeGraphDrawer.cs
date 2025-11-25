using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(PhoneMessagesGraph))]
public class PhoneMessageNodeGraphDrawer : NodeGraphEditor
{
    private PhoneMessagesGraph _phoneMessagesGraph;
    private SerializedProperty _nodeListProperty;
    private SerializedProperty _startNodeSerializedProperty;
    public override void OnGUI()
    {
        base.OnGUI();
        if (_phoneMessagesGraph == null)
        {
            _phoneMessagesGraph = target as PhoneMessagesGraph;
            _nodeListProperty = serializedObject.FindProperty("nodes");
        }
        else
        {
            serializedObject.Update();
            if (GUILayout.Button("Find Start Node", GUILayout.Width(150f)))
            {
                if (_nodeListProperty.arraySize > 0)
                {
                    _startNodeSerializedProperty = _nodeListProperty.GetArrayElementAtIndex(0);
                    CentralNode(_startNodeSerializedProperty.objectReferenceValue as StartNode);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
    private void CentralNode(BaseNode node)
    {
        window.zoom = 10;
        float flippedX = -node.position.x;
        float flippedY = -node.position.y;
        window.panOffset = new Vector2(flippedX, flippedY);
    }
}