
using System.Collections.Generic;

[System.Serializable]
public class BackgroundSaveData
{
    public List<string> ArtOpenedKeys;
    public Dictionary<string, List<AdditionalImageData>> AdditionalImagesInfo;

    public string CurrentKeyBackgroundContent;
}