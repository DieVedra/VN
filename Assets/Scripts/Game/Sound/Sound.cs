
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource1;
    [SerializeField] private AudioSource _audioSource2;

    [SerializeField] protected AudioMixer _mixer;
    private readonly float _duration = 1f;
    private readonly float _stopEndValue = 0f;
    private readonly float _playEndValue = 1f;
    private AudioEffectsCustodian _audioEffectsCustodian;
    private ReactiveCommand _playEvent;
    private AudioSource[] _audioSources;
    [SerializeField, ReadOnly] protected List<AudioClip> AudioData;
    [SerializeField, ReadOnly] protected List<AudioClip> AdditionalAudioData;
    [SerializeField, Expandable, ReadOnly] protected GlobalAudioData GlobalAudioData;
    
    public float PlayTime => _audioSource1.time;
    public float CurrentClipTime => _audioSource1.clip == null ? 0f : _audioSource1.clip.length;
    public int CurrentClipIndex { get; private set; }
    public int CurrentAdditionalClipIndex { get; private set; }
    public SmoothAudio SmoothAudio { get; private set; }

    public AudioEffectsCustodian AudioEffectsCustodian => _audioEffectsCustodian;
    public IReadOnlyList<AudioClip> Clips => AudioData;
    public IReadOnlyList<AudioClip> AdditionalClips => AdditionalAudioData;
    public ReactiveCommand PlayEvent => _playEvent;
    public virtual void Init(bool soundOn = true)
    {
        _audioSources = new[] {_audioSource1, _audioSource2};
        AudioData = new List<AudioClip>();
        if (soundOn == true)
        {
            _audioSource1.mute = false;
        }
        else
        {
            _audioSource1.mute = true;
        }
        SmoothAudio = new SmoothAudio(_audioSources, AudioData, AdditionalAudioData);
        _audioEffectsCustodian = new AudioEffectsCustodian(_mixer);
        _playEvent = new ReactiveCommand();
    }

    public void SetGlobalSoundData(GlobalAudioData globalAudioData)
    {
        GlobalAudioData = globalAudioData;
    }
    public void SetPlayTime(float time)
    {
        _audioSource1.time = time;
    }

    public void PlayAudioByIndex(int audioClipIndex, int audioSourceIndex = 0)
    {
        CurrentClipIndex = audioClipIndex;
        PlayAudioByClip(AudioData[audioClipIndex], audioSourceIndex);
    }
    public void SetVolume(float volume, int audioSourceIndex = 0)
    {
        _audioSources[audioSourceIndex].volume = volume;
    }
    public void PlayAudioByClip(AudioClip clip, int audioSourceIndex = 0)
    {
        // _playEvent.Execute();
        _audioSources[audioSourceIndex].clip = clip;

        _audioSources[audioSourceIndex].Play();
    }
    public void StopAudio()
    {
        _audioSource1.Stop();
        _audioSource2.Stop();
        SetPlayTime(0f);
    }

    public async UniTask SmoothPlayWardrobeAudio(CancellationToken cancellationToken)
    {
        _audioSource1.volume = 0f;
        PlayAudioByClip(GlobalAudioData.GetWardrobeAudioClip);
        await _audioSource1.DOFade(_playEndValue, _duration).WithCancellation(cancellationToken);
    }
    public void SetAudioDatas(List<AudioData> clips, List<AudioData> additionalClips)
    {
        AudioData = clips.SelectMany(x=>x.Clips).ToList();
        AdditionalAudioData = additionalClips.SelectMany(x=>x.Clips).ToList();
    }
}