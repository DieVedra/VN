using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class Story  : ILocalizable
{
    [SerializeField] private string _storyName;
    [SerializeField, ShowAssetPreview()] private Sprite _spriteLogo;
    [SerializeField, ShowAssetPreview()] private Sprite _spriteStorySkin;
    [SerializeField] private LocalizationString _description;
    [SerializeField] private string _nameSceneAsset;
    [SerializeField] private string _nameUISpriteAtlas;
    [SerializeField] private int _allSeriesCount;
    [SerializeField, NaughtyAttributes.ReadOnly] private int _myIndex;

    private StoryData _storyData;
    public Sprite SpriteLogo => _spriteLogo;
    public Sprite SpriteStorySkin => _spriteStorySkin;
    public LocalizationString Description => _description;

    public bool IsLiked => _storyData.IsLiked;
    public int MyIndex => _myIndex;

    public string NameSceneAsset => _nameSceneAsset;
    public string NameUISpriteAtlas => _nameUISpriteAtlas;
    public string StoryName => _storyName;
    public int ProgressPercent => _storyData.CurrentProgressPercent;
    public int CurrentSeriaIndex => _storyData.CurrentSeriaIndex;
    public int CurrentSeriaNumber => _storyData.CurrentSeriaIndex + 1;
    public int AllSeriesCount => _allSeriesCount;
    public bool StoryStarted => _storyData.StoryStarted;

    public void Init(StoryData storyData)
    {
        if (storyData != null)
        {
            _storyData = storyData;
        }
    }
    public string GetStoryName()
    {
        return _storyName;
    }
    public bool ChangeLike()
    {
        if (_storyData.IsLiked == false)
        {
            _storyData.IsLiked = true;
        }
        else
        {
            _storyData.IsLiked = false;
        }
        return _storyData.IsLiked;
    }

    public void ResetProgress()
    {
        _storyData.CurrentProgressPercent = 0;
        _storyData.CurrentSeriaIndex = 0;
        _storyData.CurrentNodeGraphIndex = 0;
        _storyData.CurrentNodeIndex = 0;
        _storyData.CurrentAudioMusicKey = null;
        _storyData.CurrentAudioAmbientKey = null;
        _storyData.IsLiked = false;
        _storyData.StoryStarted = false;
        _storyData.PutOnSwimsuitKey = false;
        _storyData.AudioEffectsIsOn?.Clear();
        _storyData.PhoneNodeIsActiveOnSave = false;
        _storyData.ReadedContactNodeCaseIndexes.Clear();
        _storyData.OnlineContactsKeys.Clear();
        _storyData.NotificationsKeys.Clear();
        _storyData.PhoneSaveDatas.Clear();
        if (_storyData.BackgroundSaveData != null)
        {
            _storyData.BackgroundSaveData.AdditionalImagesInfo?.Clear();
            _storyData.BackgroundSaveData.ArtOpenedKeys?.Clear();
            _storyData.BackgroundSaveData.CurrentBackgroundPosition = (int)BackgroundPosition.Central;
            _storyData.BackgroundSaveData.CurrentKeyBackgroundContent = null;
        }

        _storyData.Stats?.Clear();
        _storyData.WardrobeSaveDatas?.Clear();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_description};
    }
}