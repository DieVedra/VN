using UnityEngine;

public class ChoicePanelInitializerEditMode : IChoicePanelInitializer
{
    private readonly ChoiceCaseView[] _choiceCases;

    public ChoicePanelInitializerEditMode(ChoiceCaseView[] choiceCases)
    {
        _choiceCases = choiceCases;
    }
    public ChoiceCaseView[] GetChoiceCaseViews(Transform parent)
    {
        return _choiceCases;
    }
}