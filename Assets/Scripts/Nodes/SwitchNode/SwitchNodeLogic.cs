
using System;
using System.Collections.Generic;

public class SwitchNodeLogic
{
    private const string _equalSymbol = "=";
    private const string _greatSymbol = ">";
    private const string _lessSymbol = "<";
    private const string _greatEqualSymbol = ">=";
    private const string _lessEqualSymbol = "<=";
    public readonly string[] Operators;
    private List<Func<bool>> _toSwitch;
    public IReadOnlyList<Stat> GameStats;

    public SwitchNodeLogic(IReadOnlyList<Stat> gameStats)
    {
        _toSwitch = new List<Func<bool>>();
        GameStats = gameStats;
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
                    int value = GameStats[j].Value;
                    _toSwitch.Add(() => Comparison(caseBaseStat.IndexCurrentOperator, value, caseBaseStat.Value));
                }
            }

            count2 = caseForStats.AdditionalCaseStats.Count;
            for (int j = 0; j < count2; j++)
            {
                AdditionalCaseStats additionalCaseStats = caseForStats.AdditionalCaseStats[i];
                int value1 = GameStats[additionalCaseStats.IndexStat1].Value;
                int value2 = GameStats[additionalCaseStats.IndexStat2].Value;

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

    private void a()
    {
        
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