using System.Collections.Generic;
using System.Text;
using TMPro;

public class DescriptionCutter
{
    private const int _maxLines = 5;
    private const string _end = "...";
    private const char _separator = ' ';
    private readonly LineBreaksCountCalculator _lineBreaksCountCalculator = new LineBreaksCountCalculator();
    private readonly StringBuilder _stringBuilder = new StringBuilder();
    
    public void TryCutAndSet(TextMeshProUGUI textComponent, string text)
    {
        var lineCount = _lineBreaksCountCalculator.GetLineBreaksCount(textComponent, text);
        if (lineCount > _maxLines)
        {
            List<string> strings = new List<string>();
            DivisionIntoWords(strings, text);
            var targetWordsCount = CalculateWordsCount(lineCount, strings.Count);
            do
            {
                BuildString(strings, targetWordsCount);
                lineCount = _lineBreaksCountCalculator.GetLineBreaksCount(textComponent, _stringBuilder.ToString());
                targetWordsCount--;
            } while (lineCount > _maxLines);
        
        }
        
        if (_stringBuilder.Length > 0)
        {
            textComponent.text = _stringBuilder.ToString();
            _stringBuilder.Clear();
        }
        else
        {
            textComponent.text = text;
        }
    }

    private void DivisionIntoWords(List<string> strings, string text)
    {
        int count = text.Length;
        char let;
        for (int i = 0; i < count; i++)
        {
            let = text[i];
            if (let != _separator)
            {
                _stringBuilder.Append(let);
            }
            else
            {
                Add();
            }
        }
        Add();
        void Add()
        {
            if (_stringBuilder.Length > 0)
            {
                strings.Add(_stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }
    }

    private int CalculateWordsCount(int lineCount, int wordsCount)
    {
        return (_maxLines * wordsCount) / lineCount;
    }

    private void BuildString(List<string> strings, int targetWordsCount)
    {
        _stringBuilder.Clear();
        for (int i = 0; i < targetWordsCount; i++)
        {
            _stringBuilder.Append(strings[i]);
            if (i == targetWordsCount - 1)
            {
                _stringBuilder.Append(_end);
            }
            else
            {
                _stringBuilder.Append(_separator);
            }
        }
    }
}