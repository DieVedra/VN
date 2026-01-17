using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioEffectsCustodian
{
    private readonly string LowPassEffectName = "LevelCutoff";
    private readonly AudioMixer _mixer;
    private Dictionary<AudioEffect, IAudioEffectHandler> _dictionaryEffects;
    public AudioEffectsCustodian(AudioMixer mixer)
    {
        _dictionaryEffects = new Dictionary<AudioEffect, IAudioEffectHandler>()
        {
            {AudioEffect.LowPass, new LowPassAudioEffect(mixer)}
        };
    }

    public void TryEnableEffectsFromSave(List<AudioEffect> enabledEffects)
    {
        foreach (var effect in enabledEffects)
        {
            if (_dictionaryEffects.TryGetValue(effect, out var value))
            {
                value.SetEffect(true);
            }
        }
    }

    public List<AudioEffect> GetEnableEffectsToSave()
    {
        List<AudioEffect> effects = null;
        foreach (var pair in _dictionaryEffects)
        {
            if (pair.Value.EffectIsOn == true)
            {
                effects ??= new List<AudioEffect>();
                effects.Add(pair.Key);
            }
        }
        return effects;
    }
    public IAudioEffectHandler GetAudioEffect(AudioEffect effect)
    {
        return _dictionaryEffects[effect];
    }
}