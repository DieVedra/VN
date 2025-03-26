
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#890384")]
public class AddSpriteNodeToBackground : BaseNode
{
    [SerializeField] private Vector2 _localPosition;
    [SerializeField] private Color _color = Color.white;
    [SerializeField, HideInInspector] private int _indexSprite;
    [SerializeField, HideInInspector] private int _indexBackground;
    
    public IReadOnlyList<BackgroundContent> Backgrounds;

    public AdditionalImagesToBackground AdditionalImagesToBackground { get; private set; }
    public void ConstructMyAddSpriteNode(List<BackgroundContent> backgrounds, AdditionalImagesToBackground additionalImagesToBackground)
    {
        Backgrounds = backgrounds;
        AdditionalImagesToBackground = additionalImagesToBackground;
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
        Backgrounds[_indexBackground].AddContent(AdditionalImagesToBackground.Additional[_indexSprite], _localPosition, _color);
    }
}