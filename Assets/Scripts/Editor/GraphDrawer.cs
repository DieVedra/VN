using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(SeriaPartNodeGraph))]
public class GraphDrawer : NodeGraphEditor
{
    private SeriaPartNodeGraph _seriaPartNodeGraph;
    private SerializedProperty _nodeListProperty;
    private int _indexValue;
    private string _textNameNode;
    private bool _nameFinded = false;
    public override void OnGUI()
    {
        base.OnGUI();
        if (_seriaPartNodeGraph == null)
        {
            _seriaPartNodeGraph = target as SeriaPartNodeGraph;
        }
        if (_nodeListProperty == null)
        {
            _nodeListProperty = serializedObject.FindProperty("nodes");
        }

        serializedObject.Update();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Center Node By Index", GUILayout.Width(150f)))
        {
            if (_nodeListProperty != null)
            {
                if (_nodeListProperty.arraySize > _indexValue)
                {
                    SerializedProperty serializedProperty = _nodeListProperty.GetArrayElementAtIndex(_indexValue);
                    CentralNode(serializedProperty.objectReferenceValue as BaseNode);
                }
                else
                {
                    Debug.LogWarning("Node not find by index");
                }
            }
        }

        // EditorGUILayout.LabelField("Node index: ", GUILayout.Width(70f));
        _indexValue = EditorGUILayout.IntField(_indexValue, GUILayout.Width(40f));
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Center Node By Name", GUILayout.Width(150f)))
        {
            if (_nodeListProperty != null)
            {
                SerializedProperty serializedProperty;
                BaseNode node;
                for (int i = 0; i < _nodeListProperty.arraySize; i++)
                {
                   serializedProperty = _nodeListProperty.GetArrayElementAtIndex(i);
                   node = serializedProperty.objectReferenceValue as BaseNode;
                   if (node.namenode == _textNameNode)
                   {
                       _nameFinded = true;
                       CentralNode(node);
                       break;
                   }
                }
                if (_nameFinded == false)
                {
                    Debug.LogWarning("Node not find by name");
                }
                _nameFinded = false;
            }
        }
        _textNameNode = EditorGUILayout.TextField(_textNameNode, GUILayout.Width(100f));
        GUILayout.EndHorizontal();

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