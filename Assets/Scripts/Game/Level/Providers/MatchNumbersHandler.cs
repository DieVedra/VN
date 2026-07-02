using System.Collections.Generic;
using System.Text;

public class MatchNumbersHandler
{
    private const char _minSymbol = '0';
    private const char _maxSymbol = '9';
    private StringBuilder _stringBuilder = new StringBuilder();
    public bool CheckMatchNumbersSeriaWithNumberAsset(ref string resultNameAsset, List<string> names, int number)
    {
        bool result = false;
        resultNameAsset = null;
        char symbol;
        foreach (var name in names)
        {
            _stringBuilder.Clear();
            for (int i = 0; i < name.Length; i++)
            {
                symbol = name[i];
                if (symbol >= _minSymbol && symbol <= _maxSymbol)
                {
                    _stringBuilder.Append(symbol);
                }
            }
            if (int.TryParse(_stringBuilder.ToString(), out int resultNumber))
            {
                if (resultNumber == number)
                {
                    result = true;
                    resultNameAsset = name;
                    
                    break;
                }
            }
        }
        return result;
    }
}