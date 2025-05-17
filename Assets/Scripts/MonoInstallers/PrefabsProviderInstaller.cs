
using Zenject;

public class PrefabsProviderInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PrefabsProvider>().FromNew().AsSingle();
    }
}