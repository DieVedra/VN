
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class MatchNumbersHandler
{
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

                Debug.Log($"MatchNumbersHandler  {stringBuilder.ToString()}");
                if (int.TryParse(stringBuilder.ToString(), out int resultNumber))
                {
                    Debug.Log($"resultNumber {resultNumber}      number {number}");

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