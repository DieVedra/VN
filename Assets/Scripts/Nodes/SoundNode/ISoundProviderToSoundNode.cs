
using System.Collections.Generic;
using UnityEngine;

public interface ISoundProviderToSoundNode
{
    public void PlayAudioByKey(string audioClipKey, AudioSourceType audioSourceType);
    public void StopAudio(AudioSourceType audioSourceType);
    public void SetVolume(float volume, AudioSourceType audioSourceType);
    public void SetPlayTime(float time, AudioSourceType audioSourceType);
    public AudioEffectsCustodian AudioEffectsCustodian { get; }
    public SmoothAudio SmoothAudio { get; }
    public float VolumeMusic { get; }
    public float VolumeAmbient { get; }
    public float PlayTimeMusic { get; }
    public float CurrentMusicClipTime { get; }
    public float CurrentAmbientClipTime { get; }
    public float PlayTimeAmbient { get; }
    public IReadOnlyDictionary<string, AudioClip> GetMusicDictionary { get; }
    public IReadOnlyDictionary<string, AudioClip> GetAmbientDictionary { get; }
}