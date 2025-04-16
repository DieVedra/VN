
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEffectsCustodian
{
    // private readonly string LowPassEffectName = "LevelCutoff";
    // private readonly AudioMixer _mixer;
    private Dictionary<AudioEffect, IAudioEffectHandler> _dictionaryEffects;
    public bool LowPassEffectIsOn { get; private set; }
    public AudioEffectsCustodian(AudioMixer mixer)
    {
        _dictionaryEffects = new Dictionary<AudioEffect, IAudioEffectHandler>()
        {
            {AudioEffect.LowPass, new LowPassAudioEffect(mixer)}
        };
    }

    public IAudioEffectHandler GetAudioEffect(AudioEffect effect)
    {
        return _dictionaryEffects[effect];
    }
    // public async UniTask SetLowPassEffectSmooth(CancellationToken cancellationToken, bool key)
    // {
    //     LowPassEffectIsOn = key;
    //     if (key)
    //     {
    //         await _mixer.DOSetFloat(LowPassEffectName, 1000f, 1.5f).WithCancellation(cancellationToken);
    //     }
    //     else
    //     {
    //         await _mixer.DOSetFloat(LowPassEffectName, 5000f, 1.5f).WithCancellation(cancellationToken);
    //     }
    // }
    // public void SetLowPassEffect(bool key)
    // {
    //     LowPassEffectIsOn = key;
    //     if (key)
    //     {
    //         _mixer.SetFloat(LowPassEffectName, 1000f);
    //     }
    //     else
    //     {
    //         _mixer.SetFloat(LowPassEffectName, 5000f);
    //     }
    // }
}