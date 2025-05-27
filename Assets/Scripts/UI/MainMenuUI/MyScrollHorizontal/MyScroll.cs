
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class MyScroll : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private RectTransform _swipeAreaRect;
    [SerializeField] private RectTransform _swipeProgressIndicatorsParent;

    [SerializeField] private int _contentCount;
    [SerializeField, ReadOnly] private ReactiveProperty<int> _currentIndex;
    [SerializeField] private int _startIndex = 1;

    [SerializeField] private float _moveStep;
    [SerializeField] private float _moveStepIndicator;
    [SerializeField] private ReactiveProperty<bool> _isRightMove;

    [SerializeField] private float _scaleHide = 0.8f;
    [SerializeField] private float _scaleUnhide = 1f;
    [SerializeField] private AnimationCurve _scaleCurveHide;
    [SerializeField] private AnimationCurve _scaleCurveUnhide;
    [SerializeField, Range(0.5f, 1.5f)] private float _sensitivitySlider = 1f;
    [SerializeField] private InputAction _pressInputAction;
    [SerializeField] private InputAction _positionInputAction;

    private List<StoryPanel> _contentChilds;
    private List<Transform> _transformsContentChilds;
    private List<float> _contentChildsPosX;
    private SwipeDetector _swipeDetector;
    private ScrollContentIndicatorHandler _scrollContentIndicatorHandler;
    private ChangeEffectHandler _changeEffectHandler;
    private RectTransform _swipeIndicatorFill;
    private MyScrollMover _myScrollMover;
    private CompositeDisposable _compositeDisposable;
    private IReadOnlyList<Story> _stories;

    [Button()]
    public void TestInit()
    {
        Init(null, null, null, _startIndex).Forget();
    }
    
    
    public async UniTask Init(IReadOnlyList<Story> stories, PlayStoryPanelHandler playStoryPanelHandler, LevelLoader levelLoader, int startIndex)
    {
        if (stories != null)
        {
            _contentCount = stories.Count;
            _stories = stories;
        }

        _startIndex = startIndex;
        if (_contentCount > 0)
        {
            _swipeDetector = new SwipeDetector(_pressInputAction, _positionInputAction, _swipeAreaRect, _contentCount == 1 ? _sensitivitySlider *0.5f : _sensitivitySlider);
            await CreateContent(playStoryPanelHandler, levelLoader);
            _content.anchoredPosition = Vector2.zero;
            _content.anchoredPosition = new Vector2(
                _content.anchoredPosition.x + CalculateAddValueToPositionXContentToFirstContentElement(_moveStep, _contentCount), 0f);
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
        }
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
        for (int i = 0; i < _contentCount; i++)
        {
            StoryPanelAssetProvider storyPanelAssetProvider = new StoryPanelAssetProvider();

            StoryPanel panel = null;
            panel.gameObject.SetActive(true);
            _contentChilds.Add(panel);
            _transformsContentChilds.Add(panel.transform);
            if (_stories != null)
            {
                panel.Construct(_stories[i]);
                SubscribeContinueButton(panel.ButtonContinue, panel.ButtonOpen, _stories[i], levelLoader);
                SubscribeOpenButton(panel.ButtonOpen, _stories[i], playStoryPanelHandler);
            }
        }
    }

    private async UniTask TryCreateContentIndicator()
    {
        CircleIndicatorAssetProvider circleIndicatorAssetProvider = new CircleIndicatorAssetProvider();
        TryClearContent(_swipeProgressIndicatorsParent);
        _swipeIndicatorFill = await circleIndicatorAssetProvider.CreateLoadCircleIndicatorFill(transform);
        
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
        if (contentCount % 2 == 0)
        {
            resultAddValue += moveStep / 2;
            stepCount = (contentCount / 2) - 1;
        }
        else
        {
            stepCount = contentCount / 2;
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
            Destroy(trash);
        }
        else
        {
            DestroyImmediate(trash);
        }
    }

    private void SubscribeContinueButton(Button buttonContinue, Button buttonOpen, Story story, LevelLoader levelLoader)
    {
        if (story.StoryStarted)
        {
            buttonContinue.gameObject.SetActive(true);
            buttonContinue.onClick.AddListener(() =>
            {
                if (_myScrollMover.IsMove == false)
                {
                    levelLoader.StartLoadPart1(story).Forget();
                    buttonContinue.onClick.RemoveAllListeners();
                    buttonOpen.onClick.RemoveAllListeners();
                }
            });
        }
        else
        {
            buttonContinue.gameObject.SetActive(false);
        }
    }

    private void SubscribeOpenButton(Button buttonOpen, Story story, PlayStoryPanelHandler playStoryPanelHandler)
    {
        buttonOpen.onClick.AddListener(() =>
        {
            if (_myScrollMover.IsMove == false)
            {
                _swipeDetector.Disable();
                playStoryPanelHandler.Show(story).Forget();
                _compositeDisposable = new CompositeDisposable();
                playStoryPanelHandler.OnEndExit.Subscribe(_=>
                {
                    _swipeDetector.Enable();
                    _compositeDisposable.Clear();
                }).AddTo(_compositeDisposable);
            }
        });
    }
}