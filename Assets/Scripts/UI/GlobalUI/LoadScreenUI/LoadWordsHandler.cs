
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;

public class LoadWordsHandler
{
    private const float _textSize = 70;
    private const float _delay = 1.8f;
    private readonly LocalizationString _loadWordText1 = new LocalizationString(customKey: "str_F8F94E61");
    private readonly LocalizationString _loadWordText2 = new LocalizationString(customKey: "str_20964771");
    private readonly LocalizationString _loadWordText3 = new LocalizationString(customKey: "str_BC51998F");
    private readonly LocalizationString _loadWordText4 = new LocalizationString(customKey: "str_4A2CA084");
    private readonly LocalizationString _loadWordText5 = new LocalizationString(customKey: "str_B5CEE196");

    private List<LocalizationString> _loadWords;
    private TextMeshProUGUI _text;
    private float _textSizeBuffer;
    private bool _isStarting;

    public LoadWordsHandler()
    {
        _loadWords = new List<LocalizationString>
        {
            _loadWordText1,
            _loadWordText2,
            _loadWordText3,
            _loadWordText4,
            _loadWordText5
        };
        _isStarting = false;
    }

    public async UniTaskVoid StartSubstitutingWords(TextMeshProUGUI text)
    {
        if (_isStarting == true)
        {
            return;
        }

        _isStarting = true;
        int i = UnityEngine.Random.Range(0, _loadWords.Count);
        _text = text;
        _textSizeBuffer = text.fontSize;
        text.fontSize = _textSize;
        while (_isStarting)
        {
            text.text = _loadWords[i].DefaultText;
            if (i == _loadWords.Count -1)
            {
                i = 0;
            }
            else
            {
                i++;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        }
    }

    public void StopSubstitutingWords()
    {
        _isStarting = false;
        if (_text != null)
        {
            _text.fontSize = _textSizeBuffer;
            _text = null;
        }
    }
}