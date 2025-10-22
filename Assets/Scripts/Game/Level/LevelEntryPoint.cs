using UnityEngine;

public abstract class LevelEntryPoint : MonoBehaviour
{
    [SerializeField] protected CharacterViewer CharacterViewer;
    [SerializeField] protected LevelUIView LevelUIView;

    [SerializeField] protected bool LoadSaveData;

    protected const int DefaultSeriaIndex = 0;
    protected StoryData StoryData;
    protected SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    protected OnSceneTransitionEvent OnSceneTransitionEvent;
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
    }
}