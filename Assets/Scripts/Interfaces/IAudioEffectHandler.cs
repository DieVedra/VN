using System.Threading;
using Cysharp.Threading.Tasks;

public interface IAudioEffectHandler
{
    public bool EffectIsOn { get; }

    public UniTask SetEffectSmooth(CancellationToken cancellationToken, bool key);
    public void SetLowPassEffect(bool key);
}