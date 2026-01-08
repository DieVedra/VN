using UnityEngine;
using System.Collections.Generic;

public class ChoicePanelInitializerEditMode : IChoicePanelInitializer
{
    private readonly ChoiceCaseView[] _choiceCases;

    public ChoicePanelInitializerEditMode(ChoiceCaseView[] choiceCases)
    {
        _choiceCases = choiceCases;
    }

    public IReadOnlyList<ChoiceCaseView> GetChoiceCaseViews(Transform parent)
    {
        return _choiceCases;
    }
}