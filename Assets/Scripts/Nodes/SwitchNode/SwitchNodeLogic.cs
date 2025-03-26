
using System.Collections.Generic;

public class SwitchNodeLogic
{
    private readonly string[] _operators;

    public SwitchNodeLogic(string[] operators)
    {
        _operators = operators;
    }

    public SwitchNodeLogicResult GetPortIndexOnSwitchResult(IReadOnlyList<Stat> stats, List<CaseForStats> cases)
    {
        SwitchNodeLogicResult result = new SwitchNodeLogicResult();
        for (int i = 0; i < cases.Count; i++)
        {
            for (int j = 0; j < cases[i].CaseStats.Count; j++)
            {
                if (cases[i].CaseStats[j].IncludeKey == true)
                {
                    if (Switch(cases[i].CaseStats[j].IndexCurrentOperator, stats[j].Value, cases[i].CaseStats[j].Value))
                    {
                        result = new SwitchNodeLogicResult(true, i);
                        break;
                    }
                }
            }

            if (result.CaseFoundSuccessfuly == true)
            {
                break;
            }
        }
        return result;
    }

    private bool Switch(int operatorIndex, int gameStatValue, int switchNodeStatValue)
    {
        bool result = false;
        switch (_operators[operatorIndex])
        {
            case "=":
                if (gameStatValue == switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case ">":
                if (gameStatValue > switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case "<":
                if (gameStatValue < switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case ">=":
                if (gameStatValue >= switchNodeStatValue)
                {
                    result = true;
                }
                break;
            case "<=":
                if (gameStatValue <= switchNodeStatValue)
                {
                    result = true;
                }
                break;
        }
        return result;
    }
}