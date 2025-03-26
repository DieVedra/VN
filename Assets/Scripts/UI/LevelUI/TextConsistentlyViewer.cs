using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextConsistentlyViewer
{
    private readonly TextMeshProUGUI _textComponent;
    private readonly float _delay = 0.027f;

    public TextConsistentlyViewer(TextMeshProUGUI textComponent)
    {
        _textComponent = textComponent;
    }

    public void ClearText()
    {
        _textComponent.text = " ";
    }
    public async UniTask SetTextConsistently(CancellationToken cancellationToken, string text)
    {
        ClearText();
        char separator = ' ';
        string[] words = text.Split(separator);
        _textComponent.text = " ";
        for (int i = 0; i < words.Length; i++)
        {
            _textComponent.text = $"{_textComponent.text} {words[i]}";
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: cancellationToken);
        }
    }
}