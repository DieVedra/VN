using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _progressText;

    [SerializeField] private string Text;
    [SerializeField] private RectTransform _TextPanel;
    [SerializeField] private RectTransform _contentPanel;


    private void Start()
    {
        ContentHeightCalculator _contentHeightCalculator = new ContentHeightCalculator(_progressText);
        Canvas.ForceUpdateCanvases();

        Debug.Log($"_contentPanel1 {_contentPanel.rect.width}     _TextPanel1 {_TextPanel.rect.width}");
        Debug.Log($"_TextPanel.sizeDelta1 {_TextPanel.sizeDelta}");
        
        _TextPanel.sizeDelta = new Vector2(_contentPanel.rect.width, _TextPanel.sizeDelta.y);
        Debug.Log($"_TextPanel.sizeDelta2 {_TextPanel.sizeDelta}");

        _contentHeightCalculator.UpdateTextSize(Text);
        _TextPanel.anchoredPosition = new Vector2(_TextPanel.anchoredPosition.x, -(_TextPanel.sizeDelta.y/2));
        
        
        Debug.Log($"_contentPanel2 {_contentPanel.rect.width}     _TextPanel2 {_TextPanel.rect.width}");

    }

    [Button()]
    private void test1()
    {
        Debug.Log($"_contentPanel.rect.width {_contentPanel.rect.width}     _contentPanel.rect.height {_contentPanel.rect.height}");

    }
    
    [Button()]
    private void test2()
    {
        
    }
    // [Button()]
    // private void test3()
    // {
    //     Debug.Log($"{_spriteAtlas.GetSprite(name).name}");
    //
    // }
}