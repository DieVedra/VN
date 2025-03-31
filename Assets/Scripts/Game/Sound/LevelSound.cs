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
    public override void Init(bool soundOn)
    {
        base.Init(soundOn);
        for (int i = 0; i < _audioDatas.Count; ++i)
        {
            AddAudioClips(_audioDatas[i].Clips);
        }
    }

    
}