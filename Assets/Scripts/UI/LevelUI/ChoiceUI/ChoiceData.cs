
public class ChoiceData
{
    public readonly string Text1;
    public readonly string Text2;
    public readonly string Text3;

    public readonly int Choice1Price;
    public readonly int Choice2Price;
    public readonly int Choice3Price;

    public readonly int TimerValue;
    
    public readonly bool ShowChoice3;

    public ChoiceData(string text1, float choice1Price,
        string text2, float choice2Price, int timerValue)
    {
        Text1 = text1;
        Text2 = text2;
        Text3 = string.Empty;
        ShowChoice3 = false;
        Choice1Price = (int)choice1Price;
        Choice2Price = (int)choice2Price;
        Choice3Price = 0;
        TimerValue = timerValue;
    }

    public ChoiceData(string text1, float choice1Price,
        string text2, float choice2Price,
        string text3, float choice3Price,
        int timerValue)
    {
        Text1 = text1;
        Text2 = text2;
        ShowChoice3 = true;
        Text3 = text3;
        Choice1Price = (int)choice1Price;
        Choice2Price = (int)choice2Price;
        Choice3Price = (int)choice3Price;
        TimerValue = timerValue;
    }
}