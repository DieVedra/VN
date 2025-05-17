
using UnityEngine;
using Zenject;

public class LoadScreenInstaller : MonoInstaller
{
    [SerializeField] private Transform _projectContextParent;
    public override void InstallBindings()
    {
        LoadScreenUIHandler loadScreenUIHandler = new LoadScreenUIHandler(_projectContextParent);
        Container.Bind<LoadScreenUIHandler>().FromInstance(loadScreenUIHandler).AsSingle();
    }
}