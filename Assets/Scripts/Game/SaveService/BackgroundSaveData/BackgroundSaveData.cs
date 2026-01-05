
using System.Collections.Generic;

[System.Serializable]
public class BackgroundSaveData
{
    public List<string> ArtOpenedKeys;
    public List<BackgroundContentWithAdditionalImage> BackgroundContentWithAdditionalImage;

    public string CurrentKeyBackgroundContent;
}