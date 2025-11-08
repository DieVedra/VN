
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class test : MonoBehaviour
{
    [SerializeField] private SpriteAtlas _spriteAtlas;
    [SerializeField] private string name;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Button()]
    private void test1()
    {
        _spriteRenderer.sprite = _spriteAtlas.GetSprite(name);
    }
    
    [Button()]
    private void test2()
    {
        
    }
}