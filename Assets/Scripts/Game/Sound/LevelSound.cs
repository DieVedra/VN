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
    [SerializeField, Expandable] private List<AudioData> _audioDatas;
    [SerializeField, Expandable] private List<AudioData> _additionalAudioDatas;
    public override void Init(bool soundOn)
    {
        base.Init(soundOn);
        SetAudioDatas(_audioDatas, _additionalAudioDatas);
    }
}