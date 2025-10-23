using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyScrollHandler : ILocalizable
{
    private const float _scaleHide = 0.8f;
    private const float _scaleUnhide = 1f;
    private const float _halfMultiplier = 0.5f;
    private const int _divisor = 2;

    private readonly float _moveStep;
    private readonly float _moveStepIndicator;
    private readonly float _sensitivitySlider;

    private readonly LocalizationString _buttonOpenText = "Открыть";
    private readonly LocalizationString _buttonContinueText = "Продолжить";
    private readonly RectTransform _transform;
    private readonly RectTransform _content;
    private readonly HorizontalLayoutGroup _contentLayoutGroup;
    private readonly ContentSizeFitter _contentSizeFitter;
    private readonly RectTransform _swipeAreaRect;
    private readonly RectTransform _swipeProgressIndicatorsParent;
    private readonly DescriptionCutter _descriptionCutter;
    private readonly ReactiveProperty<int> _currentIndex;
    private readonly ReactiveProperty<bool> _isRightMove;
    private readonly ReactiveCommand _languageChanged;
    private readonly ReactiveCommand<bool> _swipeDetectorOff;
    private readonly AnimationCurve _scaleCurveHide;
    private readonly AnimationCurve _scaleCurveUnhide;
    private readonly InputAction _pressInputAction;
    private readonly InputAction _positionInputAction;
    
    private int _contentCount;
    private int _startIndex;
    private List<StoryPanel> _contentChilds;
    private List<Transform> _transformsContentChilds;
    private List<float> _contentChildsPosX;
    private SwipeDetector _swipeDetector;
    private ScrollContentIndicatorHandler _scrollContentIndicatorHandler;
    private ChangeEffectHandler _changeEffectHandler;
    private RectTransform _swipeIndicatorFill;
    private MyScrollMover _myScrollMover;
    private CompositeDisposable _compositeDisposablePlayStoryPanelHandler;
    private IReadOnlyList<Story> _stories;
    private CompositeDisposable _compositeDisposableLanguageChanged;

    public MyScrollHandler(MyScrollUIView myScrollUIView, ReactiveCommand languageChanged, ReactiveCommand<bool> swipeDetectorOff)
    {
        _transform = myScrollUIView.Transform;
        _content = myScrollUIView.Content;
        _swipeAreaRect = myScrollUIView.SwipeAreaRect;
        _swipeProgressIndicatorsParent = myScrollUIView.SwipeProgressIndicatorsParent;
        _scaleCurveHide = myScrollUIView.ScaleCurveHide;
        _scaleCurveUnhide = myScrollUIView.ScaleCurveUnhide;
        _pressInputAction = myScrollUIView.PressInputAction;
        _positionInputAction = myScrollUIView.PositionInputAction;
        _moveStep = myScrollUIView.MoveStep;
        _sensitivitySlider = myScrollUIView.SensitivitySlider;
        _moveStepIndicator = myScrollUIView.MoveStepIndicator;
        _currentIndex = new ReactiveProperty<int>();
        _isRightMove = new ReactiveProperty<bool>();
        _languageChanged = languageChanged;
        _swipeDetectorOff = swipeDetectorOff;
        _contentLayoutGroup = _content.GetComponent<HorizontalLayoutGroup>();
        _contentSizeFitter = _content.GetComponent<ContentSizeFitter>();
        _contentSizeFitter.enabled = false;
        _contentLayoutGroup.enabled = false;
        _descriptionCutter = new DescriptionCutter();
    }

    public async UniTask Construct(IReadOnlyList<Story> stories, PlayStoryPanelHandler playStoryPanelHandler,
        LevelLoader levelLoader, int startIndex = 0)
    {
        _compositeDisposableLanguageChanged?.Clear();
        _compositeDisposableLanguageChanged = new CompositeDisposable();
        _languageChanged.Subscribe(_ =>
        {
            LanguageChanged();
        }).AddTo(_compositeDisposableLanguageChanged);
        if (stories != null)
        {
            _contentCount = stories.Count;
            _stories = stories;
        }
        _startIndex = startIndex;
        if (_contentCount > 0)
        {
            _contentLayoutGroup.enabled = true;
            _contentSizeFitter.enabled = true;
            _swipeDetector = new SwipeDetector(_pressInputAction, _positionInputAction, _swipeAreaRect, _contentCount == 1 ? _sensitivitySlider * _halfMultiplier : _sensitivitySlider);
            await CreateContent(playStoryPanelHandler, levelLoader);
            _content.anchoredPosition = Vector2.zero;
            _content.anchoredPosition = new Vector2(
                _content.anchoredPosition.x + CalculateAddValueToPositionXContentToFirstContentElement(_moveStep, _contentCount), AnimationValuesProvider.MinValue);
            CreateContentPositions();
            TrySetToCurrentPos();
            if (_swipeIndicatorFill != null)
            {
                Annigilator(_swipeIndicatorFill.gameObject);
                _swipeIndicatorFill = null;
            }
            await TryCreateContentIndicator();

            _changeEffectHandler = new ChangeEffectHandler(_contentChilds, _currentIndex);
            ReactiveProperty<bool> outsideLeftBorder = new ReactiveProperty<bool>();
            ReactiveProperty<bool> outsideRightBorder = new ReactiveProperty<bool>();
            MyScrollScaler myScrollScaler = new MyScrollScaler(_transformsContentChilds, _currentIndex, outsideLeftBorder, outsideRightBorder,
                _scaleCurveHide, _scaleCurveUnhide, _scaleHide, _scaleUnhide);
            _myScrollMover = new MyScrollMover(_isRightMove, _currentIndex, outsideLeftBorder, outsideRightBorder, _content,
                _scrollContentIndicatorHandler, _contentChildsPosX, myScrollScaler, _changeEffectHandler, _moveStep, _contentCount, _startIndex);

            _swipeDetector.OnPress += _myScrollMover.OnPress;

            if (_contentCount == 1)
            {
                _swipeDetector.OnSwipeDirection += _myScrollMover.MoveWhenOnePanel;
                _swipeDetector.OnUnpress += _myScrollMover.StartPanelCloserWhenOnePanel;
            }
            else
            {
                _swipeDetector.OnSwipeDirection += _myScrollMover.MoveFromSwipe;
                _swipeDetector.OnUnpress += _myScrollMover.StartPanelCloser;
            }

            _swipeDetector.Enable();
            _swipeDetectorOff.Subscribe(_ =>
            {
                if (_ == true)
                {
                    _swipeDetector.Disable();
                }
                else
                {
                    _swipeDetector.Enable();
                }
            });
            _contentSizeFitter.enabled = false;

            _contentLayoutGroup.enabled = false;
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new [] {_buttonOpenText, _buttonContinueText };
    }

    public void Dispose()
    {
        if (_contentCount == 1)
        {
            _swipeDetector.OnSwipeDirection -= _myScrollMover.MoveWhenOnePanel;
            _swipeDetector.OnUnpress -= _myScrollMover.StartPanelCloserWhenOnePanel;
        }
        else
        {
            _swipeDetector.OnUnpress -= _myScrollMover.StartPanelCloser;
            _swipeDetector.OnSwipeDirection -= _myScrollMover.MoveFromSwipe;
        }
        
        
        _swipeDetector.OnPress -= _myScrollMover.OnPress;
        _swipeDetector.Disable();
        _changeEffectHandler.Dispose();
        _currentIndex.Dispose();
        _compositeDisposableLanguageChanged.Clear();
        for (int i = 0; i < _contentChilds.Count; i++)
        {
            Addressables.ReleaseInstance(_contentChilds[i].gameObject);
        }
    }

    private void TrySetToCurrentPos()
    {
        if (_startIndex > _contentCount - 1)
        {
            _startIndex = _contentCount - 1;
        }
        _content.anchoredPosition = new Vector2(_contentChildsPosX[_startIndex], _content.anchoredPosition.y);
        _currentIndex.Value = _startIndex;
    }

    private async UniTask CreateContent(PlayStoryPanelHandler playStoryPanelHandler, LevelLoader levelLoader)
    {
        TryClearContent(_content);
        _contentChilds = null;
        _transformsContentChilds = null;
        _contentChilds = new List<StoryPanel>(_contentCount);
        _transformsContentChilds = new List<Transform>(_contentCount);
        StoryPanelAssetProvider storyPanelAssetProvider = new StoryPanelAssetProvider();

        for (int i = 0; i < _contentCount; i++)
        {

            StoryPanel panel = await storyPanelAssetProvider.CreateStoryPanel(_content);
            _contentChilds.Add(panel);
            _transformsContentChilds.Add(panel.transform);
            if (_stories != null)
            {
                panel.ImageBackground.sprite = _stories[i].SpriteStorySkin;
                panel.ImageLabel.sprite = _stories[i].SpriteLogo;


                InitContinueButton(panel, _stories[i], levelLoader);
                InitOpenButton(panel, _stories[i], playStoryPanelHandler);
                // panel.TextDescription.text = _stories[i].Description;
                panel.gameObject.SetActive(true);
                _descriptionCutter.TryCutAndSet(panel.TextDescription, _stories[i].Description);
            }
            else
            {
                panel.gameObject.SetActive(true);
            }
        }
    }

    private async UniTask TryCreateContentIndicator()
    {
        CircleIndicatorAssetProvider circleIndicatorAssetProvider = new CircleIndicatorAssetProvider();
        TryClearContent(_swipeProgressIndicatorsParent);
        _swipeIndicatorFill = await circleIndicatorAssetProvider.CreateLoadCircleIndicatorFill(_transform);
        
        _swipeIndicatorFill.SetAsFirstSibling();
        _swipeIndicatorFill.gameObject.SetActive(true);
        for (int i = 0; i < _contentCount; i++)
        {
            RectTransform circle = await circleIndicatorAssetProvider.CreateLoadCircleIndicator(_swipeProgressIndicatorsParent);
            circle.gameObject.SetActive(true);
        }
        _scrollContentIndicatorHandler = new ScrollContentIndicatorHandler(_contentChildsPosX, _swipeIndicatorFill,
            _content, _swipeProgressIndicatorsParent, CalculateAddValueToPositionXContentToFirstContentElement,
            _contentCount, _moveStepIndicator, _moveStep);
        _swipeProgressIndicatorsParent.transform.gameObject.SetActive(true);
    }

    private float CalculateAddValueToPositionXContentToFirstContentElement(float moveStep, int contentCount)
    {
        if (contentCount <= 1)
        {
            return 0f;
        }
        
        int stepCount = 0;
        float resultAddValue = 0;
        if (contentCount % _divisor == 0)
        {
            resultAddValue += moveStep * _halfMultiplier;
            stepCount = (contentCount / _divisor) - 1;
        }
        else
        {
            stepCount = contentCount / _divisor;
        }

        for (int i = 0; i < stepCount; i++)
        {
            resultAddValue += moveStep;
        }

        return resultAddValue;
    }

    private void CreateContentPositions()
    {
        _contentChildsPosX = new List<float>(_contentCount);
        float firsPosX = _content.anchoredPosition.x;
        _contentChildsPosX.Add(firsPosX);
        for (int i = 0; i < _contentCount - 1; i++)
        {
            _contentChildsPosX.Add(firsPosX -= _moveStep);
        }
    }

    private void TryClearContent(Transform swipeProgressIndicatorParent)
    {
        if (swipeProgressIndicatorParent.childCount > 0)
        {
            for (int i = swipeProgressIndicatorParent.childCount - 1; i >= 0; i--)
            {
                Annigilator(swipeProgressIndicatorParent.GetChild(i).gameObject);
            }
        }
        
    }

    private void Annigilator(GameObject trash)
    {
        if (Application.isPlaying == true)
        {
            Object.Destroy(trash);
        }
        else
        {
            Object.DestroyImmediate(trash);
        }
    }

    private void InitContinueButton(StoryPanel panel, Story story, LevelLoader levelLoader)
    {
        if (story.StoryStarted)
        {
            panel.TextButtonContinue.text = _buttonContinueText;
            panel.ButtonContinue.onClick.AddListener(() =>
            {
                if (_myScrollMover.IsMove == false)
                {
                    levelLoader.StartLoadPart1(story).Forget();
                    panel.ButtonContinue.onClick.RemoveAllListeners();
                    panel.ButtonOpen.onClick.RemoveAllListeners();
                }
            });
            panel.ButtonContinue.gameObject.SetActive(true);
        }
        else
        {
            panel.ButtonContinue.gameObject.SetActive(false);
        }
    }

    private void InitOpenButton(StoryPanel panel, Story story, PlayStoryPanelHandler playStoryPanelHandler)
    {
        panel.TextButtonOpen.text = _buttonOpenText;
        panel.ButtonOpen.onClick.AddListener(() =>
        {
            if (_myScrollMover.IsMove == false)
            {
                _swipeDetector.Disable();
                playStoryPanelHandler.Show(story).Forget();
                _compositeDisposablePlayStoryPanelHandler = new CompositeDisposable();
                playStoryPanelHandler.OnEndExit.Subscribe(_=>
                {
                    _swipeDetector.Enable();
                    _compositeDisposablePlayStoryPanelHandler.Clear();
                }).AddTo(_compositeDisposablePlayStoryPanelHandler);
            }
        });
    }

    private void LanguageChanged()
    {
        for (int i = 0; i < _contentChilds.Count; i++)
        {
            _contentChilds[i].TextDescription.text = _stories[i].Description;
            _contentChilds[i].TextButtonContinue.text = _buttonContinueText;
            _contentChilds[i].TextButtonOpen.text = _buttonOpenText;
        }
    }
}