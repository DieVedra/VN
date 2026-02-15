using UnityEngine;

public class GlobalCanvasCloser
{
    private GameObject _targetGameObject;
    private int _count;
    public int Count => _count;

    public void Init(GameObject targetGameObject)
    {
        _targetGameObject = targetGameObject;
        _count = 0;
    }
    public void TryEnable()
    {
        if (_targetGameObject.activeSelf == false)
        {
            _targetGameObject.SetActive(true);
        }
        _count++;
        Debug.Log($" TryEnable() {_count}");

    }

    public void TryDisable()
    {
        if (_count > 1)
        {
            Remove();
        }
        else if (_count == 1)
        {
            Remove();
            _targetGameObject.SetActive(false);
        }
    }

    public void Remove()
    {
        if (_count > 0)
        {
            _count--;
        }
        Debug.Log($"Remove() _count {_count}");
    }
}