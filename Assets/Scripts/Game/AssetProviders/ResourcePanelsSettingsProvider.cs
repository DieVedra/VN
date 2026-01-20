using UnityEngine;

[CreateAssetMenu(fileName = "ResourcePanelsSettingsProvider", menuName = "ResourcePanelsSettingsProvider", order = 51)]
public class ResourcePanelsSettingsProvider : ScriptableObject
{
    [SerializeField] private Color _monetPanelColor;
    [SerializeField] private Color _monetPanelButtonColor;
    [SerializeField] private Color _heartsPanelColor;
    [SerializeField] private Color _heartsPanelButtonColor;
    [SerializeField] private Sprite _monetSprite;
    [SerializeField] private Sprite _heartsSprite;

    [SerializeField] private AnimationCurve _heartPositionXWithAddButtonAnimationCurve;
    [SerializeField] private AnimationCurve _heartPositionYWithAddButtonAnimationCurve;

    
    public Color MonetPanelColor => _monetPanelColor;
    public Color MonetPanelButtonColor => _monetPanelButtonColor;
    public Color HeartsPanelColor => _heartsPanelColor;
    public Color HeartsPanelButtonColor => _heartsPanelButtonColor;
    public AnimationCurve HeartPositionXWithAddButtonAnimationCurve => _heartPositionXWithAddButtonAnimationCurve;
    public AnimationCurve HeartPositionYWithAddButtonAnimationCurve => _heartPositionYWithAddButtonAnimationCurve;
    public Sprite MonetSprite => _monetSprite;
    public Sprite HeartsSprite => _heartsSprite;
}