using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioClipProvider
{
    private const string _nameMusicAsset = "MusicAudioDataSeria";
    private const string _nameAmbientAsset = "AmbientAudioDataSeria";
    private readonly DataProvider<AudioData> _musicAudioDataProvider;
    private readonly DataProvider<AudioData> _ambientAudioDataProvider;
    public DataProvider<AudioData> MusicDataProvider => _musicAudioDataProvider;

    public IParticipiteInLoad MusicAudioDataProviderParticipiteInLoad => _musicAudioDataProvider;
    public IParticipiteInLoad AmbientAudioDataProviderParticipiteInLoad => _ambientAudioDataProvider;
    public event Action<AudioSourceType, IReadOnlyList<AudioClip>> OnLoadData;
    public AudioClipProvider()
    {
        _musicAudioDataProvider = new DataProvider<AudioData>();
        _ambientAudioDataProvider = new DataProvider<AudioData>();
    }

    public async UniTask Init()
    {
        await UniTask.WhenAll(_musicAudioDataProvider.CreateNames(_nameMusicAsset),
            _ambientAudioDataProvider.CreateNames(_nameAmbientAsset));
    }
    public void Shutdown()
    {
        _musicAudioDataProvider.Shutdown();
        _ambientAudioDataProvider.Shutdown();
    }
    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        Debug.Log($"++++++++++++");

        _musicAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        Debug.Log($"----------------");

        _ambientAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }
    // public AudioClip GetClip(AudioSourceType audioSourceType, int secondAudioClipIndex)
    // {
    //     AudioClip clip = null;
    //     Debug.Log($"GetClip {MusicAudioData[secondAudioClipIndex]}   {AmbientAudioData[secondAudioClipIndex]}");
    //
    //     switch (audioSourceType)
    //     {
    //         case AudioSourceType.Music:
    //             
    //             clip = MusicAudioData[secondAudioClipIndex];
    //             break;
    //         case AudioSourceType.Ambient:
    //             
    //             clip = AmbientAudioData[secondAudioClipIndex];
    //             break;
    //     }
    //     return clip;
    // }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        if (await _musicAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            OnLoadData?.Invoke(AudioSourceType.Music, _musicAudioDataProvider.GetDatas[nextSeriaNameAssetIndex].Clips);
            // foreach (var t in _musicAudioDataProvider.GetDatas[nextSeriaNameAssetIndex].Clips)
            // {
            //     await UniTask.Yield();
            //     MusicAudioData.Add(t);
            //     MusicAudioDataDictionary.Add(t.name, t);
            // }
        }
        if (await _ambientAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            OnLoadData?.Invoke(AudioSourceType.Ambient, _musicAudioDataProvider.GetDatas[nextSeriaNameAssetIndex].Clips);
            // foreach (var t in _ambientAudioDataProvider.GetDatas[nextSeriaNameAssetIndex].Clips)
            // {
            //     await UniTask.Yield();
            //     AmbientAudioData.Add(t);
            //     AmbientAudioDataDictionary.Add(t.name, t);
            // }
        }
    }
}