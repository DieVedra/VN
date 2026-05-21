using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class StatCasesSpawner
{
    private const float _startPositionX = 0f;
    private const float _startPositionY = 0f;
    private const float _offset = 30f;
    private readonly int _casesCount = 10;
    private readonly GameStatsHandler _gameStatsHandler;
    private readonly GameEndPanelAssetProvider _gameEndPanelAssetProvider;
    private readonly SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private readonly Queue<StatCaseView> _cases;
    private readonly List<StatCaseView> _activeContent;
    private Vector2 _startPosition = new Vector2(_startPositionX, _startPositionY);
    private CompositeDisposable _compositeDisposable;
    private BackgroundData _iconsData;

    public StatCasesSpawner(GameStatsHandler gameStatsHandler, GameEndPanelAssetProvider gameEndPanelAssetProvider, 
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _gameStatsHandler = gameStatsHandler;
        _gameEndPanelAssetProvider = gameEndPanelAssetProvider;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _cases = new Queue<StatCaseView>(_casesCount);
        _activeContent = new List<StatCaseView>(_casesCount);
    }

    public async UniTask SpawnCases(RectTransform statContentRectTransform)
    {
        var iconsDataAssetProvider = new IconsDataAssetProvider();
        _iconsData = await iconsDataAssetProvider.LoadIconsDataAsset();
        foreach (var statCase in _activeContent)
        {
            _cases.Enqueue(statCase);
        }
        _activeContent.Clear();
        _compositeDisposable?.Clear();
        _compositeDisposable = new CompositeDisposable();
        foreach (var stat in _gameStatsHandler.Stats)
        {
            if (stat.ShowInEndGameResultKey)
            {
                StatCaseView statCaseView;
                if (_cases.Count > 0)
                {
                    statCaseView = _cases.Dequeue();
                }
                else
                {
                    statCaseView = await _gameEndPanelAssetProvider.LoadStatCasePrefab(statContentRectTransform);
                }

                var sprite = _iconsData.GetSprite(stat.NameKey);
                if (sprite != null)
                {
                    statCaseView.ImageCase.sprite = sprite;
                }

                _startPosition.x = statCaseView.RectTransform.anchoredPosition.x;
                float sizeDeltaY = statCaseView.RectTransform.sizeDelta.y;
                float offset = sizeDeltaY + _offset;
                _startPosition.y = _startPositionY + offset;
        
                statCaseView.RectTransform.anchoredPosition = _startPosition;
                statCaseView.TextCase.text = $"{stat.NameText} {stat.Value}";

                _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
                {
                    statCaseView.TextCase.text = $"{stat.NameText} {stat.Value}";
                }, _compositeDisposable);
                _activeContent.Add(statCaseView);
            }
        }
    }

    public void Shutdown()
    {
        _compositeDisposable?.Clear();
        if (_iconsData != null)
        {
            Addressables.Release(_iconsData);

        }
        foreach (var statCase in _activeContent)
        {
            Addressables.ReleaseInstance(statCase.gameObject);
        }
    }
}