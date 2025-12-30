using System.Collections.Generic;
using UnityEngine;

public interface IAdditionalSpritesProviderToNode
{
    public IReadOnlyDictionary<string, BackgroundContent> GetBackgroundContentDictionary { get; }
    public IReadOnlyDictionary<string, Sprite> GetAdditionalImagesToBackgroundDictionary { get; }
    public List<BackgroundContent> GetBackgroundContent { get; }
    public IReadOnlyList<Sprite> GetAdditionalImagesToBackground { get; }
    public void AddAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage, Vector2 localPosition, Color color);
    public void TryRemoveAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage);
}