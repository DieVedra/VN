
using UnityEngine;

public class ExitButtonUIHandler
{
    public const float HeightPanel = 700f;
    public const int FontSizeValue = 120;
    public readonly LocalizationString LabelTextToConfirmedPanel = "Точно?";
    public readonly LocalizationString TranscriptionTextToConfirmedPanel = "Покинуть игру?";
    public readonly LocalizationString ButtonText = "Да";
    public readonly LocalizationString ExitText = "Выход";

    public void Press()
    {
        Application.Quit();
    }
}