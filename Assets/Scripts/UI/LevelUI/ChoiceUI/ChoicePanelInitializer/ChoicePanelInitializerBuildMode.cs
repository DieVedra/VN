using UnityEngine;

public class ChoicePanelInitializerBuildMode : IChoicePanelInitializer
{
    public readonly ChoicePanelCaseAssetProvider ChoicePanelCaseAssetProvider;
    public ChoicePanelInitializerBuildMode()
    {
        ChoicePanelCaseAssetProvider = new ChoicePanelCaseAssetProvider();
    }

    public ChoiceCaseView[] GetChoiceCaseViews(Transform parent)
    {
        ChoiceCaseView[] choiceCaseView = new ChoiceCaseView[ChoiceNode.MaxCaseCount];
        var prefab = ChoicePanelCaseAssetProvider.GetAsset;
        for (int i = 0; i <= ChoiceNode.MaxCaseCount; i++)
        {
            choiceCaseView[i] = Object.Instantiate(prefab, parent);
        }
        ChoicePanelCaseAssetProvider.Unload();
        return choiceCaseView;
    }
}