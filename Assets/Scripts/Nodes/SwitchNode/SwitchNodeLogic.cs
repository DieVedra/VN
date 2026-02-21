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
    public readonly string[] Operators = {_equalSymbol, _greatSymbol, _lessSymbol, _greatEqualSymbol, _lessEqualSymbol};
    private List<Func<bool>> _toSwitch;
    private readonly IReadOnlyDictionary<string, Stat> _statsDictionary;
    public readonly IReadOnlyList<Stat> GameStatsCopied;

    public SwitchNodeLogic(IReadOnlyDictionary<string, Stat> statsDictionary, IReadOnlyList<Stat> gameStatsCopied)
    {
        _toSwitch = new List<Func<bool>>();
        _statsDictionary = statsDictionary;
        GameStatsCopied = gameStatsCopied;
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
                    Stat stat = _statsDictionary[GameStatsCopied[j].NameKey];
                    _toSwitch.Add(() => Comparison(caseBaseStat.IndexCurrentOperator, stat, caseBaseStat.Value));
                }
            }

            count2 = caseForStats.AdditionalCaseStats.Count;
            for (int j = 0; j < count2; j++)
            {
                AdditionalCaseStats additionalCaseStats = caseForStats.AdditionalCaseStats[i];
                Stat stat = _statsDictionary[additionalCaseStats.Stat1Key];
                int value2 = _statsDictionary[additionalCaseStats.Stat2Key].Value;
                _toSwitch.Add(() => Comparison(additionalCaseStats.IndexCurrentOperator, stat, value2));
            }
            if (GroupProcessing() == true)
            {
                caseFoundSuccessfuly = true;
                indexCase = i;
                break;
            }
        }
        Debug.Log($"result {caseFoundSuccessfuly} {indexCase}");

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
    private bool Comparison(int operatorIndex, Stat gameStat, int switchNodeStatValue)
    {
        bool result = false;
        Debug.Log($"Operators[operatorIndex] {Operators[operatorIndex]}");
        Debug.Log($"gameStat {gameStat.NameText} {gameStat.Value}");
        switch (Operators[operatorIndex])
        {
            case _equalSymbol:
                if (gameStat.Value == switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _greatSymbol:
                if (gameStat.Value > switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _lessSymbol:
                if (gameStat.Value < switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _greatEqualSymbol:
                if (gameStat.Value >= switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case _lessEqualSymbol:
                if (gameStat.Value <= switchNodeStatValue)
                {
                    result = true;
                }
                break;
        }
        return result;
    }
}