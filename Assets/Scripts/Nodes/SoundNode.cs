using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

[NodeTint("#00536A"), NodeWidth(200)]
public class SoundNode : BaseNode
{
    [SerializeField, HideInInspector] private int _currentSoundIndex;
    [SerializeField, HideInInspector] private int _currentAdditionalSoundIndex;
    [SerializeField, HideInInspector] private float _volumeSound = 1f;
    [SerializeField, HideInInspector] private float _volumeAdditionalSound = 1f;
    [SerializeField, HideInInspector] private bool _smoothTransitionKey;
    [SerializeField, HideInInspector] private bool _isSmoothVolumeIncrease;
    [SerializeField, HideInInspector] private bool _isSmoothVolumeDecrease;
    [SerializeField, HideInInspector] private bool _isInstantNodeTransition;
    [SerializeField, HideInInspector] private bool _mergeSoundsKey;

    [SerializeField/*, HideInInspector*/] private List<AudioEffect> _audioEffects;
    [SerializeField/*, HideInInspector*/] private List<bool> _effectKeys;
    
    private Sound _sound;
    private TaskRunner _taskRunner;
    private bool _isStarted;
    public IReadOnlyList<AudioClip> Clips => _sound.Clips;
    public Sound Sound => _sound;
    public string[] Names { get; private set; }
    public string[] AdditionalNames { get; private set; }

    public bool IsStarted => _isStarted;

    public void ConstructMySoundNode(Sound sound)
    {
        _sound = sound;
        _isStarted = false;
        _taskRunner = new TaskRunner();
        Names = _sound.Clips.Select(x => x.name.MyCutString(0,1, separator: '_')).ToArray();
        AdditionalNames = _sound.AdditionalClips.Select(x => x.name).ToArray();

        _sound.PlayEvent.Subscribe(_=>
        {
            StopAudio();
        });
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();

        if (_smoothTransitionKey == true && _isStarted == false)
        {
            _isStarted = true;
            SmoothTransition();
        }
        else if (_isSmoothVolumeIncrease == true)
        {
            SmoothPlayAudio();
        }
        else if (_isSmoothVolumeDecrease == true)
        {
            SmoothStopAudio();
        }
        TryPushEffects();

        if (_isInstantNodeTransition == false)
        {
            await _taskRunner.TryRunTasks();
        }
        else
        {
            _taskRunner.TryRunTasks().Forget();
        }
        
        // if (_isInstantNodeTransition == false)
        // {
        //     if (_smoothTransitionKey == true && _isStarted == false)
        //     {
        //         _isStarted = true;
        //         _taskRunner.AddOperationToList(()=> _sound.SmoothReplacementAudio(CancellationTokenSource.Token, _currentSoundIndex));
        //     }
        //     else if (_isSmoothVolumeIncrease == true)
        //     {
        //         _taskRunner.AddOperationToList(()=> _sound.SmoothPlayAudio(CancellationTokenSource.Token, _currentSoundIndex));
        //     }
        //     else if (_isSmoothVolumeDecrease == true)
        //     {
        //         _taskRunner.AddOperationToList(()=>_sound.SmoothStopAudio(CancellationTokenSource.Token));
        //     }
        //     
        //     TryPushEffects();
        //     await _taskRunner.TryRunTasks();
        // }
        // else
        // {
        //     if (_smoothTransitionKey == true && _isStarted == false)
        //     {
        //         _isStarted = true;
        //         _sound.SmoothReplacementAudio(CancellationTokenSource.Token, _currentSoundIndex).Forget();
        //     }
        //     else if (_isSmoothVolumeIncrease == true)
        //     {
        //         _sound.SmoothPlayAudio(CancellationTokenSource.Token, _currentSoundIndex).Forget();
        //     }
        //     else if (_isSmoothVolumeDecrease == true)
        //     {
        //         _sound.SmoothStopAudio(CancellationTokenSource.Token).Forget();
        //     }
        //     
        //     TryPushEffects();
        // }
        if (isMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }
    }
    
    private void PlayAudio()
    {
        _isStarted = false;
        _sound.PlayAudioByIndex(_currentSoundIndex);
        if (_mergeSoundsKey)
        {
            _sound.PlayAudioByClip(_sound.AdditionalClips[_currentAdditionalSoundIndex],1);
        }
        _isStarted = true;
    }

    private void StopAudio()
    {
        _sound.StopAudio();
        _isStarted = false;
    }

    private void SetVolume()
    {
        _sound.SetVolume(_volumeSound);
    }

    private void SetAdditionalVolume()
    {
        _sound.SetVolume(_volumeAdditionalSound, 1);
    }
    private void TryPushEffects()
    {
        IAudioEffectHandler handler;
        for (int i = 0; i < _audioEffects.Count; i++)
        {
            handler = _sound.AudioEffectsCustodian.GetAudioEffect(_audioEffects[i]);
            if (_effectKeys[i] == true)
            {
                if (handler.EffectIsOn == false)
                {
                    _taskRunner.AddOperationToList(() => handler.SetEffectSmooth(CancellationTokenSource.Token, true));
                }
            }
            else
            {
                if (handler.EffectIsOn == true)
                {
                    _taskRunner.AddOperationToList(() => handler.SetEffectSmooth(CancellationTokenSource.Token, false));
                }
            }
        }
    }

    private void AddEffect(AudioEffect audioEffect)
    {
        if (_audioEffects.Contains(audioEffect) == false)
        {
            _audioEffects.Add(audioEffect);
            _effectKeys.Add(false);
        }
    }

    private void RemoveEffect(int index)
    {
        _audioEffects.RemoveAt(index);
        _effectKeys.RemoveAt(index);
    }

    private void SmoothTransition()
    {
        MergeCheck(
            ()=> _sound.SmoothAudio.SmoothReplacementAudio(CancellationTokenSource.Token, _currentSoundIndex, 0),
            ()=> _sound.SmoothAudio.SmoothReplacementAudio(CancellationTokenSource.Token, _currentAdditionalSoundIndex, 1));
    }

    private void SmoothPlayAudio()
    {
        MergeCheck(
            ()=> _sound.SmoothAudio.SmoothPlayAudio(CancellationTokenSource.Token, _currentSoundIndex, 0),
            ()=> _sound.SmoothAudio.SmoothPlayAudio(CancellationTokenSource.Token, _currentAdditionalSoundIndex, 1));
    }

    private void SmoothStopAudio()
    {
        MergeCheck(
            ()=>_sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, 0),
            ()=>_sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, 1));
    }

    private void MergeCheck(Func<UniTask> operation1, Func<UniTask> operation2)
    {
        if (_mergeSoundsKey == true)
        {
            _taskRunner.AddOperationToList(operation1);
            _taskRunner.AddOperationToList(operation2);
        }
        else
        {
            _taskRunner.AddOperationToList(operation1);
        }
    }
}