
public class ChoiceData
{
    public readonly string Text1;
    public readonly string Text2;
    public readonly string Text3;

    public readonly int Choice1Price;
    public readonly int Choice2Price;
    public readonly int Choice3Price;
    
    public readonly int Choice1AdditionaryPrice;
    public readonly int Choice2AdditionaryPrice;
    public readonly int Choice3AdditionaryPrice;
    
    public readonly int TimerValue;
    
    public readonly bool ShowChoice3;
    public int GetAllPrice => Choice1Price + Choice2Price + Choice3Price;
    public int GetAllAdditionaryPrice => Choice1AdditionaryPrice + Choice2AdditionaryPrice + Choice3AdditionaryPrice;

    public ChoiceData(string text1, float choice1Price, float choice1AdditionaryPrice,
        string text2, float choice2Price, float choice2AdditionaryPrice, int timerValue)
    {
        Text1 = text1;
        Text2 = text2;
        Text3 = string.Empty;
        ShowChoice3 = false;
        Choice1Price = (int)choice1Price;
        Choice2Price = (int)choice2Price;
        Choice3AdditionaryPrice = Choice3Price = 0;
        Choice1AdditionaryPrice = (int)choice1AdditionaryPrice;
        Choice2AdditionaryPrice = (int)choice2AdditionaryPrice;
        TimerValue = timerValue;
    }

    public ChoiceData(string text1, float choice1Price, float choice1AdditionaryPrice,
        string text2, float choice2Price, float choice2AdditionaryPrice,
        string text3, float choice3Price, float choice3AdditionaryPrice,
        int timerValue)
    {
        Text1 = text1;
        Text2 = text2;
        ShowChoice3 = true;
        Text3 = text3;
        Choice1Price = (int)choice1Price;
        Choice2Price = (int)choice2Price;
        Choice3Price = (int)choice3Price;
        Choice1AdditionaryPrice = (int)choice1AdditionaryPrice;
        Choice2AdditionaryPrice = (int)choice2AdditionaryPrice;
        Choice3AdditionaryPrice = (int)choice3AdditionaryPrice;
        TimerValue = timerValue;
    }
}