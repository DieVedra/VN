using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    public List<GameObject> _gameObjects;

    private void Awake()
    {
        foreach (var VARIABLE in _gameObjects)
        {
            Destroy(VARIABLE);
        }
        Destroy(gameObject);
    }

    [Button()]
    private void On()
    {
        foreach (var VARIABLE in _gameObjects)
        {
            VARIABLE.gameObject.SetActive(true);
        }
    }
    
    [Button()]
    private void Off()
    {
        foreach (var VARIABLE in _gameObjects)
        {
            VARIABLE.gameObject.SetActive(false);
        }
    }
}