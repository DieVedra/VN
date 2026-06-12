
using Cysharp.Threading.Tasks;

public interface ISaveMethod
{
    public UniTask Construct();

    public UniTask<T> Load<T>();
    public UniTask Save(SaveData data);
}
