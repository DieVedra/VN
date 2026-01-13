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
    

    private IAdditionalSpritesProviderToNode _background;

    public IReadOnlyDictionary<string, BackgroundContentValues> GetBackgroundContentDictionary =>
        _background?.GetBackgroundContentDictionary;

    public IReadOnlyDictionary<string, BackgroundContentValues> GetAdditionalImagesToBackgroundDictionary =>
        _background?.GetAdditionalImagesToBackgroundDictionary;

    public void ConstructMyAddSpriteNode(IAdditionalSpritesProviderToNode background)
    {
        _background = background;
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
        else if (_removeFromBackground)
        {
            _background.TryRemoveAdditionalSpriteToBackgroundContent(_keyBackground, _keySprite);
        }
    }
}