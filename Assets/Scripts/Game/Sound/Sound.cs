
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceMusic;
    [SerializeField] private AudioSource _audioSourceAmbient;

    [SerializeField] protected AudioMixer _mixer;
    private AudioEffectsCustodian _audioEffectsCustodian;
    private AudioClipProvider _audioClipProvider;

    private Dictionary <AudioSourceType, AudioSource>  _audioSources;
    
    [SerializeField, ReadOnly] protected List<AudioClip> MusicAudioData;
    [SerializeField, ReadOnly] protected List<AudioClip> AmbientAudioData;
    
    [SerializeField, Expandable] protected GlobalAudioData GlobalAudioData;
    
    public float PlayTimeMusic => _audioSourceMusic.time;
    public float PlayTimeAmbient => _audioSourceAmbient.time;
    public float VolumeMusic => _audioSourceMusic.volume;
    public float VolumeAmbient => _audioSourceAmbient.volume;

    public float CurrentMusicClipTime => _audioSourceMusic.clip == null ? 0f : _audioSourceMusic.clip.length;
    public float CurrentAmbientClipTime => _audioSourceAmbient.clip == null ? 0f : _audioSourceAmbient.clip.length;
    public int CurrentMusicClipIndex { get; private set; }
    public int CurrentAdditionalClipIndex { get; private set; }
    public SmoothAudio SmoothAudio { get; private set; }

    public AudioEffectsCustodian AudioEffectsCustodian => _audioEffectsCustodian;
    public IReadOnlyList<AudioClip> Clips => MusicAudioData;
    public IReadOnlyList<AudioClip> AmbientClips => AmbientAudioData;
    protected void Init(bool soundOn = true)
    {
        _audioSources = new Dictionary<AudioSourceType, AudioSource>()
        {
            {AudioSourceType.Music, _audioSourceMusic},
            {AudioSourceType.Ambient, _audioSourceAmbient}
        };
        if (soundOn == true)
        {
            _audioSourceMusic.mute = false;
        }
        else
        {
            _audioSourceMusic.mute = true;
        }
        // _audioClipProvider = new AudioClipProvider(ref MusicAudioData, ref AmbientAudioData);
        SmoothAudio = new SmoothAudio(_audioSources, _audioClipProvider);
        _audioEffectsCustodian = new AudioEffectsCustodian(_mixer);
    }

    public virtual void Dispose()
    {
        MusicAudioData = null;
        AmbientAudioData = null;
    }
    public void SetPlayTime(float time)
    {
        _audioSourceMusic.time = time;
    }

    public void PlayAudioByIndex(int audioClipIndex, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                CurrentMusicClipIndex = audioClipIndex;
                break;
            case AudioSourceType.Ambient:
                CurrentAdditionalClipIndex = audioClipIndex;
                break;
        }
        Debug.Log($"audioSourceType {audioSourceType}   audioClipIndex {audioClipIndex}");
        PlayAudioByClip(_audioClipProvider.GetClip(audioSourceType, audioClipIndex), audioSourceType);
    }
    public void SetVolume(float volume, AudioSourceType audioSourceType)
    {
        _audioSources[audioSourceType].volume = volume;
    }
    public void PlayAudioByClip(AudioClip clip, AudioSourceType audioSourceType)
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
        await SmoothAudio.SmoothPlayWardrobeAudio(GlobalAudioData.GetWardrobeAudioClip, cancellationToken);
    }
}