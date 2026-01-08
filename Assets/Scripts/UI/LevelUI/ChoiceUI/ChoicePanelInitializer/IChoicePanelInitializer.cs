using System.Collections.Generic;
using UnityEngine;

public interface IChoicePanelInitializer
{
    public IReadOnlyList<ChoiceCaseView> GetChoiceCaseViews(Transform parent);
}