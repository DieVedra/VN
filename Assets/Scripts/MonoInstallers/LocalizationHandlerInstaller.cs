
using Zenject;

public class LocalizationHandlerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PanelsLocalizationHandler>().FromNew().AsSingle();
    }
}