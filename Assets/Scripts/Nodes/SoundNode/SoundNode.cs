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
    private NewTaskRunner _taskRunner;
    private bool _startedPlayMusicPlayMusic;
    private bool _startedPlayAmbient;
    public ISoundProviderToSoundNode Sound => _sound;

    public bool StartedPlayMusic => _startedPlayMusicPlayMusic;
    public bool StartedPlayAmbient => _startedPlayAmbient;

    public void ConstructMySoundNode(ISoundProviderToSoundNode sound)
    {
        _sound = sound;
        _startedPlayMusicPlayMusic = false;
        _startedPlayAmbient = false;
        _taskRunner = sound.SmoothAudio.SoundTaskRunner;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        List<Func<UniTask>> operations = _taskRunner.GetFreeList();
        CancellationTokenSource = new CancellationTokenSource();
        ChangeSound(operations, _smoothMusicTransitionKey, _isMusicSmoothVolumeIncrease, _isMusicSmoothVolumeDecrease, _currentMusicSoundKey,
            AudioSourceType.Music);
        ChangeSound(operations, _smoothTransitionKeyAmbientSound, _isSmoothVolumeIncreaseAmbientSound, _isSmoothVolumeDecreaseAmbientSound,
            _currentAmbientSoundKey, AudioSourceType.Ambient);
        TryPushEffects(operations);
        _taskRunner.AddToQueue(operations, CancellationTokenSource);
        if (_isInstantNodeTransition == false)
        {
            await _taskRunner.TryRun();
        }
        else
        {
            _taskRunner.TryRun().Forget();
        }
        
        
        if (isMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }
    }

    private void TryPushEffects(List<Func<UniTask>> operations)
    {
        IAudioEffectHandler handler;
        for (int i = 0; i < _audioEffects.Count; i++)
        {
            handler = _sound.AudioEffectsCustodian.GetAudioEffect(_audioEffects[i]);
            if (_effectKeys[i] == true)
            {
                if (handler.EffectIsOn == false)
                {
                    var handler1 = handler;
                    operations.Add(()=> handler1.SetEffectSmooth(CancellationTokenSource.Token, true));
                }
            }
            else
            {
                if (handler.EffectIsOn == true)
                {
                    var handler1 = handler;
                    operations.Add(()=> handler1.SetEffectSmooth(CancellationTokenSource.Token, false));
                }
            }
        }
    }

    private void ChangeSound(List<Func<UniTask>> operations, bool smoothTransitionKey, bool isSmoothVolumeIncrease, bool isSmoothVolumeDecrease,
        string currentSoundKey, AudioSourceType audioSourceType)
    {
        if (smoothTransitionKey == true )
        {
            operations.Add(()=> _sound.SmoothAudio.SmoothReplacementAudio(CancellationTokenSource.Token, currentSoundKey, audioSourceType));
        }
        else if (isSmoothVolumeIncrease == true)
        {
            operations.Add(()=> _sound.SmoothAudio.SmoothPlayAudio(CancellationTokenSource.Token, currentSoundKey, audioSourceType));
        }
        else if (isSmoothVolumeDecrease == true)
        {
            operations.Add(()=> _sound.SmoothAudio.SmoothStopAudio(CancellationTokenSource.Token, audioSourceType));
        }

        if (CheckVolume(_sound.VolumeMusic, _volumeMusicSound))
        {
            operations.Add(()=> _sound.SmoothAudio.SmoothVolumeChange(CancellationTokenSource.Token, _volumeMusicSound, AudioSourceType.Music));
        }
        if (CheckVolume(_sound.VolumeAmbient, _volumeAmbientSound))
        {
            operations.Add(()=> _sound.SmoothAudio.SmoothVolumeChange(CancellationTokenSource.Token, _volumeAmbientSound, AudioSourceType.Ambient));
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