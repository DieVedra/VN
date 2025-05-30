using UnityEngine;
using UnityEngine.UI;

public class LotView : MonoBehaviour
{
    [SerializeField] private int _resource;
    [SerializeField] private float _price;
    [SerializeField] private Button _button;
    
    public int Resource => _resource;
    public float Price => _price;
    public Button Button => _button;
}