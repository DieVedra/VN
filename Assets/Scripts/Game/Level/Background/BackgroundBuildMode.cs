using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BackgroundBuildMode : Background
{
    public void SubscribeProviders(BackgroundDataProvider backgroundDataProvider)
    {
        Debug.Log($"SubscribeProviders");

        backgroundDataProvider.OnLoadAdditionalImagesData.Subscribe(_ =>
        {
            Debug.Log($"OnLoadAdditionalImagesData {_.BackgroundContentValues.Count}");

            AddContent(ref AdditionalImagesToBackgroundDictionary, _);
        });
        backgroundDataProvider.OnLoadArtsData.Subscribe(_ =>
        {
            Debug.Log($"OnLoadArtsData  {_.BackgroundContentValues.Count}");
            AddContent(ref ArtsSpritesDictionary, _);
            Debug.Log($"ArtsSpritesDictionary  {ArtsSpritesDictionary.Count}");

        });
        backgroundDataProvider.OnLoadLocationData.Subscribe(_ =>
        {
            Debug.Log($"OnLoadLocationData {_.BackgroundContentValues.Count}");

            AddContent(ref BackgroundContentValuesDictionary, _);
        });
        backgroundDataProvider.OnLoadWardrobeData.Subscribe(_ =>
        {
            Debug.Log($"OnLoadWardrobeData {_.BackgroundContentValues.Count}");

            AddContent(ref WardrobeBackgroundContentValuesDictionary, _);
        });
    }
    public void Construct(ISetLighting setLighting, BackgroundPool backgroundPool)
    {
        BackgroundTaskRunner = new NewTaskRunner();
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
            if (dictionary.ContainsKey(bcv.NameBackground) == false)
            {
                dictionary.Add(bcv.NameBackground, bcv);
            }
        }
    }
}