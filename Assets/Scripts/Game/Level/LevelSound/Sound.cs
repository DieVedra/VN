
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
    private CancellationTokenSource _cancellationTokenSource;
    private AudioEffectsHandler _audioEffectsHandler;
    private ReactiveCommand _playEvent;
    protected AudioData AudioData;
    public float PlayTime => _audioSource1.time;
    public float CurrentClipTime => _audioSource1.clip == null ? 0f : _audioSource1.clip.length;
    public AudioEffectsHandler AudioEffectsHandler => _audioEffectsHandler;
    public IReadOnlyList<AudioClip> Clips => AudioData.Clips;
    public ReactiveCommand PlayEvent => _playEvent;
    public virtual void Init(bool soundOn = true)
    {
        _audioSource1.mute = soundOn;
        _audioEffectsHandler = new AudioEffectsHandler(_mixer);
        _playEvent = new ReactiveCommand();
    }
    public void SetPlayTime(float time)
    {
        _audioSource1.time = time;
    }

    public void PlayAudio(int audioClipIndex)
    {
        _playEvent.Execute();
        _audioSource1.clip = Clips[audioClipIndex];
        _audioSource1.Play();
    }
    public void StopAudio()
    {
        _audioSource1.Stop();
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
    public async UniTask SmoothPlayAudio(CancellationToken cancellationToken, Action effectsOperation, int secondAudioClipIndex = 0)
    {
        _audioSource1.volume = 0f;
        PlayAudio(secondAudioClipIndex);
        await _audioSource1.DOFade(_playEndValue, _duration).WithCancellation(cancellationToken);
        effectsOperation?.Invoke();
    }
    public async UniTask SmoothReplacementAudio(CancellationToken cancellationToken, Action effectsOperation, int secondAudioClipIndex)
    {
        await SmoothStopAudio(cancellationToken, effectsOperation);
        effectsOperation?.Invoke();
        await SmoothPlayAudio(cancellationToken, effectsOperation, secondAudioClipIndex);
    }
}