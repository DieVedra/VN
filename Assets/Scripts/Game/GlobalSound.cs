using System.Collections.Generic;
using UnityEngine;

public class GlobalSound : Sound
{
    private AudioClipProvider _audioClipProvider;
    public void Construct(bool soundOn)
    {
        Init(soundOn);
    }

    public void SetAudioClipProvider(AudioClipProvider audioClipProvider)
    {
        _audioClipProvider = audioClipProvider;
        audioClipProvider.OnLoadData += AddContentToDictionary;
    }
    public override void Shutdown()
    {
        _audioClipProvider.OnLoadData -= AddContentToDictionary;
        base.Shutdown();
    }
    public void SetGlobalSoundData(GlobalAudioData globalAudioData)
    {
        GlobalAudioData = globalAudioData;
    }

    private void AddContentToDictionary(AudioSourceType type, IReadOnlyList<AudioClip> clips)
    {
        switch (type)
        {
            case AudioSourceType.Music:
                Add(clips, MusicDictionary);
                break;
            case AudioSourceType.Ambient:
                Add(clips, AmbientDictionary);
                break;
        }

        void Add(IReadOnlyList<AudioClip> fromClips, Dictionary<string, AudioClip> targetDictionary)
        {
            foreach (var clip in fromClips)
            {
                if (targetDictionary.ContainsKey(clip.name) == false)
                {
                    targetDictionary.Add(clip.name, clip);
                }
            }
        }
    }
}
