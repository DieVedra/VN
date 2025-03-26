
using UnityEngine;

public class MainMenuUIView : MonoBehaviour
{
    [SerializeField] private BlackFrameView _blackFrameView;
    [SerializeField] private MyScroll _myScroll;
    public BlackFrameView BlackFrameView => _blackFrameView;
    public MyScroll MyScroll => _myScroll;
}