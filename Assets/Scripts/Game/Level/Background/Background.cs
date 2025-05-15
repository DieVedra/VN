using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] protected List<BackgroundContent> BackgroundContent;
    [SerializeField] protected List<Sprite> AdditionalImagesToBackground;
    [SerializeField] protected List<Sprite> ArtsSprites;


    [SerializeField, ReadOnly] private List<int> _artOpenedIndexes;

    [SerializeField] protected float DurationMovementDuringDialogue = 0.2f;

    private const float _startValueScale = 1.25f;
    private const float _durationScale = 2f;
    private const float _durationFade = 1.5f;
    private const float _endValueScale = 0.88f;
    private const float _endValue = 1f;

    protected const string ArtShowerName = "ArtShower";
    protected const string ArtShowerSortingLayerName = "Background";
    protected const int ArtShowerOrderInLayer = 10;
    protected const float ArtShowerValueScale = 0.88f;

    // private int _currentIndex = 0;
    protected SpriteRenderer ArtShower;
    protected BackgroundContent WardrobeBackground;
    protected BackgroundSaveData BackgroundSaveData;
    
    protected DisableNodesContentEvent DisableNodesContentEvent;
    protected ISetLighting SetLighting;
    protected SpriteRendererCreator BackgroundContentAdditionalSpriteRendererCreator;
    public int CurrentArtIndex { get; private set; }
    public int CurrentIndexAdditionalImage { get; private set; }
    public int CurrentIndexBackgroundContent { get; private set; }
    public List<BackgroundContent> GetBackgroundContent => BackgroundContent;
    public IReadOnlyList<Sprite> GetAdditionalImagesToBackground => AdditionalImagesToBackground;
    public IReadOnlyList<int> ArtOpenedIndexes => _artOpenedIndexes;

    public IReadOnlyList<Sprite> GetArtsSprites => ArtsSprites;

    public void InitSaveData(BackgroundSaveData backgroundSaveData)
    {
        BackgroundSaveData = backgroundSaveData;
        _artOpenedIndexes = backgroundSaveData.ArtOpenedIndexes;
        CurrentIndexBackgroundContent = backgroundSaveData.CurrentIndexBackgroundContent;
    }

    public BackgroundSaveData GetBackgroundSaveData()
    {
        BackgroundSaveData = new BackgroundSaveData
        {
            ArtOpenedIndexes = _artOpenedIndexes,
            CurrentIndexBackgroundContent = CurrentIndexBackgroundContent,
            IndexesBackgroundContentWithAdditionalImage = new List<IndexesBackgroundContentWithAdditionalImage>()
        };
        for (int i = 0; i < BackgroundContent.Count; i++)
        {
            if (BackgroundContent[i].GetIndexesAdditionalImage.Count > 0)
            {
                IndexesBackgroundContentWithAdditionalImage indexes = new IndexesBackgroundContentWithAdditionalImage
                {
                    IndexesAdditionalImages = BackgroundContent[i].GetIndexesAdditionalImage.ToList(),
                    IndexBackgroundContent = i
                };
                BackgroundSaveData.IndexesBackgroundContentWithAdditionalImage.Add(indexes);
            }
        }
        return BackgroundSaveData;
    }

    protected void TryAddAddebleContentToBackgroundContent(IReadOnlyList<IndexesBackgroundContentWithAdditionalImage> indexes, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            if (indexes.Count > 0)
            {
                for (int j = 0; j < indexes.Count; ++j)
                {
                    if (i == indexes[j].IndexBackgroundContent)
                    {
                        for (int k = 0; k < indexes[j].IndexesAdditionalImages.Count; ++k)
                        {
                            AddAdditionalSpriteToBackgroundContent(i,
                                indexes[j].IndexesAdditionalImages[k].IndexAdditionalImage,
                                indexes[j].IndexesAdditionalImages[k].LocalPosition,
                                indexes[j].IndexesAdditionalImages[k].Color);
                        }
                        break;
                    }
                }
            }
        }
    }
    public void AddAdditionalSpriteToBackgroundContent(int indexBackground, int indexAdditionalImage, Vector2 localPosition, Color color)
    {
        BackgroundContent[indexBackground].AddAdditionalSprite(AdditionalImagesToBackground[indexAdditionalImage], localPosition, color, indexAdditionalImage);
        CurrentIndexAdditionalImage = indexAdditionalImage;
    }

    public void ShowImage(int indexArt)
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
        CurrentArtIndex = indexArt;
        _artOpenedIndexes.Add(indexArt);
        ArtShower.transform.localScale = new Vector2(_startValueScale,_startValueScale);
        ArtShower.gameObject.SetActive(true);
        await UniTask.WhenAll(ArtShower.DOFade(_endValue, _durationFade).WithCancellation(cancellationToken),
            ArtShower.transform.DOScale(_endValueScale, _durationScale).WithCancellation(cancellationToken));
    }
    public void SetBackgroundPosition(BackgroundPosition backgroundPosition, int index)
    {
        EnableBackgroundByIndex(index);
        BackgroundContent[index].SetBackgroundPosition(backgroundPosition);
    }

    public void SetWardrobeBackground()
    {
        DisableBackground();
        WardrobeBackground.gameObject.SetActive(true);
        WardrobeBackground.SetBackgroundPosition(BackgroundPosition.Central);
    }
    public async UniTask SmoothBackgroundChangePosition(CancellationToken cancellationToken, BackgroundPosition backgroundPosition, int index)
    {
        EnableBackgroundByIndex(index);
        await BackgroundContent[index].MovementSmoothBackgroundChangePosition(cancellationToken, backgroundPosition);
    }
    public void SetBackgroundMovementDuringDialogueInEditMode(DirectionType directionType)
    {
        BackgroundContent[CurrentIndexBackgroundContent].MovementDuringDialogueInEditMode(directionType);
    }
    public async UniTask SetBackgroundMovementDuringDialogueInPlayMode(CancellationToken cancellationToken, DirectionType directionType)
    {
        await BackgroundContent[CurrentIndexBackgroundContent].MovementDuringDialogueInPlayMode(cancellationToken, directionType);
    }

    private void EnableBackgroundByIndex(int index)
    {
        DisableBackground();
        BackgroundContent[index].Activate();
        CurrentIndexBackgroundContent = index;
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
