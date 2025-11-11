
using System;
using System.Collections.Generic;
using UnityEngine;

public class SwitchNodeLogic
{
    private const string _equalSymbol = "=";
    private const string _greatSymbol = ">";
    private const string _lessSymbol = "<";
    private const string _greatEqualSymbol = ">=";
    private const string _lessEqualSymbol = "<=";
    public readonly string[] Operators;
    private List<Func<bool>> _toSwitch;
    private readonly IReadOnlyDictionary<string, Stat> _statsDictionary;
    public readonly IReadOnlyList<Stat> GameStatsCopied;

    public SwitchNodeLogic(IReadOnlyDictionary<string, Stat> statsDictionary, IReadOnlyList<Stat> gameStatsCopied)
    {
        _toSwitch = new List<Func<bool>>();
        _statsDictionary = statsDictionary;
        GameStatsCopied = gameStatsCopied;
        Operators = new[]{_equalSymbol, _greatSymbol, _lessSymbol, _greatEqualSymbol, _lessEqualSymbol};
    }

    public (bool, int) GetPortIndexOnSwitchResult(List<CaseForStats> cases)
    {
        CaseForStats caseForStats;
        int count1 = cases.Count;
        int count2;
        bool caseFoundSuccessfuly = false;
        int indexCase = 0;
        for (int i = 0; i < count1; i++)
        {
            caseForStats = cases[i];
            count2 = caseForStats.CaseStats.Count;
            for (int j = 0; j < count2; j++)
            {
                if (cases[i].CaseStats[j].IncludeKey == true)
                {
                    CaseBaseStat caseBaseStat = cases[i].CaseStats[j];
                    int value = _statsDictionary[GameStatsCopied[j].NameKey].Value;
                    _toSwitch.Add(() => Comparison(caseBaseStat.IndexCurrentOperator, value, caseBaseStat.Value));
                }
            }

            count2 = caseForStats.AdditionalCaseStats.Count;
            for (int j = 0; j < count2; j++)
            {
                AdditionalCaseStats additionalCaseStats = caseForStats.AdditionalCaseStats[i];
                int value1 = _statsDictionary[additionalCaseStats.Stat1Key].Value;
                int value2 = _statsDictionary[additionalCaseStats.Stat2Key].Value;
                _toSwitch.Add(() => Comparison(additionalCaseStats.IndexCurrentOperator, value1, value2));
            }
            if (GroupProcessing() == true)
            {
                caseFoundSuccessfuly = true;
                indexCase = i;
                break;
            }
        }
        return (caseFoundSuccessfuly, indexCase);
    }

    private bool GroupProcessing()
    {
        bool result = false;
        
        foreach (var operation in _toSwitch)
        {
            result = operation.Invoke();
        }
        _toSwitch.Clear();
        return result;
    }
    private bool Comparison(int operatorIndex, int gameStatValue, int switchNodeStatValue)
    {
        bool result = false;
        switch (Operators[operatorIndex])
        {
            case _equalSymbol:
                if (gameStatValue == switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _greatSymbol:
                if (gameStatValue > switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _lessSymbol:
                if (gameStatValue < switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _greatEqualSymbol:
                if (gameStatValue >= switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _lessEqualSymbol:
                if (gameStatValue <= switchNodeStatValue)
                {
                    result = true;
                }
                break;
        }
        return result;
    }
}