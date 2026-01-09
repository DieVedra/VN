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
        _musicAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        _ambientAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        if (await _musicAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            OnLoadData?.Invoke(AudioSourceType.Music, _musicAudioDataProvider.GetDatas[nextSeriaNameAssetIndex].Clips);
        }
        if (await _ambientAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            OnLoadData?.Invoke(AudioSourceType.Ambient, _ambientAudioDataProvider.GetDatas[nextSeriaNameAssetIndex].Clips);
        }
    }
}