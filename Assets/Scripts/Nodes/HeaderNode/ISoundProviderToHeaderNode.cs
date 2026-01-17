using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISoundProviderToHeaderNode
{
    public SmoothAudio SmoothAudio { get;}
    public IReadOnlyDictionary<string, AudioClip> GetMusicDictionary { get;}

    public UniTask SmoothPlayHeaderAudio(string audioClipKey, CancellationToken cancellationToken);
}