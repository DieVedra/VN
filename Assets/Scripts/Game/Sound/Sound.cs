
using System;
using System.Collections.Generic;
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
    private AudioEffectsHandler _audioEffectsHandler;
    private ReactiveCommand _playEvent;
    private AudioSource[] _audioSources;
    [SerializeField, ReadOnly] protected List<AudioClip> AudioData;
    [SerializeField, ReadOnly] protected List<AudioClip> AdditionalAudioData;
    [SerializeField, ReadOnly] protected GlobalAudioData GlobalAudioData;
    
    public float PlayTime => _audioSource1.time;
    public float CurrentClipTime => _audioSource1.clip == null ? 0f : _audioSource1.clip.length;
    public int CurrentClipIndex { get; private set; }
    public int CurrentAdditionalClipIndex { get; private set; }
    public AudioEffectsHandler AudioEffectsHandler => _audioEffectsHandler;
    public IReadOnlyList<AudioClip> Clips => AudioData;
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
        _audioEffectsHandler = new AudioEffectsHandler(_mixer);
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

    public void PlayAudioByIndex(int audioClipIndex)
    {
        CurrentClipIndex = audioClipIndex;
        PlayAudioByClip(AudioData[audioClipIndex]);
    }
    
    public void PlayAdditionalAudioByIndex(int audioClipIndex)
    {
        CurrentAdditionalClipIndex = audioClipIndex;
        PlayAudioByClip(AdditionalAudioData[audioClipIndex], 1);
    }

    public void SetVolume(float volume, int audioSourceIndex = 0)
    {
        _audioSources[audioSourceIndex].volume = volume;
    }
    public void PlayAudioByClip(AudioClip clip, int audioSourceIndex = 0)
    {
        _playEvent.Execute();
        _audioSources[audioSourceIndex].clip = clip;
        _audioSources[audioSourceIndex].Play();
    }
    public void StopAudio()
    {
        _audioSource1.Stop();
        _audioSource2.Stop();
        SetPlayTime(0f);
    }
    public async UniTask SmoothStopAudio(CancellationToken cancellationToken, Action effectsOperation)
    {
        if (_audioSource1.clip != null)
        {
            await _audioSource1.DOFade(_stopEndValue, _duration).WithCancellation(cancellationToken);
            effectsOperation?.Invoke();
            _audioSource1.clip = null;
        }
    }
    public async UniTask SmoothPlayAudio(CancellationToken cancellationToken, Action effectsOperation = null, int secondAudioClipIndex = 0)
    {
        _audioSource1.volume = 0f;
        PlayAudioByIndex(secondAudioClipIndex);
        await _audioSource1.DOFade(_playEndValue, _duration).WithCancellation(cancellationToken);
        effectsOperation?.Invoke();
    }
    public async UniTask SmoothReplacementAudio(CancellationToken cancellationToken, Action effectsOperation, int secondAudioClipIndex)
    {
        await SmoothStopAudio(cancellationToken, effectsOperation);
        effectsOperation?.Invoke();
        await SmoothPlayAudio(cancellationToken, effectsOperation, secondAudioClipIndex);
    }

    public async UniTask SmoothPlayWardrobeAudio(CancellationToken cancellationToken)
    {
        _audioSource1.volume = 0f;
        PlayAudioByClip(GlobalAudioData.GetWardrobeAudioClip);
        await _audioSource1.DOFade(_playEndValue, _duration).WithCancellation(cancellationToken);
    }
    protected void AddAudioClips(IReadOnlyList<AudioClip> clips)
    {
        for (int i = 0; i < clips.Count; ++i)
        {
            AudioData.Add(clips[i]);
        }
    }
}