using UnityEngine;
using Zenject;

public class GlobalSoundInstaller : MonoInstaller
{
    [SerializeField] private GlobalSound _globalSound;
    
    public override void InstallBindings()
    {
        Container.Bind<GlobalSound>().FromInstance(_globalSound).AsSingle();
    }
}