using UniRx;
using UnityEngine;

public abstract class BaseCharacterViewer : MonoBehaviour
{
    [SerializeField] protected float _timeEmergence = 0.2f;
    [SerializeField] protected float _timeDisappearance = 0.2f;
    protected ViewerCreator ViewerCreator;
    protected SpriteViewer SpriteViewer1;
    protected CompositeDisposable CompositeDisposable;
    protected abstract void TryInitViewer(SpriteViewer spriteViewer);
    public abstract void Construct(ViewerCreator viewerCreator);

    public virtual void Dispose()
    {
        CompositeDisposable?.Clear();
    }

    public void ResetCharacterView()
    {
        SpriteViewer1?.ResetCharacterView();
        gameObject.SetActive(false);
    }

    public void SetClothes(MySprite clothes)
    {
        SpriteViewer1.SetClothesTexture(clothes);
    }

    public void SetHairstyle(MySprite hairstyle)
    {
        SpriteViewer1.SetHairstyleTexture(hairstyle);
    }

    public void SetLook(MySprite look)
    {
        SpriteViewer1.SetLookTexture(look);
    }

    public void SetEmotion(MySprite emotion)
    {
        SpriteViewer1.SetEmotionTexture(emotion);
    }

    protected SpriteViewer CreateViewer()
    {
        return ViewerCreator.CreateViewer(transform);
    }

    protected void TryDestroy()
    {
        SpriteViewer[] spriteRenderers = gameObject.GetComponentsInChildren<SpriteViewer>();
        for (int i = 0; i < spriteRenderers.Length; ++i)
        {
            if (Application.isPlaying)
            {
                Destroy(spriteRenderers[i].gameObject);
            }
            else
            {
                DestroyImmediate(spriteRenderers[i].gameObject);
            }
        }
    }
}