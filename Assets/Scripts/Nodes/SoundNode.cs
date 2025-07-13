using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#00536A"), NodeWidth(200)]
public class SoundNode : BaseNode
{
    [SerializeField, HideInInspector] private int _currentMusicSoundIndex;
    [SerializeField, HideInInspector] private int _currentAmbientSoundIndex;

    [SerializeField, HideInInspector] private bool _isInstantNodeTransition;
    
    [SerializeField, HideInInspector] private float _volumeMusicSound = 1f;
    [SerializeField, HideInInspector] private float _volumeAmbientSound = 1f;

    [SerializeField, HideInInspector] private bool _smoothMusicTransitionKey;
    [SerializeField, HideInInspector] private bool _isMusicSmoothVolumeIncrease;
    [SerializeField, HideInInspector] private bool _isMusicSmoothVolumeDecrease;

    [SerializeField, HideInInspector] private bool _smoothTransitionKeyAmbientSound;
    [SerializeField, HideInInspector] private bool _isSmoothVolumeIncreaseAmbientSound;
    [SerializeField, HideInInspector] private bool _isSmoothVolumeDecreaseAmbientSound;
    
    [SerializeField, HideInInspector] private bool _showMusicSoundsKey;
    [SerializeField, HideInInspector] private bool _showAmbientSoundsKey;
    [SerializeField, HideInInspector] private bool _showEffectsKey;

    [SerializeField] private List<AudioEffect> _audioEffects;
    [SerializeField] private List<bool> _effectKeys;

    private Sound _sound;
    private TaskRunner _taskRunner;
    private bool _startedPlayMusicPlayMusic;
    private bool _startedPlayAmbient;
    public Sound Sound => _sound;

    public bool StartedPlayMusic => _startedPlayMusicPlayMusic;
    public bool StartedPlayAmbient => _startedPlayAmbient;

    public void ConstructMySoundNode(Sound sound)
    {
        _sound = sound;
        _startedPlayMusicPlayMusic = false;
        _startedPlayAmbient = false;
        _taskRunner = new TaskRunner();
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        ChangeSound(_smoothMusicTransitionKey, _isMusicSmoothVolumeIncrease, _isMusicSmoothVolumeDecrease, _currentMusicSoundIndex,
            AudioSourceType.Music);
        ChangeSound(_smoothTransitionKeyAmbientSound, _isSmoothVolumeIncreaseAmbientSound, _isSmoothVolumeDecreaseAmbientSound,
            _currentAmbientSoundIndex, AudioSourceType.Ambient);
        TryPushEffects();
        if (_isInstantNodeTransition == false)
        {
            await _taskRunner.TryRunTasks();
        }
        else
        {
            _taskRunner.TryRunTasks().Forget();
        }
        if (isMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }
    }

    private void PlayMusicAudio()
    {
        _startedPlayMusicPlayMusic = true;
        _sound.PlayAudioByIndex(_currentMusicSoundIndex, AudioSourceType.Music);
    }

    private void PlayAmbientAudio()
    {
        _startedPlayAmbient = true;
        _sound.PlayAudioByIndex(_currentAmbientSoundIndex, AudioSourceType.Ambient);
    }

    private void StopMusicAudio()
    {
        _startedPlayMusicPlayMusic = false;
        _sound.StopAudio(AudioSourceType.Music);
    }

    private void StopAmbientAudio()
    {
        _startedPlayAmbient = false;
        _sound.StopAudio(AudioSourceType.Ambient);
    }
    private void SetVolume()
    {
        _sound.SetVolume(_volumeMusicSound, AudioSourceType.Music);
    }

    private void SetAdditionalVolume()
    {
        _sound.SetVolume(_volumeAmbientSound, AudioSourceType.Ambient);
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

    private void ChangeSound(bool smoothTransitionKey, bool isSmoothVolumeIncrease, bool isSmoothVolumeDecrease, int currentSoundIndex, AudioSourceType audioSourceType)
    {
        if (smoothTransitionKey == true )
        {
            _taskRunner.AddOperationToList(
                ()=> _sound.SmoothAudio.SmoothReplacementAudio(CancellationTokenSource.Token, currentSoundIndex, audioSourceType));
        }
        else if (isSmoothVolumeIncrease == true)
        {
            _taskRunner.AddOperationToList(
                ()=> _sound.SmoothAudio.SmoothPlayAudio(CancellationTokenSource.Token, currentSoundIndex, audioSourceType));
        }
        else if (isSmoothVolumeDecrease == true)
        {
            _taskRunner.AddOperationToList(
                ()=> _sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, audioSourceType));
        }

        if (CheckVolume(_sound.VolumeMusic, _volumeMusicSound))
        {
            _taskRunner.AddOperationToList(
                ()=> _sound.SmoothAudio.SmoothVolumeChange(CancellationTokenSource.Token, _volumeMusicSound, AudioSourceType.Music));
        }
        if (CheckVolume(_sound.VolumeAmbient, _volumeAmbientSound))
        {
            _taskRunner.AddOperationToList(
                ()=> _sound.SmoothAudio.SmoothVolumeChange(CancellationTokenSource.Token, _volumeAmbientSound, AudioSourceType.Ambient));
        }
    }
    private bool CheckVolume(float currentVolume, float targetVolume)
    {
        if ((Math.Abs(currentVolume - targetVolume) > 0.05f) == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}