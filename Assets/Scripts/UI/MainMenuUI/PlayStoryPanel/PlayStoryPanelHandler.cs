using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PlayStoryPanelHandler : ILocalizable
{
    public const int FontSizeValue = 80;
    public const float HeightPanel = 850f;
    private const int _childIndex = 0;
    private readonly LocalizationString _seriaText = "Серия";
    private readonly LocalizationString _playButtonText = "Играть";
    private readonly LocalizationString _progressLabelTextToConfirmedPanel = "Прогресс";
    private readonly LocalizationString _progressQuestionTextToConfirmedPanel = "Сбросить текущий прогресс?";
    private readonly LocalizationString _cashLabelTextToConfirmedPanel = "Кэш";
    private readonly LocalizationString _cashQuestionTextToConfirmedPanel  = "Удалить кэш истории для экономии памяти?";
    private readonly LocalizationString _confirmedButtonText = "Да";
    
    private ContentHeightCalculator _contentHeightCalculator;
    private LevelLoader _levelLoader;
    private readonly Transform _parent;
    private PlayStoryPanel _playStoryPanel;
    private RectTransform _rectTransformPanel;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly SaveServiceProvider _saveServiceProvider;
    private readonly ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private readonly ReactiveCommand _onExitEndRC;
    private Story _currentStory;
    private Image _imageCash;
    private Image _imageProgress;
    private Vector2 _hideScale;
    private Vector2 _unhideScale;
    private CancellationTokenSource _cancellationTokenSource;

    public ReactiveCommand OnExitEndRC => _onExitEndRC;

    public string GetCurrentStoryName
    {
        get
        {
            if (_currentStory == null)
            {
                return String.Empty;
            }
            else
            {
                return _currentStory.StoryName;
            }
        }
    }

    public PlayStoryPanelHandler(BlackFrameUIHandler blackFrameUIHandler, SaveServiceProvider saveServiceProvider, ConfirmedPanelUIHandler confirmedPanelUIHandler)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
        _saveServiceProvider = saveServiceProvider;
        _confirmedPanelUIHandler = confirmedPanelUIHandler;
        _onExitEndRC = new ReactiveCommand();
    }

    public async UniTask Init(LevelLoader levelLoader, Transform parent)
    {
        _levelLoader = levelLoader;
        PlayStoryPanelAssetProvider storyPanelAssetProvider = new PlayStoryPanelAssetProvider();
        _playStoryPanel = await storyPanelAssetProvider.CreatePlayStoryPanel(parent);
        _playStoryPanel.gameObject.SetActive(false);
        _playStoryPanel.transform.SetSiblingIndex(_playStoryPanel.HierarchyIndex);
        _rectTransformPanel = _playStoryPanel.GetComponent<RectTransform>();
        _hideScale = new Vector2(_playStoryPanel.HideScaleValue, _playStoryPanel.HideScaleValue);
        _contentHeightCalculator = new ContentHeightCalculator(_playStoryPanel.TextDescription);

        _unhideScale = _rectTransformPanel.localScale;
        _rectTransformPanel.localScale = _hideScale;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Shutdown()
    {
        if (_playStoryPanel != null && _playStoryPanel.gameObject != null)
        {
            if (_playStoryPanel.gameObject.activeInHierarchy || _playStoryPanel.gameObject.scene.isLoaded)
            {
                Addressables.ReleaseInstance(_playStoryPanel.gameObject);
            }
        }
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new LocalizationString[] {_seriaText, _playButtonText, _progressLabelTextToConfirmedPanel,
            _progressQuestionTextToConfirmedPanel, _cashLabelTextToConfirmedPanel, _cashQuestionTextToConfirmedPanel, _confirmedButtonText};
    }

    public async UniTaskVoid Show(Story story)
    {
        _playStoryPanel.CanvasGroup.alpha = AnimationValuesProvider.MinValue;
        _currentStory = story;
        InitLikeButton();
        _playStoryPanel.ProgressText.text = $"{story.ProgressPercent}%";
        _playStoryPanel.TextSeria.text = $"{_seriaText} {story.CurrentSeriaNumber}";

        _contentHeightCalculator.UpdateTextSize(story.Description);
        _playStoryPanel.gameObject.SetActive(true);
        _playStoryPanel.PlayButtonText.text = _playButtonText;
        Transform tr = _playStoryPanel.transform;
        _blackFrameUIHandler.Transform.SetAsLastSibling();
        tr.SetAsLastSibling();
        await UniTask.WhenAll(
            _blackFrameUIHandler.CloseTranslucent(false),
            _rectTransformPanel.DOScale(_unhideScale, AnimationValuesProvider.HalfValue).SetEase(Ease.OutQuart).WithCancellation(_cancellationTokenSource.Token),
            _playStoryPanel.CanvasGroup.DOFade( AnimationValuesProvider.MaxValue,AnimationValuesProvider.HalfValue).WithCancellation(_cancellationTokenSource.Token));
        
        _playStoryPanel.ButtonOpen.onClick.AddListener(PlayChangedStory);
        _playStoryPanel.ResetProgressButton.gameObject.SetActive(true);
        
        TrySubscribeResetProgressButton(story);
        
        _playStoryPanel.ExitButton.onClick.AddListener(() =>
        {
            Hide().Forget();
        });
    }

    private void TrySubscribeResetProgressButton(Story story)
    {
        if (story.StoryStarted)
        {
            _playStoryPanel.ResetProgressButton.interactable = true;
            ChangeColorButtonIcon(ref _imageProgress, _playStoryPanel.ResetProgressButton);
            

            _playStoryPanel.ResetProgressButton.onClick.AddListener(() =>
                {
                    _playStoryPanel.ResetProgressButton.onClick.RemoveAllListeners();
                    _confirmedPanelUIHandler.Show(_progressLabelTextToConfirmedPanel, _progressQuestionTextToConfirmedPanel,
                        _confirmedButtonText,
                        HeightPanel, FontSizeValue, () =>
                        {
                            story.ResetProgress();
                            _saveServiceProvider.DeleteProgressByStory(story.StoryName);
                            TrySubscribeResetProgressButton(story);
                        },
                        () =>
                        {
                            TrySubscribeResetProgressButton(story);
                        }).Forget();
                });
        }
        else
        {
            _playStoryPanel.ResetProgressButton.onClick.RemoveAllListeners();
            _playStoryPanel.ResetProgressButton.interactable = false;
            ChangeColorButtonIcon(ref _imageProgress, _playStoryPanel.ResetProgressButton);
        }
    }

    private void ChangeColorButtonIcon(ref Image imageIcon, Button button)
    {
        if (imageIcon == null)
        {
            if (button.gameObject.transform.childCount > 0 && button.gameObject.transform.GetChild(_childIndex).TryGetComponent(out Image image))
            {
                imageIcon = image;
                Change(imageIcon);
            }
        }
        else
        {
            Change(imageIcon);
        }
        void Change(Image imageIcon1)
        {
            if (button.interactable == true)
            {
                Color color = imageIcon1.color;
                color.a = button.colors.normalColor.a;
                imageIcon1.color = color;
            }
            else
            {
                Color color = imageIcon1.color;
                color.a = button.colors.disabledColor.a;
                imageIcon1.color = color;
            }
        }
    }

    private async UniTaskVoid Hide()
    {
        UnsubscribeAllButtons();
        await UniTask.WhenAll(
            _rectTransformPanel.DOScale(_hideScale, AnimationValuesProvider.HalfValue).WithCancellation(_cancellationTokenSource.Token),
            _playStoryPanel.CanvasGroup.DOFade( AnimationValuesProvider.MinValue,AnimationValuesProvider.HalfValue).WithCancellation(_cancellationTokenSource.Token),
            _blackFrameUIHandler.OpenTranslucent()
            );
        _playStoryPanel.gameObject.SetActive(false);
        _onExitEndRC.Execute();
    }

    private void UnsubscribeAllButtons()
    {
        _playStoryPanel.ExitButton.onClick.RemoveAllListeners();
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
        UnsubscribeAllButtons();
        _levelLoader.StartLoadPart1(_currentStory).Forget();
    }
}