
using System.Collections.Generic;

[System.Serializable]
public class BackgroundSaveData
{
    public List<int> ArtOpenedIndexes;
    public List<BackgroundContentWithAdditionalImage> BackgroundContentWithAdditionalImage;

    public string CurrentKeyBackgroundContent;
}