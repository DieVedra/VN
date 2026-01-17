using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#00536A"), NodeWidth(200)]
public class SoundNode : BaseNode
{
    [SerializeField] private int _currentMusicSoundIndex;
    [SerializeField] private int _currentAmbientSoundIndex;
    
    [SerializeField] private string _currentMusicSoundKey;
    [SerializeField] private string _currentAmbientSoundKey;

    [SerializeField] private bool _isInstantNodeTransition;
    
    [SerializeField] private float _volumeMusicSound = 1f;
    [SerializeField] private float _volumeAmbientSound = 1f;

    [SerializeField] private bool _smoothMusicTransitionKey;
    [SerializeField] private bool _isMusicSmoothVolumeIncrease;
    [SerializeField] private bool _isMusicSmoothVolumeDecrease;

    [SerializeField] private bool _smoothTransitionKeyAmbientSound;
    [SerializeField] private bool _isSmoothVolumeIncreaseAmbientSound;
    [SerializeField] private bool _isSmoothVolumeDecreaseAmbientSound;
    
    [SerializeField] private bool _showMusicSoundsKey;
    [SerializeField] private bool _showAmbientSoundsKey;
    [SerializeField] private bool _showEffectsKey;

    [SerializeField] private List<AudioEffect> _audioEffects;
    [SerializeField] private List<bool> _effectKeys;

    private ISoundProviderToSoundNode _sound;
    private TaskRunner _taskRunner;
    private bool _startedPlayMusicPlayMusic;
    private bool _startedPlayAmbient;
    public ISoundProviderToSoundNode Sound => _sound;

    public bool StartedPlayMusic => _startedPlayMusicPlayMusic;
    public bool StartedPlayAmbient => _startedPlayAmbient;

    public void ConstructMySoundNode(TaskRunner taskRunner, ISoundProviderToSoundNode sound)
    {
        _sound = sound;
        _startedPlayMusicPlayMusic = false;
        _startedPlayAmbient = false;
        _taskRunner = taskRunner;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        ChangeSound(_smoothMusicTransitionKey, _isMusicSmoothVolumeIncrease, _isMusicSmoothVolumeDecrease, _currentMusicSoundKey,
            AudioSourceType.Music);
        ChangeSound(_smoothTransitionKeyAmbientSound, _isSmoothVolumeIncreaseAmbientSound, _isSmoothVolumeDecreaseAmbientSound,
            _currentAmbientSoundKey, AudioSourceType.Ambient);
        TryPushEffects();
        _taskRunner.AddOperationToList(()=> _sound.SmoothAudio.TryDoQueue());
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

    public override async UniTask Exit()
    {
        _taskRunner = null;
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

    private void ChangeSound(bool smoothTransitionKey, bool isSmoothVolumeIncrease, bool isSmoothVolumeDecrease,
        string currentSoundKey, AudioSourceType audioSourceType)
    {
        if (smoothTransitionKey == true )
        {
            _sound.SmoothAudio.AddToQueueSmoothReplacementAudio(CancellationTokenSource.Token, currentSoundKey, audioSourceType);
        }
        else if (isSmoothVolumeIncrease == true)
        {
            _sound.SmoothAudio.AddToQueueSmoothPlayAudio(CancellationTokenSource.Token, currentSoundKey, audioSourceType);
        }
        else if (isSmoothVolumeDecrease == true)
        {
            _sound.SmoothAudio.AddToQueueSmoothStopAudio(CancellationTokenSource.Token, audioSourceType);
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

    private void PlayMusicAudio()
    {
        _startedPlayMusicPlayMusic = true;
        _sound.PlayAudioByKey(_currentMusicSoundKey, AudioSourceType.Music);
    }

    private void PlayAmbientAudio()
    {
        _startedPlayAmbient = true;
        _sound.PlayAudioByKey(_currentAmbientSoundKey, AudioSourceType.Ambient);
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
}