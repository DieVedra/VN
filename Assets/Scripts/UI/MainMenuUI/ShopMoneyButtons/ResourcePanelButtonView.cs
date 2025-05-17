using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelButtonView : ResourcePanelView
{
    [SerializeField] private Button _button;

    public Button Button => _button;
}