
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IBackgroundProviderToShowArtNode
{
    public UniTask ShowImageInPlayMode(string keyArt, CancellationToken cancellationToken);
    public UniTask HideImageInPlayMode(CancellationToken cancellationToken);
    public void ShowArtImage(string keyArt);
    public IReadOnlyDictionary<string, Sprite> GetArtsSpritesDictionary { get; }
}