using UnityEngine;

public class CharacterViewer : BaseCharacterViewer, ISetLighting
{
    private readonly Vector2 _leftPosition = new Vector2(-0.89f, -0.6f);
    private readonly Vector2 _rightPosition = new Vector2(0.89f, -0.6f);
    private CharacterDirectionView _characterDirectionView;
    public SpriteViewer SpriteViewer => SpriteViewer1;
    public override void Construct(DisableNodesContentEvent disableNodesContentEvent, ViewerCreator viewerCreator)
    {
        ViewerCreator = viewerCreator;
        disableNodesContentEvent.Subscribe(ResetCharacterView);
        TryDestroy();
        SpriteViewer1 = CreateViewer();
        
        
        TryInitViewer(SpriteViewer1);
        ResetCharacterView();
        transform.position = Vector3.zero;
    }

    public override void Dispose()
    {
        SpriteViewer1.Dispose();
    }
    public void SetDirection(DirectionType directionType)
    {
        _characterDirectionView.SetDirection(directionType);
    }

    public void SetColorByBackground(Color color)
    {
        SpriteViewer1.SetColorCharacter(color);
    }
    public void SetLightingColor(Color color)
    {
        SpriteViewer1.SpriteRenderer.color = color;
    }

    protected override void TryInitViewer(SpriteViewer spriteViewer)
    {
        SpriteRenderer spriteRenderer = spriteViewer.GetComponent<SpriteRenderer>();
        _characterDirectionView = new CharacterDirectionView(spriteRenderer, _leftPosition, _rightPosition);
    
        CharacterAnimations characterAnimations = new CharacterAnimationsOnDialog(spriteViewer.transform, spriteRenderer,
            _rightPosition, _leftPosition,
            _timeEmergence, _timeDisappearance);
        spriteViewer.Construct(characterAnimations, spriteRenderer);
        _characterDirectionView.SetDirection(DirectionType.Right);
    }
}
