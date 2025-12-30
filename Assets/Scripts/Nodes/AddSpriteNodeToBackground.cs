using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[NodeWidth(350), NodeTint("#890384")]
public class AddSpriteNodeToBackground : BaseNode
{
    [SerializeField] private Vector2 _localPosition;
    [SerializeField] private Color _color = Color.white;
    [SerializeField] private bool _addToBackground;
    [SerializeField] private int _indexSprite;
    [SerializeField] private int _indexBackground;
    
    [SerializeField] private string _keySprite;
    [SerializeField] private string _keyBackground;
    
    [SerializeField] private bool _removeFromBackground;
    [SerializeField] private int _indexRemoveSprite;
    [SerializeField] private int _indexBackgroundToRemove;
    
    [SerializeField] private string _keyRemoveSprite;
    [SerializeField] private string _keyBackgroundToRemove;

    private IAdditionalSpritesProviderToNode _background;
    public IReadOnlyList<BackgroundContent> Backgrounds => _background.GetBackgroundContent;
    public IReadOnlyList<Sprite> AdditionalImagesToBackground => _background.GetAdditionalImagesToBackground;

    public IReadOnlyDictionary<string, BackgroundContent> GetBackgroundContentDictionary =>
        _background?.GetBackgroundContentDictionary;

    public IReadOnlyDictionary<string, Sprite> GetAdditionalImagesToBackgroundDictionary =>
        _background?.GetAdditionalImagesToBackgroundDictionary;

    public void ConstructMyAddSpriteNode(IAdditionalSpritesProviderToNode background)
    {
        _background = background;
        if (_addToBackground)
        {
            _keyBackground = Backgrounds[_indexBackground].gameObject.name;
            _keySprite = AdditionalImagesToBackground[_indexSprite].name;
        }

        if (_removeFromBackground)
        {
            _keyBackgroundToRemove = Backgrounds[_indexBackgroundToRemove].gameObject.name;
            _keyRemoveSprite = AdditionalImagesToBackground[_indexRemoveSprite].name;
        }
        EditorUtility.SetDirty(this);
    }
    
    public override UniTask Enter(bool isMerged = false)
    {
        SetInfoToView();
        
        if (isMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }

        return default;
    }

    protected override void SetInfoToView()
    {
        if (_addToBackground)
        {
            _background.AddAdditionalSpriteToBackgroundContent(_keyBackground, _keySprite, _localPosition, _color);
        }

        if (_removeFromBackground)
        {
            _background.TryRemoveAdditionalSpriteToBackgroundContent(_keyBackgroundToRemove, _keyRemoveSprite);
        }
    }
}