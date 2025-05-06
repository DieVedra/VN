using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LevelSoundEditMode : Sound
{
    [SerializeField, Expandable] private List<AudioData> _musicAudioData;
    [SerializeField, Expandable] private List<AudioData> _ambientAudioData;
    public void Construct(bool soundOn)
    {
        Init(soundOn);
        MusicAudioData = new List<AudioClip>();
        AmbientAudioData = new List<AudioClip>();
        SetAudioDatas(_musicAudioData, _ambientAudioData);
    }
    private void SetAudioDatas(List<AudioData> musicClips, List<AudioData> ambientClips)
    {
        for (int i = 0; i < musicClips.Count; ++i)
        {
            AddClips(MusicAudioData, musicClips[i].Clips);
        }
    
        for (int i = 0; i < ambientClips.Count; i++)
        {
            AddClips(AmbientAudioData, ambientClips[i].Clips);
    
        }
    }
    
    private void AddClips(List<AudioClip> musicAudioData, IReadOnlyList<AudioClip> clips)
    {
        for (int i = 0; i < clips.Count; i++)
        {
            musicAudioData.Add(clips[i]);
        }
    }
}