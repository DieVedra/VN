using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BackgroundContentWithAdditionalImage
{
    public string KeyBackgroundContent;
    public IReadOnlyDictionary<string, AdditionalImageData> DataAdditionalImages;
}

[Serializable]
public struct AdditionalImageData
{
    public Vector2 LocalPosition;
    public Color Color;
}