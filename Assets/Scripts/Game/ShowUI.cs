using System;
using System.Collections.Generic;
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
}