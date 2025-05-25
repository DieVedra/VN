using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopMoneyPanelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMoney;
    [SerializeField] private TextMeshProUGUI _textHearts;
    [SerializeField] private Button _buttonMonet;
    [SerializeField] private Button _buttonHearts;
    [SerializeField] private Button _exitButton;
    [SerializeField] private RectTransform _heartsPanel;
    [SerializeField] private RectTransform _monetPanel;
    [SerializeField] private TextMeshProUGUI _monetButtonText;
    [SerializeField] private TextMeshProUGUI _heartsButtonText;
    [SerializeField] private List<LotView> _monetLots;
    [SerializeField] private List<LotView> _heartLots;
    
    public TextMeshProUGUI TextMoney => _textMoney;
    public TextMeshProUGUI TextHearts => _textHearts;
    public TextMeshProUGUI MonetButtonText => _monetButtonText;
    public TextMeshProUGUI HeartsButtonText => _heartsButtonText;
    public Button ButtonMonet => _buttonMonet;
    public Button ButtonHearts => _buttonHearts;
    public Button ExitButton => _exitButton;
    public RectTransform HeartsPanel => _heartsPanel;
    public RectTransform MonetPanel => _monetPanel;
    
    public IReadOnlyList<LotView> MonetLots => _monetLots;
    public IReadOnlyList<LotView> HeartLots => _heartLots;
}