using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Audio;

public class LowPassAudioEffect : IAudioEffectHandler
{
    private const string LowPassEffectName = "LevelCutoff";
    private const float _valueOn = 1000f;
    private const float _valueOff = 5000f;
    private const float _duration = 1.5f;
    private readonly AudioMixer _mixer;
    private bool _effectIsOn;
    public bool EffectIsOn => _effectIsOn;

    public LowPassAudioEffect(AudioMixer mixer)
    {
        _mixer = mixer;
    }

    public async UniTask SetEffectSmooth(CancellationToken cancellationToken, bool key)
    {
        if (_effectIsOn != key)
        {
            _effectIsOn = key;
            if (key == true)
            {
                await _mixer.DOSetFloat(LowPassEffectName, _valueOn, _duration).WithCancellation(cancellationToken);
            }
            else
            {
                await _mixer.DOSetFloat(LowPassEffectName, _valueOff, _duration).WithCancellation(cancellationToken);
            }
        }
    }

    public void SetEffect(bool key)
    {
        if (_effectIsOn != key)
        {
            _effectIsOn = key;
            if (key == true)
            {
                _mixer.SetFloat(LowPassEffectName, _valueOn);
            }
            else
            {
                _mixer.SetFloat(LowPassEffectName, _valueOff);
            }
        }
    }
}