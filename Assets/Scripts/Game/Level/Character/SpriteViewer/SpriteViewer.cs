using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteViewer : MonoBehaviour
{
    private readonly Vector2 _scale = new Vector2(496f,702f);

    private SpriteViewerMaterialPropertiesNames _spriteViewerMaterialPropertiesNames;
    private SpriteRenderer _spriteRenderer;
    private Material _material;
    
    
    private CharacterAnimations _characterAnimations;
    private CancellationTokenSource _cancellationTokenSource;
    public CharacterAnimations CharacterAnimations => _characterAnimations;
    public SpriteRenderer SpriteRenderer => _spriteRenderer;
    
    public void Construct(CharacterAnimations characterAnimations, SpriteRenderer spriteRenderer)
    {
        _spriteViewerMaterialPropertiesNames = new SpriteViewerMaterialPropertiesNames();

        _characterAnimations = characterAnimations;
        _spriteRenderer = spriteRenderer;
        if (Application.isPlaying == false)
        {
            _material = _spriteRenderer.sharedMaterial;
        }
        else
        {
            _material = _spriteRenderer.material;
        }

        var transform1 = transform;
        transform1.localScale = _scale;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }

    public async UniTask DisappearanceCharacterOnCustomization(DirectionType type)
    {
        await _characterAnimations.DisappearanceChar(_cancellationTokenSource.Token, type);
    }
    
    public async UniTask EmergenceCharacterOnCustomization(DirectionType type)
    {
        await _characterAnimations.EmergenceChar(_cancellationTokenSource.Token, type);
    }
    public void SetCharacterView(CustomizationData customizationData)
    {
        SetLookTexture(customizationData.LookSprite);
        SetEmotionTexture(customizationData.EmotionSprite);
        SetHairstyleTexture(customizationData.HairstyleSprite);
        SetClothesTexture(customizationData.ClothesSprite);
    }

    public void SetColorCharacter(Color color)
    {
        _spriteRenderer.color = color;
    }
    public void ResetCharacterView()
    {
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddLookFloatProperty, 0f);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddEmotionFloatProperty, 0f);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddHairstyleFloatProperty, 0f);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddClothesFloatProperty, 0f);
        _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameLookTextureProperty, null);
        _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameEmotionTextureProperty, null);
        _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameHairstyleTextureProperty, null);
        _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameClothesTextureProperty, null);
    }
    public void SetLookTexture(MySprite mySprite)
    {
        _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameLookTextureProperty, mySprite.texture);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddLookFloatProperty, 1f);
    }
    public void SetEmotionTexture(MySprite mySprite)
    {
        if (mySprite == null)
        {
            _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameEmotionTextureProperty, null);
            _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddEmotionFloatProperty, 0f);
        }
        else
        {
            _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameEmotionTextureProperty, mySprite.texture);
            _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddEmotionFloatProperty, 1f);
            _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameEmotionPositionXFloatProperty, mySprite.OffsetXValue);
            _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameEmotionPositionYFloatProperty, mySprite.OffsetYValue);
            _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameEmotionScaleFloatProperty, mySprite.ScaleValue);
        }
    }
    public void SetHairstyleTexture(MySprite mySprite)
    {
        _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameHairstyleTextureProperty, mySprite.texture);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddHairstyleFloatProperty, 1f);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameHairstylePositionXFloatProperty, mySprite.OffsetXValue);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameHairstylePositionYFloatProperty, mySprite.OffsetYValue);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameHairstyleScaleFloatProperty, mySprite.ScaleValue);
    }
    public void SetClothesTexture(MySprite mySprite)
    {
        _material.SetTexture(_spriteViewerMaterialPropertiesNames.NameClothesTextureProperty, mySprite.texture);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameAddClothesFloatProperty, 1f);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameClothesPositionXFloatProperty, mySprite.OffsetXValue);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameClothesPositionYFloatProperty, mySprite.OffsetYValue);
        _material.SetFloat(_spriteViewerMaterialPropertiesNames.NameClothesScaleFloatProperty, mySprite.ScaleValue);
    }
}