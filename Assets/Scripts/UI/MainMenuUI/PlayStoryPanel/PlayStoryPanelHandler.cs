
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayStoryPanelHandler
{
    private const string _seriaText = "Серия";
    private LevelLoader _levelLoader;
    private readonly Transform _parent;
    private PlayStoryPanel _playStoryPanel;
    private RectTransform _rectTransformPanel;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private Story _currentStory;

    private Vector2 _hideScale;
    private Vector2 _unhideScale;
    private CancellationTokenSource _cancellationTokenSource;

    public PlayStoryPanelHandler(BlackFrameUIHandler blackFrameUIHandler)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
    }
    public ReactiveCommand OnEndExit { get; private set; }
    public async UniTask Init(LevelLoader levelLoader, Transform parent)
    {
        _levelLoader = levelLoader;
        OnEndExit = new ReactiveCommand();
        PlayStoryPanelAssetProvider storyPanelAssetProvider = new PlayStoryPanelAssetProvider();
        _playStoryPanel = await storyPanelAssetProvider.CreatePlayStoryPanel(parent);
        _playStoryPanel.gameObject.SetActive(false);
        _playStoryPanel.transform.SetSiblingIndex(_playStoryPanel.HierarchyIndex);
        _rectTransformPanel = _playStoryPanel.GetComponent<RectTransform>();
        _hideScale = new Vector2(_playStoryPanel.HideScaleValue, _playStoryPanel.HideScaleValue);
        _unhideScale = _rectTransformPanel.localScale;
        _rectTransformPanel.localScale = _hideScale;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        Addressables.ReleaseInstance(_playStoryPanel.gameObject);
    }
    public async UniTaskVoid Show(Story story)
    {
        
        _playStoryPanel.CanvasGroup.alpha = 0f;
        _currentStory = story;
        InitLikeButton();
        _playStoryPanel.ProgressText.text = $"{story.ProgressPercent}%";
        _playStoryPanel.TextSeria.text = $"{_seriaText} {story.CurrentSeriaIndex}";
        _playStoryPanel.gameObject.SetActive(true);

        await UniTask.WhenAll(
            _blackFrameUIHandler.CloseTranslucent(_playStoryPanel.HierarchyIndex),
            _rectTransformPanel.DOScale(_unhideScale, 0.5f).WithCancellation(_cancellationTokenSource.Token),
            _playStoryPanel.CanvasGroup.DOFade( 1f,0.5f).WithCancellation(_cancellationTokenSource.Token));
        
        _playStoryPanel.ResetProgressButton.onClick.AddListener(story.ResetProgress);
        _playStoryPanel.ButtonOpen.onClick.AddListener(PlayChangedStory);


        _playStoryPanel.ExitButton.onClick.AddListener(() =>
        {
            Hide().Forget();
            
        });
    }

    private async UniTaskVoid Hide()
    {
        UnsubscrimeAllButtons();
        await UniTask.WhenAll(
            _rectTransformPanel.DOScale(_hideScale, 0.5f).WithCancellation(_cancellationTokenSource.Token),
            _playStoryPanel.CanvasGroup.DOFade( 0f,0.5f).WithCancellation(_cancellationTokenSource.Token),
            _blackFrameUIHandler.OpenTranslucent()
            );
        await  _rectTransformPanel.DOScale(_hideScale, 0.5f).WithCancellation(_cancellationTokenSource.Token);

        
        _playStoryPanel.gameObject.SetActive(false);
        OnEndExit.Execute();
    }

    private void UnsubscrimeAllButtons()
    {
        _playStoryPanel.LikeButton.onClick.RemoveAllListeners();
        _playStoryPanel.ResetProgressButton.onClick.RemoveAllListeners();
        _playStoryPanel.ButtonOpen.onClick.RemoveAllListeners();
    }
    private void InitLikeButton()
    {
        _playStoryPanel.LikeButton.onClick.AddListener(TryLikedStory);
        if (_currentStory.IsLiked)
        {
            _playStoryPanel.LikeImage.gameObject.SetActive(true);
        }
        else
        {
            _playStoryPanel.LikeImage.gameObject.SetActive(false);
        }
    }

    private void TryLikedStory()
    {
        if (_currentStory.ChangeLike())
        {
            _playStoryPanel.LikeImage.gameObject.SetActive(true);
        }
        else
        {
            _playStoryPanel.LikeImage.gameObject.SetActive(false);
        }
    }

    private void PlayChangedStory()
    {
        UnsubscrimeAllButtons();
        _levelLoader.StartLoadPart1(_currentStory).Forget();
    }
}