using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350), NodeTint("#890384")]
public class AddSpriteNodeToBackground : BaseNode
{
    [SerializeField] private Vector2 _localPosition;
    [SerializeField] private Color _color = Color.white;
    [SerializeField, HideInInspector] private int _indexSprite;
    [SerializeField, HideInInspector] private int _indexBackground;

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
        _background.AddAdditionalSpriteToBackgroundContent(_indexBackground, _indexSprite, _localPosition, _color);
    }
}