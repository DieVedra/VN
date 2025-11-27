
public class CheckMathSeriaIndex
{
    private int _lastIndex = -1;

    public bool Check(int currentIndex)
    {
        if (_lastIndex == currentIndex)
        {
            return false;
        }
        else
        {
            _lastIndex = currentIndex;
            return true;
        }
    }
}