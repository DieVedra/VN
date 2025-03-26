using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeGraphsHandler))]
public class NodeGraphsHandlerDrawer : Editor
{ 
    private NodeGraphsHandler _nodeGraphsHandler;

    private MethodInfo _moveNextMethod;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (_nodeGraphsHandler == null)
        {
            _nodeGraphsHandler = target as NodeGraphsHandler;
        }
        else
        {
            if (Application.isPlaying == true)
            {
                if (GUILayout.Button("MoveNext"))
                {
                    TryCallMethod();
                }
            }
        }
    }

    private void TryCallMethod()
    {
        if (_moveNextMethod == null)
        {
            _moveNextMethod = _nodeGraphsHandler.GetType().GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_moveNextMethod != null)
            {
                TryCallMethod();
            }
        }
        else
        {
            _moveNextMethod.Invoke(_nodeGraphsHandler, null);
        }
    }
}