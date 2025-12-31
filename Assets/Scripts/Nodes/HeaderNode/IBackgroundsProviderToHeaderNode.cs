
using System.Collections.Generic;

public interface IBackgroundsProviderToHeaderNode
{
    public List<BackgroundContent> GetBackgroundContent { get; }
    public IReadOnlyDictionary<string, BackgroundContent> GetBackgroundContentDictionary { get; }
    public BackgroundPosition CurrentBackgroundPosition { get; }
    public void SetBackgroundPosition(BackgroundPosition backgroundPosition, string key);

    public void SetBackgroundPositionFromSlider(float positionValue, string key);
    public string CurrentKeyBackgroundContent { get; }
}