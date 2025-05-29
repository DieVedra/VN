
using UnityEngine;
using Zenject;

public class GlobalUIHandlerInstaller : MonoInstaller
{
    [SerializeField] private Transform _projectContextParent;

    public override void InstallBindings()
    {
        GlobalUIHandler globalUIHandler = new GlobalUIHandler(_projectContextParent);
        Container.Bind<GlobalUIHandler>().FromInstance(globalUIHandler).AsSingle();
    }
}