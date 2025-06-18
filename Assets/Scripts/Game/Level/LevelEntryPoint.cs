
using UniRx;
using UnityEngine;

public abstract class LevelEntryPoint : MonoBehaviour
{
    [SerializeField] protected GameObject EventSystem;
    [SerializeField] protected CharacterViewer CharacterViewer;
    [SerializeField] protected WardrobeCharacterViewer WardrobeCharacterViewer;

    [SerializeField] protected LevelUIView LevelUIView;
    [SerializeField, Space(10f)] protected int TestMonets;
    [SerializeField, Space(10f)] protected int TestHearts;
    
    [SerializeField] protected bool LoadSaveData;

    protected Wallet Wallet;
    protected SaveData SaveData;
    protected StoryData StoryData;
    protected SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    protected ReactiveCommand OnSceneTransition;
    protected LevelUIProvider LevelUIProvider;
    protected NodeGraphInitializer NodeGraphInitializer;
    protected DisableNodesContentEvent DisableNodesContentEvent;
    protected SwitchToNextNodeEvent SwitchToNextNodeEvent;
    protected SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;
    protected PrefabsProvider PrefabsProvider;
    protected SaveServiceProvider SaveServiceProvider;

    protected abstract void InitWardrobeCharacterViewer(ViewerCreator viewerCreator);
    protected abstract void InitGlobalSound();

    protected abstract void InitBackground();

    protected virtual void Dispose()
    {
        CharacterViewer.Dispose();
        LevelUIProvider.Dispose();
        // EventSystem.gameObject.SetActive(false);
    }
}