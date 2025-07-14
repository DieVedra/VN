using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(SoundNode))]
public class SoundNodeDrawer : NodeEditor
{
    private const float _minValue = 0;
    private const string _smoothTransitionFieldName = "SmoothAudioTransition: ";
    private const string _smoothVolumeIncreaseFieldName = "SmoothIncreaseVol: ";
    private const string _smoothVolumeDecreaseFieldName = "SmoothDecreaseVol: ";
    private SoundNode _soundNode;
    private SerializedProperty _instantNodeTransitionSerializedProperty;
    private SerializedProperty _currentSoundIndexSerializedProperty;
    private SerializedProperty _currentAdditionalSoundIndexSerializedProperty;
    
    private SerializedProperty _smoothMusicTransitionKeySerializedProperty;
    private SerializedProperty _smoothMusicVolumeIncreaseSerializedProperty;
    private SerializedProperty _smoothMusicVolumeDecreaseSerializedProperty;
    
    private SerializedProperty _smoothTransitionKeyAmbientSerializedProperty;
    private SerializedProperty _smoothVolumeIncreaseAmbientSerializedProperty;
    private SerializedProperty _smoothVolumeDecreaseAmbientSerializedProperty;
    
    private SerializedProperty _ambientSoundsKeySerializedProperty;
    private SerializedProperty _musicSoundsKeySerializedProperty;

    private SerializedProperty _audioEffectsSerializedProperty;
    private SerializedProperty _inputSerializedProperty;
    private SerializedProperty _outputSerializedProperty;

    private SerializedProperty _effectKeysSerializedProperty;
    private SerializedProperty _showEffectKeysSerializedProperty;
    private SerializedProperty _volumeSoundSerializedProperty;
    private SerializedProperty _volumeAdditionalSoundSerializedProperty;
    private MethodInfo _playMusicAudioMethod;
    private MethodInfo _stopMusicAudioMethod;
    
    private MethodInfo _playAmbientAudioMethod;
    private MethodInfo _stopAmbientAudioMethod;
    
    private MethodInfo _setVolumeMethod;
    private MethodInfo _setAdditionalVolumeMethod;
    
    private MethodInfo _addEffectMethod;
    private MethodInfo _removeEffectMethod;
    private LineDrawer _lineDrawer;
    private float _sliderPlayerMusicValue;
    private float _sliderPlayerAmbientValue;
    private int _currentEnumIndex;
    private AudioEffect _audioEffect;
    private string[] _namesEffects;
    private string[] _names;
    private string[] _ambientNames;
    public override void OnBodyGUI()
    {
        if (_soundNode == null)
        {
            _soundNode = target as SoundNode;
        }

        if (_currentSoundIndexSerializedProperty == null)
        {
            _currentSoundIndexSerializedProperty = serializedObject.FindProperty("_currentMusicSoundIndex");
            
            _smoothMusicTransitionKeySerializedProperty = serializedObject.FindProperty("_smoothMusicTransitionKey");
            _smoothMusicVolumeIncreaseSerializedProperty = serializedObject.FindProperty("_isMusicSmoothVolumeIncrease");
            _smoothMusicVolumeDecreaseSerializedProperty = serializedObject.FindProperty("_isMusicSmoothVolumeDecrease");
            
            _smoothTransitionKeyAmbientSerializedProperty = serializedObject.FindProperty("_smoothTransitionKeyAmbientSound");
            _smoothVolumeIncreaseAmbientSerializedProperty = serializedObject.FindProperty("_isSmoothVolumeIncreaseAmbientSound");
            _smoothVolumeDecreaseAmbientSerializedProperty = serializedObject.FindProperty("_isSmoothVolumeDecreaseAmbientSound");
            
            _instantNodeTransitionSerializedProperty = serializedObject.FindProperty("_isInstantNodeTransition");
            _currentAdditionalSoundIndexSerializedProperty = serializedObject.FindProperty("_currentAmbientSoundIndex");
            _ambientSoundsKeySerializedProperty = serializedObject.FindProperty("_showAmbientSoundsKey");
            _musicSoundsKeySerializedProperty = serializedObject.FindProperty("_showMusicSoundsKey");
            _volumeSoundSerializedProperty = serializedObject.FindProperty("_volumeMusicSound");
            _volumeAdditionalSoundSerializedProperty = serializedObject.FindProperty("_volumeAmbientSound");
            _audioEffectsSerializedProperty = serializedObject.FindProperty("_audioEffects");
            _effectKeysSerializedProperty = serializedObject.FindProperty("_effectKeys");
            _showEffectKeysSerializedProperty = serializedObject.FindProperty("_showEffectsKey");
            
            _inputSerializedProperty = serializedObject.FindProperty("Input");
            _outputSerializedProperty = serializedObject.FindProperty("Output");
            _lineDrawer = new LineDrawer();
        }
        serializedObject.Update();
        NodeEditorGUILayout.PropertyField(_inputSerializedProperty);
        NodeEditorGUILayout.PropertyField(_outputSerializedProperty);
        EditorGUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("InstantNodeTransition: ", GUILayout.Width(110f));
        _instantNodeTransitionSerializedProperty.boolValue = EditorGUILayout.Toggle(_instantNodeTransitionSerializedProperty.boolValue);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10f);
        _musicSoundsKeySerializedProperty.boolValue =
            EditorGUILayout.Foldout(_musicSoundsKeySerializedProperty.boolValue, "Music: ");
        
