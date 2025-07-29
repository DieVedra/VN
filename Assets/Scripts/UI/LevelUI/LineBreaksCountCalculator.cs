using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LineBreaksCountCalculator
{
    public int GetLineBreaksCount(TextMeshProUGUI textComponent, string text)
    {
        Color textColor = textComponent.color;
        textComponent.color = Color.clear;
        textComponent.text = text;
        Debug.Log($"{textComponent.gameObject.activeSelf}");
        Canvas.ForceUpdateCanvases();
        textComponent.ForceMeshUpdate();
        int lineCount = textComponent.textInfo.lineCount;
        int lineBreaks = 0;
        if (lineCount > 0)
        {
            lineBreaks = textComponent.textInfo.lineCount - 1;
        }
        textComponent.text = String.Empty;
        textComponent.color = textColor;
        return lineBreaks;
    }

    public async UniTask<int> GetLineBreaksCountAsync(TextMeshProUGUI textComponent, string text)
    {
        Color textColor = textComponent.color;
        textComponent.color = Color.clear;
        textComponent.text = text;
        Debug.Log($"{textComponent.gameObject.activeSelf}");
        Canvas.ForceUpdateCanvases();
        textComponent.ForceMeshUpdate();
        // await UniTask.Yield();
        await UniTask.Delay(TimeSpan.FromSeconds(0.02f));
        int lineCount = textComponent.textInfo.lineCount;
        int lineBreaks = 0;
        if (lineCount > 0)
        {
            lineBreaks = textComponent.textInfo.lineCount - 1;
        }
        textComponent.text = String.Empty;
        textComponent.color = textColor;
        return lineBreaks;
    } 
}