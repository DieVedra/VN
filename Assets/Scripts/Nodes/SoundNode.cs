using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeTint("#00536A"), NodeWidth(200)]
public class SoundNode : BaseNode
{
    [SerializeField, HideInInspector] private int _currentSoundIndex;
    [SerializeField, HideInInspector] private bool _smoothTransitionKey;
    [SerializeField, HideInInspector] private bool _isSmoothVolumeIncrease;
    [SerializeField, HideInInspector] private bool _isSmoothVolumeDecrease;
    [SerializeField, HideInInspector] private bool _isInstantNodeTransition;
    [SerializeField, HideInInspector] private bool _lowPassEffectKey;
    
    private Sound _sound;
    private string[] _names;
    private bool _isStarted;
    public IReadOnlyList<AudioClip> Clips => _sound.Clips;
    public Sound Sound => _sound;
    public string[] Names => _names;

    public bool IsStarted => _isStarted;

    public void ConstructMySoundNode(Sound sound)
    {
        _sound = sound;
        _isStarted = false;
        InitNames();
        _sound.PlayEvent.Subscribe(_=>
        {
            StopAudio();
        });
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();

        if (_isInstantNodeTransition == false)
        {
            if (_smoothTransitionKey == true && _isStarted == false)
            {
                _isStarted = true;
                await _sound.SmoothReplacementAudio(CancellationTokenSource.Token, GetEffectsOperations() ,_currentSoundIndex);
            }
            else if (_isSmoothVolumeIncrease == true)
            {
                await _sound.SmoothPlayAudio(CancellationTokenSource.Token, GetEffectsOperations(), _currentSoundIndex);
            }
            else if (_isSmoothVolumeDecrease == true)
            {
                await _sound.SmoothStopAudio(CancellationTokenSource.Token, GetEffectsOperations());
            }
            else
            {
                await _sound.AudioEffectsHandler.SetLowPassEffectSmooth(CancellationTokenSource.Token, _lowPassEffectKey);
            }
        }
        else
        {
            if (_smoothTransitionKey == true && _isStarted == false)
            {
                _isStarted = true;
                _sound.SmoothReplacementAudio(CancellationTokenSource.Token, GetEffectsOperations(), _currentSoundIndex).Forget();
            }
            else if (_isSmoothVolumeIncrease == true)
            {
                _sound.SmoothPlayAudio(CancellationTokenSource.Token, GetEffectsOperations(), _currentSoundIndex).Forget();
            }
            else if (_isSmoothVolumeDecrease == true)
            {
                _sound.SmoothStopAudio(CancellationTokenSource.Token, GetEffectsOperations()).Forget();
            }
            else
            {
                await _sound.AudioEffectsHandler.SetLowPassEffectSmooth(CancellationTokenSource.Token, _lowPassEffectKey);
            }
        }
        if (isMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }
    }

    private void InitNames()
    {
        _names = new string[Clips.Count];
        for (int i = 0; i < Clips.Count; i++)
        {
            _names[i] = Clips[i].name.MyCutString(0,1, separator: '_');
        }
    }

    private void PlayAudio()
    {
        _sound.PlayAudio(_currentSoundIndex);
        _isStarted = true;
    }

    private void StopAudio()
    {
        _sound.StopAudio();
        _isStarted = false;
    }

    private Action GetEffectsOperations()
    {
        return () => { _sound.AudioEffectsHandler.SetLowPassEffect(_lowPassEffectKey); };
    }
}