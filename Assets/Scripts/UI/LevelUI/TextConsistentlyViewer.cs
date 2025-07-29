using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;

public class TextConsistentlyViewer
{
    private readonly TextMeshProUGUI _textComponent;
    private readonly StringBuilder _stringBuilder;
    private const float _delay = 0.027f;
    private const string _space = " ";

    public TextConsistentlyViewer(TextMeshProUGUI textComponent)
    {
        _textComponent = textComponent;
        _stringBuilder = new StringBuilder();
    }

    public void ClearText()
    {
        _textComponent.text = _space;
    }
    public async UniTask SetTextConsistently(CancellationToken cancellationToken, string text)
    {
        ClearText();
        char separator = ' ';
        string[] words = text.Split(separator);
        _textComponent.text = String.Empty;
        for (int i = 0; i < words.Length; i++)
        {
            _stringBuilder.Append(words[i]);
            _stringBuilder.Append(_space);
            _textComponent.text = _stringBuilder.ToString();
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: cancellationToken);
        }
        _stringBuilder.Clear();
    }
}