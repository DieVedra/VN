using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;

public class TextConsistentlyViewer
{
    private readonly TextMeshProUGUI _textComponent;
    private readonly StringBuilder _stringBuilder;
    private const float _delay = 0.02f;
    private const char _separator = ' ';
    private const string _spaceColor = "<color=#00000000>";
    private const string _endSpaceColor = "</color>";
    private CancellationTokenSource _cancellationTokenSource;

    public bool IsRun { get; private set; }
    public TextConsistentlyViewer(TextMeshProUGUI textComponent)
    {
        _textComponent = textComponent;
        _stringBuilder = new StringBuilder();
        IsRun = false;
    }

    public void TryStop()
    {
        _cancellationTokenSource?.Cancel();
        IsRun = false;
    }
    public void ClearText()
    {
        _textComponent.text = _separator.ToString();
    }
    public async UniTask SetTextConsistently(string text)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        await SetTextConsistently(_cancellationTokenSource.Token, text);
    }
    public async UniTask SetTextConsistently(CancellationToken token, string text)
    {
        IsRun = true;
        ClearText();
        _stringBuilder.Clear();
        _stringBuilder.Append(text);
        List<string> consistentlyStrings = CreateConsistentlyStrings(CreateIndexes(), text);
        _textComponent.havePropertiesChanged = true;
        for (int i = 0; i < consistentlyStrings.Count; i++)
        {
            ClearText();
            _textComponent.gameObject.SetActive(false);
            _textComponent.text = consistentlyStrings[i];
            _textComponent.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: token);
        }
        IsRun = false;
    }
    private List<string> CreateConsistentlyStrings(List<int> indexes, string text)
    {
        List<string> consistentlyStrings = new List<string>(indexes.Count);
        _stringBuilder.Clear();
        for (int i = 0; i < indexes.Count; i++)
        {
            _stringBuilder.Append(text);
            _stringBuilder.Insert(indexes[i], _spaceColor);
            _stringBuilder.Append(_endSpaceColor);
            consistentlyStrings.Add(_stringBuilder.ToString());
            _stringBuilder.Clear();
        }
        consistentlyStrings.Add(text);
        return consistentlyStrings;
    }

    private List<int> CreateIndexes()
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < _stringBuilder.Length; i++)
        {
            if (_stringBuilder[i] == _separator)
            {
                if (i != _stringBuilder.Length - 1)
                {
                    indexes.Add(i);
                }
            }
        }
        return indexes;
    }
}