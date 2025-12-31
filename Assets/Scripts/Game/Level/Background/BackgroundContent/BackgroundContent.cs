using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class BackgroundContent : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _leftBordTransform;
    [SerializeField] private Transform _centralTransform;
    [SerializeField] private Transform _rightBordTransform;
    [SerializeField] private float _movementDuringDialogueValue = 0.25f;
    [SerializeField] private Color _colorLighting = Color.white;

    private const float _durationMovementSmoothBackgroundChangePosition = 2f;
    private const float _multiplier = 2f;
    private const float _defaultPosValue = 0f;

    private Dictionary<string, AdditionalImageData> _keysAdditionalImage;
    private SpriteRendererCreator _spriteRendererCreator;
    private Transform _transformSprite;
    private ISetLighting _setLighting;
    private BackgroundPosition _currentBackgroundPosition;

    private Vector3 _movementDuringDialogueAddend;
    private Vector3 _currentPos;
    private Dictionary<string, SpriteRenderer> _addContent;


    public Transform LeftBordTransform => _leftBordTransform;
    public Transform CentralBordTransform => _centralTransform;
    public Transform RightBordTransform => _rightBordTransform;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public IReadOnlyDictionary<string, AdditionalImageData> GetKeysAdditionalImage => _keysAdditionalImage;

#if UNITY_EDITOR
    private CompositeDisposable _compositeDisposable;
    public void Construct(DisableNodesContentEvent disableNodesContentEvent,
        ISetLighting setLighting, SpriteRendererCreator spriteRendererCreator, Sprite sprite, BackgroundContentValues backgroundContentValues)
    {
        Construct(setLighting, spriteRendererCreator, sprite, backgroundContentValues);
        _compositeDisposable = disableNodesContentEvent.SubscribeWithCompositeDisposable(DestroyAllAddContent);
    }
    public void Dispose()
    {
        _compositeDisposable?.Clear();
    }
