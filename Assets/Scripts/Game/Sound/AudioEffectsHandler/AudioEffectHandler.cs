
using System.Threading;
using Cysharp.Threading.Tasks;

public abstract class AudioEffectHandler
{
    public bool EffectIsOn { get; private set; }

    public abstract UniTask SetEffectSmooth(CancellationToken cancellationToken, bool key);
    public abstract void SetLowPassEffect(bool key);

}