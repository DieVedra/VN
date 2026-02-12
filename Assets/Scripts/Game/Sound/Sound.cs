using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour, ISoundProviderToHeaderNode, ISoundProviderToSoundNode
{
    [SerializeField] private AudioSource _audioSourceMusic;
    [SerializeField] private AudioSource _audioSourceAmbient;

    [SerializeField] protected AudioMixer _mixer;
    [SerializeField, Expandable] protected GlobalAudioData GlobalAudioData;

    private AudioEffectsCustodian _audioEffectsCustodian;
    private ReactiveProperty<bool> _soundStatus;
    private ReactiveProperty<bool> _soundPause;
    private ReactiveProperty<string> _currentMusicClipKeyRP;
    private ReactiveProperty<string> _currentAdditionalClipKeyRP;
    private Dictionary <AudioSourceType, AudioSource>  _audioSources;
    protected Dictionary<string, AudioClip> MusicDictionary;
    protected Dictionary<string, AudioClip> AmbientDictionary;
    private CompositeDisposable _compositeDisposableClipsKeysRP;
    private CompositeDisposable _compositeDisposableSoundStatusRP;
    private CancellationTokenSource _cancellationTokenSource;
    public float PlayTimeMusic => _audioSourceMusic.time;
    public float PlayTimeAmbient => _audioSourceAmbient.time;
    public float VolumeMusic => _audioSourceMusic.volume;
    public float VolumeAmbient => _audioSourceAmbient.volume;

    public float CurrentMusicClipTime => _audioSourceMusic.clip == null ? 0f : _audioSourceMusic.clip.length;
    public float CurrentAmbientClipTime => _audioSourceAmbient.clip == null ? 0f : _audioSourceAmbient.clip.length;
    public string CurrentMusicClipKey => _currentMusicClipKeyRP.Value;
    public string CurrentAdditionalClipKey => _currentAdditionalClipKeyRP.Value;
    public SmoothAudio SmoothAudio { get; private set; }

    public AudioEffectsCustodian AudioEffectsCustodian => _audioEffectsCustodian;
    public IReadOnlyDictionary<string, AudioClip> GetMusicDictionary => MusicDictionary;
    public IReadOnlyDictionary<string, AudioClip> GetAmbientDictionary => AmbientDictionary;
    public IReactiveProperty<bool> SoundStatus => _soundStatus;

    public IReactiveProperty<bool> GetSoundPause()
    {
        if (_soundPause == null)
        {
            _soundPause = new ReactiveProperty<bool>(false);
            _soundPause.Subscribe(_ =>
                {
                    if (_ == true)
                    {
                        _audioSourceMusic.Pause();
                        _audioSourceAmbient.Pause();
                    }
                    else
                    {
                        _audioSourceMusic.Play();
                        _audioSourceAmbient.Play();
                    }
                }
                );
        }
        return _soundPause;
    }

    public virtual void Construct(bool soundOn = true)
    {
        MusicDictionary = new Dictionary<string, AudioClip>();
        AmbientDictionary = new Dictionary<string, AudioClip>();
        _audioSources = new Dictionary<AudioSourceType, AudioSource>()
        {
            {AudioSourceType.Music, _audioSourceMusic},
            {AudioSourceType.Ambient, _audioSourceAmbient}
        };
        _compositeDisposableSoundStatusRP = new CompositeDisposable();
        _soundStatus = new ReactiveProperty<bool>().AddTo(_compositeDisposableSoundStatusRP);
        _soundStatus.Subscribe(ChangeSound);
        _soundStatus.Value = soundOn;
        _audioEffectsCustodian = new AudioEffectsCustodian(_mixer);
    }

    public void Init(List<AudioEffect> enabledEffects = null, string currentMusicClipKey = null, string currentAdditionalClipKey = null)
    {
        _compositeDisposableClipsKeysRP = new CompositeDisposable();
        _cancellationTokenSource = new CancellationTokenSource();
        _currentMusicClipKeyRP = new ReactiveProperty<string>().AddTo(_compositeDisposableClipsKeysRP);
        _currentAdditionalClipKeyRP = new ReactiveProperty<string>().AddTo(_compositeDisposableClipsKeysRP);
        SmoothAudio = new SmoothAudio(_currentMusicClipKeyRP, _currentAdditionalClipKeyRP,
            _audioSources, MusicDictionary, AmbientDictionary);
        
        _currentMusicClipKeyRP.Value = currentMusicClipKey;
        if (currentMusicClipKey != null)
        {
            SmoothAudio.AddToQueueSmoothPlayAudio(_cancellationTokenSource.Token, currentMusicClipKey, AudioSourceType.Music);
        }
        
        _currentAdditionalClipKeyRP.Value = currentAdditionalClipKey;
        if (currentAdditionalClipKey != null)
        {
            SmoothAudio.AddToQueueSmoothPlayAudio(_cancellationTokenSource.Token, currentAdditionalClipKey, AudioSourceType.Ambient);
        }

        _audioEffectsCustodian.TryEnableEffectsFromSave(enabledEffects);
    }
    private void ChangeSound(bool key)
    {
        if (key == true)
        {
            _audioSourceMusic.mute = false;
            _audioSourceAmbient.mute = false;
        }
        else
        {
            _audioSourceMusic.mute = true;
            _audioSourceAmbient.mute = true;
        }
    }
    public void ShutdownFromLevel()
    {
        _compositeDisposableClipsKeysRP?.Clear();
        _cancellationTokenSource?.Cancel();
        MusicDictionary.Clear();
        AmbientDictionary.Clear();
    }
    public void ShutdownFromMenu()
    {
        _compositeDisposableSoundStatusRP?.Clear();
    }
    public void SetPlayTime(float time, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                _audioSourceMusic.time = time;
                break;
            case AudioSourceType.Ambient:
                _audioSourceAmbient.time = time;
                break;
        }
    }

    public void PlayAudioByKey(string audioClipKey, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                if (MusicDictionary.TryGetValue(audioClipKey, out var clipMusic))
                {
                    _currentMusicClipKeyRP.Value = audioClipKey;
                    PlayAudioByClip(clipMusic, audioSourceType);
                }
                break;
            case AudioSourceType.Ambient:
                if (AmbientDictionary.TryGetValue(audioClipKey, out var clipAmbient))
                {
                    _currentAdditionalClipKeyRP.Value = audioClipKey;
                    PlayAudioByClip(clipAmbient, audioSourceType);
                }
                break;
        }
    }
    public void SetVolume(float volume, AudioSourceType audioSourceType)
    {
        _audioSources[audioSourceType].volume = volume;
    }

    private void PlayAudioByClip(AudioClip clip, AudioSourceType audioSourceType)
    {
        _audioSources[audioSourceType].clip = clip;
        _audioSources[audioSourceType].Play();
    }
    public void StopAudio(AudioSourceType audioSourceType)
    {
        _audioSources[audioSourceType].Stop();
        _audioSources[audioSourceType].time = 0f;
    }

    public async UniTask SmoothPlayWardrobeAudio(CancellationToken cancellationToken)
    {
        await SmoothAudio.SmoothPlayAudio(GlobalAudioData.GetWardrobeAudioClip, cancellationToken);
    }
    public async UniTask SmoothPlayHeaderAudio(string audioClipKey, CancellationToken cancellationToken)
    {
        await SmoothAudio.SmoothPlayAudio(MusicDictionary[audioClipKey], cancellationToken);
    }
}