
using System.Collections.Generic;

[System.Serializable]
public class BackgroundSaveData
{
    public HashSet<string> ArtOpenedKeys;
    public List<BackgroundContentWithAdditionalImage> BackgroundContentWithAdditionalImage;

    public string CurrentKeyBackgroundContent;
}