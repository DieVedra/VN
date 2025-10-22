
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BackgroundBuildMode : Background
{
    private BackgroundDataProvider _backgroundDataProvider;
    private BackgroundContentCreator _backgroundContentCreator;

    public void Construct(BackgroundDataProvider backgroundDataProvider, BackgroundContentCreator backgroundContentCreator, ISetLighting setLighting, SpriteRendererCreatorBuild spriteRendererCreatorBuild)
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
            TryAddAddebleContentToBackgroundContent(BackgroundSaveData.IndexesBackgroundContentWithAdditionalImage, BackgroundContent.Count);
        }
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
            for (int i = 0; i < datas.Count; i++)
            {
                operation.Invoke(datas[i]);
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
    }

    private void InitLocations(BackgroundData backgroundData)
    {
        BackgroundContentValues values = null;
        BackgroundContent content = null;
        for (int i = 0; i < backgroundData.BackgroundContentValues.Count; i++)
        {
            content = _backgroundContentCreator.TryGetInstantiatedBackgroundContent();
            if (content != null)
            {
                values = backgroundData.BackgroundContentValues[i];
                InitBackgroundContent(
                    content,
                    backgroundData.GetSprite(values.NameSprite), values);
                BackgroundContent.Add(content);
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
        InitSpriteList(AdditionalImagesToBackground, backgroundData);
    }
    
    private void InitArts(BackgroundData backgroundData)
    {
        InitSpriteList(ArtsSprites, backgroundData);
    }

    private void InitSpriteList(List<Sprite> list, BackgroundData backgroundData)
    {
        for (int i = 0; i < backgroundData.BackgroundContentValues.Count; ++i)
        {
            list.Add(backgroundData.GetSprite(backgroundData.BackgroundContentValues[i].NameSprite));
        }
    }

    private void OnDestroy()
    {
        _backgroundContentCreator.OnCreateContent -= InitLocations;
    }
}