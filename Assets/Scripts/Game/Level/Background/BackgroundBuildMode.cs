using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BackgroundBuildMode : Background
{
    private BackgroundDataProvider _backgroundDataProvider;

    public void Construct(BackgroundDataProvider backgroundDataProvider, ISetLighting setLighting, BackgroundPool backgroundPool)
    {
        _backgroundDataProvider = backgroundDataProvider;
        BackgroundPool = backgroundPool;
        BackgroundContent1.Construct(setLighting, BackgroundPool);
        BackgroundContent2.Construct(setLighting, BackgroundPool);
        ColorOverlay.color = Color.clear;
        
        _backgroundDataProvider.OnLoadAdditionalImagesData.Subscribe(_ =>
        {
            AddContent(ref AdditionalImagesToBackgroundDictionary, _);
        });
        _backgroundDataProvider.OnLoadArtsData.Subscribe(_ =>
        {
            AddContent(ref ArtsSpritesDictionary, _);
        });
        _backgroundDataProvider.OnLoadLocationData.Subscribe(_ =>
        {
            AddContent(ref BackgroundContentValuesDictionary, _);
        });
        _backgroundDataProvider.OnLoadWardrobeData.Subscribe(_ =>
        {
            AddContent(ref WardrobeBackgroundContentValuesDictionary, _);
        });

        if (BackgroundSaveData != null)
        {
            TryAddAddebleContentToBackgroundContent(BackgroundSaveData.AdditionalImagesInfo);
        }
    }

    private void AddContent(ref Dictionary<string, BackgroundContentValues> dictionary, BackgroundData backgroundData)
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, BackgroundContentValues>();
        }

        foreach (var bcv in backgroundData.BackgroundContentValues)
        {
            dictionary.Add(bcv.NameBackground, bcv);
        }
    }
}