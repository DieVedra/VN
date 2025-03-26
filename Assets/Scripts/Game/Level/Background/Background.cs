using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private SpriteRenderer ShowerImage;
    [SerializeField, Expandable] private AdditionalImagesToBackground _additionalImagesToBackground;
    [SerializeField] private List<BackgroundContent> _backgroundContent;

    [SerializeField] private float DurationMovementDuringDialogue = 0.2f;

    private readonly float _durationScale = 2f;
    private readonly float _durationFade = 1.5f;
    private readonly float _startValueScale = 1.25f;
    private readonly float _endValueScale = 0.88f;
    private int _currentIndex = 0;
    private BackgroundContent _wardrobeBackground;
    public List<BackgroundContent> GetBackgroundContent => _backgroundContent;
    public AdditionalImagesToBackground AdditionalImagesToBackground => _additionalImagesToBackground;


    public void Construct(DisableNodesContentEvent disableNodesContentEvent, ISetLighting setLighting,
        SpriteRendererCreator spriteRendererCreator, BackgroundContent wardrobeBackground)
    {
        _wardrobeBackground = wardrobeBackground;
        foreach (var content in _backgroundContent)
        {
            content.Construct(disableNodesContentEvent, setLighting, spriteRendererCreator, DurationMovementDuringDialogue);
        }
    }
    public void ShowImage(Sprite sprite)
    {
        ShowerImage.color = new Color(1f,1f,1f,1f);
        ShowerImage.transform.localScale = new Vector2(_endValueScale,_endValueScale);
        ShowerImage.gameObject.SetActive(true);
        ShowerImage.sprite = sprite;
    }
    public async UniTask ShowImageInPlayMode(Sprite sprite, CancellationToken cancellationToken)
    {
        ShowerImage.color = new Color(1f,1f,1f,0f);
        ShowerImage.sprite = sprite;
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
        _backgroundContent[_currentIndex].MovementDuringDialogueInEditMode(directionType);
    }
    public async UniTask SetBackgroundMovementDuringDialogueInPlayMode(CancellationToken cancellationToken, DirectionType directionType)
    {
        await _backgroundContent[_currentIndex].MovementDuringDialogueInPlayMode(cancellationToken, directionType);
    }

    private void EnableBackgroundByIndex(int index)
    {
        DisableBackground();
        _backgroundContent[index].Activate();
        _currentIndex = index;
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