#endif
    public void Construct(ISetLighting setLighting, SpriteRendererCreator spriteRendererCreator,
        Sprite sprite, BackgroundContentValues backgroundContentValues)
    {
        gameObject.name = backgroundContentValues.NameBackground;
        _spriteRenderer.sprite = sprite;
        _movementDuringDialogueAddend = new Vector3(_movementDuringDialogueValue, _defaultPosValue,_defaultPosValue);
        _colorLighting = backgroundContentValues.ColorLighting;
        _spriteRenderer.color = backgroundContentValues.Color;
        _movementDuringDialogueValue = backgroundContentValues.MovementDuringDialogueValue;
        _spriteRenderer.transform.localScale = backgroundContentValues.Scale;
        SetPositionBorders(_leftBordTransform, new Vector3(backgroundContentValues.LeftPosition, _defaultPosValue,_defaultPosValue));
        SetPositionBorders(_centralTransform, new Vector3(backgroundContentValues.CentralPosition,_defaultPosValue, _defaultPosValue));
        SetPositionBorders(_rightBordTransform, new Vector3(backgroundContentValues.RightPosition,_defaultPosValue, _defaultPosValue));
        _spriteRendererCreator = spriteRendererCreator;
        _transformSprite = _spriteRenderer.transform;
        _setLighting = setLighting;
        _currentBackgroundPosition = BackgroundPosition.Central;
        _addContent = null;
        _keysAdditionalImage = new Dictionary<string, AdditionalImageData>();
        gameObject.SetActive(false);
    }
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
    public void SetBackgroundPositionFromSlider(float position)
    {
        var newPos = Mathf.Lerp( _leftBordTransform.localPosition.x, _rightBordTransform.localPosition.x,position);
        var myTransform = _spriteRenderer.transform;
        var position1 = myTransform.localPosition;
        position1 = new Vector3(newPos, position1.y, position1.z);
        myTransform.localPosition = position1;
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
    public void AddAdditionalSprite(Sprite sprite, Vector2 localPosition, Color color, string keyAdditionalImage)
    {
        if (_keysAdditionalImage.ContainsKey(keyAdditionalImage) == false)
        {
            if (_addContent == null)
            {
                _addContent = new Dictionary<string, SpriteRenderer>();
            }

            AdditionalImageData additionalImageData = new AdditionalImageData
            {
                LocalPosition = localPosition, Color = color
            };
            _keysAdditionalImage.Add(keyAdditionalImage, additionalImageData);
            SpriteRenderer spriteRenderer = _spriteRendererCreator.CreateAddContent(_spriteRenderer.transform);
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.transform.localPosition = localPosition;
            spriteRenderer.sortingOrder = _addContent.Count + 1;
            _addContent.Add(sprite.name, spriteRenderer);
        }
    }
    public void RemoveAdditionalSprite(string nameSprite, string keyAdditionalImage)
    {
        if (_keysAdditionalImage != null && _keysAdditionalImage.Count > 0)
        {
            _keysAdditionalImage.Remove(keyAdditionalImage);
        }
        if (_addContent != null && _addContent.Count > 0)
        {
            if (_addContent.TryGetValue(nameSprite, out SpriteRenderer renderer))
            {
                Destroy(renderer.gameObject);
                _addContent.Remove(nameSprite);
            }
        }
    }
    // public void AddAdditionalSprite(Sprite sprite, Vector2 localPosition, Color color, int indexAdditionalImage)
    // {
    //     if (_addContent == null)
    //     {
    //         _addContent = new Dictionary<string, SpriteRenderer>();
    //     }
    //
    //     AdditionalImageData additionalImageData = new AdditionalImageData
    //     {
    //         LocalPosition = localPosition, Color = color
    //     };
    //     _indexesAdditionalImage.Add(additionalImageData);
    //     SpriteRenderer spriteRenderer = _spriteRendererCreator.CreateAddContent(_spriteRenderer.transform);
    //     spriteRenderer.sprite = sprite;
    //     spriteRenderer.color = color;
    //     spriteRenderer.transform.localPosition = localPosition;
    //     spriteRenderer.sortingOrder = _addContent.Count + 1;
    //     _addContent.Add(sprite.name, spriteRenderer);
    // }
    // public void RemoveAdditionalSprite(string nameSprite, int indexAdditionalImage)
    // {
    //     if (_indexesAdditionalImage != null && _indexesAdditionalImage.Count > 0)
    //     {
    //         for (int i = 0; i < _indexesAdditionalImage.Count; i++)
    //         {
    //             // if (_indexesAdditionalImage[i].IndexAdditionalImage == indexAdditionalImage)
    //             // {
    //             //     _indexesAdditionalImage.Remove(_indexesAdditionalImage[i]);
    //             // }
    //             
    //         }
    //     }
    //     if (_addContent != null && _addContent.Count > 0)
    //     {
    //         if (_addContent.TryGetValue(nameSprite, out SpriteRenderer renderer))
    //         {
    //             Destroy(renderer.gameObject);
    //             _addContent.Remove(nameSprite);
    //         }
    //     }
    // }
    public async UniTask MovementDuringDialogueInPlayMode(CancellationToken cancellationToken, DirectionType directionType)
    {
        switch (directionType)
        {
            case DirectionType.Left:
                await SmoothMovement(cancellationToken, _currentPos + _movementDuringDialogueAddend, Ease.OutSine, _movementDuringDialogueValue);
                break;
            case DirectionType.Right:
                await SmoothMovement(cancellationToken, _currentPos - _movementDuringDialogueAddend, Ease.OutSine, _movementDuringDialogueValue);
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
        _spriteRenderer.transform.position = bordTransform.position;;
        _currentPos = bordTransform.position;
    }
    private async UniTask SmoothMovementBord(CancellationToken cancellationToken, Transform bordTransform, BackgroundPosition newBackgroundPosition)
    {
        float duration = _durationMovementSmoothBackgroundChangePosition;
        
        if (_currentBackgroundPosition == BackgroundPosition.Left && newBackgroundPosition == BackgroundPosition.Right)
        {
            duration *= _multiplier;
        }
        else if (_currentBackgroundPosition == BackgroundPosition.Right && newBackgroundPosition == BackgroundPosition.Left)
        {
            duration *= _multiplier;
        }
        await SmoothMovement(cancellationToken, bordTransform.position, Ease.InOutSine, duration);
    }

    private void SetPositionBorders(Transform position, Vector3 value)
    {
        position.localPosition = value;
    }
    
#if UNITY_EDITOR
    private void DestroyAllAddContent()
    {
        if (Application.isPlaying == false)
        {
            if (_addContent != null)
            {
                foreach (var renderer in _addContent.Values)
                {
                    DestroyImmediate(renderer.gameObject);

                }
                _addContent = null;
            }
        }
    }
#endif
}