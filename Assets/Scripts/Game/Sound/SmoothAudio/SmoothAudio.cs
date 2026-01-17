using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class SmoothAudio
{
    private const float _duration = 1f;
    private const float _stopEndValue = 0f;
    private const float _playEndValue = 1f;
    private readonly ReactiveProperty<string> _currentMusicClipKeyRP;
    private readonly ReactiveProperty<string> _currentAdditionalClipKeyRP;
    private readonly Dictionary<AudioSourceType, AudioSource> _audioSources;
    private readonly IReadOnlyDictionary<string, AudioClip> _musicAudioDictionary;
    private readonly IReadOnlyDictionary<string, AudioClip> _ambientAudioDictionary;
    private readonly Queue<Func<UniTask>> _queue;
    private bool _isRun;
    public SmoothAudio(ReactiveProperty<string> currentMusicClipKeyRP, ReactiveProperty<string> currentAdditionalClipKeyRP,
        Dictionary<AudioSourceType, AudioSource> audioSources,
        IReadOnlyDictionary<string, AudioClip> musicAudioDictionary,
        IReadOnlyDictionary<string, AudioClip> ambientAudioDictionary)
    {
        _currentMusicClipKeyRP = currentMusicClipKeyRP;
        _currentAdditionalClipKeyRP = currentAdditionalClipKeyRP;
        _audioSources = audioSources;
        _musicAudioDictionary = musicAudioDictionary;
        _ambientAudioDictionary = ambientAudioDictionary;
        _queue = new Queue<Func<UniTask>>();
        _isRun = false;
    }

    public async UniTask TryDoQueue()
    {
        if (_isRun == false)
        {
            _isRun = true;
            while (_queue.Count > 0)
            {
                await _queue.Dequeue().Invoke();
            }
            _isRun = false;
        }
    }
    public void AddToQueueSmoothReplacementAudio(CancellationToken cancellationToken, string secondAudioClipKey, AudioSourceType audioSourceType)
    {
        _queue.Enqueue(()=> SmoothReplacementAudio(cancellationToken, secondAudioClipKey, audioSourceType));
    }
    public void AddToQueueSmoothPlayAudio(CancellationToken cancellationToken, string secondAudioClipKey, AudioSourceType audioSourceType)
    {
        _queue.Enqueue(()=> SmoothPlayAudio(cancellationToken, secondAudioClipKey, audioSourceType));
    }
    public void AddToQueueSmoothStopAudio(CancellationToken cancellationToken, AudioSourceType audioSourceType, string audioClipKey = null)
    {
        _queue.Enqueue(()=> SmoothStopAudio(cancellationToken, audioSourceType, audioClipKey));
    }
    public async UniTask SmoothReplacementAudio(CancellationToken cancellationToken, string secondAudioClipKey, AudioSourceType audioSourceType)
    {
        await SmoothStopAudio(cancellationToken, audioSourceType, secondAudioClipKey);
        await SmoothPlayAudio(cancellationToken, secondAudioClipKey, audioSourceType);
    }
    public async UniTask SmoothPlayAudio(CancellationToken cancellationToken, string secondAudioClipKey, AudioSourceType audioSourceType)
    {
        SetCurrentAudioKey(secondAudioClipKey, audioSourceType);
        MinVolume(audioSourceType);
        SetClip(secondAudioClipKey, audioSourceType);
        _audioSources[audioSourceType].Play();
        await Fade(cancellationToken, _playEndValue, _duration, audioSourceType);
    }
    public async UniTask SmoothStopAudio(CancellationToken cancellationToken, AudioSourceType audioSourceType, string audioClipKey = null)
    {
        SetCurrentAudioKey(audioClipKey, audioSourceType);
        if (_audioSources[audioSourceType].clip != null)
        {
            await Fade(cancellationToken, _stopEndValue, _duration, audioSourceType);
            _audioSources[audioSourceType].clip = null;
        }
    }
    public async UniTask SmoothPlayAudio(AudioClip audioClip, CancellationToken cancellationToken)
    {
        MinVolume(AudioSourceType.Music);
        _audioSources[AudioSourceType.Music].clip = audioClip;
        _audioSources[AudioSourceType.Music].Play();
        await Fade(cancellationToken, _playEndValue, _duration, AudioSourceType.Music);
    }
    public async UniTask SmoothVolumeChange(CancellationToken cancellationToken, float volume, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                await Fade(cancellationToken, volume, CalculateDuration(_audioSources[audioSourceType].volume, volume), audioSourceType);
                break;
            case AudioSourceType.Ambient:
                await Fade(cancellationToken, volume, CalculateDuration(_audioSources[audioSourceType].volume, volume), audioSourceType);
                break;
        }
    }

    private bool CheckVolume(AudioSource audioSource, float volume)
    {
        if ((Math.Abs(audioSource.volume - volume) > 0.05f) == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private async UniTask Fade(CancellationToken cancellationToken, float endValue, float duration, AudioSourceType audioSourceType)
    {
        await _audioSources[audioSourceType].DOFade(endValue, duration).WithCancellation(cancellationToken);
    }

    private void SetClip(string secondAudioClipKey, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                if (_musicAudioDictionary.TryGetValue(secondAudioClipKey, out var musicClip))
                {
                    _audioSources[audioSourceType].clip = musicClip;
                }
                break;
            case AudioSourceType.Ambient:
                if (_ambientAudioDictionary.TryGetValue(secondAudioClipKey, out var ambientClip))
                {
                    _audioSources[audioSourceType].clip = ambientClip;
                }
                break;
        }
    }

    private void SetCurrentAudioKey(string audioClipKey, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                _currentMusicClipKeyRP.Value = audioClipKey;
                break;
            case AudioSourceType.Ambient:
                _currentAdditionalClipKeyRP.Value = audioClipKey;
                break;
        }

    }
    private void MinVolume(AudioSourceType audioSourceType)
    {
        _audioSources[audioSourceType].volume = 0f;
    }

    private float CalculateDuration(float currentVolume, float targetVolume)
    {
        float section = Math.Abs(currentVolume - targetVolume);
        return Mathf.Lerp(0f, _duration, section);
    }
}