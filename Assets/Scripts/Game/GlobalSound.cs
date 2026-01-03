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
                foreach (var clip in clips)
                {
                    MusicDictionary.Add(clip.name, clip);
                }
                break;
            case AudioSourceType.Ambient:
                foreach (var clip in clips)
                {
                    AmbientDictionary.Add(clip.name, clip);
                }
                break;
        }
    }
}
