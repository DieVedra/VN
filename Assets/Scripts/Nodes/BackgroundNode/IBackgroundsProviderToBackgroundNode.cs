using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IBackgroundsProviderToBackgroundNode
{
    public IReadOnlyDictionary<string, BackgroundContent> GetBackgroundContentDictionary { get; }
    // List<BackgroundContent> GetBackgroundContent  { get; }
    public UniTask SmoothBackgroundChangePosition(CancellationToken cancellationToken, BackgroundPosition backgroundPosition, string key);

    public UniTask SmoothChangeBackground(string keyTo, float duration, BackgroundPosition toBackgroundPosition, CancellationToken cancellationToken);

    public UniTask SetColorOverlayBackground(Color color, CancellationToken cancellationToken, float duration, bool enable);
    public void SmoothChangeBackgroundEmmidiately(string keyTo, BackgroundPosition toBackgroundPosition);
    public void SetBackgroundPosition(BackgroundPosition backgroundPosition, string key);
    public void SetColorOverlayBackground(Color color, bool enable);
}