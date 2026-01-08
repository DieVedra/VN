using System.Collections.Generic;
using UnityEngine;

public class ChoicePanelInitializerBuildMode : IChoicePanelInitializer
{
    public readonly ChoicePanelCasePrefabProvider ChoicePanelCasePrefabProvider;
    public ChoicePanelInitializerBuildMode()
    {
        ChoicePanelCasePrefabProvider = new ChoicePanelCasePrefabProvider();
    }

    public IReadOnlyList<ChoiceCaseView> GetChoiceCaseViews(Transform parent)
    {
        List<ChoiceCaseView> choiceCaseView = new List<ChoiceCaseView>(ChoiceNode.MaxCaseCount);
        for (int i = 0; i <= ChoiceNode.MaxCaseCount; i++)
        {
            choiceCaseView.Add(Object.Instantiate(ChoicePanelCasePrefabProvider.GetPrefab, parent).GetComponent<ChoiceCaseView>());
        }
        ChoicePanelCasePrefabProvider.Unload();
        return choiceCaseView;
    }
}