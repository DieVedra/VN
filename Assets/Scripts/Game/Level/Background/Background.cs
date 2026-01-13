using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Background : MonoBehaviour , IBackgroundsProviderToBackgroundNode, IAdditionalSpritesProviderToNode,
    IBackgroundsProviderToHeaderNode, IBackgroundProviderToShowArtNode, IBackgroundProviderToCustomizationNode
{
    [SerializeField] protected SpriteRenderer ColorOverlay;
    [SerializeField] protected SpriteRenderer ArtShower;
    [SerializeField] protected BackgroundContent BackgroundContent1;
    [SerializeField] protected BackgroundContent BackgroundContent2;

    [SerializeField] protected float DurationMovementDuringDialogue = 0.2f;

    private const float _startValueScale = 1.25f;
    private const float _durationScale = 2f;
    private const float _durationFade = 1.5f;
    private const float _endValueScale = 0.88f;
    private const float _endFadeValue = 1f;
    private const float _startFadeValue = 0f;
    private const float _minAlpha = 0f;
    private const float _maxAlpha = 1f;
    private const int _orderFrom = 0;
    private const int _orderTo = 1;
    public const int WardrobeContentValuesIndex = 0;
    protected const int WardrobeSortOrder = 100;

    // protected const string ArtShowerName = "ArtShower";
    protected const string Space = " ";
    // protected const string ArtShowerSortingLayerName = "Background";
    // protected const int ArtShowerOrderInLayer = 10;
    // protected const float ArtShowerValueScale = 0.88f;

    protected Dictionary<string, BackgroundContentValues> BackgroundContentValuesDictionary;
    protected Dictionary<string, BackgroundContentValues> AdditionalImagesToBackgroundDictionary;
    protected Dictionary<string, BackgroundContentValues> ArtsSpritesDictionary;

    
    
    // protected Dictionary<string, BackgroundContent> BackgroundContentDictionary;
    // protected Dictionary<string, Sprite> AdditionalImagesToBackgroundDictionary;
    // protected Dictionary<string, Sprite> ArtsSpritesDictionary;
    protected List<string> ArtOpenedKeys;
    // protected SpriteRenderer ArtShower;
    protected BackgroundContentValues WardrobeBackgroundContentValues;
    protected BackgroundSaveData BackgroundSaveData;

    // protected DisableNodesContentEvent DisableNodesContentEvent;
    protected ISetLighting SetLighting;
    // protected SpriteRendererCreator BackgroundContentAdditionalSpriteRendererCreator;
    public string CurrentKeyBackgroundContent { get; private set; } = Space;
    public string CurrentArtKey { get; private set; } = Space;
    public BackgroundPosition CurrentBackgroundPosition { get; private set; }

    public IReadOnlyDictionary<string, BackgroundContentValues> GetBackgroundContentDictionary => BackgroundContentValuesDictionary;
    public IReadOnlyDictionary<string, BackgroundContentValues> GetAdditionalImagesToBackgroundDictionary => AdditionalImagesToBackgroundDictionary;
    public IReadOnlyDictionary<string, BackgroundContentValues> GetArtsSpritesDictionary => ArtsSpritesDictionary;

    public void InitSaveData(BackgroundSaveData backgroundSaveData)
    {
        BackgroundSaveData = backgroundSaveData;
        ArtOpenedKeys = backgroundSaveData.ArtOpenedKeys;
        CurrentKeyBackgroundContent = backgroundSaveData.CurrentKeyBackgroundContent;
    }

    public BackgroundSaveData GetBackgroundSaveData()
    {
        BackgroundSaveData = new BackgroundSaveData
        {
            ArtOpenedKeys = ArtOpenedKeys,
            CurrentKeyBackgroundContent = CurrentKeyBackgroundContent,
            BackgroundContentWithAdditionalImage = new List<BackgroundContentWithAdditionalImage>()
        };
        // foreach (var pair in BackgroundContentValuesDictionary)
        // {
        //     if (pair.Value.GetKeysAdditionalImage.Count > 0)
        //     {
        //         BackgroundContentWithAdditionalImage indexes = new BackgroundContentWithAdditionalImage
        //         {
        //             KeyBackgroundContent = pair.Key,
        //             DataAdditionalImages = BackgroundContentDictionary[pair.Key].GetKeysAdditionalImage
        //         };
        //         BackgroundSaveData.BackgroundContentWithAdditionalImage.Add(indexes);
        //     }
        // }
        return BackgroundSaveData;
    }

    // protected void TryAddAddebleContentToBackgroundContent(IReadOnlyList<BackgroundContentWithAdditionalImage> indexes)
    // {
    //     foreach (var pair1 in BackgroundContentDictionary)
    //     {
    //         foreach (var item in indexes)
    //         {
    //             if (item.KeyBackgroundContent == pair1.Key)
    //             {
    //                 foreach (var pair2 in item.DataAdditionalImages)
    //                 {
    //                     AddAdditionalSpriteToBackgroundContent(pair1.Key,
    //                         pair2.Key,
    //                         pair2.Value.LocalPosition,
    //                         pair2.Value.Color);
    //                 }
    //                 break;
    //             }
    //         }
    //     }
    // }

    public void AddAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage, Vector2 localPosition, Color color)
    {
        if (string.IsNullOrEmpty(keyAdditionalImage) == false &&
            AdditionalImagesToBackgroundDictionary.TryGetValue(keyAdditionalImage, out var value))
        {
            BackgroundContent1.AddAdditionalSprite(
                value.GetSprite(), localPosition, color, keyAdditionalImage);
        }
    }

    public void TryRemoveAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage)
    {
        if (string.IsNullOrEmpty(keyAdditionalImage) == false &&
            AdditionalImagesToBackgroundDictionary.TryGetValue(keyAdditionalImage, out var value))
        {
            BackgroundContent1.RemoveAdditionalSprite(value.NameSprite, keyAdditionalImage);
        }
    }

    public void ShowArtImage(string keyArt)
    {
        if (string.IsNullOrEmpty(keyArt) == false && ArtsSpritesDictionary.TryGetValue(keyArt, out var value))
        {
            ArtShower.color = new Color(1f,1f,1f,1f);
            ArtShower.gameObject.SetActive(true);
            ArtShower.sprite = value.GetSprite();
        }
    }

    public async UniTask ShowImageInPlayMode(string keyArt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(keyArt) == false && ArtsSpritesDictionary.TryGetValue(keyArt, out var value))
        {
            ArtShower.color = new Color(1f,1f,1f,0f);
            ArtShower.sprite = value.GetSprite();
            CurrentArtKey = keyArt;
            ArtOpenedKeys ??= new List<string>();
            ArtOpenedKeys.Add(keyArt);
            ArtShower.transform.localScale = new Vector2(_startValueScale,_startValueScale);
            ArtShower.gameObject.SetActive(true);
            await UniTask.WhenAll(ArtShower.DOFade(_endFadeValue, _durationFade).WithCancellation(cancellationToken),
                ArtShower.transform.DOScale(_endValueScale, _durationScale).WithCancellation(cancellationToken));
        }
    }
    public async UniTask HideImageInPlayMode(CancellationToken cancellationToken)
    {
        await ArtShower.DOFade(_startFadeValue, _durationFade).WithCancellation(cancellationToken);
        ArtShower.gameObject.SetActive(false);
        ArtShower.transform.localScale = new Vector2(_endValueScale,_endValueScale);
    }

    public void SetBackgroundPosition(BackgroundPosition backgroundPosition, string key)
    {
        if (string.IsNullOrEmpty(key) == false && BackgroundContentValuesDictionary.TryGetValue(key, out var value))
        {
            BackgroundContent1.Activate(value);
            CurrentBackgroundPosition = backgroundPosition;
            BackgroundContent1.SetBackgroundPosition(backgroundPosition);
        }
    }

    public void SetBackgroundPositionFromSlider(float positionValue, string key)
    {
        if (string.IsNullOrEmpty(key) == false && BackgroundContentValuesDictionary.TryGetValue(key, out var value))
        {
            BackgroundContent1.Activate(value);
            CurrentKeyBackgroundContent = key;
            BackgroundContent1.SetBackgroundPositionFromSlider(positionValue);
        }
    }

    public void EnableWardrobeBackground()
    {
        BackgroundContent2.ChangeLightingColorOfTheCharacter();
        BackgroundContent2.Activate(WardrobeBackgroundContentValues);
        BackgroundContent2.SetBackgroundPosition(BackgroundPosition.Central);
    }
    public void DisableWardrobeBackground()
    {
        BackgroundContent2.Diactivate();
    }
    public async UniTask SmoothBackgroundChangePosition(CancellationToken cancellationToken, BackgroundPosition backgroundPosition, string key)
    {
        if (string.IsNullOrEmpty(key) == false && BackgroundContentValuesDictionary.TryGetValue(key, out var value))
        {
            BackgroundContent2.Diactivate();
            BackgroundContent1.Activate(value);
            CurrentKeyBackgroundContent = key;
            await BackgroundContent1.MovementSmoothBackgroundChangePosition(cancellationToken, backgroundPosition);
        }
    }
    public async UniTask SmoothChangeBackground(string keyTo, float duration, BackgroundPosition toBackgroundPosition, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(keyTo) == false && BackgroundContentValuesDictionary.TryGetValue(keyTo, out var value)
                                                 && BackgroundContentValuesDictionary.ContainsKey(CurrentKeyBackgroundContent))
        {
            SetAlphaAndOrder(BackgroundContent1, _maxAlpha);
            SetAlphaAndOrder(BackgroundContent2, _minAlpha);
            BackgroundContent2.Activate(value);
            BackgroundContent2.SetBackgroundPosition(toBackgroundPosition);
            UniTask.WhenAll(
                BackgroundContent2.SmoothChangeLightingColorOfTheCharacter(duration, cancellationToken),
                BackgroundContent2.SpriteRenderer.DOFade(_maxAlpha, duration).WithCancellation(cancellationToken));
            BackgroundContent1.Activate(value);
            BackgroundContent2.Diactivate();
            SetAlphaAndOrder(BackgroundContent2, _maxAlpha);
            CurrentKeyBackgroundContent = keyTo;
        }
    }

    public void SmoothChangeBackgroundEmmidiately(string keyTo, BackgroundPosition toBackgroundPosition)
    {
        if (string.IsNullOrEmpty(keyTo) == false && BackgroundContentValuesDictionary.TryGetValue(keyTo, out var value)
                                                 && BackgroundContentValuesDictionary.ContainsKey(CurrentKeyBackgroundContent))
        {
            SetAlphaAndOrder(BackgroundContent1, _maxAlpha);
            SetAlphaAndOrder(BackgroundContent2, _minAlpha);
            BackgroundContent2.SetBackgroundPosition(toBackgroundPosition);
            BackgroundContent2.Activate(value);
            BackgroundContent1.Diactivate();
            SetAlphaAndOrder(BackgroundContent2, _maxAlpha);
            CurrentKeyBackgroundContent = keyTo;
        }
    }

    private void SetAlphaAndOrder(BackgroundContent content, float alpha)
    {
        Color color = content.SpriteRenderer.color;
        color.a = alpha;
        content.SpriteRenderer.color = color;
    }

    public async UniTask SetColorOverlayBackground(Color color, CancellationToken cancellationToken, float duration, bool enable)
    {
        if (enable == true)
        {
            ColorOverlay.gameObject.SetActive(true);
        }
        await ColorOverlay.DOColor(color, duration).WithCancellation(cancellationToken);
        if (enable == false)
        {
            ColorOverlay.gameObject.SetActive(false);
        }
    }

    public void SetColorOverlayBackground(Color color, bool enable)
    {
        if (enable == true)
        {
            ColorOverlay.gameObject.SetActive(true);
        }
        ColorOverlay.color = color;
        if (enable == false)
        {
            ColorOverlay.gameObject.SetActive(false);
        }
    }

    public void SetBackgroundMovementDuringDialogueInEditMode(DirectionType directionType)
    {
        BackgroundContent1.MovementDuringDialogueInEditMode(directionType);
    }

    public async UniTask SetBackgroundMovementDuringDialogueInPlayMode(CancellationToken cancellationToken, DirectionType directionType)
    {
        await BackgroundContent1.MovementDuringDialogueInPlayMode(cancellationToken, directionType);
    }
    // private void DisableBackground()
    // {
    //     ArtShower.gameObject.SetActive(false);
    //     WardrobeBackground.gameObject.SetActive(false);
    //     ArtShower.sprite = null;
    //     foreach (var pair in BackgroundContentDictionary)
    //     {
    //         pair.Value.Diactivate();
    //     }
    // }
    // protected void AddBackgroundDataContent(Dictionary<string, Sprite> sprites, IReadOnlyList<BackgroundContentValues> backgroundContentValues)
    // {
    //     foreach (var t in backgroundData.BackgroundContentValues)
    //     {
    //         if (sprites.ContainsKey(t.NameSprite) == false)
    //         {
    //             sprites.Add(t.NameSprite, backgroundData.GetSprite(t.NameSprite));
    //         }
    //     }
    // }
}