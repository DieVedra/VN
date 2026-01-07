using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BackgroundBuildMode : Background
{
    private BackgroundDataProvider _backgroundDataProvider;
    private BackgroundContentCreator _backgroundContentCreator;

    public void Construct(BackgroundDataProvider backgroundDataProvider, BackgroundContentCreator backgroundContentCreator,
 ISetLighting setLighting, SpriteRendererCreatorBuild spriteRendererCreatorBuild)
    {
        _backgroundDataProvider = backgroundDataProvider;
        _backgroundContentCreator = backgroundContentCreator;
        SetLighting = setLighting;
        BackgroundContentAdditionalSpriteRendererCreator = spriteRendererCreatorBuild;
        SetArtShower();
        InitWardrobeBackground();
        
        InitContent(backgroundDataProvider);
        _backgroundContentCreator.OnCreateContent += InitLocations;
        _backgroundDataProvider.OnLoadAdditionalImagesData.Subscribe(InitAdditionalImages);
        _backgroundDataProvider.OnLoadArtsData.Subscribe(InitArts);
        
        if (BackgroundSaveData != null)
        {
            TryAddAddebleContentToBackgroundContent(BackgroundSaveData.BackgroundContentWithAdditionalImage);
        }
    }

    public void Shutdown()
    {
        _backgroundContentCreator.OnCreateContent -= InitLocations;
    }
    private void InitContent(BackgroundDataProvider backgroundDataProvider)
    {
        if (backgroundDataProvider.LocationDataLoadProviderParticipiteInLoad.ParticipiteInLoad == true)
        {
            Foo(backgroundDataProvider.GetLocationDatas, InitLocations);
        }

        if (backgroundDataProvider.AdditionalImagesDataLoadProviderParticipiteInLoad.ParticipiteInLoad == true)
        {
            Foo(backgroundDataProvider.GetAdditionalImagesDatas, InitAdditionalImages);
        }

        if (backgroundDataProvider.ArtsDataLoadProviderParticipiteInLoad.ParticipiteInLoad == true)
        {
            Foo(backgroundDataProvider.GetArtsDatas, InitArts);
        }
        void Foo(IReadOnlyList<BackgroundData> datas, Action<BackgroundData> operation)
        {
            foreach (var data in datas)
            {
                operation.Invoke(data);
            }
        }
    }

    private void SetArtShower()
    {
        ArtShower = _backgroundContentCreator.ArtShower;
        InitArtShower();
    }
    private void InitWardrobeBackground()
    {
        InitBackgroundContent(_backgroundContentCreator.WardrobeBackground,
            _backgroundDataProvider.GetWardrobeBackgroundData.GetSprite(_backgroundDataProvider.GetWardrobeBackgroundData.BackgroundContentValues[0].NameSprite),
        _backgroundDataProvider.GetWardrobeBackgroundData.BackgroundContentValues[0]);
        WardrobeBackground = _backgroundContentCreator.WardrobeBackground;
        WardrobeBackground.SpriteRenderer.sortingOrder = WardrobeSortOrder;
    }

    private void InitLocations(BackgroundData backgroundData)
    {
        BackgroundContent content = null;
        foreach (var contentValue in backgroundData.BackgroundContentValues)
        {
            content = _backgroundContentCreator.TryGetInstantiatedBackgroundContent();
            if (BackgroundContentDictionary.ContainsKey(contentValue.NameSprite) == false && content != null)
            {
                InitBackgroundContent(
                    content,
                    backgroundData.GetSprite(contentValue.NameSprite), contentValue);
                BackgroundContentDictionary.Add(contentValue.NameSprite, content);
            }
        }
    }
    private void InitBackgroundContent(BackgroundContent backgroundContent, Sprite sprite, BackgroundContentValues backgroundContentValues)
    {
        backgroundContent.Construct(SetLighting, BackgroundContentAdditionalSpriteRendererCreator,
        sprite, backgroundContentValues);
    }

    private void InitAdditionalImages(BackgroundData backgroundData)
    {
        AddBackgroundDataContent(AdditionalImagesToBackgroundDictionary, backgroundData);
    }
    
    private void InitArts(BackgroundData backgroundData)
    {
        AddBackgroundDataContent(ArtsSpritesDictionary, backgroundData);
    }
}