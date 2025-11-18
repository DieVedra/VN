using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextConsistentlyViewer
{
    private readonly TextMeshProUGUI _textComponent;
    private readonly StringBuilder _stringBuilder;
    private const float _delay = 0.02f;
    private List<string> _consistentlyStrings;
    private List<int> _indexes;
    private int _count;
    private CancellationTokenSource _cancellationTokenSource;

    public bool IsRun { get; private set; }
    public TextConsistentlyViewer(TextMeshProUGUI textComponent)
    {
        _textComponent = textComponent;
        _stringBuilder = new StringBuilder();
        _consistentlyStrings = new List<string>();
        _indexes = new List<int>();
        IsRun = false;
    }

    public void TryStop()
    {
        _cancellationTokenSource?.Cancel();
        IsRun = false;
    }
    public void ClearText()
    {
        _textComponent.text = FirstLineCustomizer.Separator.ToString();
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
        Create(text);
        _textComponent.havePropertiesChanged = true;
        _count = _consistentlyStrings.Count;
        for (int i = 0; i < _count; i++)
        {
            ClearText();
            _textComponent.gameObject.SetActive(false);
            _textComponent.text = _consistentlyStrings[i];
            _textComponent.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: token);
        }
        IsRun = false;

        void Create(string text1)
        {
            CreateIndexes(text1);
            CreateConsistentlyStrings(text1);
        }
    }
    private void CreateConsistentlyStrings(string text)
    {
        _consistentlyStrings.Clear();
        _stringBuilder.Clear();
        for (int i = 0; i < _indexes.Count; i++)
        {
            _stringBuilder.Append(text);
            _stringBuilder.Insert(_indexes[i], FirstLineCustomizer.SpaceColor);
            _stringBuilder.Append(FirstLineCustomizer.EndSpaceColor);
            _consistentlyStrings.Add(_stringBuilder.ToString());
            _stringBuilder.Clear();
        }
        _consistentlyStrings.Add(text);
    }

    private void CreateIndexes(string text)
    {
        _indexes.Clear();
        _stringBuilder.Clear();
        _stringBuilder.Append(text);
        _count = _stringBuilder.Length;
        for (int i = 0; i < _count; i++)
        {
            if (_stringBuilder[i] == FirstLineCustomizer.Separator)
            {
                if (i != _count - 1)
                {
                    _indexes.Add(i);
                }
            }
        }
    }
}