
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SmoothAudio
{
    private readonly float _duration = 1f;
    private readonly float _stopEndValue = 0f;
    private readonly float _playEndValue = 1f;
    private readonly AudioSource[] _audioSources;
    private readonly IReadOnlyList<AudioClip> _audioData;
    private readonly IReadOnlyList<AudioClip> _additionalAudioData;

    public event Action<AudioClip, int> OnPlayAudioByClip;
    public SmoothAudio(AudioSource[] audioSources, IReadOnlyList<AudioClip> audioData, IReadOnlyList<AudioClip> additionalAudioData)
    {
        _audioSources = audioSources;
        _audioData = audioData;
        _additionalAudioData = additionalAudioData;
    }

    public async UniTask SmoothReplacementAudio(CancellationToken cancellationToken, int secondAudioClipIndex, int audioSourceIndex)
    {
        await SmoothStopAudio(cancellationToken, audioSourceIndex);
        await SmoothPlayAudio(cancellationToken, secondAudioClipIndex, audioSourceIndex);
    }
    public async UniTask SmoothPlayAudio(CancellationToken cancellationToken, int secondAudioClipIndex, int audioSourceIndex)
    {
        _audioSources[audioSourceIndex].volume = 0f;
        _audioSources[audioSourceIndex].clip = audioSourceIndex == 0 ? _audioData[secondAudioClipIndex] : _additionalAudioData[secondAudioClipIndex];
        _audioSources[audioSourceIndex].Play();
        await _audioSources[audioSourceIndex].DOFade(_playEndValue, _duration).WithCancellation(cancellationToken);
    }
    public async UniTask SmoothStopAudio(CancellationToken cancellationToken, int audioSourceIndex = 0)
    {
        if (_audioSources[audioSourceIndex].clip != null)
        {
            await _audioSources[audioSourceIndex].DOFade(_stopEndValue, _duration).WithCancellation(cancellationToken);
            _audioSources[audioSourceIndex].clip = null;
        }
    }
}