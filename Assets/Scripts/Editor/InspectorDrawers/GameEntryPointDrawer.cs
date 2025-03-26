

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEntryPointEditor))]
public class GameEntryPointDrawer : Editor
{
    private LevelEntryPointEditor _gameEntryPoint;
    private void OnEnable()
    {
        _gameEntryPoint = target as LevelEntryPointEditor;
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