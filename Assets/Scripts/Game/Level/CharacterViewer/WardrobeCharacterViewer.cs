using Cysharp.Threading.Tasks;
using UnityEngine;

public class WardrobeCharacterViewer : BaseCharacterViewer, ICharacterCustomizationView
{
    private const int _sortingOrderDefaultValue = 0;
    private const int _sortingOrderAddableValue = 5;
    private const float _viewerPositionX = 0f;
    private const float _viewerPositionY = -0.6f;
    private const float _wardrobePositionX = -0.58f;
    private const float _wardrobePositionY = 3.12f;
    private const float _wardrobePositionZ = 0f;
    private readonly Vector3 _wardrobePosition = new Vector3(_wardrobePositionX, _wardrobePositionY, _wardrobePositionZ);
    private readonly Vector2 _viewerPosition = new Vector2(_viewerPositionX, _viewerPositionY);
    private SpriteViewer _spriteViewer2;
    private ParticleSystem _particleSystem;
    public int CustomizableCharacterIndex { get; private set; }

    public override void Construct(ViewerCreator viewerCreator)
    {
        ViewerCreator = viewerCreator;
        TryDestroy();
        SpriteViewer1 = CreateViewer();
        TryInitViewer(SpriteViewer1);
        _spriteViewer2 = CreateViewer();
        TryInitViewer(_spriteViewer2);
        transform.position = _wardrobePosition;
    }
    public void Construct(DisableNodesContentEvent disableNodesContentEvent, ViewerCreator viewerCreator)
    {
        CompositeDisposable = disableNodesContentEvent.SubscribeWithCompositeDisposable(ResetCharacterView);
        Construct(viewerCreator);
    }
    public override void Dispose()
    {
        base.Dispose();
        SpriteViewer1.Dispose();
        _spriteViewer2.Dispose();
    }

    public async UniTask SetCharacterCustomizationFromRightArrow(CustomizationData newCustomizationData)
    {
        SetCharacterCustomization(newCustomizationData);
        await UniTask.WhenAll(
            SpriteViewer1.DisappearanceCharacterOnCustomization(DirectionType.Left),
            _spriteViewer2.EmergenceCharacterOnCustomization(DirectionType.Left));
        SpriteViewer1.SetCharacterView(newCustomizationData);
    }

    public async UniTask SetCharacterCustomizationFromLeftArrow(CustomizationData newCustomizationData)
    {
        SetCharacterCustomization(newCustomizationData);
        await UniTask.WhenAll(
            SpriteViewer1.DisappearanceCharacterOnCustomization(DirectionType.Right),
            _spriteViewer2.EmergenceCharacterOnCustomization(DirectionType.Right));
        SpriteViewer1.SetCharacterView(newCustomizationData);
    }

    public void SetCharacterCustomization(CustomizationData newCustomizationData)
    {
        _spriteViewer2.SetCharacterView(newCustomizationData);
    }

    public void InitParticleSystem(ParticleSystem particleSystem)
    {
        _particleSystem = particleSystem;
    }

    public void PlayPSEndCustomizationEffect()
    {
        _particleSystem.Play();
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

    public void SetCustomizableCharacterIndex(int index)
    {
        CustomizableCharacterIndex = index;
    }
}