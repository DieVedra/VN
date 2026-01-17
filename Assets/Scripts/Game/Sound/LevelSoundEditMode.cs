using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelSoundEditMode : Sound
{
    [SerializeField, Expandable] private List<AudioData> _musicAudioData;
    [SerializeField, Expandable] private List<AudioData> _ambientAudioData;
     
    [SerializeField] private SerializedDictionary<string, AudioClip> MusicSerializedDictionary;
    [SerializeField] private SerializedDictionary<string, AudioClip> AmbientSerializedDictionary;
    public override void Construct(bool soundOn = true)
    {
        base.Construct(soundOn);
        AddClips(_musicAudioData, MusicDictionary);
        AddClips(_ambientAudioData, AmbientDictionary);
        MusicSerializedDictionary.AddRange(MusicDictionary);
        AmbientSerializedDictionary.AddRange(AmbientDictionary);
    }
    
    private void AddClips(List<AudioData> dataClips, Dictionary<string, AudioClip> targetDictionary)
    {
        foreach (var data in dataClips)
        {
            foreach (var clip in data.Clips)
            {
                if (targetDictionary.ContainsKey(clip.name) == false)
                {
                    targetDictionary.Add(clip.name, clip);
                }
            }
        }
    }
}