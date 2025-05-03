using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

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