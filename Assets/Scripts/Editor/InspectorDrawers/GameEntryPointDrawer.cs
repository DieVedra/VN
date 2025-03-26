

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEntryPoint))]
public class GameEntryPointDrawer : Editor
{
    private GameEntryPoint _gameEntryPoint;
    private void OnEnable()
    {
        _gameEntryPoint = target as GameEntryPoint;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(20f);
        if (GameInitializer.IsGamePlayMode == false)
        {
            if (GUILayout.Button("Initialize"))
            {
                _gameEntryPoint.Init();
            }
        }
    }
}