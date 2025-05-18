
using UnityEngine;

public class ExitButtonUIHandler
{
    public readonly string LabelTextToConfirmedPanel = "Точно?";
    public readonly string TranscriptionTextToConfirmedPanel = "Покинуть игру?";
    public readonly string ButtonText = "Да";
    public const float HeightPanel = 700f;
    public const int FontSizeValue = 120;
    
    public void Press()
    {
        Application.Quit();
    }
}