
using UnityEngine;

public class ExitButtonUIHandler
{
    public readonly string LabelTextToConfirmedPanel = "Точно?";
    public readonly string TranscriptionTextToConfirmedPanel = "Покинуть игру?";
    public readonly string ButtonText = "Да";
    public readonly float HeightPanel = 700f;
    public readonly int FontSizeValue = 120;
    
    public void Press()
    {
        Application.Quit();
    }
}