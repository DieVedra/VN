
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class MatchNumbersHandler
{
    private const char _minSymbol = '0';
    private const char _maxSymbol = '9';
    private StringBuilder _stringBuilder = new StringBuilder();
    public bool CheckMatchNumbersSeriaWithNumberAsset1(out string resultNameAsset, List<string> names, int number)
    {
        bool result = false;
        resultNameAsset = null;
        _stringBuilder.Clear();
        char symbol;
        foreach (var name in names)
        {
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
    public bool CheckMatchNumbersSeriaWithNumberAsset(List<string> names, int number, int index)
    {
        bool result = false;
        if (names.Count > index)
        {
            MatchCollection digits = Regex.Matches(names[index], @"\d");
            if (digits.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < digits.Count; ++i)
                {
                    stringBuilder.Append(digits[i].Value);
                }
                if (int.TryParse(stringBuilder.ToString(), out int resultNumber))
                {
                    if (resultNumber == number)
                    {
                        result = true;
                    }
                }
            }
        }
        return result;
    }
    public int GetMaxCount(params int[] counts)
    {
        int max = 0;
        for (int i = 0; i < counts.Length; ++i)
        {
            if (counts[i] > max)
            {
                max = counts[i];
            }
        }
        return max;
    }
}