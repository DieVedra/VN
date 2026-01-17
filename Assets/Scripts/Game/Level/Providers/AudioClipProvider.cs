using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class AudioClipProvider
{
    private const string _nameMusicAsset = "MusicAudioDataSeria";
    private const string _nameAmbientAsset = "AmbientAudioDataSeria";
    private readonly DataProvider<AudioData> _musicAudioDataProvider;
    private readonly DataProvider<AudioData> _ambientAudioDataProvider;
    public IParticipiteInLoad MusicAudioDataProviderParticipiteInLoad => _musicAudioDataProvider;
    public IParticipiteInLoad AmbientAudioDataProviderParticipiteInLoad => _ambientAudioDataProvider;
    public IReactiveCommand<AudioData> OnLoadMusicAudioData => _musicAudioDataProvider.OnLoad;
    public IReactiveCommand<AudioData> OnLoadAmbientAudioData => _ambientAudioDataProvider.OnLoad;

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
        await _musicAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex);
        await _ambientAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex);
    }
}