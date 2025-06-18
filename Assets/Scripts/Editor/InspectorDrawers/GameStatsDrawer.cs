using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameStatsViewer))]
public class GameStatsDrawer : Editor
{
    private GameStatsViewer _gameStatsViewer;
    private GUIStyle _guiStyle;

    private void OnEnable()
    {
        _gameStatsViewer = target as GameStatsViewer;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (_gameStatsViewer.StatsToView != null && _gameStatsViewer.StatsToView.Count > 0)
        {
            for (int i = 0; i < _gameStatsViewer.StatsToView.Count; i++)
            {
                DrawFieldColor(
                    _gameStatsViewer.StatsToView[i].LocalizationName.DefaultText,
                    _gameStatsViewer.StatsToView[i].Value,
                    _gameStatsViewer.StatsToView[i].ShowInEndGameResultKey,
                    _gameStatsViewer.StatsToView[i].ColorField);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFieldColor(string fieldName, int value, bool showInEndGameResultKey, Color color)
    {
        EditorGUILayout.Space(20f);
        EditorGUILayout.BeginHorizontal();
        Color oldColor = GUI.color;
        _guiStyle = new GUIStyle(GUI.skin.label);
        _guiStyle.normal.textColor = color;
        DrawField(fieldName, _guiStyle, 200f, 2);
        _guiStyle.normal.textColor = oldColor;
        DrawField(value.ToString(), _guiStyle, 30f, 2);

        DrawField($"ShowInGameResult: {showInEndGameResultKey}", _guiStyle, 200f, -1);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawField(string fieldName, GUIStyle style, float width = 0f, int addFontSize = 0, FontStyle fontStyle = FontStyle.Normal)
    {
        int fontSize = style.fontSize;
        style.fontSize = style.fontSize + addFontSize;
        style.fontStyle = fontStyle;
        EditorGUILayout.LabelField(fieldName, style, GUILayout.Width(width));
        
        
        style.fontSize = fontSize;
    }
}