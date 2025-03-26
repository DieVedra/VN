using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewHandler : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Scrollbar _scrollBar;
    
    [SerializeField, Range(-0.2f,1.2f)] private float _value = 0f;
    
    private ReactiveProperty<float> _reactiveProperty;
    [SerializeField] private Vector2 _vector2;
    // public void test()
    // {
    //     Debug.Log($" _scrollRect.velocity { _scrollBar.value}");
    //     // _scrollRect.horizontalScrollbar.
    // }
    // [ButtonExit()]
    // private void getinfo()
    // {
    //     Debug.Log($" _scrollBar.value { _scrollBar.value}");
    //
    //     _scrollRect.velocity = _vector2;
    // }
    // private void OnValidate()
    // {
    //     _scrollBar.value = _value;
    // }
}