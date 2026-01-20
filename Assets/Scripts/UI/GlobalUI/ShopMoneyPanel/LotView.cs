using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LotView : MonoBehaviour
{
    [SerializeField] private int _resource;
    [SerializeField] private float _price;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _textPrice;
    [SerializeField] private TextMeshProUGUI _textResource;
    [SerializeField] private TextMeshProUGUI _textСurrency;
    
    public int Resource => _resource;
    public float Price => _price;
    public Button Button => _button;
    public TextMeshProUGUI TextPrice => _textPrice;
    public TextMeshProUGUI TextResource => _textResource;
    public TextMeshProUGUI TextСurrency => _textСurrency;
}