using System.Collections.Generic;
using UnityEngine;
public struct IndexesBackgroundContentWithAdditionalImage
{
    public int IndexBackgroundContent;
    public List<AdditionalImageData> IndexesAdditionalImages;
}

public struct AdditionalImageData
{
    public int IndexAdditionalImage;
    public Vector2 LocalPosition;
    public Color Color;
}