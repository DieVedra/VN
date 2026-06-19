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
    [SerializeField] private LocalizationString _textLabelGameEndPanel;
    [SerializeField] private LocalizationString _textDescriptionGameEndPanel;
    [SerializeField] private string _nameSceneAsset;
    [SerializeField] private string _nameUISpriteAtlas;
    [SerializeField] private int _allSeriesCount;
    [SerializeField, NaughtyAttributes.ReadOnly] private int _myIndex;

    private StoryData _storyData;
    public Sprite SpriteLogo => _spriteLogo;
    public Sprite SpriteStorySkin => _spriteStorySkin;
    public LocalizationString Description => _description;
    public LocalizationString TextLabelGameEndPanel => _textLabelGameEndPanel;
    public LocalizationString TextDescriptionGameEndPanel => _textDescriptionGameEndPanel;

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

    public void ResetProgress(StoryData storyData)
    {
        storyData.CurrentProgressPercent = 0;
        storyData.CurrentSeriaIndex = 0;
        storyData.CurrentNodeGraphIndex = 0;
        storyData.CurrentNodeIndex = 0;
        storyData.CurrentAudioMusicKey = null;
        storyData.CurrentAudioAmbientKey = null;
        storyData.IsLiked = false;
        storyData.StoryStarted = false;
        storyData.PutOnSwimsuitKey = false;
        storyData.AudioEffectsIsOn?.Clear();
        storyData.PhoneNodeIsActiveOnSave = false;
        storyData.ReadedContactNodeCaseIndexes.Clear();
        storyData.OnlineContactsKeys.Clear();
        storyData.NotificationsKeys.Clear();
        storyData.PhoneSaveDatas.Clear();
        if (storyData.BackgroundSaveData != null)
        {
            storyData.BackgroundSaveData.AdditionalImagesInfo?.Clear();
            storyData.BackgroundSaveData.ArtOpenedKeys?.Clear();
            storyData.BackgroundSaveData.CurrentBackgroundPosition = (int)BackgroundPosition.Central;
            storyData.BackgroundSaveData.CurrentKeyBackgroundContent = null;
        }

        storyData.Stats?.Clear();
        storyData.WardrobeSaveDatas?.Clear();
    }
    public void ResetProgress()
    {
        ResetProgress(_storyData);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_description, _textLabelGameEndPanel, _textDescriptionGameEndPanel};
    }
}