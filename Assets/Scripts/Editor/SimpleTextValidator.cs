
using UnityEngine;

public class SimpleTextValidator
{
    public bool TryValidate(ref string text, int maxSymbolsValue)
    {
        if (text.Length <= maxSymbolsValue)
        {
            return true;
        }
        else
        {
            Debug.LogWarning($"Размер строки превышен   {text.Length}");
            return false;
        }
    }
}