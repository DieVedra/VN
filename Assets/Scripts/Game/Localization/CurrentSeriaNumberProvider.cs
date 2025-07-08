
public static class CurrentSeriaNumberProvider
{
    private const int _summandValue = 2;
    public static int GetCurrentSeriaNumber(int index)
    {
        return ++index;
    }
    public static int GetSecondSeriaNumber(int index)
    {
        return index + _summandValue;
    }
}