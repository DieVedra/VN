using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350), NodeTint("#890384")]
public class AddSpriteNodeToBackground : BaseNode
{
    [SerializeField] private Vector2 _localPosition;
    [SerializeField] private Color _color = Color.white;
    [SerializeField] private bool _addToBackground;
    [SerializeField] private int _indexSprite;
    [SerializeField] private int _indexBackground;
    [SerializeField] private bool _removeFromBackground;
    [SerializeField] private int _indexRemoveSprite;
    [SerializeField] private int _indexBackgroundToRemove;

    private Background _background;
    public IReadOnlyList<BackgroundContent> Backgrounds => _background.GetBackgroundContent;
    public IReadOnlyList<Sprite> AdditionalImagesToBackground => _background.GetAdditionalImagesToBackground;
    public void ConstructMyAddSpriteNode(Background background)
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
        // if (_addToBackground)
        // {
        //     _background.AddAdditionalSpriteToBackgroundContent(_indexBackground, _indexSprite, _localPosition, _color);
        // }
        //
        // if (_removeFromBackground)
        // {
        //     _background.TryRemoveAdditionalSpriteToBackgroundContent(_indexBackgroundToRemove, _indexRemoveSprite);
        // }
    }
}