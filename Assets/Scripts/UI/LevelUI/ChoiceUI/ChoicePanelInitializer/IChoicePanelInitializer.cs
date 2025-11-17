using UnityEngine;

public interface IChoicePanelInitializer
{
    public ChoiceCaseView[] GetChoiceCaseViews(Transform parent);
}