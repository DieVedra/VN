using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BackgroundBuildMode : Background
{
    public void SubscribeProviders(BackgroundDataProvider backgroundDataProvider)
    {
        backgroundDataProvider.OnLoadAdditionalImagesData.Subscribe(_ =>
        {
            AddContent(ref AdditionalImagesToBackgroundDictionary, _);
        });
        backgroundDataProvider.OnLoadArtsData.Subscribe(_ =>
        {
            AddContent(ref ArtsSpritesDictionary, _);
        });
        backgroundDataProvider.OnLoadLocationData.Subscribe(_ =>
        {
            AddContent(ref BackgroundContentValuesDictionary, _);
        });
        backgroundDataProvider.OnLoadWardrobeData.Subscribe(_ =>
        {
            AddContent(ref WardrobeBackgroundContentValuesDictionary, _);
        });
    }
    public void Construct(ISetLighting setLighting, BackgroundPool backgroundPool)
    {
        BackgroundPool = backgroundPool;
        BackgroundContent1.Construct(setLighting, BackgroundPool);
        BackgroundContent2.Construct(setLighting, BackgroundPool);
        ColorOverlay.color = Color.clear;
        
        if (BackgroundSaveData != null)
        {
            SetBackgroundPosition((BackgroundPosition)BackgroundSaveData.CurrentBackgroundPosition, BackgroundSaveData.CurrentKeyBackgroundContent);
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