using System.Collections.Generic;
using UnityEngine;

public interface IAdditionalSpritesProviderToNode
{
    public IReadOnlyDictionary<string, BackgroundContentValues> GetBackgroundContentDictionary { get; }
    public IReadOnlyDictionary<string, BackgroundContentValues> GetAdditionalImagesToBackgroundDictionary { get; }
    public void AddAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage, Vector2 localPosition, Color color);
    public void TryRemoveAdditionalSpriteToBackgroundContent(string keyBackground, string keyAdditionalImage);
}