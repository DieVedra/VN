
using NaughtyAttributes;
using UniRx;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public class Story
{
    [SerializeField, ShowAssetPreview()] private Sprite _spriteLogo;
    [SerializeField, ShowAssetPreview()] private Sprite _spriteStorySkin;
    [SerializeField, TextArea] private string _description;
    [SerializeField] private string _nameSceneAsset;
    [SerializeField] private int _progressPercent;
    [SerializeField] private int _currentSeriaIndex;
    [SerializeField] private ReactiveProperty<bool> _isLiked;
    [SerializeField, NaughtyAttributes.ReadOnly] private int _myIndex;
    private StoryData _storyData;
    private CompositeDisposable _compositeDisposable;
    public Sprite SpriteLogo => _spriteLogo;
    public Sprite SpriteStorySkin => _spriteStorySkin;
    public string Description => _description;

    public bool IsLiked => _isLiked.Value;
    public int MyIndex => _myIndex;

    public string NameSceneAsset => _nameSceneAsset;
    public int ProgressPercent => _progressPercent;
    public int CurrentSeriaIndex => _currentSeriaIndex;

    public void Init(StoryData storyData)
    {
        _storyData = storyData;
        _currentSeriaIndex = storyData.CurrentSeriaIndex;
        _progressPercent = storyData.CurrentProgressPercent;
        _isLiked.Value = storyData.IsLiked;
        if (storyData != null)
        {
            _compositeDisposable = new CompositeDisposable();
            _isLiked.Subscribe(_ =>
            {
                storyData.IsLiked = _isLiked.Value;
            }).AddTo(_compositeDisposable);
        }
    }

    public StoryData GetStoryData()
    {
        if (_storyData == null)
        {
            _storyData = new StoryData(_nameSceneAsset);
        }
        return _storyData;
    }
    public void Dispose()
    {
        _compositeDisposable?.Clear();
    }
    public bool ChangeLike()
    {
        if (_isLiked.Value == false)
        {
            _isLiked.Value = true;
        }
        else
        {
            _isLiked.Value = false;
        }
        return _isLiked.Value;
    }

    public void ResetProgress()
    {
        _progressPercent = 0;
        _currentSeriaIndex = 0;
        _isLiked.Value = false;
        if (_storyData != null)
        {
            _storyData.Stats = null;
            _storyData.IsLiked = false;
            _storyData.CurrentNodeIndex = 0;
            _storyData.CurrentProgressPercent = 0;
            _storyData.CurrentSeriaIndex = 0;
        }
    }
}