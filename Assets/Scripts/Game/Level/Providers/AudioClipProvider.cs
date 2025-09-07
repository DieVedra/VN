using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioClipProvider
{
    private const string _nameMusicAsset = "MusicAudioDataSeria";
    private const string _nameAmbientAsset = "AmbientAudioDataSeria";
    private readonly DataProvider<AudioData> _musicAudioDataProvider;
    private readonly DataProvider<AudioData> _ambientAudioDataProvider;
    public readonly List<AudioClip> MusicAudioData;
    public readonly List<AudioClip> AmbientAudioData;
    public IParticipiteInLoad MusicAudioDataProviderParticipiteInLoad => _musicAudioDataProvider;
    public IParticipiteInLoad AmbientAudioDataProviderParticipiteInLoad => _ambientAudioDataProvider;

    public AudioClipProvider()
    {
        _musicAudioDataProvider = new DataProvider<AudioData>();
        _ambientAudioDataProvider = new DataProvider<AudioData>();
        MusicAudioData = new List<AudioClip>();
        AmbientAudioData = new List<AudioClip>();
    }

    public async UniTask Init()
    {
        await UniTask.WhenAll(_musicAudioDataProvider.CreateNames(_nameMusicAsset),
            _ambientAudioDataProvider.CreateNames(_nameAmbientAsset));
    }
    public void Dispose()
    {
        _musicAudioDataProvider.Dispose();
        _ambientAudioDataProvider.Dispose();
    }
    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        _musicAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        _ambientAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }
    public AudioClip GetClip(AudioSourceType audioSourceType, int secondAudioClipIndex)
    {
        AudioClip clip = null;
        Debug.Log($"GetClip {MusicAudioData[secondAudioClipIndex]}   {AmbientAudioData[secondAudioClipIndex]}");

        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                
                clip = MusicAudioData[secondAudioClipIndex];
                break;
            case AudioSourceType.Ambient:
                
                clip = AmbientAudioData[secondAudioClipIndex];
                break;
        }
        return clip;
    }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        if (await _musicAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            for (int i = 0; i < _musicAudioDataProvider.Datas[nextSeriaNameAssetIndex].Clips.Count; i++)
            {
                await UniTask.Yield();
                MusicAudioData.Add(_musicAudioDataProvider.Datas[nextSeriaNameAssetIndex].Clips[i]);
            }
            
        }
        if (await _ambientAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            for (int i = 0; i < _ambientAudioDataProvider.Datas[nextSeriaNameAssetIndex].Clips.Count; i++)
            {
                await UniTask.Yield();
                AmbientAudioData.Add(_ambientAudioDataProvider.Datas[nextSeriaNameAssetIndex].Clips[i]);
            }
        }
    }
}