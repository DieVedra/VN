using NaughtyAttributes;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float x;
    [SerializeField] private string _prop;
    [Button()]
    private void test1()
    {
        _material.SetFloat(_prop, x);
    }
    
    [Button()]
    private void test2()
    {
        
    }
}