using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private BackgroundData backgroundData;
    
    [SerializeField] private SpriteRenderer ShowerImage;
    [SerializeField] private List<Sprite> _additionalImagesToBackground;
    [SerializeField] private List<Sprite> _arts;
    [SerializeField] private List<BackgroundContent> _backgroundContent;
    
    [SerializeField, ReadOnly] private List<int> _artOpenedIndexes;

    [SerializeField] private float DurationMovementDuringDialogue = 0.2f;

    private readonly float _durationScale = 2f;
    private readonly float _durationFade = 1.5f;
    private readonly float _startValueScale = 1.25f;
    private readonly float _endValueScale = 0.88f;
    // private int _currentIndex = 0;
    private BackgroundContent _wardrobeBackground;
    private DisableNodesContentEvent _disableNodesContentEvent;
    private ISetLighting _setLighting;
    private SpriteRendererCreator _spriteRendererCreator;
    // private BackgroundSaveData _backgroundSaveData;
    public int CurrentArtIndex { get; private set; }
    public int CurrentIndexAdditionalImage { get; private set; }
    public int CurrentIndexBackgroundContent { get; private set; }
    public List<BackgroundContent> GetBackgroundContent => _backgroundContent;
    public IReadOnlyList<Sprite> AdditionalImagesToBackground => _additionalImagesToBackground;
    public IReadOnlyList<int> ArtOpenedIndexes => _artOpenedIndexes;

    public IReadOnlyList<Sprite> Arts => _arts;

    public void ConstructSaveOn(BackgroundSaveData backgroundSaveData,
        DisableNodesContentEvent disableNodesContentEvent, ISetLighting setLighting,
        SpriteRendererCreator spriteRendererCreator, BackgroundContent wardrobeBackground)
    {
        // _backgroundSaveData = backgroundSaveData;
        _artOpenedIndexes = backgroundSaveData.ArtOpenedIndexes;
        // CurrentIndexBackgroundContent = backgroundSaveData.CurrentIndexBackgroundContent;
        
        

        ConstructSaveOff(disableNodesContentEvent, setLighting, spriteRendererCreator, wardrobeBackground);
        TryAddAddebleContentToBackgroundContent(backgroundSaveData.IndexesBackgroundContentWithAdditionalImage, _backgroundContent.Count);
    }

    public void ConstructSaveOff(DisableNodesContentEvent disableNodesContentEvent, ISetLighting setLighting,
        SpriteRendererCreator spriteRendererCreator, BackgroundContent wardrobeBackground)
    {
        _wardrobeBackground = wardrobeBackground;
        _disableNodesContentEvent = disableNodesContentEvent;
        _setLighting = setLighting;
        _spriteRendererCreator = spriteRendererCreator;


        ConstructBackgroundContent(_backgroundContent);
    }

    public BackgroundSaveData GetBackgroundSaveData()
    {
        BackgroundSaveData backgroundSaveData;
        backgroundSaveData.ArtOpenedIndexes = _artOpenedIndexes;
        backgroundSaveData.CurrentIndexBackgroundContent = CurrentIndexBackgroundContent;
        backgroundSaveData.IndexesBackgroundContentWithAdditionalImage = new List<IndexesBackgroundContentWithAdditionalImage>();
        for (int i = 0; i < _backgroundContent.Count; i++)
        {
            if (_backgroundContent[i].GetIndexesAdditionalImage.Count > 0)
            {
                IndexesBackgroundContentWithAdditionalImage indexes;
                indexes.IndexesAdditionalImages = _backgroundContent[i].GetIndexesAdditionalImage.ToList();
                indexes.IndexBackgroundContent = i;
                backgroundSaveData.IndexesBackgroundContentWithAdditionalImage.Add(indexes);
            }
        }
        return backgroundSaveData;
    }

    private void ConstructBackgroundContent(List<BackgroundContent> backgroundContent)
    {
        for (int i = 0; i < backgroundContent.Count; ++i)
        {
            backgroundContent[i].Construct(_disableNodesContentEvent, _setLighting, _spriteRendererCreator, DurationMovementDuringDialogue);
        }
    }

    private void TryAddAddebleContentToBackgroundContent(IReadOnlyList<IndexesBackgroundContentWithAdditionalImage> indexes, int count)
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
                            AddContent(i,
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
    public void AddContent(int indexBackground, int indexAdditionalImage, Vector2 localPosition, Color color)
    {
        _backgroundContent[indexBackground].AddContent(_additionalImagesToBackground[indexAdditionalImage], localPosition, color, indexAdditionalImage);
        CurrentIndexAdditionalImage = indexAdditionalImage;
    }

    public void AddToAdditionalImagesToBackground(List<Sprite> addableSprites)
    {
        _additionalImagesToBackground.AddRange(addableSprites);
    }
    public void AddToArts(List<Sprite> addableSprites)
    {
        _arts.AddRange(addableSprites);
    }
    
    
    public void ShowImage(int indexArt)
    {
        ShowerImage.color = new Color(1f,1f,1f,1f);
        ShowerImage.transform.localScale = new Vector2(_endValueScale,_endValueScale);
        ShowerImage.gameObject.SetActive(true);
        ShowerImage.sprite = _arts[indexArt];
    }
    public async UniTask ShowImageInPlayMode(int indexArt, CancellationToken cancellationToken)
    {
        ShowerImage.color = new Color(1f,1f,1f,0f);
        ShowerImage.sprite = _arts[indexArt];
        CurrentArtIndex = indexArt;
        _artOpenedIndexes.Add(indexArt);
        ShowerImage.transform.localScale = new Vector2(_startValueScale,_startValueScale);
        ShowerImage.gameObject.SetActive(true);
        await UniTask.WhenAll(ShowerImage.DOFade(1f, _durationFade).WithCancellation(cancellationToken),
            ShowerImage.transform.DOScale(_endValueScale, _durationScale).WithCancellation(cancellationToken));
    }
    public void SetBackgroundPosition(BackgroundPosition backgroundPosition, int index)
    {
        EnableBackgroundByIndex(index);
        _backgroundContent[index].SetBackgroundPosition(backgroundPosition);
    }

    public void SetWardrobeBackground()
    {
        DisableBackground();
        _wardrobeBackground.gameObject.SetActive(true);
        _wardrobeBackground.SetBackgroundPosition(BackgroundPosition.Central);
    }
    public async UniTask SmoothBackgroundChangePosition(CancellationToken cancellationToken, BackgroundPosition backgroundPosition, int index)
    {
        EnableBackgroundByIndex(index);
        await _backgroundContent[index].MovementSmoothBackgroundChangePosition(cancellationToken, backgroundPosition);
    }
    public void SetBackgroundMovementDuringDialogueInEditMode(DirectionType directionType)
    {
        _backgroundContent[CurrentIndexBackgroundContent].MovementDuringDialogueInEditMode(directionType);
    }
    public async UniTask SetBackgroundMovementDuringDialogueInPlayMode(CancellationToken cancellationToken, DirectionType directionType)
    {
        await _backgroundContent[CurrentIndexBackgroundContent].MovementDuringDialogueInPlayMode(cancellationToken, directionType);
    }

    private void EnableBackgroundByIndex(int index)
    {
        DisableBackground();
        _backgroundContent[index].Activate();
        CurrentIndexBackgroundContent = index;
    }

    private void DisableBackground()
    {
        ShowerImage.gameObject.SetActive(false);
        _wardrobeBackground.gameObject.SetActive(false);
        ShowerImage.sprite = null;
        foreach (var background in _backgroundContent)
        {
            background.Diactivate();
        }
    }
    
}
