using Cysharp.Threading.Tasks;
using UnityEngine;

public class WardrobeCharacterViewer : BaseCharacterViewer, ICharacterCustomizationView
{
    private readonly int _sortingOrderDefaultValue = 0;
    private readonly int _sortingOrderAddableValue = 5;
    private readonly Vector3 _wardrobePosition = new Vector3(-0.58f,3.12f,0f);
    private readonly Vector2 _viewerPosition = new Vector2(0f, -0.6f);

    private SpriteViewer _spriteViewer2;

    public override void Construct(DisableNodesContentEvent disableNodesContentEvent, ViewerCreator viewerCreator)
    {
        ViewerCreator = viewerCreator;
        disableNodesContentEvent.Subscribe(ResetCharacterView);
        TryDestroy();
        SpriteViewer1 = CreateViewer();
        TryInitViewer(SpriteViewer1);
        _spriteViewer2 = CreateViewer();
        TryInitViewer(_spriteViewer2);
        transform.position = _wardrobePosition;
    }
    public override void Dispose()
    {
        SpriteViewer1.Dispose();
        _spriteViewer2.Dispose();
    }

    public async UniTask SetCharacterCustomizationFromRightArrow(CustomizationData newCustomizationData)
    {
        SetCharacterCustomization(newCustomizationData);
        // await UniTask.WhenAll(
        //     SpriteViewer1.DisappearanceCharacterOnCustomization(MoveType.RightToLeft),
        //     _spriteViewer2.EmergenceCharacterOnCustomization(MoveType.RightToLeft));
        await UniTask.WhenAll(
            SpriteViewer1.DisappearanceCharacterOnCustomization(DirectionType.Left),
            _spriteViewer2.EmergenceCharacterOnCustomization(DirectionType.Left));
        SpriteViewer1.SetCharacterView(newCustomizationData);
    }

    public async UniTask SetCharacterCustomizationFromLeftArrow(CustomizationData newCustomizationData)
    {
        SetCharacterCustomization(newCustomizationData);
        // await UniTask.WhenAll(
        //     SpriteViewer1.DisappearanceCharacterOnCustomization(MoveType.LeftToRight),
        //     _spriteViewer2.EmergenceCharacterOnCustomization(MoveType.LeftToRight));
        await UniTask.WhenAll(
            SpriteViewer1.DisappearanceCharacterOnCustomization(DirectionType.Right),
            _spriteViewer2.EmergenceCharacterOnCustomization(DirectionType.Right));
        SpriteViewer1.SetCharacterView(newCustomizationData);
    }

    public void SetCharacterCustomization(CustomizationData newCustomizationData)
    {
        _spriteViewer2.SetCharacterView(newCustomizationData);
    }

    protected override void TryInitViewer(SpriteViewer spriteViewer)
    {
        SpriteRenderer spriteRenderer = spriteViewer.GetComponent<SpriteRenderer>();
        Transform transformSR = spriteViewer.transform;
        spriteRenderer.flipX = false;
        transformSR.localPosition = _viewerPosition;
        spriteRenderer.sortingOrder = spriteRenderer.sortingOrder - _sortingOrderAddableValue;

        CharacterAnimations characterAnimations = new CharacterAnimationsOnCustomization(transformSR, spriteRenderer,
            _timeEmergence, _timeDisappearance, _sortingOrderDefaultValue, _sortingOrderAddableValue);
        spriteViewer.Construct(characterAnimations, spriteRenderer);
    }
}