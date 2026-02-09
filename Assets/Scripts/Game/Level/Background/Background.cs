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
    [SerializeField] protected Transform _poolParent;

    private const float _startValueScale = 1.25f;
    private const float _durationScale = 2f;
    private const float _durationFade = 1.5f;
    private const float _endValueScale = 0.88f;
    private const float _endFadeValue = 1f;
    private const float _startFadeValue = 0f;
    private const float _minAlpha = 0f;
    private const float _maxAlpha = 1f;

    protected const string Space = " ";

    protected Dictionary<string, BackgroundContentValues> BackgroundContentValuesDictionary;
    protected Dictionary<string, BackgroundContentValues> WardrobeBackgroundContentValuesDictionary;
    protected Dictionary<string, BackgroundContentValues> AdditionalImagesToBackgroundDictionary;
    protected Dictionary<string, BackgroundContentValues> ArtsSpritesDictionary;
    protected List<string> ArtOpenedKeys;
    protected BackgroundSaveData BackgroundSaveData;

    protected BackgroundPool BackgroundPool;
    public string CurrentKeyBackgroundContent { get; private set; } = Space;
    public BackgroundPosition CurrentBackgroundPosition { get; private set; }
    public Transform PoolParent => _poolParent;
    public IReadOnlyDictionary<string, BackgroundContentValues> GetBackgroundContentDictionary => BackgroundContentValuesDictionary;
    public IReadOnlyDictionary<string, BackgroundContentValues> GetAdditionalImagesToBackgroundDictionary => AdditionalImagesToBackgroundDictionary;
    public IReadOnlyDictionary<string, BackgroundContentValues> GetArtsSpritesDictionary => ArtsSpritesDictionary;
    public IReadOnlyDictionary<string, BackgroundContentValues> GetWardrobeBackgroundContentValuesDictionary => WardrobeBackgroundContentValuesDictionary;

    public void InitSaveData(BackgroundSaveData backgroundSaveData)
    {
        BackgroundSaveData = backgroundSaveData;
        ArtOpenedKeys = backgroundSaveData.ArtOpenedKeys;
        CurrentKeyBackgroundContent = backgroundSaveData.CurrentKeyBackgroundContent;
    }

    public void FillSaveData(StoryData storyData)
    {
        BackgroundSaveData = new BackgroundSaveData
        {
            ArtOpenedKeys = ArtOpenedKeys,
            CurrentKeyBackgroundContent = CurrentKeyBackgroundContent,
            CurrentBackgroundPosition = (int)CurrentBackgroundPosition,
            AdditionalImagesInfo = new Dictionary<string, List<AdditionalImageData>>()
        };
        Vector2 posToSave = new Vector2();
        foreach (var pair1 in BackgroundContent1.GetAdditionalImages)
        {
            var dataAdditionalImages = new List<AdditionalImageData>();
            
            foreach (var pair2 in pair1.Value)
            {
                posToSave = pair2.Value.transform.localPosition;
                dataAdditionalImages.Add(new AdditionalImageData(pair2.Key, posToSave.x, posToSave.y, pair2.Value.color));
            }
            BackgroundSaveData.AdditionalImagesInfo.Add(pair1.Key, dataAdditionalImages);
        }
        storyData.BackgroundSaveData = BackgroundSaveData;
    }

    protected void TryAddAddebleContentToBackgroundContent(Dictionary<string, List<AdditionalImageData>> additionalImagesInfo)
    {
        foreach (var pair1 in additionalImagesInfo)
        {
            if (pair1.Value?.Count > 0)
            {
                foreach (var data in pair1.Value)
                {
                    if (AdditionalImagesToBackgroundDictionary.TryGetValue(data.Key, out var value))
                    {
                        AddAdditionalSpriteToBackgroundContent(pair1.Key, data.Key, new Vector2(data.PosX, data.PosY),
                            new Color(data.ColorR, data.ColorG, data.ColorB, data.ColorA));
                    }
                }
            }
        }
    }

    public void AddAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage, Vector2 localPosition, Color color)
    {
        if (string.IsNullOrEmpty(keyAdditionalImage) == false &&
            AdditionalImagesToBackgroundDictionary.TryGetValue(keyAdditionalImage, out var value))
        {
            BackgroundContent1.AddAdditionalSprite(
                value.GetSprite(), localPosition, color, keyAdditionalImage, keyBackground);
        }
    }

    public void TryRemoveAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage)
    {
        if (string.IsNullOrEmpty(keyAdditionalImage) == false && string.IsNullOrEmpty(keyBackground) == false)
        {
            BackgroundContent1.RemoveAdditionalSprite(keyBackground, keyAdditionalImage);
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
            CurrentKeyBackgroundContent = key;
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

    public void EnableWardrobeBackground(float positionValue, string backgroundKey)
    {
        if (string.IsNullOrEmpty(backgroundKey) == false &&
            WardrobeBackgroundContentValuesDictionary.TryGetValue(backgroundKey, out var value))
        {
            BackgroundContent2.ChangeLightingColorOfTheCharacter();
            BackgroundContent2.Activate(value);
            BackgroundContent2.SetBackgroundPositionFromSlider(positionValue);
        }
    }
    public void DisableWardrobeBackground()
    {
        BackgroundContent2.Diactivate();
        BackgroundContent2.SetBackgroundPosition(BackgroundPosition.Central);
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
        if (string.IsNullOrEmpty(keyTo) == false && BackgroundContentValuesDictionary.TryGetValue(keyTo, out var value))
        {
            SetAlphaAndOrder(BackgroundContent1, _maxAlpha);
            BackgroundContent2.Activate(value, false);
            SetAlphaAndOrder(BackgroundContent2, _minAlpha);
            BackgroundContent2.SetBackgroundPosition(toBackgroundPosition);
            BackgroundContent2.gameObject.SetActive(true);
            await UniTask.WhenAll(
                BackgroundContent2.SmoothChangeLightingColorOfTheCharacter(duration, cancellationToken),
                BackgroundContent2.SpriteRenderer.DOFade(_maxAlpha, duration).WithCancellation(cancellationToken));
            BackgroundContent1.SetBackgroundPosition(toBackgroundPosition);
            BackgroundContent1.Activate(value);
            BackgroundContent2.Diactivate();
            SetAlphaAndOrder(BackgroundContent2, _maxAlpha);
            CurrentKeyBackgroundContent = keyTo;
        }
    }

    public void SmoothChangeBackgroundEmmidiately(string keyTo, BackgroundPosition toBackgroundPosition)
    {
        if (string.IsNullOrEmpty(keyTo) == false && BackgroundContentValuesDictionary.TryGetValue(keyTo, out var value))
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
}