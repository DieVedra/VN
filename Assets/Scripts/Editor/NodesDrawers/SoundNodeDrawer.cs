
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(SoundNode))]
public class SoundNodeDrawer : NodeEditor
{
    private string _smoothTransitionFieldName = "SmoothAudioTransition: ";
    private string _smoothVolumeIncreaseFieldName = "SmoothIncreaseVol: ";
    private string _smoothVolumeDecreaseFieldName = "SmoothDecreaseVol: ";
    private SoundNode _soundNode;
    private SerializedProperty _instantNodeTransitionSerializedProperty;
    private SerializedProperty _currentSoundIndexSerializedProperty;
    private SerializedProperty _smoothTransitionKeySerializedProperty;
    private SerializedProperty _smoothVolumeIncreaseSerializedProperty;
    private SerializedProperty _smoothVolumeDecreaseSerializedProperty;
    private SerializedProperty _lowPassEffectSerializedProperty;
    private MethodInfo _playAudioMethod;
    private MethodInfo _stopAudioMethod;
    private float _sliderValue;

    public override void OnBodyGUI()
    {
        if (_soundNode == null)
        {
            _soundNode = target as SoundNode;
        }

        if (_currentSoundIndexSerializedProperty == null)
        {
            _currentSoundIndexSerializedProperty = serializedObject.FindProperty("_currentSoundIndex");
            _smoothTransitionKeySerializedProperty = serializedObject.FindProperty("_smoothTransitionKey");
            _smoothVolumeIncreaseSerializedProperty = serializedObject.FindProperty("_isSmoothVolumeIncrease");
            _smoothVolumeDecreaseSerializedProperty = serializedObject.FindProperty("_isSmoothVolumeDecrease");
            _instantNodeTransitionSerializedProperty = serializedObject.FindProperty("_isInstantNodeTransition");
            _lowPassEffectSerializedProperty = serializedObject.FindProperty("_lowPassEffectKey");
        }
        serializedObject.Update();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Input"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("InstantNodeTransition: ", GUILayout.Width(110f));
        _instantNodeTransitionSerializedProperty.boolValue = EditorGUILayout.Toggle(_instantNodeTransitionSerializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10f);

        ChangeMode();

        DrawPlayerAndPopup();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("LowPassEffect ", GUILayout.Width(100f));
        _lowPassEffectSerializedProperty.boolValue =
            EditorGUILayout.Toggle(_lowPassEffectSerializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            _soundNode.Sound.AudioEffectsHandler.SetLowPassEffect(_lowPassEffectSerializedProperty.boolValue);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ChangeMode()
    {
        DrawToggle(ref _smoothTransitionFieldName, _smoothVolumeIncreaseSerializedProperty, _smoothVolumeDecreaseSerializedProperty, _smoothTransitionKeySerializedProperty);
        DrawToggle(ref _smoothVolumeIncreaseFieldName, _smoothTransitionKeySerializedProperty, _smoothVolumeDecreaseSerializedProperty, _smoothVolumeIncreaseSerializedProperty);
        DrawToggle(ref _smoothVolumeDecreaseFieldName,_smoothVolumeIncreaseSerializedProperty, _smoothTransitionKeySerializedProperty, _smoothVolumeDecreaseSerializedProperty);
    }

    private void DrawToggle(ref string fieldName, SerializedProperty serializedProperty1, SerializedProperty serializedProperty2, SerializedProperty serializedProperty3)
    {
        if (serializedProperty1.boolValue == false && serializedProperty2.boolValue == false)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(fieldName);
            serializedProperty3.boolValue = EditorGUILayout.Toggle(serializedProperty3.boolValue);
            EditorGUILayout.EndHorizontal();

        }
    }

    private void DrawPlayerAndPopup()
    {
        if (_smoothTransitionKeySerializedProperty.boolValue == true || _smoothVolumeIncreaseSerializedProperty.boolValue == true)
        {
            if (_soundNode.Names != null)
            {
                EditorGUILayout.LabelField("Audio Clips: ");
                _currentSoundIndexSerializedProperty.intValue = EditorGUILayout.Popup(_currentSoundIndexSerializedProperty.intValue,  _soundNode.Names);
                EditorGUILayout.Space(10f);

                EditorGUI.BeginChangeCheck();


                _sliderValue = GUILayout.HorizontalSlider(_sliderValue, 0f, _soundNode.Sound.CurrentClipTime, GUILayout.Width(170f));

                if (_soundNode.IsStarted)
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        _soundNode.Sound.SetPlayTime(_sliderValue);
                    }
                    else
                    {
                        _sliderValue = _soundNode.Sound.PlayTime;
                    }
                }

                EditorGUILayout.Space(20f);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Play"))
                {
                    InvokeMethod(_playAudioMethod, "PlayAudio");
                }

                if (GUILayout.Button("Stop"))
                {
                    InvokeMethod(_stopAudioMethod, "StopAudio");
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void InvokeMethod(MethodInfo methodInfo, string name)
    {
        if (methodInfo == null)
        {
            methodInfo =  _soundNode.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        if (methodInfo != null)
        {
            methodInfo.Invoke(_soundNode, null);
        }
    }
}