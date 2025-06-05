
using TMPro;
using UnityEngine;

public class HeaderSeriesPanelHandlerUI
{
    private readonly HeaderSeriesPanelUI _headerSeriesPanelUI;

    public HeaderSeriesPanelHandlerUI(HeaderSeriesPanelUI headerSeriesPanelUI)
    {
        _headerSeriesPanelUI = headerSeriesPanelUI;
    }

    public void SetHeader(string textChapterTitle, string textTitle,
        Color colorField1, Color colorField2, int textSize1, int textSize2)
    {
        SetValues(_headerSeriesPanelUI.Text1, colorField1, textSize1, textChapterTitle);
        SetValues(_headerSeriesPanelUI.Text2, colorField2, textSize2, textTitle);
        _headerSeriesPanelUI.gameObject.SetActive(true);
    }

    public void OffHeader()
    {
        _headerSeriesPanelUI.gameObject.SetActive(false);
    }

    private void SetValues(TextMeshProUGUI textMeshProUGUI, Color color, int size, string text)
    {
        textMeshProUGUI.text = text;
        textMeshProUGUI.fontSize = size;
        textMeshProUGUI.color = color;
    }
}