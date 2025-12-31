using System;
using System.Collections.Generic;

[Serializable]
public class BackgroundContentWithAdditionalImage
{
    public string KeyBackgroundContent;
    public IReadOnlyDictionary<string, AdditionalImageData> DataAdditionalImages;
}