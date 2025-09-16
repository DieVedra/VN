using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class BackgroundEditMode : Background
{
    [SerializeField, HorizontalLine(color:EColor.Blue), Expandable] private List<BackgroundData> _locationsDatas;
    [SerializeField, Expandable] private BackgroundData _wardrobeBackgroundData;
    [SerializeField, Expandable] private List<BackgroundData> _additionalImagesDatas;
    [SerializeField, Expandable] private List<BackgroundData> _artsDatas;

    [SerializeField, HorizontalLine(color:EColor.Blue)] private SpriteRenderer _spriteRendererPrefab;

    [SerializeField] private BackgroundContent _backgroundContentPrefab;
    private const int _wardrobeBackgroundContentIndex = 0;
    public void Construct(DisableNodesContentEvent disableNodesContentEvent, ISetLighting setLighting)
    {
        DisableNodesContentEvent = disableNodesContentEvent;
        SetLighting = setLighting;
        BackgroundContentAdditionalSpriteRendererCreator = new SpriteRendererCreatorEditor(_spriteRendererPrefab);
        CreateWardrobeBackground();
        CreateArtShower();
        AddAdditionalSprites();
        AddArtsSprites();
        
        ClearBackgroundContent();
        ConstructBackgroundContent();
        
        if (BackgroundSaveData != null)
        {
            TryAddAddebleContentToBackgroundContent(BackgroundSaveData.IndexesBackgroundContentWithAdditionalImage, BackgroundContent.Count);
        }
    }

    private void CreateWardrobeBackground()
    {
        Transform wardrobeBackgroundContentTransform = GetTransformOnExistingByName(_wardrobeBackgroundData.BackgroundContentValues[_wardrobeBackgroundContentIndex].NameBackground);
        if (wardrobeBackgroundContentTransform == null)
        {
            WardrobeBackground = InstantiateBackgroundContent(
                _wardrobeBackgroundData.GetSprite(_wardrobeBackgroundData.BackgroundContentValues[_wardrobeBackgroundContentIndex].NameSprite),
                _wardrobeBackgroundData.BackgroundContentValues[_wardrobeBackgroundContentIndex]);
        }
        else
        {
            WardrobeBackground = wardrobeBackgroundContentTransform.GetComponent<BackgroundContent>();
        }
    }
    private void ConstructBackgroundContent()
    {
        for (int i = 0; i < _locationsDatas.Count; ++i)
        {
            CreateBackgroundContent(_locationsDatas[i]);
        }
    }

    private void ClearBackgroundContent()
    {
        if (BackgroundContent.Count > 0)
        {
            for (int i = 0; i < BackgroundContent.Count; ++i)
            {
                if (BackgroundContent[i] != null)
                {
                    BackgroundContent[i].Dispose();
                    DestroyGameObject(BackgroundContent[i].gameObject);
                }
            }
        }
        BackgroundContent = new List<BackgroundContent>();
    }
    private void CreateBackgroundContent(BackgroundData backgroundData)
    {
        for (int j = 0; j < backgroundData.BackgroundContentValues.Count; ++j)
        {
            BackgroundContent.Add(
                InstantiateBackgroundContent(backgroundData.GetSprite(backgroundData.BackgroundContentValues[j].NameSprite),
                    backgroundData.BackgroundContentValues[j]));
        }
    }

    private BackgroundContent InstantiateBackgroundContent(Sprite sprite, BackgroundContentValues backgroundContentValues)
    {
        BackgroundContent backgroundContent = Instantiate(_backgroundContentPrefab, parent: transform);
        backgroundContent.Construct(DisableNodesContentEvent, SetLighting, BackgroundContentAdditionalSpriteRendererCreator,
            sprite, backgroundContentValues);
        return backgroundContent;
    }
    private void CreateArtShower()
    {
        Transform artShowerTransform = GetTransformOnExistingByName(ArtShowerName);
        if (artShowerTransform == null)
        {
            ArtShower = Instantiate(_spriteRendererPrefab, parent: transform);
            InitArtShower();
        }
        else
        {
            ArtShower = artShowerTransform.GetComponent<SpriteRenderer>();
        }
    }

    private void AddAdditionalSprites()
    {
        AdditionalImagesToBackground = new List<Sprite>();
        for (int i = 0; i < _additionalImagesDatas.Count; ++i)
        {
            AddBackgroundDataContent(ref AdditionalImagesToBackground, _additionalImagesDatas[i]);
        }
    }

    private void AddArtsSprites()
    {
        ArtsSprites = new List<Sprite>();
        for (int i = 0; i < _artsDatas.Count; i++)
        {
            AddBackgroundDataContent(ref ArtsSprites, _artsDatas[i]);
        }
    }
    private void AddBackgroundDataContent(ref List<Sprite> sprites, BackgroundData backgroundData)
    {
        for (int i = 0; i < backgroundData.BackgroundContentValues.Count; ++i)
        {
            sprites.Add(backgroundData.GetSprite(backgroundData.BackgroundContentValues[i].NameSprite));
        }
    }

    private void DestroyGameObject(GameObject transferredGameObject)
    {
        if (Application.isPlaying)
        {
            Destroy(transferredGameObject);
        }
        else
        {
            DestroyImmediate(transferredGameObject);
        }
    }

    private Transform GetTransformOnExistingByName(string name)
    {
        Transform returnTransform = null;
        foreach (Transform childTransform in transform)
        {
            if (childTransform.name == name)
            {
                returnTransform = childTransform;
                break;
            }
        }
        return returnTransform;
    }
}