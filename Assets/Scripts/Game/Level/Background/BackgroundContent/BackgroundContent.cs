using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public abstract class BackgroundContent : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform _leftBordTransform;
    [SerializeField] private Transform _centralTransform;
    [SerializeField] private Transform _rightBordTransform;
    [SerializeField] private float _movementDuringDialogueValue = 0.25f;
    [SerializeField] private Color _colorLighting = Color.white;

// #if UNITY_EDITOR
//     private SpriteRenderer _spriteRendererPrefab;
// #endif
    
    private readonly float _durationMovementSmoothBackgroundChangePosition = 2f;
    private float _durationMovementDuringDialogue;

    // private SpriteRenderer _spriteRendererPrefab;
    private Transform _transformSprite;
    private ISetLighting _setLighting;
    private BackgroundPosition _currentBackgroundPosition;
    
    private Vector3 _currentPos;
    private List<SpriteRenderer> _addContent;

    private Vector3 _movementDuringDialogueAddend => new Vector3(_movementDuringDialogueValue, 0f,0f);
    
    public Transform LeftBordTransform => _leftBordTransform;
    public Transform CentralBordTransform => _centralTransform;
    public Transform RightBordTransform => _rightBordTransform;

    public void Construct(DisableNodesContentEvent disableNodesContentEvent,
        ISetLighting setLighting, float durationMovementDuringDialogue)
    {
        _durationMovementDuringDialogue = durationMovementDuringDialogue;
        _transformSprite = SpriteRenderer.transform;
        _setLighting = setLighting;
        _currentBackgroundPosition = BackgroundPosition.Central;
        _addContent = null;
        disableNodesContentEvent.Subscribe(DestroyAddContent);
        gameObject.SetActive(false);
    }
// #if UNITY_EDITOR
//     public void SetPrefab(SpriteRenderer spriteRendererPrefab)
//     {
//         _spriteRendererPrefab = spriteRendererPrefab;
//     }
// #endif
    public void Activate()
    {
        gameObject.SetActive(true);
        _setLighting.SetLightingColor(_colorLighting);
    }

    public void Diactivate()
    {
        gameObject.SetActive(false);
        _setLighting.SetLightingColor(Color.white);
    }
    public void SetBackgroundPosition(BackgroundPosition backgroundPosition)
    {
        _currentBackgroundPosition = backgroundPosition;
        SwitchBackgroundPosition();
    }
    
    public async UniTask MovementSmoothBackgroundChangePosition(CancellationToken cancellationToken, BackgroundPosition backgroundPosition)
    {
        switch (backgroundPosition)
        {
            case BackgroundPosition.Left:
                await SmoothMovementBord(cancellationToken, _leftBordTransform, backgroundPosition);
                break;
            case BackgroundPosition.Central:
                await SmoothMovementBord(cancellationToken, _centralTransform, backgroundPosition);
                break;
            case BackgroundPosition.Right:
                await SmoothMovementBord(cancellationToken, _rightBordTransform, backgroundPosition);
                break;
        }
        SetBackgroundPosition(backgroundPosition);
    }
    
    public void MovementDuringDialogueInEditMode(DirectionType directionType)
    {
        SwitchBackgroundPosition();
        switch (directionType)
        {
            case DirectionType.Left:
                MovementDuringDialogue(_currentPos + _movementDuringDialogueAddend);
                break;
            case DirectionType.Right:
                MovementDuringDialogue(_currentPos - _movementDuringDialogueAddend);
                break;
        }
    }

    public void AddContent(Sprite sprite, Vector2 localPosition, Color color)
    {
        if (_addContent == null)
        {
            _addContent = new List<SpriteRenderer>();
        }
        // SpriteRenderer spriteRenderer = Instantiate(_spriteRendererPrefab, SpriteRenderer.transform, true);
        SpriteRenderer spriteRenderer = CreateAddContent();
        
        // if (_gameMode == GameMode.BuildMode)
        // {
        //     spriteRenderer =
        //         PrefabsProvider.SpriteRendererAssetProvider.CreateSpriteRenderer(SpriteRenderer.transform);
        // }
        // else
        // {
        //     spriteRenderer = Instantiate(_spriteRendererPrefab, SpriteRenderer.transform, true);
        // }

        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
        spriteRenderer.transform.localPosition = localPosition;
        spriteRenderer.sortingOrder = _addContent.Count + 1;
        _addContent.Add(spriteRenderer);
    }
    public async UniTask MovementDuringDialogueInPlayMode(CancellationToken cancellationToken, DirectionType directionType)
    {
        switch (directionType)
        {
            case DirectionType.Left:
                await SmoothMovement(cancellationToken, _currentPos + _movementDuringDialogueAddend, Ease.OutSine, _durationMovementDuringDialogue);
                break;
            case DirectionType.Right:
                await SmoothMovement(cancellationToken, _currentPos - _movementDuringDialogueAddend, Ease.OutSine, _durationMovementDuringDialogue);
                break;
        }
    }

    private void SwitchBackgroundPosition()
    {
        switch (_currentBackgroundPosition)
        {
            case BackgroundPosition.Left:
                MovementBord(_leftBordTransform);
                break;
            case BackgroundPosition.Central:
                MovementBord(_centralTransform);
                break;
            case BackgroundPosition.Right:
                MovementBord(_rightBordTransform);
                break;
        }
    }
    private async UniTask SmoothMovement(CancellationToken cancellationToken, Vector3 endPos, Ease ease, float duration)
    {
        await _transformSprite.DOMove(endPos, duration).SetEase(Ease.OutSine).WithCancellation(cancellationToken);
    }
    
    private void MovementDuringDialogue(Vector3 endPos)
    {
        _transformSprite.position = endPos;
    }
    private void MovementBord(Transform bordTransform)
    {
        SpriteRenderer.transform.position = bordTransform.position;;
        
        _currentPos = bordTransform.position;
    }
    private async UniTask SmoothMovementBord(CancellationToken cancellationToken, Transform bordTransform, BackgroundPosition newBackgroundPosition)
    {
        float duration = _durationMovementSmoothBackgroundChangePosition;
        
        if (_currentBackgroundPosition == BackgroundPosition.Left && newBackgroundPosition == BackgroundPosition.Right)
        {
            duration *= 2f;
        }
        else if (_currentBackgroundPosition == BackgroundPosition.Right && newBackgroundPosition == BackgroundPosition.Left)
        {
            duration *= 2f;
        }
        await SmoothMovement(cancellationToken, bordTransform.position, Ease.InOutSine, duration);
    }

    protected abstract SpriteRenderer CreateAddContent();
    private void DestroyAddContent()
    {
        if (Application.isPlaying == false)
        {
            if (_addContent != null)
            {
                for (int i = 0; i < _addContent.Count; i++)
                {
                    DestroyImmediate(_addContent[i].gameObject);
                }

                _addContent = null;
            }
        }
    }
}