
using System.Collections.Generic;
using UnityEngine;

public class ClipProvider
{
    private readonly List<AudioClip> _musicAudioData;
    private readonly List<AudioClip> _ambientAudioData;

    public ClipProvider(ref List<AudioClip> musicAudioData, ref List<AudioClip> ambientAudioData)
    {
        _musicAudioData = musicAudioData;
        _ambientAudioData = ambientAudioData;
    }

    public AudioClip GetClip(AudioSourceType audioSourceType, int secondAudioClipIndex)
    {
        AudioClip clip = null;
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                
                Debug.Log($"_musicAudioData {_musicAudioData.Count} secondAudioClipIndex {secondAudioClipIndex}");
                clip = _musicAudioData[secondAudioClipIndex];
                
                
                break;
            case AudioSourceType.Ambient:
                
                Debug.Log($"_ambientAudioData {_ambientAudioData.Count} secondAudioClipIndex {secondAudioClipIndex}");


                clip = _ambientAudioData[secondAudioClipIndex];
                break;
        }
        return clip;
    }
}