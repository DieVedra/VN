using System.Text;
using UnityEngine;
using UnityEngine.U2D;

public class KeyNameSpriteFinder
{
    private const char _symbol = '_';
    private const string _codeToFind = "str";
    private const string _toRemove = "(Clone)";
    private const int _codeCount = 8;
    private StringBuilder _stringBuilder = new StringBuilder();
    
    public string GetNameWithKey(SpriteAtlas atlas, string key)
    {
        int count = atlas.spriteCount;
        Sprite[] sprites = new Sprite[count];
        atlas.GetSprites(sprites);
        string word = null;
        bool resultFind = false;
        string code = null;
        _stringBuilder.Clear();
        _stringBuilder.Append(_codeToFind);
        _stringBuilder.Append(_symbol);
        string cropKey = key.Substring(_stringBuilder.Length);
        for (int i = 0; i < sprites.Length; i++)
        {
            word = sprites[i].name;
            for (int j = 0; j < word.Length; j++)
            {
                if (word[j] == _symbol)
                {
                    resultFind = CheckSTR(word, j);
                    if (resultFind == true)
                    {
                        code = ExtractCode(word, j);
                        break;
                    }
                } 
            }

            if (code == cropKey)
            {
                break;
            }
        }

        if (resultFind == false)
        {
            word = null;
        }
        else
        {
            word = TryRemoveClone(word);
        }
        return word;
    }

    private string TryRemoveClone(string word)
    {
        _stringBuilder.Clear();
        for (int i = word.Length - _toRemove.Length; i < word.Length; i++)
        {
            _stringBuilder.Append(word[i]);
        }

        if (_stringBuilder.ToString() == _toRemove)
        {
            word = word.Remove(word.Length - _toRemove.Length);
        }

        _stringBuilder.Clear();
        return word;
    }

    public string GetNameWithoutKey(SpriteAtlas atlas, string name)
    {
        int count = atlas.spriteCount;
        Sprite[] sprites = new Sprite[count];
        atlas.GetSprites(sprites);
        string word = null;
        _stringBuilder.Clear();
        for (int i = 0; i < sprites.Length; i++)
        {
            word = sprites[i].name;
            for (int j = 0; j < word.Length; j++)
            {
                if (word[j] == _symbol)
                {
                    break;
                }

                _stringBuilder.Append(word[j]);
            }
            word = TryRemoveClone(_stringBuilder.ToString());
            if (word == name)
            {
                break;
            }

            _stringBuilder.Clear();
        }
        return word;
    }

    private bool CheckSTR(string word, int index)
    {
        _stringBuilder.Clear();

        for (int i = 0; i < _codeToFind.Length; i++)
        {
            _stringBuilder.Append(word[++index]);
        }
        if (_stringBuilder.ToString() == _codeToFind)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private string ExtractCode(string word, int index)
    {
        _stringBuilder.Clear();
        int iteration = 0;
        for (int i = index + _codeToFind.Length + 2; i < word.Length; i++)
        {
            if (word[i] == _symbol)
            {
                break;
            }

            if (iteration == _codeCount)
            {
                break;
            }
            iteration++;
            _stringBuilder.Append(word[i]);
        }
        return _stringBuilder.ToString();
    }
}