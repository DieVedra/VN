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

    private Dictionary<string, Dictionary<string, SpriteRenderer>> _additionalImages;
    private BackgroundPool _backgroundPool;
    private Transform _transformSpriteRenderer;
    private ISetLighting _setLighting;
    private BackgroundPosition _currentBackgroundPosition;

    private Vector3 _movementDuringDialogueAddend;
    private Vector3 _currentPos;
    private Vector3 _leftPosition;
    private Vector3 _centralPosition;
    private Vector3 _rightPosition;
    public Transform LeftBordTransform => _leftBordTransform;
    public Transform CentralBordTransform => _centralTransform;
    public Transform RightBordTransform => _rightBordTransform;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    public IReadOnlyDictionary<string, Dictionary<string, SpriteRenderer>> GetAdditionalImages => _additionalImages;

#if UNITY_EDITOR
    private CompositeDisposable _compositeDisposable;
    public void Construct(DisableNodesContentEvent disableNodesContentEvent, ISetLighting setLighting, BackgroundPool backgroundPool)
    {
        Construct(setLighting, backgroundPool);
        _compositeDisposable = disableNodesContentEvent.SubscribeWithCompositeDisposable(DestroyAllAddContent);
    }
    public void Shutdown()
    {
        _compositeDisposable?.Clear();
    }
#endif
    public void Construct(ISetLighting setLighting, BackgroundPool backgroundPool)
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            for (int i = 0; i < _transformSpriteRenderer.childCount; i++)
            {
                DestroyImmediate(_transformSpriteRenderer.GetChild(i).gameObject);
            }
        }
#endif
        _movementDuringDialogueAddend = new Vector3(_movementDuringDialogueValue, _defaultPosValue,_defaultPosValue);
        _leftPosition = new Vector3(_defaultPosValue, _defaultPosValue,_defaultPosValue);
        _centralPosition = new Vector3(_defaultPosValue,_defaultPosValue, _defaultPosValue);
        _rightPosition = new Vector3(_defaultPosValue,_defaultPosValue, _defaultPosValue);
        _backgroundPool = backgroundPool;
        _transformSpriteRenderer = _spriteRenderer.transform;
        _setLighting = setLighting;
        _additionalImages = new Dictionary<string, Dictionary<string, SpriteRenderer>>();
        gameObject.SetActive(false);
    }
    public void Activate(BackgroundContentValues backgroundContentValues, bool enable = true)
    {
        _spriteRenderer.sprite = backgroundContentValues.GetSprite();
        _colorLighting = backgroundContentValues.ColorLighting;
        _spriteRenderer.color = backgroundContentValues.Color;
        _movementDuringDialogueValue = backgroundContentValues.MovementDuringDialogueValue;
        _spriteRenderer.transform.localScale = backgroundContentValues.Scale;
        _leftPosition.x = backgroundContentValues.LeftPosition;
        _centralPosition.x = backgroundContentValues.CentralPosition;
        _rightPosition.x = backgroundContentValues.RightPosition;
        SetPositionBorders(_leftBordTransform, _leftPosition);
        SetPositionBorders(_centralTransform, _centralPosition);
        SetPositionBorders(_rightBordTransform, _rightPosition);
        DisableAllAdditionalSprite();
        if (_additionalImages.TryGetValue(backgroundContentValues.NameBackground, out var value))
        {
            foreach (var pair in value)
            {
                pair.Value.gameObject.SetActive(true);
            }
        }
        gameObject.SetActive(enable);
    }

    public void Diactivate()
    {
        gameObject.SetActive(false);
        DisableAllAdditionalSprite();
    }

    public void ChangeLightingColorOfTheCharacter()
    {
        _setLighting.ChangeLightingColorOfTheCharacter(_colorLighting);
    }

    public async UniTask SmoothChangeLightingColorOfTheCharacter(float duration, CancellationToken cancellationToken)
    {
        await _setLighting.SmoothChangeLightingColorOfTheCharacter(_colorLighting, duration, cancellationToken);
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
    public void AddAdditionalSprite(Sprite sprite, Vector2 localPosition, Color color, string keyAdditionalImage, string keyBackground)
    {
        if (string.IsNullOrEmpty(keyBackground) == false)
        {
            if (_additionalImages.TryGetValue(keyBackground, out var value1) == true)
            {
                if (value1.TryGetValue(keyAdditionalImage, out var value2))
                {
                    value2.transform.localPosition = localPosition;
                    value2.color = color;
                    value2.sprite = sprite;
                }
                else
                {
                    SpriteRenderer spriteRenderer = _backgroundPool.GetRenderer();
                    value1.Add(keyAdditionalImage, spriteRenderer);
                    FillSpriteRenderer(value1.Count, spriteRenderer);
                }
            }
            else
            {
                Dictionary<string, SpriteRenderer> dictionary = new Dictionary<string, SpriteRenderer>();
                SpriteRenderer spriteRenderer = _backgroundPool.GetRenderer();
                dictionary.Add(keyAdditionalImage, spriteRenderer);
                FillSpriteRenderer(dictionary.Count, spriteRenderer);
                _additionalImages.Add(keyBackground, dictionary);
            }
        }
        void FillSpriteRenderer(int dictionaryCount, SpriteRenderer spriteRenderer)
        {
            Transform transform1;
            (transform1 = spriteRenderer.transform).SetParent(_spriteRenderer.transform);
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            transform1.localPosition = localPosition;
            spriteRenderer.sortingOrder = dictionaryCount;
            spriteRenderer.gameObject.SetActive(true);
        }
    }
    public void RemoveAdditionalSprite(string keyAdditionalImage, string keyBackground)
    {
        if (string.IsNullOrEmpty(keyBackground) == false && string.IsNullOrEmpty(keyAdditionalImage) == false)
        {
            if (_additionalImages.TryGetValue(keyBackground, out var dictionary))
            {
                if (dictionary.TryGetValue(keyAdditionalImage, out var value))
                {
                    Destroy(value.gameObject);
                    dictionary.Remove(keyAdditionalImage);
                }
            }
        }
    }
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
        await _transformSpriteRenderer.DOMove(endPos, duration).SetEase(Ease.OutSine).WithCancellation(cancellationToken);
    }
    
    private void MovementDuringDialogue(Vector3 endPos)
    {
        _transformSpriteRenderer.position = endPos;
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

    private void DisableAllAdditionalSprite()
    {
        for (int i = 0; i < _transformSpriteRenderer.childCount; i++)
        {
            _transformSpriteRenderer.GetChild(i).gameObject.SetActive(false);
        }
    }
    
#if UNITY_EDITOR
    private void DestroyAllAddContent()
    {
        if (Application.isPlaying == false)
        {
            if (_additionalImages != null)
            {
                foreach (var pair1 in _additionalImages)
                {
                    foreach (var pair2 in pair1.Value)
                    {
                        DestroyImmediate(pair2.Value.gameObject);
                    }
                }
                _additionalImages.Clear();
            }
        }
    }
#endif
}