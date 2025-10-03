using System;
using TMPro;
using UnityEngine;

public class LineBreaksCountCalculator
{
    public int GetLineBreaksCount(TextMeshProUGUI textComponent, string text, bool clear = true)
    {
        Color textColor = textComponent.color;
        textComponent.color = Color.clear;
        textComponent.text = text;
        if (textComponent.gameObject.activeSelf == false)
        {
            textComponent.gameObject.SetActive(true);
        }
        textComponent.ForceMeshUpdate();
        int lineCount = textComponent.textInfo.lineCount;
        int lineBreaks = 0;
        if (lineCount > 0)
        {
            lineBreaks = textComponent.textInfo.lineCount - 1;
        }

        if (clear == true)
        {
            textComponent.text = String.Empty;
        }

        textComponent.color = textColor;
        return lineBreaks;
    }
}