        if (_musicSoundsKeySerializedProperty.boolValue == true)
        {
            ChangeMode();
            if (_smoothMusicTransitionKeySerializedProperty.boolValue == true ||
                _smoothMusicVolumeIncreaseSerializedProperty.boolValue == true)
            {
                InitMusicNames();
                DrawPopupClips(_names, _currentSoundIndexSerializedProperty, "Audio Clips: ");
                DrawVolumeSlider(ref  _setVolumeMethod, _volumeSoundSerializedProperty, "SetVolume", "Volume: ");
                TryDrawMusicPlayer();
            }
        }

        _ambientSoundsKeySerializedProperty.boolValue =
            EditorGUILayout.Foldout(_ambientSoundsKeySerializedProperty.boolValue, "Ambient: ");
        
        if (_ambientSoundsKeySerializedProperty.boolValue == true)
        {
            ChangeModeAdditionalAudioClips();
            if (_smoothTransitionKeyAmbientSerializedProperty.boolValue == true ||
                _smoothVolumeIncreaseAmbientSerializedProperty.boolValue == true)
            {
                InitAmbientNames();
                DrawPopupClips(_ambientNames, _currentAdditionalSoundIndexSerializedProperty, "Additional Audio Clips: ");
                DrawVolumeSlider(ref _setAdditionalVolumeMethod, _volumeAdditionalSoundSerializedProperty, "SetAdditionalVolume", "Volume: ");
                TryDrawAmbientPlayer();
            }
        }
        DrawPlayStopButtonsFull();
        _showEffectKeysSerializedProperty.boolValue = EditorGUILayout.Foldout(_showEffectKeysSerializedProperty.boolValue, "Effects: ");
        if (_showEffectKeysSerializedProperty.boolValue)
        {
            DrawAddEffectsPanel();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ChangeMode()
    {
        DrawToggle(_smoothTransitionFieldName, _smoothMusicVolumeIncreaseSerializedProperty, _smoothMusicVolumeDecreaseSerializedProperty, _smoothMusicTransitionKeySerializedProperty);
        DrawToggle(_smoothVolumeIncreaseFieldName, _smoothMusicTransitionKeySerializedProperty, _smoothMusicVolumeDecreaseSerializedProperty, _smoothMusicVolumeIncreaseSerializedProperty);
        DrawToggle(_smoothVolumeDecreaseFieldName,_smoothMusicVolumeIncreaseSerializedProperty, _smoothMusicTransitionKeySerializedProperty, _smoothMusicVolumeDecreaseSerializedProperty);
    }

    private void ChangeModeAdditionalAudioClips()
    {
        DrawToggle(_smoothTransitionFieldName, _smoothVolumeIncreaseAmbientSerializedProperty, _smoothVolumeDecreaseAmbientSerializedProperty, _smoothTransitionKeyAmbientSerializedProperty);
        DrawToggle(_smoothVolumeIncreaseFieldName, _smoothTransitionKeyAmbientSerializedProperty, _smoothVolumeDecreaseAmbientSerializedProperty, _smoothVolumeIncreaseAmbientSerializedProperty);
        DrawToggle(_smoothVolumeDecreaseFieldName,_smoothVolumeIncreaseAmbientSerializedProperty, _smoothTransitionKeyAmbientSerializedProperty, _smoothVolumeDecreaseAmbientSerializedProperty);
    }
    private void DrawToggle(string fieldName, SerializedProperty serializedProperty1, SerializedProperty serializedProperty2, SerializedProperty serializedProperty3)
    {
        if (serializedProperty1.boolValue == false && serializedProperty2.boolValue == false)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(fieldName);
            serializedProperty3.boolValue = EditorGUILayout.Toggle(serializedProperty3.boolValue);
            EditorGUILayout.EndHorizontal();
        }
    }

