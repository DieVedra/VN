using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpriteData", menuName = "Character/SpriteData", order = 51)]
public class SpriteData : ScriptableObject
{
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private List<MySprite> _mySprites;
    public List<Sprite> Sprites => _sprites;
    public List<MySprite> MySprites => _mySprites;

    public string[] GetNames()
    {
        List<string> names = new List<string>(_sprites.Count);
        for (int i = 0; i < _sprites.Count; i++)
        {
            if (_sprites[i] != null)
            {
                names.Add(_sprites[i].name);
            }
        }
        return names.ToArray();
    }

    public void InitMySprites()
    {
        if (_sprites.Count != 0)
        {
            List<MySprite> mySprites = new List<MySprite>(_sprites.Count);
            for (int i = 0; i < _sprites.Count; i++)
            {
                if (_mySprites.Count  > i && _mySprites[i] != null)
                {
                    mySprites.Add(new MySprite(_sprites[i], _mySprites[i].OffsetXValue, _mySprites[i].OffsetYValue, _mySprites[i].ScaleValue, _mySprites[i].Price));
                }
                else
                {
                    mySprites.Add(new MySprite(_sprites[i]));
                }
            }

            _mySprites = mySprites;
        }
    }
    private void Awake()
    {
        if (_sprites == null)
        {
            _sprites = new List<Sprite>();
        }
        if (_mySprites == null)
        {
            _mySprites = new List<MySprite>();
        }
    }
}