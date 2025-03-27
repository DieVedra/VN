using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SeriaNodeGraphsHandler))]
public class NodeGraphsHandlerDrawer : Editor
{ 
    private SeriaNodeGraphsHandler _seriaNodeGraphsHandler;

    private MethodInfo _moveNextMethod;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (_seriaNodeGraphsHandler == null)
        {
            _seriaNodeGraphsHandler = target as SeriaNodeGraphsHandler;
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
            _moveNextMethod = _seriaNodeGraphsHandler.GetType().GetMethod("MoveNext", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_moveNextMethod != null)
            {
                TryCallMethod();
            }
        }
        else
        {
            _moveNextMethod.Invoke(_seriaNodeGraphsHandler, null);
        }
    }
}