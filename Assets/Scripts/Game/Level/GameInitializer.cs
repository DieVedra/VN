using System;
using UniRx;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
public static class GameInitializer
{
    private const float _timerTime = 0.04f;
    public static bool IsGamePlayMode { get; private set; }
    private static LevelEntryPointEditor _levelEntryPointEditor;
    private static CompositeDisposable _compositeDisposable;
    
    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        TryInitWithTimer(_timerTime);
    }
    private static void OnPlayModeStateChanged(PlayModeStateChange playModeState)
    {
        switch (playModeState)
        {
            case PlayModeStateChange.ExitingEditMode:
                IsGamePlayMode = true;
                // Debug.Log($"ExitingEditMode");
                break;
            
            case PlayModeStateChange.ExitingPlayMode:
                IsGamePlayMode = false;
                // Debug.Log($"ExitingPlayMode");
                break;
            case PlayModeStateChange.EnteredPlayMode:
                IsGamePlayMode = true;
                // Debug.Log($"EnteredPlayMode");
                break;
            case PlayModeStateChange.EnteredEditMode:
                IsGamePlayMode = false;
                TryInitWithTimer(_timerTime);
                // Debug.Log($"EnteredEditMode");
                break;
        }
    }

    private static void TryInitWithTimer(float time)
    {
        _compositeDisposable = new CompositeDisposable();
        Observable.Timer(TimeSpan.FromSeconds(time)).Repeat().Subscribe(_ =>
        {
            if (TryGetGameEntryPoint() == null)
            {
                IsGamePlayMode = false;
            }
            if (TryInitGameEntryPoint())
            {
                _compositeDisposable.Clear();
            }
            else if(IsGamePlayMode == true)
            {
                _compositeDisposable.Clear();
            }
        }).AddTo(_compositeDisposable);
    }

    private static bool TryInitGameEntryPoint()
    {
        if (_levelEntryPointEditor != null && IsGamePlayMode == false)
        {
            if (_levelEntryPointEditor.InitializeInEditMode)
            {
                _levelEntryPointEditor.Init();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private static LevelEntryPointEditor TryGetGameEntryPoint()
    {
        if (_levelEntryPointEditor == null)
        {
            _levelEntryPointEditor = Object.FindObjectOfType<LevelEntryPointEditor>();
        }
        if (_levelEntryPointEditor == null)
        {
            return null;
        }
        else
        {
            return _levelEntryPointEditor;
        }
    }
}
#endif