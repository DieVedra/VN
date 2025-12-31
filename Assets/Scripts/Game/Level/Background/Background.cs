using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Background : MonoBehaviour , IBackgroundsProviderToBackgroundNode, IAdditionalSpritesProviderToNode
{
    [SerializeField] private SpriteRenderer _colorOverlay;
    [SerializeField, NaughtyAttributes.ReadOnly] protected List<BackgroundContent> BackgroundContent;
    [SerializeField, NaughtyAttributes.ReadOnly] protected List<Sprite> AdditionalImagesToBackground;
    [SerializeField, NaughtyAttributes.ReadOnly] protected List<Sprite> ArtsSprites;
    
    
    // [SerializeField, NaughtyAttributes.ReadOnly] private List<int> _artOpenedIndexes;

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

    protected const string ArtShowerName = "ArtShower";
    protected const string ArtShowerSortingLayerName = "Background";
    protected const int ArtShowerOrderInLayer = 10;
    protected const float ArtShowerValueScale = 0.88f;

    protected Dictionary<string, BackgroundContent> BackgroundContentDictionary;
    protected Dictionary<string, Sprite> AdditionalImagesToBackgroundDictionary;
    protected Dictionary<string, Sprite> ArtsSpritesDictionary;
    protected HashSet<string> ArtOpenedKeys;
    protected SpriteRenderer ArtShower;
    protected BackgroundContent WardrobeBackground;
    protected BackgroundSaveData BackgroundSaveData;

    protected DisableNodesContentEvent DisableNodesContentEvent;
    protected ISetLighting SetLighting;
    protected SpriteRendererCreator BackgroundContentAdditionalSpriteRendererCreator;
    public string CurrentKeyBackgroundContent { get; private set; }
    public BackgroundPosition CurrentBackgroundPosition { get; private set; }
    public List<BackgroundContent> GetBackgroundContent => BackgroundContent;
    public IReadOnlyList<Sprite> GetAdditionalImagesToBackground => AdditionalImagesToBackground;

    public IReadOnlyList<Sprite> GetArtsSprites => ArtsSprites;

    public IReadOnlyDictionary<string, BackgroundContent> GetBackgroundContentDictionary => BackgroundContentDictionary;
    public IReadOnlyDictionary<string, Sprite> GetAdditionalImagesToBackgroundDictionary => AdditionalImagesToBackgroundDictionary;

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
        foreach (var pair in BackgroundContentDictionary)
        {
            if (pair.Value.GetKeysAdditionalImage.Count > 0)
            {
                BackgroundContentWithAdditionalImage indexes = new BackgroundContentWithAdditionalImage
                {
                    KeyBackgroundContent = pair.Key,
                    DataAdditionalImages = BackgroundContentDictionary[pair.Key].GetKeysAdditionalImage
                };
                BackgroundSaveData.BackgroundContentWithAdditionalImage.Add(indexes);
            }
        }
        return BackgroundSaveData;
    }

    protected void TryAddAddebleContentToBackgroundContent(IReadOnlyList<BackgroundContentWithAdditionalImage> indexes)
    {
        foreach (var pair1 in BackgroundContentDictionary)
        {
            foreach (var item in indexes)
            {
                if (item.KeyBackgroundContent == pair1.Key)
                {
                    foreach (var pair2 in item.DataAdditionalImages)
                    {
                        AddAdditionalSpriteToBackgroundContent(pair1.Key,
                            pair2.Key,
                            pair2.Value.LocalPosition,
                            pair2.Value.Color);
                    }
                    break;
                }
            }
        }
    }

    public void AddAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage, Vector2 localPosition, Color color)
    {
        BackgroundContentDictionary[keyBackground].AddAdditionalSprite(
            AdditionalImagesToBackgroundDictionary[keyAdditionalImage], localPosition, color, keyAdditionalImage);
    }

    public void TryRemoveAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage)
    {
        BackgroundContentDictionary[keyBackground].RemoveAdditionalSprite(AdditionalImagesToBackgroundDictionary[keyAdditionalImage].name, keyAdditionalImage);
    }

    public void ShowArtImage(int indexArt)
    {
        ArtShower.color = new Color(1f,1f,1f,1f);
        ArtShower.transform.localScale = new Vector2(_endValueScale,_endValueScale);
        ArtShower.gameObject.SetActive(true);
        ArtShower.sprite = ArtsSprites[indexArt];
    }

    public async UniTask ShowImageInPlayMode(int indexArt, CancellationToken cancellationToken)
    {
        ArtShower.color = new Color(1f,1f,1f,0f);
        ArtShower.sprite = ArtsSprites[indexArt];
        // CurrentArtIndex = indexArt;
        // _artOpenedIndexes.Add(indexArt);
        ArtShower.transform.localScale = new Vector2(_startValueScale,_startValueScale);
        ArtShower.gameObject.SetActive(true);
        await UniTask.WhenAll(ArtShower.DOFade(_endFadeValue, _durationFade).WithCancellation(cancellationToken),
            ArtShower.transform.DOScale(_endValueScale, _durationScale).WithCancellation(cancellationToken));
    }

    public async UniTask HideImageInPlayMode(CancellationToken cancellationToken)
    {
        ArtShower.color = new Color(1f,1f,1f,1f);
        ArtShower.transform.localScale = new Vector2(_endValueScale,_endValueScale);
        ArtShower.gameObject.SetActive(true);
        await ArtShower.DOFade(_startFadeValue, _durationFade).WithCancellation(cancellationToken);
    }

    public void SetBackgroundPosition(BackgroundPosition backgroundPosition, string key)
    {
        EnableBackgroundByKey(key);
        CurrentBackgroundPosition = backgroundPosition;
        BackgroundContentDictionary[key].SetBackgroundPosition(backgroundPosition);
    }

    public void SetBackgroundPositionFromSlider(float positionValue, string key)
    {
        EnableBackgroundByKey(key);
        BackgroundContentDictionary[key].SetBackgroundPositionFromSlider(positionValue);
    }

    public void SetWardrobeBackground()
    {
        DisableBackground();
        WardrobeBackground.gameObject.SetActive(true);
        WardrobeBackground.SetBackgroundPosition(BackgroundPosition.Central);
    }
    public async UniTask SmoothBackgroundChangePosition(CancellationToken cancellationToken, BackgroundPosition backgroundPosition, string key)
    {
        EnableBackgroundByKey(key);
        await BackgroundContentDictionary[key].MovementSmoothBackgroundChangePosition(cancellationToken, backgroundPosition);
    }
    public async UniTask SmoothChangeBackground(string keyTo, float duration, BackgroundPosition toBackgroundPosition, CancellationToken cancellationToken)
    {
        DisableBackground();
        var contentTo = BackgroundContentDictionary[keyTo];
        var contentFrom = BackgroundContentDictionary[CurrentKeyBackgroundContent];
        SetAlphaAndOrder(contentTo, _minAlpha, _orderTo);
        SetAlphaAndOrder(contentFrom, _maxAlpha, _orderFrom);
        contentTo.SetBackgroundPosition(toBackgroundPosition);
        contentTo.Activate();
        await contentTo.SpriteRenderer.DOFade(_maxAlpha, duration).WithCancellation(cancellationToken);
        contentFrom.Diactivate();
        SetAlphaAndOrder(contentTo, _maxAlpha, _orderFrom);
        CurrentKeyBackgroundContent = keyTo;
    }

    public void SmoothChangeBackgroundEmmidiately(string keyTo, BackgroundPosition toBackgroundPosition)
    {
        var contentTo = BackgroundContentDictionary[keyTo];
        var contentFrom = BackgroundContentDictionary[CurrentKeyBackgroundContent];
        SetAlphaAndOrder(contentTo, _maxAlpha, _orderFrom);
        SetAlphaAndOrder(contentFrom, _maxAlpha, _orderFrom);
        contentFrom.Diactivate();
        contentTo.SetBackgroundPosition(toBackgroundPosition);
        contentTo.Activate();
        CurrentKeyBackgroundContent = keyTo;
    }

    private void SetAlphaAndOrder(BackgroundContent content, float alpha, int order)
    {
        Color color = content.SpriteRenderer.color;
        color.a = alpha;
        content.SpriteRenderer.color = color;
        content.SpriteRenderer.sortingOrder = order;
    }

    public async UniTask SetColorOverlayBackground(Color color, CancellationToken cancellationToken, float duration, bool enable)
    {
        if (enable == true)
        {
            _colorOverlay.gameObject.SetActive(true);
        }
        await _colorOverlay.DOColor(color, duration).WithCancellation(cancellationToken);
        if (enable == false)
        {
            _colorOverlay.gameObject.SetActive(false);
        }
    }

    public void SetColorOverlayBackground(Color color, bool enable)
    {
        if (enable == true)
        {
            _colorOverlay.gameObject.SetActive(true);
        }
        _colorOverlay.color = color;
        if (enable == false)
        {
            _colorOverlay.gameObject.SetActive(false);
        }
    }

    public void SetBackgroundMovementDuringDialogueInEditMode(DirectionType directionType)
    {
        BackgroundContentDictionary[CurrentKeyBackgroundContent].MovementDuringDialogueInEditMode(directionType);
    }

    public async UniTask SetBackgroundMovementDuringDialogueInPlayMode(CancellationToken cancellationToken, DirectionType directionType)
    {
        await BackgroundContentDictionary[CurrentKeyBackgroundContent].MovementDuringDialogueInPlayMode(cancellationToken, directionType);
    }

    private void EnableBackgroundByKey(string key)
    {
        DisableBackground();
        if (BackgroundContentDictionary.ContainsKey(key))
        {
            BackgroundContentDictionary[key].Activate();
        }

        CurrentKeyBackgroundContent = key;
    }
    private void DisableBackground()
    {
        ArtShower.gameObject.SetActive(false);
        WardrobeBackground.gameObject.SetActive(false);
        ArtShower.sprite = null;
        foreach (var background in BackgroundContent)
        {
            background.Diactivate();
        }
    }

    protected void InitArtShower()
    {
        GameObject o;
        (o = ArtShower.gameObject).SetActive(false);
        o.name = ArtShowerName;
        ArtShower.transform.localScale = new Vector2(ArtShowerValueScale, ArtShowerValueScale);
        ArtShower.sortingLayerName = ArtShowerSortingLayerName;
        ArtShower.sortingOrder = ArtShowerOrderInLayer;
    }
}