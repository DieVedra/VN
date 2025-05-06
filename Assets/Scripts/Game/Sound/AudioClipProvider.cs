
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioClipProvider
{
    private const string _nameMusicAsset = "MusicAudioDataSeria";
    private const string _nameAmbientAsset = "AmbientAudioDataSeria";
    private readonly ScriptableObjectAssetLoader _scriptableObjectAssetLoader;
    private readonly AssetExistsHandler _assetExistsHandler;
    private List<string> _namesMusicAssets;
    private List<string> _namesAmbientAssets;
    public List<AudioClip> MusicAudioData { get; private set; }
    public List<AudioClip> AmbientAudioData { get; private set; }

    public AudioClipProvider()
    {
        _scriptableObjectAssetLoader = new ScriptableObjectAssetLoader();
        _assetExistsHandler = new AssetExistsHandler();
        MusicAudioData = new List<AudioClip>();
        AmbientAudioData = new List<AudioClip>();
    }
    public void Dispose()
    {
        _scriptableObjectAssetLoader.UnloadAll();
    }
    public AudioClip GetClip(AudioSourceType audioSourceType, int secondAudioClipIndex)
    {
        AudioClip clip = null;
        switch (audioSourceType)
        {
            case AudioSourceType.Music:
                
                Debug.Log($"MusicAudioData {MusicAudioData.Count} secondAudioClipIndex {secondAudioClipIndex}");
                clip = MusicAudioData[secondAudioClipIndex];
                
                
                break;
            case AudioSourceType.Ambient:
                
                Debug.Log($"AmbientAudioData {AmbientAudioData.Count} secondAudioClipIndex {secondAudioClipIndex}");


                clip = AmbientAudioData[secondAudioClipIndex];
                break;
        }
        return clip;
    }
    
    public async UniTask LoadFirstSeriaAudioContent()
    {
        var resultNames =  await UniTask.WhenAll(
            _assetExistsHandler.CheckExistsAssetsNames(_nameMusicAsset),
            _assetExistsHandler.CheckExistsAssetsNames(_nameAmbientAsset));
        _namesMusicAssets = resultNames.Item1;
        _namesAmbientAssets = resultNames.Item2;
        if (_namesMusicAssets.Count > 0 && _namesAmbientAssets.Count > 0)
        {
            var resultAudioData =  await UniTask.WhenAll(
                _scriptableObjectAssetLoader.Load<AudioData>(_namesMusicAssets[0]),
                _scriptableObjectAssetLoader.Load<AudioData>(_namesAmbientAssets[0]));
            MusicAudioData.AddRange(resultAudioData.Item1.Clips);
            AmbientAudioData.AddRange(resultAudioData.Item2.Clips);
            LoadOtherSeriaAudioContent(MusicAudioData, _namesMusicAssets).Forget();
            LoadOtherSeriaAudioContent(AmbientAudioData, _namesAmbientAssets).Forget();
        }
    }

    private async UniTaskVoid LoadOtherSeriaAudioContent(List<AudioClip> audioData, List<string> namesAssets)
    {
        for (int i = 1; i < namesAssets.Count; i++)
        {
            AudioData audioDataAsset = await _scriptableObjectAssetLoader.Load<AudioData>(namesAssets[i]);
            audioData.AddRange(audioDataAsset.Clips);
        }
    }
}