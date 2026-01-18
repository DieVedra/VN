using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GlobalSound : Sound
{
    public void SetAudioClipProvider(AudioClipProvider audioClipProvider)
    {
        audioClipProvider.OnLoadMusicAudioData.Subscribe(_ =>
        {
            Add(_.Clips, MusicDictionary);
        });
        audioClipProvider.OnLoadAmbientAudioData.Subscribe(_ =>
        {
            Add(_.Clips, AmbientDictionary);
        });
    }

    public void FillStoryDataToSave(StoryData storyData)
    {
        storyData.AudioEffectsIsOn.Clear();
        storyData.CurrentAudioMusicKey = CurrentMusicClipKey;
        storyData.CurrentAudioAmbientKey = CurrentAdditionalClipKey;
        
        Debug.Log($"2552535  {CurrentMusicClipKey}   {CurrentAdditionalClipKey}");

        var effects = AudioEffectsCustodian.GetEnableEffectsToSave();
        if (effects != null)
        {
            storyData.AudioEffectsIsOn.AddRange(effects);
        }
    }
    public async UniTask TryPlayOnLoadSave()
    {
        await SmoothAudio.TryDoQueue();
        
    }
    public void SetGlobalSoundData(GlobalAudioData globalAudioData)
    {
        GlobalAudioData = globalAudioData;
    }
    private void Add(IReadOnlyList<AudioClip> fromClips, Dictionary<string, AudioClip> targetDictionary)
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
