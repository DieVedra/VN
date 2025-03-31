using System;
using System.Linq;
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
    private SerializedProperty _currentAdditionalSoundIndexSerializedProperty;
    private SerializedProperty _smoothTransitionKeySerializedProperty;
    private SerializedProperty _smoothVolumeIncreaseSerializedProperty;
    private SerializedProperty _smoothVolumeDecreaseSerializedProperty;
    private SerializedProperty _mergeSoundsKeySerializedProperty;

    private SerializedProperty _audioEffectsSerializedProperty;

    private SerializedProperty _effectKeysSerializedProperty;
    private SerializedProperty _volumeSoundSerializedProperty;
    private SerializedProperty _volumeAdditionalSoundSerializedProperty;
    private MethodInfo _playAudioMethod;
    private MethodInfo _stopAudioMethod;
    private MethodInfo _setVolumeMethod;
    private MethodInfo _setAdditionalVolumeMethod;
    
    private MethodInfo _addEffectMethod;
    private MethodInfo _removeEffectMethod;
    private LineDrawer _lineDrawer;
    private float _sliderValue;
    private int _currentEnumIndex;
    private AudioEffect _audioEffect;
    private string[] _namesEffects; 
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
            _currentAdditionalSoundIndexSerializedProperty = serializedObject.FindProperty("_currentAdditionalSoundIndex");
            _mergeSoundsKeySerializedProperty = serializedObject.FindProperty("_mergeSoundsKey");
            _volumeSoundSerializedProperty = serializedObject.FindProperty("_volumeSound");
            _volumeAdditionalSoundSerializedProperty = serializedObject.FindProperty("_volumeAdditionalSound");
            _audioEffectsSerializedProperty = serializedObject.FindProperty("_audioEffects");
            _effectKeysSerializedProperty = serializedObject.FindProperty("_effectKeys");
            _lineDrawer = new LineDrawer();
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

        // EditorGUI.BeginChangeCheck();
        // EditorGUILayout.BeginHorizontal();
        
        // EditorGUILayout.LabelField("LowPassEffect ", GUILayout.Width(100f));
        // _lowPassEffectSerializedProperty.boolValue =
        //     EditorGUILayout.Toggle(_lowPassEffectSerializedProperty.boolValue);
        // EditorGUILayout.EndHorizontal();
        //
        //
        // if (EditorGUI.EndChangeCheck())
        // {
        //     _soundNode.Sound.AudioEffectsCustodian.SetLowPassEffect(_lowPassEffectSerializedProperty.boolValue);
        // }
        DrawAddEffectsPanel();
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
                DrawPopupClips(_soundNode.Names, _currentSoundIndexSerializedProperty, "Audio Clips: ");
                DrawVolumeSlider(ref  _setVolumeMethod, _volumeSoundSerializedProperty, "SetVolume", "Volume: ");
                _mergeSoundsKeySerializedProperty.boolValue =
                    EditorGUILayout.Toggle("MergeSounds: ",_mergeSoundsKeySerializedProperty.boolValue);
                if (_mergeSoundsKeySerializedProperty.boolValue == true)
                {
                    DrawPopupClips(_soundNode.AdditionalNames, _currentAdditionalSoundIndexSerializedProperty, "Additional Audio Clips: ");
                    DrawVolumeSlider(ref _setAdditionalVolumeMethod, _volumeAdditionalSoundSerializedProperty, "SetAdditionalVolume", "Volume: ");
                }
                EditorGUI.BeginChangeCheck();
                
                EditorGUILayout.Space(25f);
                EditorGUILayout.LabelField($"Play Time: {_sliderValue}");
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
                    InvokeMethod(ref _playAudioMethod, "PlayAudio");
                }

                if (GUILayout.Button("Stop"))
                {
                    InvokeMethod(ref _stopAudioMethod, "StopAudio");
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void InvokeMethod(ref MethodInfo methodInfo, string name, object[] parameter = null)
    {
        if (methodInfo == null)
        {
            methodInfo =  _soundNode.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        else if (methodInfo == null)
        {
            methodInfo = _soundNode.GetType().GetMethods()
                .FirstOrDefault(m => m.Name == name && m.GetParameters().Length == 1);

        }
        if (methodInfo != null)
        {
            methodInfo.Invoke(_soundNode, parameter);
        }
    }

    private void DrawPopupClips(string[] names, SerializedProperty serializedProperty, string label)
    {
        EditorGUILayout.LabelField(label);
        serializedProperty.intValue = EditorGUILayout.Popup(serializedProperty.intValue,  names);
        EditorGUILayout.Space(10f);
    }

    private void DrawVolumeSlider(ref MethodInfo methodInfo, SerializedProperty serializedProperty, string nameMethod, string label)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField(label);
        serializedProperty.floatValue = GUILayout.HorizontalSlider(serializedProperty.floatValue, 0f, 1f, GUILayout.Width(170f));
        if (EditorGUI.EndChangeCheck())
        {
            InvokeMethod(ref methodInfo, nameMethod);
        }
        EditorGUILayout.Space(25f);
    }

    private void DrawAddEffectsPanel()
    {
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        _namesEffects = Enum.GetNames(typeof(AudioEffect));
        _currentEnumIndex = EditorGUILayout.Popup(_currentEnumIndex, _namesEffects);

        if (GUILayout.Button("Add") && _currentEnumIndex != 0)
        {
            AddEffect();
        }
        EditorGUILayout.EndHorizontal();

        if (_audioEffectsSerializedProperty.arraySize > 0)
        {
            for (int i = 0; i < _audioEffectsSerializedProperty.arraySize; ++i)
            {
                DrawEffectField(_audioEffectsSerializedProperty.GetArrayElementAtIndex(i),
                    _effectKeysSerializedProperty.GetArrayElementAtIndex(i),
                    i);
            }
        }
    }

    private void DrawEffectField(SerializedProperty audioEffectsSerializedProperty, SerializedProperty effectKeysSerializedProperty, int currentIndex)
    {
        _audioEffect = (AudioEffect)audioEffectsSerializedProperty.enumValueIndex;
        EditorGUILayout.LabelField($"Effect {_audioEffect.ToString()}");
        EditorGUILayout.BeginHorizontal();

        // EditorGUILayout.LabelField($"Effect {_audioEffect.ToString()}", GUILayout.Width(50f));

        effectKeysSerializedProperty.boolValue = EditorGUILayout.Toggle("IsOn", effectKeysSerializedProperty.boolValue);
        
        if (GUILayout.Button("X"))
        {
            RemoveEffect(currentIndex);
        }
        EditorGUILayout.EndHorizontal();
        _lineDrawer.DrawHorizontalLine(Color.cyan);
    }
    private void AddEffect()
    {
        InvokeMethod(ref _addEffectMethod, "AddEffect", new object[]{(AudioEffect)_currentEnumIndex});
    }

    private void RemoveEffect(int index)
    {
        InvokeMethod(ref _removeEffectMethod, "RemoveEffect", new object[]{index});

    }
}