    private void TryDrawMusicPlayer()
    {
        if (_soundNode.Sound != null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Progress: ");
            _sliderPlayerMusicValue = GUILayout.HorizontalSlider(_sliderPlayerMusicValue,
                _minValue, _soundNode.Sound.CurrentMusicClipTime, GUILayout.Width(170f));
            if (_soundNode.StartedPlayMusic)
            {
                if (EditorGUI.EndChangeCheck())
                {
                    _soundNode.Sound.SetPlayTime(_sliderPlayerMusicValue, AudioSourceType.Music);
                }
                else
                {
                    _sliderPlayerMusicValue = _soundNode.Sound.PlayTimeMusic;
                }
            }
            DrawPlayStopButtons(() => { InvokeMethod(ref _playMusicAudioMethod, "PlayMusicAudio");},
                ()=>{InvokeMethod(ref _stopMusicAudioMethod, "StopMusicAudio"); });
        }
    }

    private void TryDrawAmbientPlayer()
    {
        if (_soundNode.Sound != null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Progress: ");
            _sliderPlayerAmbientValue = GUILayout.HorizontalSlider(_sliderPlayerAmbientValue,
                _minValue, _soundNode.Sound.CurrentAmbientClipTime, GUILayout.Width(170f));
            if (_soundNode.StartedPlayAmbient)
            {
                if (EditorGUI.EndChangeCheck())
                {
                    _soundNode.Sound.SetPlayTime(_sliderPlayerAmbientValue, AudioSourceType.Ambient);
                }
                else
                {
                    _sliderPlayerAmbientValue = _soundNode.Sound.PlayTimeAmbient;
                }
            }
            DrawPlayStopButtons(() => { InvokeMethod(ref _playAmbientAudioMethod, "PlayAmbientAudio");},
                ()=>{InvokeMethod(ref _stopAmbientAudioMethod, "StopAmbientAudio"); });
        }
    }

    private void DrawPlayStopButtonsFull()
    {
        if ((_smoothMusicTransitionKeySerializedProperty.boolValue && _smoothTransitionKeyAmbientSerializedProperty.boolValue)
            ||
            (_smoothMusicVolumeIncreaseSerializedProperty.boolValue && _smoothVolumeIncreaseAmbientSerializedProperty.boolValue)
            ||
            (_smoothMusicTransitionKeySerializedProperty.boolValue && _smoothVolumeIncreaseAmbientSerializedProperty.boolValue)
            ||
            (_smoothMusicVolumeIncreaseSerializedProperty.boolValue && _smoothTransitionKeyAmbientSerializedProperty.boolValue))
        {
            DrawPlayStopButtons(
                () =>
                {
                    InvokeMethod(ref _playMusicAudioMethod, "PlayMusicAudio");
                    InvokeMethod(ref _playAmbientAudioMethod, "PlayAmbientAudio");
                },
                () =>
                {
                    InvokeMethod(ref _stopMusicAudioMethod, "StopMusicAudio");
                    InvokeMethod(ref _stopAmbientAudioMethod, "StopAmbientAudio");
                }, "PlayAll", "StopAll");
        }
    }
    private void DrawPlayStopButtons(Action playOperation, Action stopOperation, string play = "Play", string stop = "Stop")
    {
        EditorGUILayout.Space(20f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(play))
        {
            playOperation?.Invoke();
        }

        if (GUILayout.Button(stop))
        {
            stopOperation?.Invoke();
        }

        EditorGUILayout.EndHorizontal();
    }
    private void InvokeMethod(ref MethodInfo methodInfo, string nameMethod, object[] parameter = null)
    {
        if (methodInfo == null)
        {
            methodInfo =  _soundNode.GetType().GetMethod(nameMethod, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        else if (methodInfo == null)
        {
            methodInfo = _soundNode.GetType().GetMethods()
                .FirstOrDefault(m => m.Name == nameMethod && m.GetParameters().Length == 1);

        }
        if (methodInfo != null)
        {
            methodInfo.Invoke(_soundNode, parameter);
        }
    }

    private void DrawPopupClips(string[] names, SerializedProperty serializedProperty, string label)
    {
        if (names != null && names.Length > 0)
        {
            EditorGUILayout.LabelField(label);
            // EditorGUI.BeginChangeCheck();

            serializedProperty.intValue = EditorGUILayout.Popup(serializedProperty.intValue,  names);
            // if (EditorGUI.EndChangeCheck())
            // {
            //     InvokeMethod(ref methodInfo, nameMethod);
            // }
            EditorGUILayout.Space(10f);
        }
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
    private void InitMusicNames()
    {
        InitNames(ref _names, _soundNode.Sound.Clips);
    }
    private void InitAmbientNames()
    {
        InitNames(ref _ambientNames, _soundNode.Sound.AmbientClips);
    }
    private void InitNames(ref string[] names, IReadOnlyList<AudioClip> clips)
    {
        List<string> names1 = new List<string>(clips.Count);
        foreach (var clip in clips)
        {
            names1.Add(clip.name);
        }

        names = names1.ToArray();
    }
}