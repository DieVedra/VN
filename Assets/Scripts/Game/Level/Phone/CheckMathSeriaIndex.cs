
public class CheckMathSeriaIndex
{
    public int LastIndex { get; private set; }= -1;

    public bool Check(int currentIndex)
    {
        if (LastIndex == currentIndex)
        {
            return false;
        }
        else
        {
            LastIndex = currentIndex;
            return true;
        }
    }
}