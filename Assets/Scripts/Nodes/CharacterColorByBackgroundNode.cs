
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterColorByBackgroundNode : BaseNode
{
    [SerializeField] private Color _color = Color.white;
    private CharacterViewer _characterViewer;

    public void Construct(CharacterViewer characterViewer)
    {
        _characterViewer = characterViewer;
    }

    public override UniTask Enter(bool isMerged = false)
    {
        SetInfoToView();
        return default;
    }

    protected override void SetInfoToView()
    {
        _characterViewer.SetColorByBackground(_color);
    }
}