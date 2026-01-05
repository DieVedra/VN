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
        AddClips(_musicAudioData, MusicAudioData, MusicDictionary);
        AddClips(_ambientAudioData, AmbientAudioData, AmbientDictionary);
    }
    
    private void AddClips(List<AudioData> dataClips, List<AudioClip> target, Dictionary<string, AudioClip> targetDictionary)
    {
        foreach (var data in dataClips)
        {
            foreach (var clip in data.Clips)
            {
                target.Add(clip);
                if (targetDictionary.ContainsKey(clip.name) == false)
                {
                    targetDictionary.Add(clip.name, clip);
                }
            }
        }
    }
}