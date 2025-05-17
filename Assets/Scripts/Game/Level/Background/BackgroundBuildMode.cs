
using System.Collections.Generic;
using System.Linq;
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
        if (backgroundDataProvider.LocationDataLoadProviderParticipiteInLoad.ParticipiteInLoad == true)
        {
            InitLocations(backgroundDataProvider.GetLocationDatas[LevelLoadDataHandler.IndexFirstSeriaData]);
        }

        if (backgroundDataProvider.AdditionalImagesDataLoadProviderParticipiteInLoad.ParticipiteInLoad == true)
        {
            InitAdditionalImages(backgroundDataProvider.GetAdditionalImagesDatas[LevelLoadDataHandler.IndexFirstSeriaData]);
        }

        if (backgroundDataProvider.ArtsDataLoadProviderParticipiteInLoad.ParticipiteInLoad == true)
        {
            InitArts(backgroundDataProvider.GetArtsDatas[LevelLoadDataHandler.IndexFirstSeriaData]);
        }

        _backgroundContentCreator.OnCreateContent += InitLocations;
        _backgroundDataProvider.OnLoadAdditionalImagesData.Subscribe(InitAdditionalImages);
        _backgroundDataProvider.OnLoadArtsData.Subscribe(InitArts);


        if (BackgroundSaveData != null)
        {
            TryAddAddebleContentToBackgroundContent(BackgroundSaveData.IndexesBackgroundContentWithAdditionalImage, BackgroundContent.Count);
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

    private void InitLocations( BackgroundData backgroundData)
    {
        Debug.Log($"_instantiatedBackgroundContent {_backgroundContentCreator.InstantiatedBackgroundContent.Count}");
        Debug.Log($"_backgroundData  {backgroundData.BackgroundContentValues.Count}");

        for (int i = 0; i < _backgroundContentCreator.InstantiatedBackgroundContent.Count; ++i)
        {
            Debug.Log($" index{i}");
            InitBackgroundContent(_backgroundContentCreator.InstantiatedBackgroundContent[i],
                backgroundData.GetSprite(backgroundData.BackgroundContentValues[i].NameSprite),
                backgroundData.BackgroundContentValues[i]);
            BackgroundContent.Add(_backgroundContentCreator.InstantiatedBackgroundContent[i]);
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