using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class CharacterViewer : BaseCharacterViewer, ISetLighting
{
    private readonly Vector2 _leftPosition = new Vector2(-0.89f, -0.6f);
    private readonly Vector2 _rightPosition = new Vector2(0.89f, -0.6f);
    private CharacterDirectionView _characterDirectionView;
    public SpriteViewer SpriteViewer => SpriteViewer1;
    public override void Construct(ViewerCreator viewerCreator)
    {
        ViewerCreator = viewerCreator;
        TryDestroy();
        SpriteViewer1 = CreateViewer();
        
        
        TryInitViewer(SpriteViewer1);
        ResetCharacterView();
        transform.position = Vector3.zero;
    }

    public void Construct(DisableNodesContentEvent disableNodesContentEvent, ViewerCreator viewerCreator)
    {
        CompositeDisposable = disableNodesContentEvent.SubscribeWithCompositeDisposable(ResetCharacterView);
        Construct(viewerCreator);
    }
    public override void Dispose()
    {
        base.Dispose();
        SpriteViewer1?.Dispose();
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

    public void SetLightingColorOnSmoothChangeBackground(Color color, float time, CancellationToken cancellationToken)
    {
        SpriteViewer1.SpriteRenderer.DOColor(color, time).WithCancellation(cancellationToken).Forget();
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
