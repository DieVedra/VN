using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Transform _parent;
    public Button Button => _button;
    public Transform Parent => _parent;
}