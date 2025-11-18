using System.Text;
using TMPro;

public class FirstLineCustomizer
{
    public const string SpaceColor = "<color=#00000000>";
    public const string EndSpaceColor = "</color>";
    public const char Separator = ' ';

    private const char _spaceSymbol = '-';
    private const int _maxCountSymbolsInLine = 23;
    private const int _minCountSymbolsInLine = 19;
    private const int _firstIndex = 0;
    private readonly StringBuilder _stringBuilder = new StringBuilder();

    public bool CheckChangeFirstLineLength(out string result, TextMeshProUGUI textComponent, string text)
    {
        textComponent.ForceMeshUpdate();
        TMP_LineInfo firstLine = textComponent.textInfo.lineInfo[_firstIndex];
        if (firstLine.characterCount > _minCountSymbolsInLine)
        {
            int index = 0;
            _stringBuilder.Clear();
            _stringBuilder.Append(text);
            for (int i = firstLine.characterCount - 1; i >= 0; i--)
            {
                if (i <= _minCountSymbolsInLine && _stringBuilder[i] == Separator)
                {
                    index = i;
                    break;
                }
            }
            var diff = _maxCountSymbolsInLine - index;
            int firstOffset = diff;
            int endOffset = diff;
            string bufer = _stringBuilder.ToString();
            string ghostlineEnd = GetGhostLine(_spaceSymbol.ToString(), endOffset);
            string ghostlineStart = GetGhostLine(_spaceSymbol.ToString(), firstOffset);
            _stringBuilder.Clear();
            _stringBuilder.Append(bufer);
            _stringBuilder.Insert(index, ghostlineEnd);
            _stringBuilder.Insert(_firstIndex, ghostlineStart);
            result = _stringBuilder.ToString();
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    public string GetGhostLine(string line, int count = 1)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(SpaceColor);
        for (int i = 0; i < count; i++)
        {
            _stringBuilder.Append(line);
        }
        _stringBuilder.Append(EndSpaceColor);
        return _stringBuilder.ToString();
    }
}