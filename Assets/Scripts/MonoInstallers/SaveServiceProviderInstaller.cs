
using Zenject;

public class SaveServiceProviderInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SaveServiceProvider>().FromNew().AsSingle();
    }
}