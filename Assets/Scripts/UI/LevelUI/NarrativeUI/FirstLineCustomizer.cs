using System.Text;
using TMPro;
using UnityEngine;

public class FirstLineCustomizer
{
    public const string SpaceColor = "<color=#00000000>";
    public const string EndSpaceColor = "</color>";
    public const char Separator = ' ';
    private const char _spaceSymbol = '-';
    private const float _maxLengthLine = 520f;
    private const int _firstIndex = 0;
    private readonly StringBuilder _stringBuilder1 = new StringBuilder();
    private readonly StringBuilder _stringBuilder2 = new StringBuilder();
    private bool _firstLineOffset => _narrativePanelUI.FirstLineOffsetKey;
    private TMP_LineInfo _firstLine;
    private AnimationCurve _animationCurve => _narrativePanelUI.AnimationCurve;
    private NarrativePanelUI _narrativePanelUI;
    public FirstLineCustomizer(NarrativePanelUI narrativePanelUI)
    {
        _narrativePanelUI = narrativePanelUI;
    }

    public bool CheckChangeFirstLineLength(out string result, TextMeshProUGUI textComponent, string text)
    {
        if (_firstLineOffset == false)
        {
            result = null;
            return false;
        }
        textComponent.ForceMeshUpdate();
        SetFirstLine(textComponent);
        if (_firstLine.length > _maxLengthLine)
        {
            _stringBuilder1.Clear();
            _stringBuilder2.Clear();
            _stringBuilder1.Append(text);
            FindFirstLineString(textComponent, _firstLine.characterCount - 1);
            string a = _stringBuilder1.ToString();
            TryFillSeparatorsFirstLineString(textComponent);
            _stringBuilder2.Append(a);
            textComponent.ForceMeshUpdate();
            result = _stringBuilder2.ToString();
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    private void FindFirstLineString(TextMeshProUGUI textComponent, int startIndex)
    {
        int delCount;
        for (int i = startIndex; i >= 0; i--)
        {
            if (_stringBuilder1[i] == Separator)
            {
                _stringBuilder2.Append(_stringBuilder1);
                delCount = _stringBuilder2.Length - i;
                _stringBuilder2.Remove(i, delCount);
                textComponent.text = _stringBuilder2.ToString();
                SetFirstLine(textComponent);
                if (_firstLine.length < _maxLengthLine)
                {
                    _stringBuilder1.Remove(_firstIndex, i);
                    break;
                }
            }
        }
    }

    private void TryFillSeparatorsFirstLineString(TextMeshProUGUI textComponent)
    {
        if (textComponent.transform is RectTransform rectTransform)
        {
            float width = rectTransform.sizeDelta.x;
            float countSepar = _animationCurve.Evaluate(width / _firstLine.length);
            int count = (int)countSepar;
            _stringBuilder2.Insert(_firstIndex, GetGhostLine(_spaceSymbol.ToString(), count));
            _stringBuilder2.Append(GetGhostLine(_spaceSymbol.ToString(), count));
        }
        
        
    }
    public string GetGhostLine(string line, int count = 1)
    {
        _stringBuilder1.Clear();
        _stringBuilder1.Append(SpaceColor);
        for (int i = 0; i < count; i++)
        {
            _stringBuilder1.Append(line);
        }
        _stringBuilder1.Append(EndSpaceColor);
        return _stringBuilder1.ToString();
    }

    private void SetFirstLine(TextMeshProUGUI textComponent)
    {
        textComponent.ForceMeshUpdate();
        _firstLine = textComponent.textInfo.lineInfo[_firstIndex];
    }
}