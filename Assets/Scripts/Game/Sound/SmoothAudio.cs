﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class SmoothAudio
{
    private const float _duration = 1f;
    private const float _stopEndValue = 0f;
    private const float _playEndValue = 1f;
    private readonly Dictionary<AudioSourceType, AudioSource> _audioSources;
    private readonly IReadOnlyList<AudioClip> _musicAudioData;
    private readonly IReadOnlyList<AudioClip> _ambientAudioData;
    public SmoothAudio(Dictionary<AudioSourceType, AudioSource> audioSources,
        IReadOnlyList<AudioClip> musicAudioData, IReadOnlyList<AudioClip> ambientAudioData)
    {
        _audioSources = audioSources;
        _musicAudioData = musicAudioData;
        _ambientAudioData = ambientAudioData;
    }

    public async UniTask SmoothReplacementAudio(CancellationToken cancellationToken, int secondAudioClipIndex, AudioSourceType audioSourceType)
    {
        await SmoothStopAudio(cancellationToken, audioSourceType);
        await SmoothPlayAudio(cancellationToken, secondAudioClipIndex, audioSourceType);
    }
    public async UniTask SmoothPlayAudio(CancellationToken cancellationToken, int secondAudioClipIndex, AudioSourceType audioSourceType)
    {
        MinVolume(audioSourceType);
        if (audioSourceType == AudioSourceType.Music)
        {
            SetClip(_musicAudioData[secondAudioClipIndex], audioSourceType);
        }
        else
        {
            SetClip(_ambientAudioData[secondAudioClipIndex], audioSourceType);
        }
        _audioSources[audioSourceType].clip = (int)audioSourceType == 0 ? _musicAudioData[secondAudioClipIndex] : _ambientAudioData[secondAudioClipIndex];
        _audioSources[audioSourceType].Play();
        await Fade(cancellationToken, _playEndValue, _duration, audioSourceType);
    }
    public async UniTask SmoothStopAudio(CancellationToken cancellationToken, AudioSourceType audioSourceType)
    {
        if (_audioSources[audioSourceType].clip != null)
        {
            await Fade(cancellationToken, _stopEndValue, _duration, audioSourceType);
            _audioSources[audioSourceType].clip = null;
        }
    }
    public async UniTask SmoothPlayAudio(AudioClip audioClip, CancellationToken cancellationToken)
    {
        MinVolume(AudioSourceType.Music);
        SetClip(audioClip, AudioSourceType.Music);
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

    private void SetClip(AudioClip audioClip, AudioSourceType audioSourceType)
    {
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                _audioSources[audioSourceType].clip = audioClip;
                break;
            case AudioSourceType.Ambient:
                _audioSources[audioSourceType].clip = audioClip;
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