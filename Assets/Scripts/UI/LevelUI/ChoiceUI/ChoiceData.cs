using System.Collections.Generic;

public class ChoiceData
{
    public readonly IReadOnlyList<ChoiceCase> ChoiceCases;
    public readonly int TimerValue;
    public readonly int AllPrice;
    public readonly int AllAdditionaryPrice;
    public int ButtonsCount => ChoiceCases.Count;

    public ChoiceData(IReadOnlyList<ChoiceCase> choiceCases, int timerValue)
    {
        ChoiceCases = choiceCases;
        TimerValue = timerValue;
        foreach (var choiceCase in choiceCases)
        {
            AllPrice += choiceCase.ChoicePrice;
            AllAdditionaryPrice += choiceCase.ChoiceAdditionaryPrice;
        }
    }
}