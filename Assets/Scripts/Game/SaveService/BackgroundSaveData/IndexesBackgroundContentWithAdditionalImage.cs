using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class IndexesBackgroundContentWithAdditionalImage
{
    public int IndexBackgroundContent;
    public List<AdditionalImageData> IndexesAdditionalImages;
}

[Serializable]
public class AdditionalImageData
{
    public int IndexAdditionalImage;
    public Vector2 LocalPosition;
    public Color Color;
}