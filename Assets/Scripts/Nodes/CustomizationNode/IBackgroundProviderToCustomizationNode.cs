
using System.Collections.Generic;

public interface IBackgroundProviderToCustomizationNode
{
    public void EnableWardrobeBackground(float positionValue, string backgroundKey);
    public void DisableWardrobeBackground();
    public IReadOnlyDictionary<string, BackgroundContentValues> GetWardrobeBackgroundContentValuesDictionary { get; }
}