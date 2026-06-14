using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.U2D;

public class test : MonoBehaviour
{
    [SerializeField] private SpriteAtlas _spriteAtlas;
    [SerializeField] private string code;
    [SerializeField] private string name;
    [Button()]
    private void test1()
    {
        var keyNameSpriteFinder = new KeyNameSpriteFinder();
        
        Sprite[] s = new Sprite[_spriteAtlas.spriteCount];
        _spriteAtlas.GetSprites(s);
        
        
        Debug.Log($"{keyNameSpriteFinder.GetNameWithoutKey(_spriteAtlas, name)}");
    }
    
    [Button()]
    private void test2()
    {
        Sprite[] s = new Sprite[_spriteAtlas.spriteCount];
        _spriteAtlas.GetSprites(s);
        foreach (var VARIABLE in s)
        {
            Debug.Log($"{VARIABLE.name}");

        }
    }
    [Button()]
    private void test3()
    {
        Debug.Log($"{_spriteAtlas.GetSprite(name).name}");

    }
}