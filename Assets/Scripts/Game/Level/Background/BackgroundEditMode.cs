using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class BackgroundEditMode : Background
{
    [SerializeField] private GameObject _prefabSpriteRenderer;
    [SerializeField, HorizontalLine(color:EColor.Blue), Expandable] private List<BackgroundData> _locationsDatas;
    
    [SerializeField, Expandable] private List<BackgroundData> _wardrobeBackgroundData;
    [SerializeField, Expandable] private List<BackgroundData> _additionalImagesDatas;
    [SerializeField, Expandable] private List<BackgroundData> _artsDatas;
    
    [SerializeField] private SerializedDictionary<string, BackgroundContentValues> _backgroundContentDictionary;
    [SerializeField] private SerializedDictionary<string, BackgroundContentValues> _additionalImagesToBackgroundDictionary;
    [SerializeField] private SerializedDictionary<string, BackgroundContentValues> _artsSpritesDictionary;
    [SerializeField] private SerializedDictionary<string, BackgroundContentValues> _wardrobeSpritesDictionary;
    public void Construct(DisableNodesContentEvent disableNodesContentEvent, ISetLighting setLighting)
    {
        disableNodesContentEvent.Subscribe(() =>
        {
            BackgroundContent1.gameObject.SetActive(false);
            BackgroundContent2.gameObject.SetActive(false);
            ColorOverlay.gameObject.SetActive(false);
            ArtShower.gameObject.SetActive(false);
        });
        if (_poolParent.childCount > 0)
        {
            for (int i = _poolParent.childCount - 1; i >= 0; i--)
            {
                var child = _poolParent.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }
        BackgroundPool = new BackgroundPool(_prefabSpriteRenderer, _poolParent);
        BackgroundContent1.Construct(disableNodesContentEvent, setLighting, BackgroundPool);
        BackgroundContent2.Construct(disableNodesContentEvent, setLighting, BackgroundPool);
        ColorOverlay.color = Color.clear;
        ConstructDictionary(ref _backgroundContentDictionary, ref BackgroundContentValuesDictionary, _locationsDatas);
        ConstructDictionary(ref _additionalImagesToBackgroundDictionary, ref AdditionalImagesToBackgroundDictionary, _additionalImagesDatas);
        ConstructDictionary(ref _artsSpritesDictionary, ref ArtsSpritesDictionary, _artsDatas);
        ConstructDictionary(ref _wardrobeSpritesDictionary, ref WardrobeBackgroundContentValuesDictionary, _wardrobeBackgroundData);
        
        if (Application.isPlaying && BackgroundSaveData != null)
        {
            TryAddAddebleContentToBackgroundContent(BackgroundSaveData.AdditionalImagesInfo);
        }
    }
    private void ConstructDictionary(ref SerializedDictionary<string, BackgroundContentValues> contentDictionary,
        ref Dictionary<string, BackgroundContentValues> baseContentDictionary, List<BackgroundData> locationsDatas)
    {
        if (contentDictionary == null)
        {
            contentDictionary = new SerializedDictionary<string, BackgroundContentValues>();
        }
        else
        {
            contentDictionary.Clear();
        }
        foreach (var bd in locationsDatas)
        {
            foreach (var bcv in bd.BackgroundContentValues)
            {
                if (contentDictionary.ContainsKey(bcv.NameBackground) == false)
                {
                    contentDictionary.Add(bcv.NameBackground, bcv);
                }
            }
        }
        baseContentDictionary = contentDictionary;
    }

    private void OnDestroy()
    {
        BackgroundContent1.Shutdown();
        BackgroundContent2.Shutdown();
    }
}