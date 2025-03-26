using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

public class LevelSound : Sound
{
    [SerializeField, Expandable] protected AudioData _audioData;
    public override void Init(bool soundOn = true)
    {
        base.Init(soundOn);
        AudioData = _audioData;
    }
}