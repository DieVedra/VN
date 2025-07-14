
using UnityEngine;

public class SimpleTextValidator
{
    private readonly int _symbolMaxCount;

    public string ValidText;

    public SimpleTextValidator(int symbolMaxCount)
    {
        _symbolMaxCount = symbolMaxCount;
    }

    public bool TryValidate()
    {
        if (ValidText != null)
        {
            if (ValidText.Length <= _symbolMaxCount)
            {
                return true;
            }
            else
            {
                Debug.LogWarning($"Размер строки превышен   {ValidText.Length} {ValidText}");
                return false;
            }
        }
        else
        {
            return true;
        }
    }
}