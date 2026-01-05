
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

    [SerializeField, ReadOnly] protected List<AudioClip> MusicAudioData;
    [SerializeField, ReadOnly] protected List<AudioClip> AmbientAudioData;

    [SerializeField, Expandable] protected GlobalAudioData GlobalAudioData;

    private AudioEffectsCustodian _audioEffectsCustodian;
    private ReactiveProperty<bool> _soundStatus;
    private Dictionary <AudioSourceType, AudioSource>  _audioSources;
    protected Dictionary<string, AudioClip> MusicDictionary;
    protected Dictionary<string, AudioClip> AmbientDictionary;
    public float PlayTimeMusic => _audioSourceMusic.time;
    public float PlayTimeAmbient => _audioSourceAmbient.time;
    public float VolumeMusic => _audioSourceMusic.volume;
    public float VolumeAmbient => _audioSourceAmbient.volume;

    public float CurrentMusicClipTime => _audioSourceMusic.clip == null ? 0f : _audioSourceMusic.clip.length;
    public float CurrentAmbientClipTime => _audioSourceAmbient.clip == null ? 0f : _audioSourceAmbient.clip.length;
    public int CurrentMusicClipIndex { get; private set; }
    public string CurrentMusicClipKey { get; private set; }
    public string CurrentAdditionalClipKey { get; private set; }
    public SmoothAudio SmoothAudio { get; private set; }

    public AudioEffectsCustodian AudioEffectsCustodian => _audioEffectsCustodian;
    public IReadOnlyList<AudioClip> Clips => MusicAudioData;
    public IReadOnlyList<AudioClip> AmbientClips => AmbientAudioData;
    public IReadOnlyDictionary<string, AudioClip> GetMusicDictionary => MusicDictionary;
    public IReadOnlyDictionary<string, AudioClip> GetAmbientDictionary => AmbientDictionary;
    public IReactiveProperty<bool> SoundStatus => _soundStatus;

    protected void Init(bool soundOn = true)
    {
        MusicDictionary = new Dictionary<string, AudioClip>();
        AmbientDictionary = new Dictionary<string, AudioClip>();
        _audioSources = new Dictionary<AudioSourceType, AudioSource>()
        {
            {AudioSourceType.Music, _audioSourceMusic},
            {AudioSourceType.Ambient, _audioSourceAmbient}
        };
        
        _soundStatus = new ReactiveProperty<bool>();
        _soundStatus.Subscribe(ChangeSound);
        _soundStatus.Value = soundOn;
        
        SmoothAudio = new SmoothAudio(_audioSources, MusicDictionary, AmbientDictionary);
        _audioEffectsCustodian = new AudioEffectsCustodian(_mixer);
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
    public virtual void Shutdown()
    {
        MusicAudioData = null;
        AmbientAudioData = null;
        
        MusicDictionary = null;
        AmbientDictionary = null;
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

    public void PlayAudioByIndex(string audioClipKey, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                if (MusicDictionary.TryGetValue(audioClipKey, out var clipMusic))
                {
                    CurrentMusicClipKey = audioClipKey;
                    PlayAudioByClip(clipMusic, audioSourceType);
                }
                break;
            case AudioSourceType.Ambient:
                if (AmbientDictionary.TryGetValue(audioClipKey, out var clipAmbient))
                {
                    CurrentAdditionalClipKey = audioClipKey;
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