using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

public class BackgroundEditMode : Background
{
    [SerializeField, NaughtyAttributes.ReadOnly] protected List<BackgroundContent> BackgroundContent;
    [SerializeField, NaughtyAttributes.ReadOnly] protected List<Sprite> AdditionalImagesToBackground;
    [SerializeField, NaughtyAttributes.ReadOnly] protected List<Sprite> ArtsSprites;
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
        ColorOverlay.color = Color.clear;
        BackgroundContentAdditionalSpriteRendererCreator = new SpriteRendererCreatorEditor(_spriteRendererPrefab);
        CreateWardrobeBackground();
        CreateArtShower();
        AddAdditionalSprites();
        AddArtsSprites();
        
        ClearBackgroundContent();
        ConstructBackgroundContent();
        
        if (BackgroundSaveData != null)
        {
            TryAddAddebleContentToBackgroundContent(BackgroundSaveData.BackgroundContentWithAdditionalImage);
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
        WardrobeBackground.SpriteRenderer.sortingOrder = WardrobeSortOrder;
    }
    private void ConstructBackgroundContent()
    {
        foreach (var t in _locationsDatas)
        {
            CreateBackgroundContent(t);
        }
    }

    private void ClearBackgroundContent()
    {
        if (BackgroundContentDictionary != null)
        {
            foreach (var pair in BackgroundContentDictionary)
            {
                pair.Value.Dispose();
                DestroyGameObject(pair.Value.gameObject);
            }
            BackgroundContentDictionary.Clear();
        }
        else
        {
            BackgroundContentDictionary = new Dictionary<string, BackgroundContent>();
        }
        
        if (BackgroundContent != null)
        {
            foreach (var t in BackgroundContent)
            {
                if (t != null)
                {
                    t.Dispose();
                    DestroyGameObject(t.gameObject);
                }
            }
            BackgroundContent.Clear();
        }
        else
        {
            BackgroundContent = new List<BackgroundContent>();
        }
    }
    private void CreateBackgroundContent(BackgroundData backgroundData)
    {
        foreach (var valuese in backgroundData.BackgroundContentValues)
        {
            var sprt = backgroundData.GetSprite(valuese.NameSprite);
            var bc = InstantiateBackgroundContent(backgroundData.GetSprite(valuese.NameSprite),
                valuese);
            BackgroundContentDictionary.Add(valuese.NameSprite, bc);
            BackgroundContent.Add(bc);
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
        if (AdditionalImagesToBackgroundDictionary == null)
        {
            AdditionalImagesToBackgroundDictionary = new Dictionary<string, Sprite>();
        }
        else
        {
            AdditionalImagesToBackgroundDictionary.Clear();
        }
        
        foreach (var data in _additionalImagesDatas)
        {
            AddBackgroundDataContent(AdditionalImagesToBackground, data);
            AddBackgroundDataContent(AdditionalImagesToBackgroundDictionary, data);
        }
    }

    private void AddArtsSprites()
    {
        ArtsSprites = new List<Sprite>();
        if (ArtsSpritesDictionary == null)
        {
            ArtsSpritesDictionary = new Dictionary<string, Sprite>();
        }
        else
        {
            ArtsSpritesDictionary.Clear();
        }

        foreach (var t in _artsDatas)
        {
            AddBackgroundDataContent(ArtsSprites, t);
            AddBackgroundDataContent(ArtsSpritesDictionary, t);
        }
    }
    private void AddBackgroundDataContent(List<Sprite> sprites, BackgroundData backgroundData)
    {
        foreach (var t in backgroundData.BackgroundContentValues)
        {
            sprites.Add(backgroundData.GetSprite(t.NameSprite));
        }
    }
    // private void AddBackgroundDataContent(Dictionary<string, Sprite> sprites, BackgroundData backgroundData)
    // {
    //     foreach (var t in backgroundData.BackgroundContentValues)
    //     {
    //         sprites.Add(t.NameSprite, backgroundData.GetSprite(t.NameSprite));
    //     }
    // }
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